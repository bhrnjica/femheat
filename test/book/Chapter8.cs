using NumIntegration;

namespace Book
{
    internal class Chapter8
    {
        // ====================================================================
        // Zadatak 8.02: 1D nestacionarni stap — Mesh1D + theta-metoda
        // Aluminijski stap L=0.1m, A=0.01m2, λ=50, ρcp=6e6
        // 2 linearna KE, Dirichlet 200°C / 20°C, pocetno 20°C
        // Eksplicitni Euler i Crank-Nicolson, dt=5s
        // ====================================================================
        public static void Zadatak_08_02()
        {
            Console.WriteLine("=== Zadatak 8.02: 1D nestacionarni stap ===");
            Console.WriteLine("Eksplicitna Eulerova + Crank-Nicolsonova shema\n");

            // ---------- 1. Fizicki parametri ----------
            double L = 0.1;           // m
            double A = 0.01;          // m2
            double lambda = 50.0;     // W/(m·°C)
            double rho_cp = 6.0e6;    // J/(m3·°C)
            double theta_left = 200.0;
            double theta_right = 20.0;
            double l = L / 2.0;       // duzina elementa = 0.05 m

            // ---------- 2. Mreza: 2 linijska KE, 3 cvora ----------
            var nodes = new Node[]
            {
                new Node("1", 0.00, 0.0),
                new Node("2", 0.05, 0.0),
                new Node("3", 0.10, 0.0),
            };

            var elements = new FiniteElement[]
            {
                new() { nodes = new[] { nodes[0], nodes[1] },
                        ft = FEType.Line, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[1], nodes[2] },
                        ft = FEType.Line, fo = FEOrder.Linear },
            };

            double[] conductivities = { lambda, lambda };
            double[] areas = { A, A };

            var mesh = new Mesh1D(elements, conductivities, areas)
            {
                HeatSource = 0.0,
                LeftHeatFlux = 0.0,
                RightConvectionCoeff = 0.0,
            };

            // ---------- 3. [K] iz Mesh1D ----------
            var (K_static, F_static) = mesh.Assemble();
            int n = mesh.NodeCount;
            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, cvorova: {n}");

            // ---------- 4. Konzistentna matrica kapaciteta [C] ----------
            double[,] C = new double[n, n];
            double Ce_coeff = rho_cp * A * l / 6.0;  // 500
            for (int e = 0; e < elements.Length; e++)
            {
                int i = e;
                int j = e + 1;
                C[i, i] += 2.0 * Ce_coeff;
                C[i, j] += 1.0 * Ce_coeff;
                C[j, i] += 1.0 * Ce_coeff;
                C[j, j] += 2.0 * Ce_coeff;
            }

            // ---------- 5. Dirichlet: cvorovi 1 i 3 ----------
            // Samo cvor 2 (indeks 1) je slobodan
            double K22 = K_static[1, 1];
            double C22 = C[1, 1];
            double R = -(K_static[1, 0] * theta_left
                       + K_static[1, 2] * theta_right);

            // ---------- 6. Vremenska integracija ----------
            double dt = 5.0;
            int nSteps = 30;
            double[] theta_exp = { theta_left, 20.0, theta_right };
            double[] theta_cn  = { theta_left, 20.0, theta_right };

            Console.WriteLine($"  dt = {dt} s,  nSteps = {nSteps}");
            Console.WriteLine("  Stacionarno: theta2(inf) = 110.00 °C\n");

            // --- Eksplicitni Euler (theta=0) ---
            Console.WriteLine("--- Eksplicitni Euler ---");
            Console.WriteLine("   t [s]  |  theta2 [°C]");
            Console.WriteLine("  --------|-------------");
            for (int step = 0; step <= nSteps; step++)
            {
                double t = step * dt;
                if (step <= 3 || step % 10 == 0)
                    Console.WriteLine($"  {t,7:F0}  |    {theta_exp[1],8:F4}");

                double theta2 = theta_exp[1];
                double dtheta2 = (R - K22 * theta2) / C22;
                theta_exp[1] = theta2 + dt * dtheta2;
            }

