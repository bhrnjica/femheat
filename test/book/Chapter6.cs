using NumIntegration;

namespace Book
{
    internal class Chapter6
    {
        // ====================================================================
        // Zadatak 6.02: Kvadratna ploča 0.6×0.6 m, λ=1.4 — 8 trouglova
        // MKE rješenje s izoparametarskom formulacijom i Gaussovom integracijom.
        // Poređenje s analitičkim rješenjem (Holman Fourierov red).
        // ====================================================================
        public static void Zadatak_06_02()
        {
            Console.WriteLine("=== Zadatak 6.02: Kvadratna ploča (8 trouglova) ===");
            Console.WriteLine("MKE rješenje korištenjem izoparametarske formulacije\n");

            // ---------- 1. Fizički parametri ----------
            double lambda = 1.4;    // W/(m·°C) — šamotna opeka
            double d = 1.0;         // m — debljina ploče

            // ---------- 2. Čvorovi mreže (3×3, h=0.3 m) ----------
            var nodes = new Node[]
            {
                new Node("1", 0.0, 0.0), new Node("2", 0.3, 0.0), new Node("3", 0.6, 0.0),
                new Node("4", 0.0, 0.3), new Node("5", 0.3, 0.3), new Node("6", 0.6, 0.3),
                new Node("7", 0.0, 0.6), new Node("8", 0.3, 0.6), new Node("9", 0.6, 0.6)
            };

            // ---------- 3. Definicija 8 trouglih KE (CCW) ----------
            var elements = new FiniteElement[]
            {
                new() { nodes = new[] { nodes[0], nodes[1], nodes[4] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },  // e1: 1-2-5
                new() { nodes = new[] { nodes[0], nodes[4], nodes[3] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },  // e2: 1-5-4
                new() { nodes = new[] { nodes[1], nodes[2], nodes[5] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },  // e3: 2-3-6
                new() { nodes = new[] { nodes[1], nodes[5], nodes[4] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },  // e4: 2-6-5
                new() { nodes = new[] { nodes[3], nodes[4], nodes[7] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },  // e5: 4-5-8
                new() { nodes = new[] { nodes[3], nodes[7], nodes[6] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },  // e6: 4-8-7
                new() { nodes = new[] { nodes[4], nodes[5], nodes[8] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },  // e7: 5-6-9
                new() { nodes = new[] { nodes[4], nodes[8], nodes[7] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },  // e8: 5-9-8
            };

            double[] conductivities = Enumerable.Repeat(lambda, 8).ToArray();

            // ---------- 4. Formiranje Mesh2D ----------
            var mesh = new Mesh2D(elements, conductivities)
            {
                Thickness = d,
                HeatSource = 0.0  // G = 0 — nema unutrašnjeg izvora
            };

            // ---------- 5. Sklapanje globalnog sistema ----------
            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;

            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, čvorova: {n}");

            // ---------- 6. Dirichletovi granični uvjeti ----------
            var knownTemps = new Dictionary<int, double>
            {
                {1, 20}, {2, 20}, {3, 20},  // donja ivica
                {4, 20}, {6, 20},            // bočne ivice
                {7, 100}, {8, 100}, {9, 100} // gornja ivica
            };

            foreach (var (nodeId, theta) in knownTemps)
            {
                int k = nodeId - 1;
                for (int i = 0; i < n; i++)
                {
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                    {
                        F[i] -= K[i, k] * theta;
                        K[i, k] = 0;
                    }
                }
                for (int j = 0; j < n; j++)
                    K[k, j] = 0;
                K[k, k] = 1;
                F[k] = theta;
            }

            // ---------- 7. Rješavanje sistema ----------
            double[] thetaVec = Gaussian.Solve(K, F, n);

            Console.WriteLine("\n--- Rezultati ---");
            string[] positions =
            {
                "x=0.0, y=0.0 (donji lijevi)",   // 1
                "x=0.3, y=0.0 (donji srednji)",  // 2
                "x=0.6, y=0.0 (donji desni)",    // 3
                "x=0.0, y=0.3 (lijevi srednji)", // 4
                "x=0.3, y=0.3 (centralni)",      // 5
                "x=0.6, y=0.3 (desni srednji)",  // 6
                "x=0.0, y=0.6 (gornji lijevi)",  // 7
                "x=0.3, y=0.6 (gornji srednji)", // 8
                "x=0.6, y=0.6 (gornji desni)"    // 9
            };
            for (int i = 0; i < n; i++)
                Console.WriteLine($"  ϑ{i + 1} = {thetaVec[i]:F2} °C  ({positions[i]})");

            // ---------- 8. Poređenje sa analitičkim rješenjem ----------
            Console.WriteLine("\n--- Poređenje sa analitičkim rješenjem (Holman) ---");
            Console.WriteLine("  Čvor |   MKE (°C)  |  Analit. (°C) |  Razlika");
            Console.WriteLine("  -----|-------------|---------------|---------");

            static double Analytical(double x, double y, double L = 0.6,
                double Tside = 20, double Ttop = 100, int nTerms = 30)
            {
                double T = Tside;
                for (int m = 0; m < nTerms; m++)
                {
                    int nf = 2 * m + 1;
                    T += (Ttop - Tside) * 4.0 / Math.PI
                       * Math.Sin(nf * Math.PI * x / L)
                       * Math.Sinh(nf * Math.PI * y / L)
                       / (nf * Math.Sinh(nf * Math.PI));
                }
                return T;
            }

            double[] nodeX = { 0, 0.3, 0.6, 0, 0.3, 0.6, 0, 0.3, 0.6 };
            double[] nodeY = { 0, 0, 0, 0.3, 0.3, 0.3, 0.6, 0.6, 0.6 };
            for (int i = 0; i < n; i++)
            {
                double Ta = Analytical(nodeX[i], nodeY[i]);
                double diff = Math.Abs(thetaVec[i] - Ta);
                Console.WriteLine(
                    $"    {i + 1,2}  |"
                    + $"    {thetaVec[i],8:F2}  |"
                    + $"    {Ta,11:F2}  |"
                    + $"  {diff,7:F4}");
            }

            Console.WriteLine("\n=== Kraj zadatka 6.02 ===");
        }

        // ====================================================================
        // Zadatak 6.04: Pravougaona ploča 5×10 cm, λ=200 — 2 četvorougla
        // Mješoviti granični uvjeti: Dirichlet, konvekcija, toplotni tok.
        // Dvije zone s različitim zapreminskim izvorom toplote G.
        // ====================================================================
        public static void Zadatak_06_04()
        {
            Console.WriteLine("=== Zadatak 6.04: Ploča 5×10 cm (2 četvorougla) ===");
            Console.WriteLine("Mješoviti granični uvjeti --- MKE rješenje\n");

            // ---------- 1. Fizički parametri ----------
            double lambda = 200.0;   // W/(m·°C)
            double Lx = 0.05;        // m — širina ploče (5 cm)
            double Ly = 0.10;        // m — visina ploče (10 cm)
            double h = Ly / 2.0;     // m — visina jednog elementa (5 cm)
            double d = 0.01;         // m — debljina ploče (1 cm)
            double G2 = 1.2e6;       // W/m³ — izvor u gornjem elementu
            double q_left = 2.0e4;   // W/m² — toplotni tok na lijevoj ivici
            double alpha = 1.2e4;    // W/(m²·°C) — koef. konvekcije
            double theta_inf = 30.0; // °C — temperatura okoline

            // ---------- 2. Čvorovi mreže (3×2) ----------
            var nodes = new Node[]
            {
                new Node("1", 0.00, 0.00),  // donji lijevi
                new Node("2", 0.05, 0.00),  // donji desni
                new Node("3", 0.00, 0.05),  // srednji lijevi
                new Node("4", 0.05, 0.05),  // srednji desni
                new Node("5", 0.00, 0.10),  // gornji lijevi
                new Node("6", 0.05, 0.10),  // gornji desni
            };

            // ---------- 3. Definicija 2 četvorougla KE (CCW) ----------
            var elements = new FiniteElement[]
            {
                // e1: donji element — čvorovi 1-2-4-3 (CCW)
                new()
                {
                    nodes = new[] { nodes[0], nodes[1], nodes[3], nodes[2] },
                    ft = FEType.Rectangle, fo = FEOrder.Linear
                },
                // e2: gornji element — čvorovi 3-4-6-5 (CCW)
                new()
                {
                    nodes = new[] { nodes[2], nodes[3], nodes[5], nodes[4] },
                    ft = FEType.Rectangle, fo = FEOrder.Linear
                },
            };

            double[] conductivities = { lambda, lambda };

            // ---------- 4. Formiranje Mesh2D ----------
            var mesh = new Mesh2D(elements, conductivities)
            {
                Thickness = d,
                HeatSource = 0.0  // ručno dodajemo G za e2
            };

            // ---------- 5. Granični uvjeti po ivicama ----------
            // Lijeva ivica: toplotni tok q=2e4 W/m² (ULAZI u ploču)
            //   Konvencija Mesh2D: HeatFlux > 0 = toplota napušta domen,
            //   pa se za ulaznu toplotu zadaje negativna vrijednost.
            //   e1 ivica 3 (lokalni 3→0): čvorovi 3→1
            //   e2 ivica 3 (lokalni 3→0): čvorovi 5→3
            // Desna ivica: Dirichlet ϑ=100 °C — ručno
            // Gornja ivica (e2 ivica 2, lokalni 2→3): konvekcija (čvorovi 6→5)
            // Donja ivica: izolirana (q=0) — prirodni uvjet

            mesh.EdgeBCs = new List<EdgeBoundaryCondition>
            {
                new()
                {
                    ElementIndex = 0, EdgeIndex = 3,
                    BCType = BoundaryConditionType.HeatFlux,
                    HeatFlux = -q_left  // negativno = toplota ulazi
                },
                new()
                {
                    ElementIndex = 1, EdgeIndex = 3,
                    BCType = BoundaryConditionType.HeatFlux,
                    HeatFlux = -q_left  // negativno = toplota ulazi
                },
                new()
                {
                    ElementIndex = 1, EdgeIndex = 2,
                    BCType = BoundaryConditionType.Convection,
                    ConvectionCoeff = alpha,
                    AmbientTemp = theta_inf
                },
            };

            // ---------- 6. Sklapanje globalnog sistema ----------
            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;

            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, čvorova: {n}");

            // ---------- 7. Ručno dodavanje izvora toplote za e2 ----------
            // Gauss 2×2: f_G = G·d·(Lx·h/4)·[1 1 1 1]^T
            double fG_val = G2 * d * Lx * h / 4.0;   // = 7.5
            int[] e2_nodes = { 2, 3, 5, 4 };  // 0-based global indeksi: 3,4,6,5
            for (int i = 0; i < 4; i++)
                F[e2_nodes[i]] += fG_val;

            // ---------- 8. Dirichletovi granični uvjeti ----------
            var knownTemps = new Dictionary<int, double>
            {
                {2, 100.0}, {4, 100.0}, {6, 100.0}  // desna ivica
            };

            foreach (var (nodeId, theta) in knownTemps)
            {
                int k = nodeId - 1;
                for (int i = 0; i < n; i++)
                {
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                    {
                        F[i] -= K[i, k] * theta;
                        K[i, k] = 0;
                    }
                }
                for (int j = 0; j < n; j++)
                    K[k, j] = 0;
                K[k, k] = 1;
                F[k] = theta;
            }

            // ---------- 9. Rješavanje sistema ----------
            double[] thetaVec = Gaussian.Solve(K, F, n);

            Console.WriteLine("\n--- Rezultati ---");
            string[] positions =
            {
                "donji lijevi  (x=0.00, y=0.00)",   // 1
                "donji desni   (x=0.05, y=0.00)",   // 2
                "srednji lijevi (x=0.00, y=0.05)",  // 3
                "srednji desni (x=0.05, y=0.05)",   // 4
                "gornji lijevi (x=0.00, y=0.10)",   // 5
                "gornji desni  (x=0.05, y=0.10)",   // 6
            };
            for (int i = 0; i < n; i++)
                Console.WriteLine($"  ϑ{i + 1} = {thetaVec[i]:F1} °C  ({positions[i]})");

            // ---------- 10. Poređenje sa ručnim proračunom ----------
            Console.WriteLine("\n--- Poređenje sa ručnim proračunom (zad. 6.03) ---");
            double[] expected = { 103.7, 100.0, 99.6, 100.0, 40.7, 100.0 };
            Console.WriteLine("  Čvor |  Mesh2D (°C) |  Ručno (°C) |  Razlika");
            Console.WriteLine("  -----|--------------|-------------|---------");
            for (int i = 0; i < n; i++)
            {
                double diff = Math.Abs(thetaVec[i] - expected[i]);
                Console.WriteLine(
                    $"    {i + 1}  |"
                    + $"      {thetaVec[i],6:F1}  |"
                    + $"     {expected[i],6:F1}  |"
                    + $"  {diff,7:F4}");
            }

            Console.WriteLine("\n=== Kraj zadatka 6.04 ===");
        }

        // ====================================================================
        // Zadatak 6.06: Ploča 0.6×0.3 m sa promjenjivom debljinom
        // Linearna promjena: d(x)=0.020–0.010·x/0.6 [m].
        // 2 četvorougla, λ=50, G=0.  Mesh2D s NodeThicknesses.
        // Poređenje s analitičkim rješenjem.
        // ====================================================================
        public static void Zadatak_06_06()
        {
            Console.WriteLine("=== Zadatak 6.06: Ploča sa promjenjivom debljinom ===");
            Console.WriteLine("MKE rješenje sa interpolacijom debljine\n");

            // ---------- 1. Fizički parametri ----------
            double lambda = 50.0;    // W/(m·°C)
            double Lx = 0.6;         // m
            double Ly = 0.3;         // m
            double hx = Lx / 2.0;    // 0.3 m — širina elementa

            // ---------- 2. Čvorovi mreže (2×3) ----------
            var nodes = new Node[]
            {
                new Node("1", 0.0, 0.0),  new Node("2", hx, 0.0), new Node("3", Lx, 0.0),
                new Node("4", 0.0, Ly),   new Node("5", hx, Ly),  new Node("6", Lx, Ly),
            };

            // ---------- 3. Definicija 2 četvorougla KE (CCW) ----------
            var elements = new FiniteElement[]
            {
                // e1: lijevi element — čvorovi 1-2-5-4
                new()
                {
                    nodes = new[] { nodes[0], nodes[1], nodes[4], nodes[3] },
                    ft = FEType.Rectangle, fo = FEOrder.Linear
                },
                // e2: desni element — čvorovi 2-3-6-5
                new()
                {
                    nodes = new[] { nodes[1], nodes[2], nodes[5], nodes[4] },
                    ft = FEType.Rectangle, fo = FEOrder.Linear
                },
            };

            double[] conductivities = { lambda, lambda };

            // ---------- 4. Formiranje Mesh2D sa promjenjivom debljinom ----------
            var mesh = new Mesh2D(elements, conductivities)
            {
                HeatSource = 0.0,  // G = 0
                NodeThicknesses = new Dictionary<int, double>
                {
                    {1, 0.020}, {2, 0.015}, {3, 0.010},  // donji red
                    {4, 0.020}, {5, 0.015}, {6, 0.010},  // gornji red
                }
            };

            // ---------- 5. Sklapanje globalnog sistema ----------
            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;

            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, čvorova: {n}");

            // ---------- 6. Dirichletovi granični uvjeti ----------
            var knownTemps = new Dictionary<int, double>
            {
                {1, 200.0}, {4, 200.0},  // lijeva ivica
                {3, 20.0},  {6, 20.0},   // desna ivica
            };

            foreach (var (nodeId, theta) in knownTemps)
            {
                int k = nodeId - 1;
                for (int i = 0; i < n; i++)
                {
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                    {
                        F[i] -= K[i, k] * theta;
                        K[i, k] = 0;
                    }
                }
                for (int j = 0; j < n; j++)
                    K[k, j] = 0;
                K[k, k] = 1;
                F[k] = theta;
            }

            // ---------- 7. Rješavanje sistema ----------
            double[] thetaVec = Gaussian.Solve(K, F, n);

            Console.WriteLine("\n--- Rezultati ---");
            string[] positions =
            {
                "donji lijevi   (x=0.0, y=0.0)",  // 1
                "donji srednji  (x=0.3, y=0.0)",  // 2
                "donji desni    (x=0.6, y=0.0)",  // 3
                "gornji lijevi  (x=0.0, y=0.3)",  // 4
                "gornji srednji (x=0.3, y=0.3)",  // 5
                "gornji desni   (x=0.6, y=0.3)",  // 6
            };
            for (int i = 0; i < n; i++)
                Console.WriteLine($"  ϑ{i + 1} = {thetaVec[i]:F4} °C  ({positions[i]})");

            // ---------- 8. Poređenje sa analitičkim rješenjem ----------
            Console.WriteLine("\n--- Poređenje sa analitičkim rješenjem ---");
            Console.WriteLine("  Čvor |   MKE (°C)  |  Analit. (°C) |  Razlika");
            Console.WriteLine("  -----|-------------|---------------|---------");

            static double Analytical(double x)
            {
                double C1 = 3.0 / Math.Log(2.0);
                double C2 = 200.0 - 60.0 * C1 * Math.Log(0.020);
                return C2 + 60.0 * C1 * Math.Log(0.020 - x / 60.0);
            }

            double[] nodeX = { 0.0, 0.3, 0.6, 0.0, 0.3, 0.6 };
            for (int i = 0; i < n; i++)
            {
                double Ta = Analytical(nodeX[i]);
                double diff = Math.Abs(thetaVec[i] - Ta);
                Console.WriteLine(
                    $"    {i + 1}  |"
                    + $"     {thetaVec[i],7:F3}  |"
                    + $"     {Ta,9:F3}  |"
                    + $"  {diff,7:F4}");
            }

            Console.WriteLine("\n=== Kraj zadatka 6.06 ===");
        }
    }
}
