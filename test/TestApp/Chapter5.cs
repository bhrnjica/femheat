using NumIntegration;

namespace Book
{
    internal class Chapter5
    {

        // ====================================================================
        // Zadatak 5.02: Implementirati program za rješavanje zadatka 05-01
        // korištenjem NumIntegration biblioteke. Program definiše tri
        // linijska KE (po jedan za svaki sloj kompozitnog zida:
        // stiropor–drvo–čelik), formira globalnu matricu krutosti [K] i
        // vektor opterećenja {F} korištenjem klase Mesh1D, te rješava
        // sistem Gaussovom eliminacijom (Gaussian.Solve).
        // ====================================================================
        public static void Zadatak_05_02()
        {
            Console.WriteLine("=== Zadatak 5.02: Kompozitni zid sa tri sloja ===");
            Console.WriteLine("MKE rješenje korištenjem izoparametarske formulacije\n");

            // ---------- 1. Fizički parametri problema ----------
            double lambda1 = 0.035;  // W/(m·°C) — stiropor
            double lambda2 = 0.17;   // W/(m·°C) — drvo
            double lambda3 = 50.0;   // W/(m·°C) — čelik

            double l1 = 0.02;  // m — stiropor  (2 cm)
            double l2 = 0.08;  // m — drvo      (8 cm)
            double l3 = 0.05;  // m — čelik     (5 cm)

            double A = 1.0;           // m² — jedinična površina presjeka
            double q = 20.0;          // W/m² — toplotni tok s lijeve strane
            double alpha = 25.0;      // W/(m²·°C) — koef. konvekcije (desno)
            double thetaInf = 20.0;   // °C — temperatura okoline

            // ---------- 2. Definicija konačnih elemenata ----------
            // Svaki sloj diskretiziran sa po jednim linijskim KE sa 2 čvora.
            // Lokalne koordinate: čvor 1 na x=0, čvor 2 na x=l_e

            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 0.0),      // lijevi kraj stiropora
                    new Node("2", l1),       // desni kraj stiropora (spoj sa drvetom)
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            var fe2 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 0.0),      // početak drveta (lokalno)
                    new Node("2", l2),       // kraj drveta (spoj sa čelikom)
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            var fe3 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 0.0),      // početak čelika (lokalno)
                    new Node("2", l3),       // desna površina čelika
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            // ---------- 3. Verifikacija dužina elemenata numeričkom integracijom ----------
            Console.WriteLine("--- Provjera dužina konačnih elemenata (NumIntegration) ---");
            var numI1 = new Numeric(fe1);
            var numI2 = new Numeric(fe2);
            var numI3 = new Numeric(fe3);

            double L1_num = numI1.Integrate("1", DOP.Linear);
            double L2_num = numI2.Integrate("1", DOP.Linear);
            double L3_num = numI3.Integrate("1", DOP.Linear);

            Console.WriteLine($"  Element 1 (stiropor): l₁ = {L1_num:F4} m "
                + $"(analitički: {l1:F4} m)");
            Console.WriteLine($"  Element 2 (drvo):     l₂ = {L2_num:F4} m "
                + $"(analitički: {l2:F4} m)");
            Console.WriteLine($"  Element 3 (čelik):    l₃ = {L3_num:F4} m "
                + $"(analitički: {l3:F4} m)\n");

            // ---------- 4. Koeficijenti kondukcije pojedinačnih KE ----------
            // Prema izrazu: k_e = λ_e * A / l_e
            double k1 = lambda1 * A / l1;  // = 1.75 W/°C
            double k2 = lambda2 * A / l2;  // = 2.125 W/°C
            double k3 = lambda3 * A / l3;  // = 1000 W/°C
            double alphaA = alpha * A;     // = 25 W/°C — konvektivni član

            Console.WriteLine("--- Koeficijenti kondukcije elemenata ---");
            Console.WriteLine($"  k₁ = λ₁·A/l₁ = {lambda1}·{A}/{l1} = {k1} W/°C");
            Console.WriteLine($"  k₂ = λ₂·A/l₂ = {lambda2}·{A}/{l2} = {k2} W/°C");
            Console.WriteLine($"  k₃ = λ₃·A/l₃ = {lambda3}·{A}/{l3} = {k3} W/°C");
            Console.WriteLine($"  αA = {alpha}·{A} = {alphaA} W/°C\n");

            // ---------- 5. Numerička verifikacija koeficijenata kondukcije ----------
            // Matrica kondukcije za linijski KE: K₁₁ = λA ∫ (dN₁/dx)² dx = λA/l
            // dN₁/dx = -1/l, dN₂/dx = 1/l → (dN₁/dx)² = 1/l²
            Console.WriteLine("--- Numerička verifikacija k₁ (Gaussova integracija) ---");
            // Integracija konstante (λA/l²) po domenu [0, l] daje λA/l
            string exprK1 = $"{lambda1}*{A}/({l1}*{l1})";
            double k1_linear = numI1.Integrate(exprK1.Replace(',','.'), DOP.Linear);
            double k1_quad   = numI1.Integrate(exprK1.Replace(',','.'), DOP.Quadratic);
            double k1_cubic  = numI1.Integrate(exprK1.Replace(',','.'), DOP.Cubic);
            Console.WriteLine($"  k₁ (Lin. integracija):  {k1_linear:F6} W/°C");
            Console.WriteLine($"  k₁ (Quad. integracija): {k1_quad:F6} W/°C");
            Console.WriteLine($"  k₁ (Cubic integracija): {k1_cubic:F6} W/°C");
            Console.WriteLine($"  k₁ (analitički):        {k1:F6} W/°C\n");

            // ---------- 6. Formiranje mreže i sklapanje globalnog sistema ----------
            // Mesh1D prima FiniteElement objekte i odgovarajuće nizove
            // materijalnih parametara (λ, A). Sklapanje matrice krutosti [K]
            // i vektora opterećenja {F} vrši se automatski prema (jed:05-27).
            var mesh = new Mesh1D([fe1, fe2, fe3 ], 
                                  [lambda1, lambda2, lambda3],
                                  [A, A, A ] )
            {
                LeftHeatFlux = q,
                RightConvectionCoeff = alpha,
                RightAmbientTemp = thetaInf
            };

            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} ---");
            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, broj čvorova: {n}");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < n; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < n; j++)
                    Console.Write($"{K[i, j],10:F4} ");
                Console.WriteLine("]");
            }
            Console.WriteLine("\nVektor opterećenja {F}:");
            Console.Write("  { ");
            for (int i = 0; i < n; i++)
                Console.Write($"{F[i],10:F4} ");
            Console.WriteLine("}\n");

            // ---------- 7. Rješavanje sistema Gaussovom eliminacijom ----------
            double[] theta = Gaussian.Solve(K, F, n);

            Console.WriteLine("--- Rezultati ---");
            Console.WriteLine($"  ϑ₁ = {theta[0]:F2} °C  (lijevi kraj stiropora)");
            Console.WriteLine($"  ϑ₂ = {theta[1]:F2} °C  (spoj stiropor–drvo)");
            Console.WriteLine($"  ϑ₃ = {theta[2]:F2} °C  (spoj drvo–čelik)");
            Console.WriteLine($"  ϑ₄ = {theta[3]:F2} °C  (desni kraj čelika)\n");

            // ---------- 8. Usporedba s analitičkim rješenjem ----------
            Console.WriteLine("--- Usporedba s analitičkim rješenjem iz knjige ---");
            double[] thetaExact = { 41.66, 30.23, 20.82, 20.80 };
            string[] labels = { "ϑ₁", "ϑ₂", "ϑ₃", "ϑ₄" };
            Console.WriteLine("  Čvor |   FEM (C#)  |  Analitički |  Razlika");
            Console.WriteLine("  ------|-------------|-------------|---------");
            for (int i = 0; i < n; i++)
            {
                double diff = Math.Abs(theta[i] - thetaExact[i]);
                Console.WriteLine($"  {labels[i]}   |"
                    + $"   {theta[i],8:F2}   |"
                    + $"   {thetaExact[i],8:F2}   |"
                    + $"  {diff,6:F4}");
            }

            // ---------- 9. Provjera uvrštavanjem u sistem ----------
            Console.WriteLine("\n--- Provjera rješenja uvrštavanjem u sistem [K]{ϑ} = {F} ---");
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                    sum += K[i, j] * theta[j];
                Console.WriteLine($"  Jednačina {i + 1}: "
                    + $"ΣK_{i+1},j·ϑ_j = {sum,10:F4}"
                    + $"  (očekivano: {F[i],10:F4})"
                    + $"  {(Math.Abs(sum - F[i]) < 1e-6 ? "✓" : "✗")}");
            }

            // ---------- 10. Fizička interpretacija ----------
            Console.WriteLine("\n--- Fizička interpretacija ---");
            double deltaT1 = theta[0] - theta[1];
            double deltaT2 = theta[1] - theta[2];
            double deltaT3 = theta[2] - theta[3];
            Console.WriteLine($"  Pad temperature u stiroporu: {deltaT1:F2} °C "
                + $"(najveći pad — niska provodnost λ₁={lambda1})");
            Console.WriteLine($"  Pad temperature u drvetu:    {deltaT2:F2} °C "
                + $"(umjeren pad — srednja provodnost λ₂={lambda2})");
            Console.WriteLine($"  Pad temperature u čeliku:    {deltaT3:F2} °C "
                + $"(zanemariv pad — visoka provodnost λ₃={lambda3})");

            Console.WriteLine("\n=== Kraj zadatka 5.02 ===");
        }

        // ====================================================================
        // Zadatak 5.05: Ravni zid sa unutrašnjim izvorom toplote — FEM rješenje
        // Određivanje raspodjele temperature u ravnom zidu debljine 60 mm
        // s unutrašnjom generacijom toplote q_dot=0.3 MW/m³ i λ=21 W/(m·°C).
        // Temperatura površine zida: ϑ_s=40 °C. Diskretizacija: 4 KE.
        // ====================================================================
        public static void Zadatak_05_05()
        {
            Console.WriteLine("=== Zadatak 5.05: Ravni zid sa unutrašnjim izvorom ===");
            Console.WriteLine("MKE rješenje s 4 linearna KE\n");

            // ---------- 1. Fizički parametri ----------
            double lambda = 21.0;       // W/(m·°C)
            double qDot   = 0.3e6;      // W/m³  (0.3 MW/m³)
            double L      = 0.060;      // m     (60 mm)
            double thetaS = 40.0;       // °C
            double A      = 1.0;        // m²
            int    nElem  = 4;          // broj KE

            double le = L / nElem;      // dužina jednog elementa
            double ke = lambda * A / le; // koeficijent kondukcije elementa

            // ---------- 2. Parametri sistema ----------
            int nNodes = nElem + 1;  // broj čvorova (5)
            double[,] K = new double[nNodes, nNodes];
            double[]  F = new double[nNodes];

            // ---------- 3. Sklapanje globalnog sistema ----------
            for (int e = 0; e < nElem; e++)
            {
                // Lokalni vektor opterećenja (konzistentan)
                double fe = qDot * A * le / 2.0;

                // Lokalna matrica krutosti
                K[e, e]       += ke;
                K[e, e + 1]   -= ke;
                K[e + 1, e]   -= ke;
                K[e + 1, e + 1] += ke;

                // Lokalni vektor opterećenja
                F[e]     += fe;
                F[e + 1] += fe;
            }

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} (prije G.U.) ---");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < nNodes; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < nNodes; j++)
                    Console.Write($"{K[i, j],8:F1} ");
                Console.WriteLine("]");
            }
            Console.Write("\nVektor opterećenja {F}: { ");
            for (int i = 0; i < nNodes; i++)
                Console.Write($"{F[i],8:F1} ");
            Console.WriteLine("}\n");

            // ---------- 4. Granični uvjeti (Dirichlet) ----------
            // ϑ₁ = ϑ₅ = ϑ_s
            K[0, 0] = 1; K[0, 1] = 0;
            F[0] = thetaS;

            K[nNodes - 1, nNodes - 1] = 1;
            K[nNodes - 1, nNodes - 2] = 0;
            F[nNodes - 1] = thetaS;

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} (nakon G.U.) ---");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < nNodes; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < nNodes; j++)
                    Console.Write($"{K[i, j],8:F1} ");
                Console.WriteLine("]");
            }
            Console.Write("\nVektor opterećenja {F}: { ");
            for (int i = 0; i < nNodes; i++)
                Console.Write($"{F[i],8:F1} ");
            Console.WriteLine("}\n");

            // ---------- 5. Rješavanje sistema ----------
            double[] theta = Gaussian.Solve(K, F, nNodes);

            Console.WriteLine("--- Rezultati ---");
            string[] posLabels = { "x=0 (lijeva površina)",
                                   "x=15 mm",
                                   "x=30 mm (sredina)",
                                   "x=45 mm",
                                   "x=60 mm (desna površina)" };
            for (int i = 0; i < nNodes; i++)
                Console.WriteLine($"  ϑ{i + 1} = {theta[i]:F2} °C  ({posLabels[i]})");

            // ---------- 6. Analitičko rješenje ----------
            Console.WriteLine("\n--- Usporedba s analitičkim rješenjem ---");
            double[] xPos = { 0, 0.015, 0.030, 0.045, 0.060 };
            Console.WriteLine("  Čvor |   x (mm)  |   FEM (°C)  |  Analit. (°C) |  Razlika");
            Console.WriteLine("  -----|-----------|-------------|---------------|---------");
            for (int i = 0; i < nNodes; i++)
            {
                double x = xPos[i];
                double thetaExact = thetaS
                    + (qDot * L * L) / (2 * lambda) * (x / L - x * x / (L * L));
                double diff = Math.Abs(theta[i] - thetaExact);
                Console.WriteLine($"  {i + 1,4} |"
                    + $"  {x * 1000,6:F1}  |"
                    + $"  {theta[i],9:F3}  |"
                    + $"  {thetaExact,11:F3}  |"
                    + $"  {diff,7:F4}");
            }

            // ---------- 7. Fizička interpretacija ----------
            Console.WriteLine("\n--- Fizička interpretacija ---");
            double deltaTmax = theta[2] - thetaS;
            Console.WriteLine($"  Maksimalna temperatura: ϑ_max = {theta[2]:F2} °C "
                + $"(na sredini zida, x=30 mm)");
            Console.WriteLine($"  Porast temperature: Δϑ = {deltaTmax:F2} °C "
                + $"(od površine do sredine)");
            Console.WriteLine($"  Generisana toplota: q̇ = {qDot / 1e6:F1} MW/m³ "
                + $"→ ukupno {qDot * L * A / 1000:F1} kW/m²");

            Console.WriteLine("\n=== Kraj zadatka 5.05 ===");
        }

    }
}