            // --- Crank-Nicolson (theta=0.5) ---
            double thetaCN = 0.5;
            double Acn = C22 / dt + thetaCN * K22;
            Console.WriteLine("\n--- Crank-Nicolson ---");
            Console.WriteLine("   t [s]  |  theta2 [°C]");
            Console.WriteLine("  --------|-------------");
            for (int step = 0; step <= nSteps; step++)
            {
                double t = step * dt;
                if (step <= 3 || step % 10 == 0)
                    Console.WriteLine($"  {t,7:F0}  |    {theta_cn[1],8:F4}");

                double t_old = theta_cn[1];
                double rhs = (C22 / dt - (1.0 - thetaCN) * K22) * t_old + R;
                theta_cn[1] = rhs / Acn;
            }

            Console.WriteLine($"\n  Eksplicitni (t={nSteps*dt}s): theta2 = {theta_exp[1]:F4} °C");
            Console.WriteLine($"  Crank-Nicolson  (t={nSteps*dt}s): theta2 = {theta_cn[1]:F4} °C");
            Console.WriteLine("  Stacionarno analiticki:  theta2 = 110.00 °C");
            Console.WriteLine("\n=== Kraj zadatka 8.02 ===");
        }

        // ====================================================================
        // Zadatak 8.03: Sistem s koncentrisanim kapacitetom — čelična kugla
        // R=0.02m, λ=50, ρcp=4e6, θ0=500°C, θ∞=25°C, α=30
        // Biotov broj, analiticko rjesenje, eksplicitni Euler
        // ====================================================================
        public static void Zadatak_08_03()
        {
            Console.WriteLine("=== Zadatak 8.03: Koncentrisani kapacitet — kugla ===");
            Console.WriteLine("Biotov broj + analiticko + numericko rjesenje\n");

            // ---------- 1. Parametri ----------
            double R = 0.02;            // m
            double lambda = 50.0;       // W/(m·°C)
            double rho_cp = 4.0e6;      // J/(m3·°C)
            double theta0 = 500.0;      // °C
            double theta_inf = 25.0;    // °C
            double alpha = 30.0;        // W/(m2·°C)

            // ---------- 2. Biotov broj ----------
            double Lc = R / 3.0;        // karakteristicna duzina za kuglu
            double Bi = alpha * Lc / lambda;
            Console.WriteLine($"  Lc = V/A = R/3 = {Lc:F6} m");
            Console.WriteLine($"  Bi = alpha*Lc/lambda = {Bi:F6}");
            Console.WriteLine($"  Bi {(Bi < 0.1 ? "<" : ">")} 0.1 "
                + $"→ model {(Bi < 0.1 ? "JE" : "NIJE")} opravdan\n");

            // ---------- 3. Analiticko rjesenje ----------
            double V = 4.0 / 3.0 * Math.PI * Math.Pow(R, 3);
            double A_surf = 4.0 * Math.PI * R * R;
            double tau = rho_cp * V / (alpha * A_surf);
            Console.WriteLine($"  V = {V:E3} m3,  A = {A_surf:E3} m2");
            Console.WriteLine($"  tau = rho*cp*V/(alpha*A) = {tau:F2} s\n");

            // ---------- 4. Numericko (eksplicitni Euler) ----------
            double dt = 10.0;
            int nSteps = 30;  // 30 × 10 = 300 s
            double theta_num = theta0;

            Console.WriteLine("   t [s]  |  theta_num  |  theta_an  |  greska");
            Console.WriteLine("  --------|-------------|------------|--------");
            for (int step = 0; step <= nSteps; step++)
            {
                double t = step * dt;
                double theta_an = theta_inf
                    + (theta0 - theta_inf) * Math.Exp(-t / tau);

                if (step <= 3 || step % 10 == 0)
                {
                    double err = Math.Abs(theta_num - theta_an);
                    Console.WriteLine(
                        $"  {t,7:F0}  |"
                        + $"    {theta_num,8:F2}  |"
                        + $"  {theta_an,8:F2}  |"
                        + $"  {err,6:F3}");
                }

                // Eksplicitni Euler
                theta_num = theta_num
                    - dt / tau * (theta_num - theta_inf);
            }

            Console.WriteLine("\n=== Kraj zadatka 8.03 ===");
        }

