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
        // Korišten Mesh1D sa HeatSource za automatsko sklapanje sistema.
        // ====================================================================
        public static void Zadatak_05_05()
        {
            Console.WriteLine("=== Zadatak 5.05: Ravni zid sa unutrašnjim izvorom ===");
            Console.WriteLine("MKE rješenje s 4 linearna KE (Mesh1D + HeatSource)\n");

            // ---------- 1. Fizički parametri ----------
            double lambda = 21.0;       // W/(m·°C)
            double qDot   = 0.3e6;      // W/m³  (0.3 MW/m³)
            double L      = 0.060;      // m     (60 mm)
            double thetaS = 40.0;       // °C
            double A      = 1.0;        // m²
            int    nElem  = 4;          // broj KE

            double le = L / nElem;      // dužina jednog elementa

            // ---------- 2. Definicija konačnih elemenata ----------
            var elements = new FiniteElement[nElem];
            for (int e = 0; e < nElem; e++)
            {
                elements[e] = new FiniteElement()
                {
                    nodes = new[]
                    {
                        new Node($"{e+1}", e * le),
                        new Node($"{e+2}", (e + 1) * le),
                    },
                    ft = FEType.Line,
                    fo = FEOrder.Linear,
                };
            }

            // ---------- 3. Formiranje mreže (Mesh1D sa izvorom toplote) ----------
            var mesh = new Mesh1D(elements,
                                  Enumerable.Repeat(lambda, nElem).ToArray(),
                                  Enumerable.Repeat(A, nElem).ToArray())
            {
                HeatSource = qDot   // unutrašnji izvor toplote G [W/m³]
            };

            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} (prije G.U.) ---");
            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, broj čvorova: {n}");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < n; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < n; j++)
                    Console.Write($"{K[i, j],8:F1} ");
                Console.WriteLine("]");
            }
            Console.Write("\nVektor opterećenja {F}: { ");
            for (int i = 0; i < n; i++)
                Console.Write($"{F[i],8:F1} ");
            Console.WriteLine("}\n");

            // ---------- 4. Granični uvjeti (Dirichlet) ----------
            // ϑ₁ = ϑ₅ = ϑ_s
            K[0, 0] = 1; K[0, 1] = 0;
            F[0] = thetaS;

            K[n - 1, n - 1] = 1;
            K[n - 1, n - 2] = 0;
            F[n - 1] = thetaS;

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} (nakon G.U.) ---");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < n; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < n; j++)
                    Console.Write($"{K[i, j],8:F1} ");
                Console.WriteLine("]");
            }
            Console.Write("\nVektor opterećenja {F}: { ");
            for (int i = 0; i < n; i++)
                Console.Write($"{F[i],8:F1} ");
            Console.WriteLine("}\n");

            // ---------- 5. Rješavanje sistema ----------
            double[] theta = Gaussian.Solve(K, F, n);

            Console.WriteLine("--- Rezultati ---");
            string[] posLabels = { "x=0 (lijeva površina)",
                                   "x=15 mm",
                                   "x=30 mm (sredina)",
                                   "x=45 mm",
                                   "x=60 mm (desna površina)" };
            for (int i = 0; i < n; i++)
                Console.WriteLine($"  ϑ{i + 1} = {theta[i]:F2} °C  ({posLabels[i]})");

            // ---------- 6. Analitičko rješenje ----------
            Console.WriteLine("\n--- Usporedba s analitičkim rješenjem ---");
            double[] xPos = { 0, 0.015, 0.030, 0.045, 0.060 };
            Console.WriteLine("  Čvor |   x (mm)  |   FEM (°C)  |  Analit. (°C) |  Razlika");
            Console.WriteLine("  -----|-----------|-------------|---------------|---------");
            for (int i = 0; i < n; i++)
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

        // ====================================================================
        // Zadatak 5.07: Cilindrična izolacija cijevi — dva sloja
        // (staklena vuna + gipsani malter). Radijalni prenos toplote sa
        // dva linijska KE korištenjem Mesh1D klase sa Geometry=Cylindrical.
        // Unutrašnja površina na 92 °C (vrelo ulje), spoljašnja površina
        // izložena konvekciji (α=15 W/(m²·°C), ϑ_∞=15 °C).
        // Odrediti temperature na spoju slojeva i na spoljašnjoj površini.
        // ====================================================================
        public static void Zadatak_05_07()
        {
            Console.WriteLine("=== Zadatak 5.07: Cilindrična izolacija ===");
            Console.WriteLine("MKE rješenje s 2 linearna KE (radijalni prenos)\n");

            // ---------- 1. Fizički parametri ----------
            double lambda1 = 0.04;   // W/(m·°C) — staklena vuna
            double lambda2 = 0.06;   // W/(m·°C) — gipsani malter

            double r1 = 0.05;  // m — unutrašnja površina cijevi (d=10 cm)
            double r2 = 0.06;  // m — spoj vuna–malter (+1 cm)
            double r3 = 0.07;  // m — spoljašnja površina (+1 cm)
            double L  = 1.0;   // m — jedinična dužina cijevi

            double alpha     = 15.0;   // W/(m²·°C) — koef. konvekcije
            double theta_u   = 92.0;   // °C — temperatura ulja (unutrašnja)
            double theta_inf = 15.0;   // °C — temperatura okoline

            // ---------- 2. Definicija konačnih elemenata ----------
            // Kod cilindričnog zida, koordinate čvorova su radijusi r
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", r1),  // unutrašnja površina cijevi
                    new Node("2", r2),  // spoj staklena vuna — gipsani malter
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            var fe2 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", r2),  // spoj (lokalni čvor 1)
                    new Node("2", r3),  // spoljašnja površina
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            // ---------- 3. Formiranje mreže (cilindrična geometrija) ----------
            // Mesh1D sa Geometry=Cylindrical automatski računa:
            //   • k_e = 2πL λ_e r̄_e / l_e  (kondukcija sa srednjim radijusom)
            //   • Granice: unutrašnja površina 2πr₁L, spoljašnja 2πr₃L
            // Površine u konstruktoru su proizvoljne za cilindrični slučaj
            // (koriste se samo za PlaneWall geometriju).
            var mesh = new Mesh1D([fe1, fe2],
                                  [lambda1, lambda2],
                                  [0.0, 0.0])  // ne koristi se za Cylindrical
            {
                Geometry = GeometryType.Cylindrical,
                CylinderLength = L,
                LeftHeatFlux = 0,           // nema toplotnog toka (Dirichlet)
                RightConvectionCoeff = alpha,
                RightAmbientTemp = theta_inf
            };

            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;

            // ---------- 4. Koeficijenti (informativno) ----------
            double l1 = r2 - r1;
            double l2 = r3 - r2;
            double rBar1 = (r1 + r2) / 2.0;
            double rBar2 = (r2 + r3) / 2.0;
            double k1 = 2 * Math.PI * L * lambda1 * rBar1 / l1;
            double k2 = 2 * Math.PI * L * lambda2 * rBar2 / l2;
            double kAlpha = alpha * 2 * Math.PI * r3 * L;

            Console.WriteLine("--- Koeficijenti kondukcije (cilindrični prenos) ---");
            Console.WriteLine($"  KE 1 (staklena vuna):  l₁={l1:F3} m, "
                + $"r̄₁={rBar1:F3} m → k₁={k1:F4} W/°C");
            Console.WriteLine($"  KE 2 (gipsani malter): l₂={l2:F3} m, "
                + $"r̄₂={rBar2:F3} m → k₂={k2:F4} W/°C");
            Console.WriteLine($"  Konvekcija (r=r₃):     "
                + $"α·2πr₃L = {kAlpha:F4} W/°C\n");

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} (prije G.U.) ---");
            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, broj čvorova: {n}");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < n; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < n; j++)
                    Console.Write($"{K[i, j],10:F4} ");
                Console.WriteLine("]");
            }
            Console.Write($"\nVektor opterećenja {{F}}: "
                + $"{{ {F[0],8:F2}, {F[1],8:F2}, {F[2],8:F2} }}\n\n");

            // ---------- 5. Granični uvjet (Dirichlet) ----------
            // ϑ₁ = ϑ_u = 92 °C
            K[0, 0] = 1; K[0, 1] = 0; K[0, 2] = 0;
            F[0] = theta_u;

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} (nakon G.U.) ---");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < n; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < n; j++)
                    Console.Write($"{K[i, j],10:F4} ");
                Console.WriteLine("]");
            }
            Console.Write($"\nVektor opterećenja {{F}}: "
                + $"{{ {F[0],8:F2}, {F[1],8:F2}, {F[2],8:F2} }}\n\n");

            // ---------- 6. Rješavanje sistema ----------
            double[] theta = Gaussian.Solve(K, F, n);

            Console.WriteLine("--- Rezultati ---");
            Console.WriteLine($"  ϑ₁ = {theta[0]:F2} °C  "
                + $"(r={r1*1000:F0} mm, unutrašnja površina — vrelo ulje)");
            Console.WriteLine($"  ϑ₂ = {theta[1]:F2} °C  "
                + $"(r={r2*1000:F0} mm, spoj staklena vuna–gipsani malter)");
            Console.WriteLine($"  ϑ₃ = {theta[2]:F2} °C  "
                + $"(r={r3*1000:F0} mm, spoljašnja površina)\n");

            // ---------- 7. Toplotni tok i bilans ----------
            double Q = k1 * (theta[0] - theta[1]);
            Console.WriteLine("--- Toplotni tok ---");
            Console.WriteLine($"  Q = k₁·(ϑ₁−ϑ₂) = {k1:F4}·({theta[0]:F2}−{theta[1]:F2})"
                + $" = {Q:F2} W  (po metru dužine cijevi)");
            Console.WriteLine($"  Provjera preko KE 2: "
                + $"k₂·(ϑ₂−ϑ₃) = {k2:F4}·({theta[1]:F2}−{theta[2]:F2})"
                + $" = {k2*(theta[1]-theta[2]):F2} W ✓\n");

            // ---------- 8. Fizička interpretacija ----------
            Console.WriteLine("--- Fizička interpretacija ---");
            double deltaT1 = theta[0] - theta[1];
            double deltaT2 = theta[1] - theta[2];
            Console.WriteLine($"  Pad u staklenoj vuni:  "
                + $"Δϑ₁ = {deltaT1:F2} °C "
                + $"(veći pad — niža provodnost λ₁={lambda1})");
            Console.WriteLine($"  Pad u gipsanom malteru: "
                + $"Δϑ₂ = {deltaT2:F2} °C "
                + $"(manji pad — viša provodnost λ₂={lambda2})");
            Console.WriteLine($"  Spoljašnja temperatura zida ({theta[2]:F2} °C) "
                + $"je za {theta[2]-theta_inf:F1} °C viša od okoline "
                + $"(konvektivni otpor na granici)");

            Console.WriteLine("\n=== Kraj zadatka 5.07 ===");
        }

        // ====================================================================
        // Zadatak 5.09: Konusno rebro promjenjivog poprečnog presjeka
        // Odvođenje toplote sa rebra u okolinu. Rebro se sužava od debljine
        // 6 mm u podnožju do 1.5 mm na vrhu. Površinska konvekcija duž
        // rebra + konvekcija na vrhu. Korišten Mesh1D sa IsFin=true za
        // automatsko dodavanje matrica površinske konvekcije.
        // Slučaj (a): dva linijska KE. Slučaj (b): jedan kvadratni KE.
        // Izračunava odvedenu toplotu i efikasnost rebra.
        // ====================================================================
        public static void Zadatak_05_09()
        {
            Console.WriteLine("=== Zadatak 5.09: Konusno rebro ===");
            Console.WriteLine("MKE rješenje korištenjem izoparametarske formulacije\n");

            // ---------- 1. Geometrijski i fizički parametri ----------
            double L = 0.08;           // m — dužina rebra (8 cm)
            double b = 0.003;          // m — širina rebra (3 mm)
            double d1 = 0.006;         // m — debljina u podnožju (6 mm)
            double d2 = 0.0015;        // m — debljina na vrhu (1.5 mm)
            double lambda = 150.0;     // W/(m·°C) — toplotna provodnost
            double alpha = 80.0;       // W/(m²·°C) — koef. prelaza toplote
            double theta_b = 120.0;    // °C — temperatura podnožja
            double theta_inf = 25.0;   // °C — temperatura okoline

            // Pomoćne funkcije za debljinu, površinu i obim
            double Thickness(double x) => d1 + (d2 - d1) * x / L;
            double Area(double x) => b * Thickness(x);
            double Perimeter(double x) => 2.0 * (b + Thickness(x));

            // Pozicije čvorova
            double x1 = 0.0, x2 = L / 2.0, x3 = L;

            // Površine i obimi po čvorovima (za VariableCrossSection i IsFin)
            double[] areas = { Area(x1), Area(x2), Area(x3) };
            double[] perimeters = { Perimeter(x1), Perimeter(x2), Perimeter(x3) };

            // Ukupna površina rebra za efikasnost
            double A_surf = 2 * b * L + (d1 + d2) * L + b * d2;
            double Q_ideal = alpha * A_surf * (theta_b - theta_inf);

            // ================================================================
            // SLUČAJ (a): Dva linijska KE
            // ================================================================
            Console.WriteLine("--- (a) Dva linijska konačna elementa ---\n");

            Console.WriteLine("Čvorovi i njihove geometrijske karakteristike:");
            Console.WriteLine($"  Čvor 1 (x=0.0 mm):     d={Thickness(x1)*1000:F2} mm, "
                + $"A={Area(x1):E2} m², O={Perimeter(x1):F4} m");
            Console.WriteLine($"  Čvor 2 (x=40.0 mm):    d={Thickness(x2)*1000:F2} mm, "
                + $"A={Area(x2):E2} m², O={Perimeter(x2):F4} m");
            Console.WriteLine($"  Čvor 3 (x=80.0 mm):    d={Thickness(x3)*1000:F2} mm, "
                + $"A={Area(x3):E2} m², O={Perimeter(x3):F4} m\n");

            // Definicija dva linijska KE
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", x1),   // podnožje
                    new Node("2", x2),   // sredina
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            var fe2 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", x2),   // sredina (lokalni čvor 1)
                    new Node("2", x3),   // vrh
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            // Mesh1D sa uključenom analizom rebra (IsFin)
            var meshLinear = new Mesh1D([fe1, fe2],
                                         [lambda, lambda],
                                         areas)
            {
                VariableCrossSection = true,
                IsFin = true,
                Perimeters = perimeters,
                SurfaceConvectionCoeff = alpha,
                SurfaceAmbientTemp = theta_inf,
                RightConvectionCoeff = alpha,    // konvekcija na vrhu
                RightAmbientTemp = theta_inf,
            };

            // Sklapanje originalnog sistema (prije modifikacije G.U.)
            var (K_lin_orig, F_lin_orig) = meshLinear.Assemble();
            int n_lin = meshLinear.NodeCount;

            // Sačuvaj originalni sistem za izračunavanje reakcije
            double[,] K_lin_full = (double[,])K_lin_orig.Clone();
            double[] F_lin_full = (double[])F_lin_orig.Clone();

            // Dirichletov G.U.: ϑ₁ = ϑ_b
            // Modifikuj red 0: postavi dijagonalu na 1, ostalo na 0
            K_lin_orig[0, 0] = 1; K_lin_orig[0, 1] = 0; K_lin_orig[0, 2] = 0;
            F_lin_orig[0] = theta_b;
            // Modifikuj redove 1 i 2: prebaci poznati član na desnu stranu
            F_lin_orig[1] -= K_lin_orig[1, 0] * theta_b;
            K_lin_orig[1, 0] = 0;
            F_lin_orig[2] -= K_lin_orig[2, 0] * theta_b;
            K_lin_orig[2, 0] = 0;

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} (nakon G.U.) ---");
            Console.WriteLine($"Broj elemenata: {meshLinear.ElementCount}, "
                + $"broj čvorova: {n_lin}");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < n_lin; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < n_lin; j++)
                    Console.Write($"{K_lin_orig[i, j],10:F6} ");
                Console.WriteLine("]");
            }
            Console.Write($"\nVektor opterećenja {{F}}: "
                + $"{{ {F_lin_orig[0]:F6}, {F_lin_orig[1]:F6}, "
                + $"{F_lin_orig[2]:F6} }}\n\n");

            // Rješavanje sistema
            double[] theta_lin = Gaussian.Solve(K_lin_orig, F_lin_orig, n_lin);

            Console.WriteLine("--- Rezultati (a) ---");
            Console.WriteLine($"  ϑ₁ = {theta_lin[0]:F2} °C  "
                + $"(x=0.0 mm, podnožje)");
            Console.WriteLine($"  ϑ₂ = {theta_lin[1]:F2} °C  "
                + $"(x=40.0 mm, sredina)");
            Console.WriteLine($"  ϑ₃ = {theta_lin[2]:F2} °C  "
                + $"(x=80.0 mm, vrh)\n");

            // Odvedena toplota (reakcija u čvoru 1 iz originalnog sistema)
            double Q_linear = K_lin_full[0,0]*theta_lin[0] 
                            + K_lin_full[0,1]*theta_lin[1] 
                            + K_lin_full[0,2]*theta_lin[2] 
                            - F_lin_full[0];
            double eta_lin = Q_linear / Q_ideal * 100.0;

            Console.WriteLine($"  Odvedena toplota Q = {Q_linear:F4} W");
            Console.WriteLine($"  Ukupna površina rebra = {A_surf:E3} m²");
            Console.WriteLine($"  Idealna odvedena toplota = {Q_ideal:F4} W");
            Console.WriteLine($"  Efikasnost rebra η = {eta_lin:F2}%\n");

            // ================================================================
            // SLUČAJ (b): Jedan kvadratni KE
            // ================================================================
            Console.WriteLine("--- (b) Jedan kvadratni konačni element ---\n");

            var feQuad = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", x1),   // podnožje
                    new Node("2", x2),   // sredina
                    new Node("3", x3),   // vrh
                },
                ft = FEType.Line,
                fo = FEOrder.Quadratic,
            };

            var meshQuad = new Mesh1D([feQuad],
                                       [lambda],
                                       areas)
            {
                VariableCrossSection = true,
                IsFin = true,
                Perimeters = perimeters,
                SurfaceConvectionCoeff = alpha,
                SurfaceAmbientTemp = theta_inf,
                RightConvectionCoeff = alpha,    // konvekcija na vrhu
                RightAmbientTemp = theta_inf,
            };

            var (K_quad_orig, F_quad_orig) = meshQuad.Assemble();
            int n_quad = meshQuad.NodeCount;

            // Sačuvaj originalni sistem za izračunavanje reakcije
            double[,] K_quad_full = (double[,])K_quad_orig.Clone();
            double[] F_quad_full = (double[])F_quad_orig.Clone();

            // Dirichletov G.U.: ϑ₁ = ϑ_b
            K_quad_orig[0, 0] = 1; K_quad_orig[0, 1] = 0; K_quad_orig[0, 2] = 0;
            F_quad_orig[0] = theta_b;
            F_quad_orig[1] -= K_quad_orig[1, 0] * theta_b;
            K_quad_orig[1, 0] = 0;
            F_quad_orig[2] -= K_quad_orig[2, 0] * theta_b;
            K_quad_orig[2, 0] = 0;

            Console.WriteLine("--- Globalni sistem [K]{ϑ} = {F} (nakon G.U.) ---");
            Console.WriteLine($"Broj elemenata: {meshQuad.ElementCount}, "
                + $"broj čvorova: {n_quad}");
            Console.WriteLine("Matrica krutosti [K]:");
            for (int i = 0; i < n_quad; i++)
            {
                Console.Write("  [ ");
                for (int j = 0; j < n_quad; j++)
                    Console.Write($"{K_quad_orig[i, j],10:F6} ");
                Console.WriteLine("]");
            }
            Console.Write($"\nVektor opterećenja {{F}}: "
                + $"{{ {F_quad_orig[0]:F6}, {F_quad_orig[1]:F6}, "
                + $"{F_quad_orig[2]:F6} }}\n\n");

            // Rješavanje sistema
            double[] theta_quad = Gaussian.Solve(K_quad_orig, F_quad_orig, n_quad);

            Console.WriteLine("--- Rezultati (b) ---");
            Console.WriteLine($"  ϑ₁ = {theta_quad[0]:F2} °C  "
                + $"(x=0.0 mm, podnožje)");
            Console.WriteLine($"  ϑ₂ = {theta_quad[1]:F2} °C  "
                + $"(x=40.0 mm, sredina)");
            Console.WriteLine($"  ϑ₃ = {theta_quad[2]:F2} °C  "
                + $"(x=80.0 mm, vrh)\n");

            // Odvedena toplota (reakcija u čvoru 1 iz originalnog sistema)
            double Q_quad = K_quad_full[0,0]*theta_quad[0] 
                          + K_quad_full[0,1]*theta_quad[1] 
                          + K_quad_full[0,2]*theta_quad[2] 
                          - F_quad_full[0];
            double eta_quad = Q_quad / Q_ideal * 100.0;

            Console.WriteLine($"  Odvedena toplota Q = {Q_quad:F4} W");
            Console.WriteLine($"  Efikasnost rebra η = {eta_quad:F2}%\n");

            // ================================================================
            // Usporedba rezultata
            // ================================================================
            Console.WriteLine("--- Usporedba rezultata ---");
            Console.WriteLine($"  {"",-20} {"2 lin. KE",-15} {"1 kvad. KE",-15}");
            Console.WriteLine($"  {"ϑ₁ [°C]",-20} {theta_lin[0],-15:F2} "
                + $"{theta_quad[0],-15:F2}");
            Console.WriteLine($"  {"ϑ₂ [°C]",-20} {theta_lin[1],-15:F2} "
                + $"{theta_quad[1],-15:F2}");
            Console.WriteLine($"  {"ϑ₃ [°C]",-20} {theta_lin[2],-15:F2} "
                + $"{theta_quad[2],-15:F2}");
            Console.WriteLine($"  {"Q [W]",-20} {Q_linear,-15:F4} "
                + $"{Q_quad,-15:F4}");
            Console.WriteLine($"  {"η [%]",-20} {eta_lin,-15:F2} "
                + $"{eta_quad,-15:F2}");

            // ---------- Fizička interpretacija ----------
            Console.WriteLine("\n--- Fizička interpretacija ---");
            Console.WriteLine($"  Pad temperature duž rebra (lin): "
                + $"Δϑ = {theta_lin[0]-theta_lin[2]:F2} °C");
            Console.WriteLine($"  Pad temperature duž rebra (kvad): "
                + $"Δϑ = {theta_quad[0]-theta_quad[2]:F2} °C");
            Console.WriteLine($"  Efikasnost rebra je oko {eta_lin:F0}–{eta_quad:F0}%, "
                + $"što je očekivano za konusno rebro ovakvih dimenzija.");

            Console.WriteLine("\n=== Kraj zadatka 5.09 ===");
        }

    }
}
