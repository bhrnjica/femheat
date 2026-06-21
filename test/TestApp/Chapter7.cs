using NumIntegration;

namespace Book
{
    internal class Chapter7
    {
        // ====================================================================
        // Zadatak 7.02: Osno-simetricni cilindar R=0.05 m, H=0.20 m
        // Celik λ=50 W/(m·°C), konvekcija na lateralnoj povrsini
        // MKE rjesenje s 4 linearna trougla i izoparametarskom formulacijom.
        // Koristi Mesh2D sa IsAxisymmetric = true za automatski 2πr faktor.
        // ====================================================================
        public static void Zadatak_07_02()
        {
            Console.WriteLine("=== Zadatak 7.02: Osno-simetricni cilindar ===");
            Console.WriteLine("MKE rjesenje s 4 trougla (izoparametarski)\n");

            // ---------- 1. Fizicki parametri ----------
            double lambda = 50.0;       // W/(m·°C) — celik
            double alpha = 200.0;       // W/(m²·°C) — koef. konvekcije
            double theta_inf = 20.0;    // °C — temperatura okoline
            double theta_bottom = 200.0; // °C — temperatura dna

            // ---------- 2. Cvorovi mreze (3×2 u r-z ravni) ----------
            var nodes = new Node[]
            {
                new Node("1", 0.000, 0.00),  // dno, osa simetrije
                new Node("2", 0.025, 0.00),  // dno, sredina
                new Node("3", 0.050, 0.00),  // dno, lateralna povrsina
                new Node("4", 0.000, 0.20),  // vrh, osa simetrije
                new Node("5", 0.025, 0.20),  // vrh, sredina
                new Node("6", 0.050, 0.20),  // vrh, lateralna povrsina
            };

            // ---------- 3. Definicija 4 trougla KE (CCW orijentacija) ----------
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
            };

            double[] conductivities = Enumerable.Repeat(lambda, elements.Length).ToArray();

            // ---------- 4. Formiranje Mesh2D (osno-simetricno) ----------
            var mesh = new Mesh2D(elements, conductivities)
            {
                IsAxisymmetric = true,   // ukljucuje faktor 2πr u sve integrale
                HeatSource = 0.0
            };

            // Robinov G.U. na lateralnoj povrsini
            // e3 (indeks 2), ivica 1: cvorovi 3(r=R,z=0) → 6(r=R,z=H)
            mesh.EdgeBCs = new List<EdgeBoundaryCondition>
            {
                new()
                {
                    ElementIndex = 2,     // e3 (0-based)
                    EdgeIndex = 1,        // ivica 1 trougla: lokalni 1→2
                    BCType = BoundaryConditionType.Convection,
                    ConvectionCoeff = alpha,
                    AmbientTemp = theta_inf
                }
            };

            // ---------- 5. Sklapanje globalnog sistema ----------
            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;

            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, cvorova: {n}");

            // ---------- 6. Dirichletovi G.U. na dnu ----------
            // Poznate temperature: ϑ₁ = ϑ₂ = ϑ₃ = 200 °C
            // Eliminacija poznatih temperatura iz sistema
            var knownTemps = new Dictionary<int, double>
            {
                {1, theta_bottom},
                {2, theta_bottom},
                {3, theta_bottom}
            };

            foreach (var (nodeId, thetaVal) in knownTemps)
            {
                int k = nodeId - 1;
                // Prilagodi desnu stranu za preostale redove
                for (int i = 0; i < n; i++)
                {
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                    {
                        F[i] -= K[i, k] * thetaVal;
                        K[i, k] = 0.0;
                    }
                }
                // Postavi Dirichletov red
                for (int j = 0; j < n; j++)
                    K[k, j] = 0.0;
                K[k, k] = 1.0;
                F[k] = thetaVal;
            }

            // ---------- 7. Rjesavanje sistema ----------
            double[] theta = Gaussian.Solve(K, F, n);

            Console.WriteLine("\n--- Rjesenje ---");
            for (int i = 0; i < n; i++)
                Console.WriteLine($"  ϑ{i + 1} = {theta[i]:F2} °C");

            Console.WriteLine("\n=== Kraj zadatka 7.02 ===");
        }

        // ====================================================================
        // Zadatak 7.04: 3D kocka s mjesovitim granicnim uvjetima
        // Celicna kocka a=0.1 m, λ=50 W/(m·°C)
        // Dno: Dirichlet ϑ=100°C, vrh: konvekcija α=200, ϑ∞=20°C
        // Bocne strane: izolovane (q=0)
        // MKE rjesenje s 1 heksaedrom, koristi UniformMesh3D.Assemble().
        // ====================================================================
        public static void Zadatak_07_04()
        {
            Console.WriteLine("=== Zadatak 7.04: 3D kocka s mjesovitim G.U. ===");
            Console.WriteLine("MKE rjesenje s 1 heksaedrom (8 cvorova)\n");

            // ---------- 1. Fizicki parametri ----------
            double a = 0.1;             // m — stranica kocke
            double lambda = 50.0;       // W/(m·°C) — celik
            double alpha = 200.0;       // W/(m²·°C) — koef. konvekcije
            double theta_inf = 20.0;    // °C — temperatura okoline
            double theta_bottom = 100.0; // °C — temperatura dna

            // ---------- 2. Uniformna heksaedarska mreza 1×1×1 ----------
            var mesh3D = UniformMesh3D.CreateUniformHexMesh(
                0.0, a, 1,   // x: [0, a]
                0.0, a, 1,   // y: [0, a]
                0.0, a, 1,   // z: [0, a]
                lambda);      // λ za sve elemente

            // Robinov G.U. na gornjoj strani (ζ=+1, face 1)
            mesh3D.FaceBCs = new List<FaceBoundaryCondition>
            {
                new()
                {
                    ElementIndex = 0,
                    FaceIndex = 1,       // gornja strana (ζ=+1)
                    BCType = FaceBoundaryConditionType.Convection,
                    ConvectionCoeff = alpha,
                    AmbientTemp = theta_inf
                }
            };

            // ---------- 3. Sklapanje globalnog sistema ----------
            var (K, F) = mesh3D.Assemble();
            int n = mesh3D.NodeCount;

            Console.WriteLine($"Broj elemenata: {mesh3D.ElementsCount}, cvorova: {n}");

            // ---------- 4. Dirichletovi G.U. na dnu ----------
            int[] bottomNodes = { 0, 1, 2, 3 }; // cvorovi 1-4 (0-based)
            foreach (int k in bottomNodes)
            {
                for (int i = 0; i < n; i++)
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                        { F[i] -= K[i, k] * theta_bottom; K[i, k] = 0; }
                for (int j = 0; j < n; j++) K[k, j] = 0;
                K[k, k] = 1; F[k] = theta_bottom;
            }

            // ---------- 5. Rjesavanje ----------
            double[] theta = Gaussian.Solve(K, F, n);

            Console.WriteLine("\n--- Rjesenje ---");
            for (int i = 0; i < n; i++)
                Console.WriteLine($"  ϑ{i + 1} = {theta[i]:F2} °C");

            Console.WriteLine("\n=== Kraj zadatka 7.04 ===");
        }
    }
}