        // ====================================================================
        // Zadatak 8.04: 2D nestacionarna ploca — 8 trouglova, eksplicitni Euler
        // L=0.6m, λ=1.4, ρcp=2e6, θ0=20°C, gornja ivica→100°C
        // Pracenje theta5(t) do stacionarnog stanja
        // ====================================================================
        public static void Zadatak_08_04()
        {
            Console.WriteLine("=== Zadatak 8.04: 2D nestacionarna ploca ===");
            Console.WriteLine("8 trouglova KE, eksplicitni Euler\n");

            // ---------- 1. Parametri ----------
            double lambda = 1.4;       // W/(m·°C)
            double rho_cp = 2.0e6;     // J/(m3·°C)
            double d = 1.0;            // m
            double L = 0.6;            // m
            double h = L / 2.0;        // 0.3 m
            double Ae = h * h / 2.0;   // povrsina trougla = 0.045 m2

            // ---------- 2. Cvorovi (3x3) ----------
            var nodes = new Node[]
            {
                new Node("1", 0.0, 0.0), new Node("2", h, 0.0),   new Node("3", L, 0.0),
                new Node("4", 0.0, h),   new Node("5", h, h),     new Node("6", L, h),
                new Node("7", 0.0, L),   new Node("8", h, L),     new Node("9", L, L),
            };

            // ---------- 3. 8 trouglova KE ----------
            var elements = new FiniteElement[]
            {
                new() { nodes = new[] { nodes[0], nodes[1], nodes[4] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[0], nodes[4], nodes[3] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[1], nodes[2], nodes[5] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[1], nodes[5], nodes[4] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[3], nodes[4], nodes[7] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[3], nodes[7], nodes[6] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[4], nodes[5], nodes[8] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[4], nodes[8], nodes[7] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
            };

            double[] conductivities = Enumerable.Repeat(lambda, 8).ToArray();

            var mesh = new Mesh2D(elements, conductivities)
            {
                Thickness = d,
                HeatSource = 0.0,
            };

            // ---------- 4. [K] ----------
            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;
            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, cvorova: {n}");

            // ---------- 5. Koncentrisana [C] ----------
            double C_lump_per_node = rho_cp * Ae / 3.0 * d;  // 30000
            double[] C_lump = new double[n];
            foreach (var el in elements)
            {
                int i0 = Array.IndexOf(nodes, el.nodes[0]);
                int i1 = Array.IndexOf(nodes, el.nodes[1]);
                int i2 = Array.IndexOf(nodes, el.nodes[2]);
                C_lump[i0] += C_lump_per_node;
                C_lump[i1] += C_lump_per_node;
                C_lump[i2] += C_lump_per_node;
            }

            // ---------- 6. Dirichlet ----------
            // Donja (1,2,3)=20, Lijeva(4)+Desna(6)=20, Gornja(7,8,9)=100
            int[] fixedNodes = { 0, 1, 2, 3, 5, 6, 7, 8 };
            double[] theta = new double[n];
            for (int i = 0; i < n; i++) theta[i] = 20.0;
            theta[6] = theta[7] = theta[8] = 100.0;

            // ---------- 7. Eksplicitni Euler ----------
            double dt = 50.0;
            int nSteps = 100;

            Console.WriteLine($"  dt = {dt} s,  nSteps = {nSteps}");
            Console.WriteLine("  Stacionarno MKE (Zad. 6.1): theta5 = 40.00 °C\n");
            Console.WriteLine("   t [s]   |  theta5 [°C]");
            Console.WriteLine("  ---------|-------------");

            for (int step = 0; step <= nSteps; step++)
            {
                double t = step * dt;
                if (step <= 5 || step % 20 == 0)
                    Console.WriteLine($"  {t,7:F0}  |    {theta[4],8:F3}");

                // Racunaj theta_dot
                double[] theta_dot = new double[n];
                for (int i = 0; i < n; i++)
                {
                    if (fixedNodes.Contains(i)) continue;
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                        sum += K[i, j] * theta[j];
                    theta_dot[i] = -sum / C_lump[i];
                }

                // Azuriraj
                for (int i = 0; i < n; i++)
                {
                    if (fixedNodes.Contains(i)) continue;
                    theta[i] += dt * theta_dot[i];
                }
            }

            Console.WriteLine($"\n  Krajnja temperatura: theta5 = {theta[4]:F2} °C");
            Console.WriteLine("  Stacionarno MKE (Zad. 6.1): theta5 = 40.00 °C");
            Console.WriteLine("\n=== Kraj zadatka 8.04 ===");
        }

        // ====================================================================
        // Zadatak 8.05: Kaljenje celicnog cilindra — osno-simetricno
        // R=0.0125m, H=0.05m, λ=50, ρcp=3.9e6, θ0=850°C
        // Temperaturski zavisan α(θ), eksplicitni Euler, dt=0.1s
        // Pracenje: centar, sredina, povrsina
        // Ref: Hrnjica & Behrem (2022)
        // ====================================================================
        public static void Zadatak_08_05()
        {
            Console.WriteLine("=== Zadatak 8.05: Kaljenje celicnog cilindra ===");
            Console.WriteLine("Osno-simetricna MKE, temperaturski zavisan α\n");

            // ---------- 1. Parametri ----------
            double R = 0.0125;         // m
            double H = 0.050;          // m
            double lambda = 50.0;      // W/(m·°C)
            double rho_cp = 3.9e6;     // J/(m3·°C)
            double theta0 = 850.0;     // °C
            double theta_inf = 25.0;   // °C

            // ---------- 2. Cvorovi (3x3) ----------
            double dr = R / 2.0;       // 0.00625 m
            double dz = H / 2.0;       // 0.025 m
            var nodes = new Node[]
            {
                new Node("1", 0, 0),      new Node("2", dr, 0),     new Node("3", R, 0),
                new Node("4", 0, dz),     new Node("5", dr, dz),    new Node("6", R, dz),
                new Node("7", 0, H),      new Node("8", dr, H),     new Node("9", R, H),
            };

            // ---------- 3. 8 trouglova KE ----------
            var elements = new FiniteElement[]
            {
                new() { nodes = new[] { nodes[0], nodes[1], nodes[4] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[0], nodes[4], nodes[3] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[1], nodes[2], nodes[5] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[1], nodes[5], nodes[4] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[3], nodes[4], nodes[7] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[3], nodes[7], nodes[6] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[4], nodes[5], nodes[8] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[4], nodes[8], nodes[7] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
            };

            double[] conductivities = Enumerable.Repeat(lambda, 8).ToArray();

            var mesh = new Mesh2D(elements, conductivities)
            {
                IsAxisymmetric = true,   // 2πr faktor !!!
                Thickness = 1.0,
                HeatSource = 0.0,
            };

            // ---------- 4. [K] (sa 2πr) ----------
            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;
            Console.WriteLine($"Broj elemenata: {mesh.ElementCount}, cvorova: {n}");

            // ---------- 5. Koncentrisana [C] ----------
            double Ae = dr * dz / 2.0;  // 7.8125e-5 m2
            double C_node = rho_cp * Ae / 3.0;
            double[] C_lump = new double[n];
            foreach (var el in elements)
            {
                int i0 = Array.IndexOf(nodes, el.nodes[0]);
                int i1 = Array.IndexOf(nodes, el.nodes[1]);
                int i2 = Array.IndexOf(nodes, el.nodes[2]);
                C_lump[i0] += C_node;
                C_lump[i1] += C_node;
                C_lump[i2] += C_node;
            }

            // ---------- 6. Temperaturski zavisan α ----------
            static double Alpha(double theta_s)
            {
                if (theta_s > 500.0)
                    return 250.0;
                else if (theta_s > 300.0)
                    return 250.0 + 1750.0 / 200.0 * (500.0 - theta_s);
                else if (theta_s > 100.0)
                    return 2000.0;
                else
                    return 2000.0 - 1200.0 / 75.0 * (100.0 - theta_s);
            }

            // ---------- 7. Pocetni uvjet ----------
            double[] theta = new double[n];
            for (int i = 0; i < n; i++) theta[i] = theta0;

            // ---------- 8. Vremenska petlja ----------
            double dt = 0.1;
            int nSteps = 600;  // 60 s

            // Kljucni cvorovi za pracenje
            int centerNode = 0;   // cvor 1: r=0, z=0
            int midNode = 4;      // cvor 5: r=R/2, z=H/2
            int surfNode = 8;     // cvor 9: r=R, z=H

            Console.WriteLine($"  dt = {dt} s,  nSteps = {nSteps}");
            Console.WriteLine("  Pocetna temperatura: 850 °C\n");
            Console.WriteLine("   t [s] | theta_center | theta_mid | theta_surf");
            Console.WriteLine("  -------|--------------|-----------|----------");

            // Povrsinski cvorovi
            int[] sideNodes = { 2, 5, 8 };  // desna ivica, normala +r
            int[] topNodes  = { 6, 7, 8 };  // gornja ivica, normala +z
            int[] botNodes  = { 0, 1, 2 };  // donja ivica, normala -z

            for (int step = 0; step <= nSteps; step++)
            {
                double t = step * dt;
                if (step % 60 == 0)
                    Console.WriteLine(
                        $"  {t,6:F1} |"
                        + $"    {theta[centerNode],7:F1}   |"
                        + $"   {theta[midNode],6:F1}   |"
                        + $"   {theta[surfNode],6:F1}");

                // --- Racunaj theta_dot (kondukcija) ---
                double[] theta_dot = new double[n];
                for (int i = 0; i < n; i++)
                {
                    double sumK = 0;
                    for (int j = 0; j < n; j++)
                        sumK += K[i, j] * theta[j];
                    theta_dot[i] = -sumK / C_lump[i];
                }

                // --- Konvekcija na povrsinama ---
                // Desna ivica
                foreach (int si in sideNodes)
                {
                    double ri = nodes[si].X;
                    if (ri < 1e-12) continue;
                    double ai = Alpha(theta[si]);
                    double dA = 2.0 * Math.PI * ri * dz;
                    theta_dot[si] -= ai * (theta[si] - theta_inf) * dA / C_lump[si];
                }
                // Gornja ivica
                foreach (int ti in topNodes)
                {
                    double ri = nodes[ti].X;
                    double ai = Alpha(theta[ti]);
                    double dA = 2.0 * Math.PI * ri * dr;
                    theta_dot[ti] -= ai * (theta[ti] - theta_inf) * dA / C_lump[ti];
                }
                // Donja ivica
                foreach (int bi in botNodes)
                {
                    double ri = nodes[bi].X;
                    if (ri < 1e-12) continue;
                    double ai = Alpha(theta[bi]);
                    double dA = 2.0 * Math.PI * ri * dr;
                    theta_dot[bi] -= ai * (theta[bi] - theta_inf) * dA / C_lump[bi];
                }

                // --- Eksplicitni Euler ---
                for (int i = 0; i < n; i++)
                    theta[i] += dt * theta_dot[i];
            }

            Console.WriteLine("\n=== Kraj zadatka 8.05 ===");
        }
    }
}
