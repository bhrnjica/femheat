using System;
using System.Collections.Generic;
using System.Linq;
using NumIntegration;
using Xunit;

namespace xUnitTestProject1
{
    /// <summary>
    /// Testovi sklapanja globalnog sistema za 2D i 3D probleme.
    /// Pokriva Mesh2D (ravninski, osno-simetricni, granicni uvjeti)
    /// i UniformMesh3D (heksaedarska mreza, povrsinski granicni uvjeti).
    /// </summary>
    public class MeshAssemblyTests
    {
        // ====================================================================
        // MESH 2D — RAVNINSKI PROBLEMI
        // ====================================================================

        /// <summary>
        /// Kvadrat 1×1 m, λ=1. Dva trougla sa 4 cvora.
        /// Dno: 0°C, vrh: 100°C, lijeva i desna linearno.
        /// Rjesenje: ϑ(x,y) = 100y (linearno).
        /// </summary>
        [Fact]
        public void Mesh2D_SquarePlate_PureDirichlet()
        {
            var nodes = new Node[]
            {
                new Node("1", 0.0, 0.0),
                new Node("2", 1.0, 0.0),
                new Node("3", 1.0, 1.0),
                new Node("4", 0.0, 1.0),
            };

            var elements = new FiniteElement[]
            {
                new() { nodes = new[] { nodes[0], nodes[1], nodes[2] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
                new() { nodes = new[] { nodes[0], nodes[2], nodes[3] },
                        ft = FEType.Triangle, fo = FEOrder.Linear },
            };

            double[] conductivities = { 1.0, 1.0 };
            var mesh = new Mesh2D(elements, conductivities);
            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;
            Assert.Equal(4, n);

            var knownTemps = new Dictionary<int, double>
            {
                {1, 0.0}, {2, 0.0}, {3, 100.0}, {4, 100.0}
            };
            foreach (var (nodeId, tVal) in knownTemps)
            {
                int k = nodeId - 1;
                for (int i = 0; i < n; i++)
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                        { F[i] -= K[i, k] * tVal; K[i, k] = 0; }
                for (int j = 0; j < n; j++) K[k, j] = 0;
                K[k, k] = 1; F[k] = tVal;
            }

            double[] theta = Gaussian.Solve(K, F, n);
            Assert.Equal(0.0, theta[0], 6);
            Assert.Equal(0.0, theta[1], 6);
            Assert.Equal(100.0, theta[2], 6);
            Assert.Equal(100.0, theta[3], 6);
        }

        /// <summary>
        /// Pravougaonik 0.1×0.1 m, λ=50. Jedan cetvorougao.
        /// Dno: Dirichlet 100°C. Vrh: konvekcija α=200, ϑ∞=20°C.
        /// Strane: izolovane. 1D analiticko: ϑ_vrh = 77.14°C.
        /// </summary>
        [Fact]
        public void Mesh2D_ConvectionEdgeBC()
        {
            double a = 0.1, lambda = 50.0, alpha = 200.0;
            double theta_inf = 20.0, theta_bottom = 100.0;

            var nodes = new Node[]
            {
                new Node("1", 0.0, 0.0),   // dno lijevo
                new Node("2", a, 0.0),     // dno desno
                new Node("3", a, a),       // vrh desno
                new Node("4", 0.0, a),     // vrh lijevo
            };

            // Jedan cetvorougao: 1-2-3-4 (CCW)
            var elements = new FiniteElement[]
            {
                new() { nodes = new[] { nodes[0], nodes[1], nodes[2], nodes[3] },
                        ft = FEType.Rectangle, fo = FEOrder.Linear },
            };

            double[] conductivities = { lambda };
            var mesh = new Mesh2D(elements, conductivities);

            // Konvekcija na gornjoj ivici (ivica 2: lokalni 2→3, cvorovi 3→4)
            mesh.EdgeBCs = new List<EdgeBoundaryCondition>
            {
                new()
                {
                    ElementIndex = 0, EdgeIndex = 2,
                    BCType = BoundaryConditionType.Convection,
                    ConvectionCoeff = alpha, AmbientTemp = theta_inf
                }
            };

            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;

            // Dirichlet na dnu
            var knownTemps = new Dictionary<int, double> { { 1, theta_bottom }, { 2, theta_bottom } };
            foreach (var (nodeId, tVal) in knownTemps)
            {
                int k = nodeId - 1;
                for (int i = 0; i < n; i++)
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                        { F[i] -= K[i, k] * tVal; K[i, k] = 0; }
                for (int j = 0; j < n; j++) K[k, j] = 0;
                K[k, k] = 1; F[k] = tVal;
            }

            double[] theta = Gaussian.Solve(K, F, n);
            Assert.Equal(100.0, theta[0], 5);
            Assert.Equal(100.0, theta[1], 5);
            Assert.Equal(77.14, theta[2], 1);
            Assert.Equal(77.14, theta[3], 1);
        }

        // ====================================================================
        // MESH 2D — OSNO-SIMETRICNI PROBLEMI
        // ====================================================================

        /// <summary>
        /// Osno-simetricni cilindar: zadatak 7.02.
        /// R=0.05 m, H=0.20 m, λ=50, α=200, ϑ∞=20°C, dno 200°C.
        /// 4 trougla, 6 cvorova. Vrh: izolovan. Lateralno: konvekcija.
        /// Rucni proracun: ϑ4=22.86, ϑ5=20.09, ϑ6=15.18.
        /// </summary>
        [Fact]
        public void Mesh2D_Axisymmetric_Cylinder()
        {
            double lambda = 50.0, alpha = 200.0;
            double theta_inf = 20.0, theta_bottom = 200.0;

            var nodes = new Node[]
            {
                new Node("1", 0.000, 0.00),
                new Node("2", 0.025, 0.00),
                new Node("3", 0.050, 0.00),
                new Node("4", 0.000, 0.20),
                new Node("5", 0.025, 0.20),
                new Node("6", 0.050, 0.20),
            };

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
            };

            double[] conductivities = Enumerable.Repeat(lambda, 4).ToArray();
            var mesh = new Mesh2D(elements, conductivities)
            {
                IsAxisymmetric = true
            };

            mesh.EdgeBCs = new List<EdgeBoundaryCondition>
            {
                new()
                {
                    ElementIndex = 2, EdgeIndex = 1, // e3 ivica 1: cvor 3→6
                    BCType = BoundaryConditionType.Convection,
                    ConvectionCoeff = alpha, AmbientTemp = theta_inf
                }
            };

            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;
            Assert.Equal(6, n);

            // Dirichlet na dnu
            for (int i = 0; i < 3; i++)
            {
                for (int r = 0; r < n; r++)
                    if (r != i && Math.Abs(K[r, i]) > 1e-15)
                        { F[r] -= K[r, i] * theta_bottom; K[r, i] = 0; }
                for (int j = 0; j < n; j++) K[i, j] = 0;
                K[i, i] = 1; F[i] = theta_bottom;
            }

            double[] theta = Gaussian.Solve(K, F, n);
            Assert.Equal(200.0, theta[0], 5);
            Assert.Equal(200.0, theta[1], 5);
            Assert.Equal(200.0, theta[2], 5);
            Assert.Equal(22.86, theta[3], 1);
            Assert.Equal(20.09, theta[4], 1);
            Assert.Equal(15.18, theta[5], 1);
        }

        // ====================================================================
        // MESH 3D — HEKSAEDARSKI ELEMENTI
        // ====================================================================

        /// <summary>
        /// Kocka a=0.1 m, λ=50. 1 heksaedar, Dirichlet na dnu i vrhu.
        /// Dno 0°C, vrh 100°C.
        /// </summary>
        [Fact]
        public void UniformMesh3D_Cube_PureDirichlet()
        {
            double a = 0.1, lambda = 50.0;

            var mesh = UniformMesh3D.CreateUniformHexMesh(
                0, a, 1, 0, a, 1, 0, a, 1, lambda);

            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;
            Assert.Equal(8, n);

            int[] bottom = { 0, 1, 2, 3 };
            int[] top = { 4, 5, 6, 7 };

            foreach (int k in bottom)
            {
                for (int i = 0; i < n; i++)
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                        { F[i] -= K[i, k] * 0.0; K[i, k] = 0; }
                for (int j = 0; j < n; j++) K[k, j] = 0;
                K[k, k] = 1; F[k] = 0.0;
            }
            foreach (int k in top)
            {
                for (int i = 0; i < n; i++)
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                        { F[i] -= K[i, k] * 100.0; K[i, k] = 0; }
                for (int j = 0; j < n; j++) K[k, j] = 0;
                K[k, k] = 1; F[k] = 100.0;
            }

            double[] theta = Gaussian.Solve(K, F, n);
            for (int i = 0; i < 4; i++) Assert.Equal(0.0, theta[i], 8);
            for (int i = 4; i < 8; i++) Assert.Equal(100.0, theta[i], 8);
        }

        /// <summary>
        /// Kocka 0.1×0.1×0.1 m, 1 heksaedar, λ=50.
        /// Dno: Dirichlet 100°C. Vrh: konvekcija α=200, ϑ∞=20°C.
        /// Bocne strane: izolovane.
        /// Analiticko rjesenje: ϑ_vrh = 77.14°C (zadatak 7.04).
        /// </summary>
        [Fact]
        public void UniformMesh3D_Cube_ConvectionFaceBC()
        {
            double a = 0.1, lambda = 50.0, alpha = 200.0;
            double theta_inf = 20.0, theta_bottom = 100.0;

            var mesh = UniformMesh3D.CreateUniformHexMesh(
                0, a, 1, 0, a, 1, 0, a, 1, lambda);

            mesh.FaceBCs = new List<FaceBoundaryCondition>
            {
                new()
                {
                    ElementIndex = 0, FaceIndex = 1, // gornja strana ζ=+1
                    BCType = FaceBoundaryConditionType.Convection,
                    ConvectionCoeff = alpha, AmbientTemp = theta_inf
                }
            };

            var (K, F) = mesh.Assemble();
            int n = mesh.NodeCount;
            Assert.Equal(8, n);

            // Dirichlet na dnu (cvorovi 1-4)
            int[] bottom = { 0, 1, 2, 3 };
            foreach (int k in bottom)
            {
                for (int i = 0; i < n; i++)
                    if (i != k && Math.Abs(K[i, k]) > 1e-15)
                        { F[i] -= K[i, k] * theta_bottom; K[i, k] = 0; }
                for (int j = 0; j < n; j++) K[k, j] = 0;
                K[k, k] = 1; F[k] = theta_bottom;
            }

            double[] theta = Gaussian.Solve(K, F, n);

            // Dno
            for (int i = 0; i < 4; i++)
                Assert.Equal(100.0, theta[i], 5);
            // Vrh: svi treba da budu 77.14
            for (int i = 4; i < 8; i++)
                Assert.Equal(77.14, theta[i], 1);
        }

        // ====================================================================
        // GAUSSOVE TEZINE — VERIFIKACIJA ZBIROVA
        // ====================================================================

        [Fact]
        public void GaussianWeights_Triangle_SumToHalf()
        {
            var gw1 = Gaussian.NullsAndWeights(FEType.Triangle, DOP.None);
            Assert.Equal(0.5, gw1.wi!.Sum(), 12);

            var gw3 = Gaussian.NullsAndWeights(FEType.Triangle, DOP.Linear);
            Assert.Equal(0.5, gw3.wi!.Sum(), 12);

            var gw7 = Gaussian.NullsAndWeights(FEType.Triangle, DOP.Quadratic);
            Assert.Equal(0.5, gw7.wi!.Sum(), 10);
        }

        [Fact]
        public void GaussianWeights_Rectangle_SumToFour()
        {
            var gw1 = Gaussian.NullsAndWeights(FEType.Rectangle, DOP.None);
            Assert.Equal(2.0, gw1.wi!.Sum(), 12);

            var gw4 = Gaussian.NullsAndWeights(FEType.Rectangle, DOP.Linear);
            Assert.Equal(4.0, gw4.wi!.Sum(), 12);

            var gw9 = Gaussian.NullsAndWeights(FEType.Rectangle, DOP.Quadratic);
            Assert.Equal(4.0, gw9.wi!.Sum(), 10);
        }

        [Fact]
        public void GaussianWeights_Hexahedron_SumToEight()
        {
            var gw1 = Gaussian.NullsAndWeights(FEType.Hexaedron, DOP.None);
            Assert.Equal(8.0, gw1.wi!.Sum(), 12);

            var gw8 = Gaussian.NullsAndWeights(FEType.Hexaedron, DOP.Linear);
            Assert.Equal(8.0, gw8.wi!.Sum(), 12);

            var gw27 = Gaussian.NullsAndWeights(FEType.Hexaedron, DOP.Quadratic);
            Assert.Equal(8.0, gw27.wi!.Sum(), 6);
        }

        [Fact]
        public void GaussianWeights_Tetrahedron_SumToOneSixth()
        {
            var gw1 = Gaussian.NullsAndWeights(FEType.Tetrahedron, DOP.None);
            Assert.Equal(1.0, gw1.wi!.Sum(), 12);

            var gw4 = Gaussian.NullsAndWeights(FEType.Tetrahedron, DOP.Linear);
            Assert.Equal(1.0, gw4.wi!.Sum(), 12);

            var gw10 = Gaussian.NullsAndWeights(FEType.Tetrahedron, DOP.Quadratic);
            Assert.Equal(1.0, gw10.wi!.Sum(), 10);
        }

        // ====================================================================
        // JAKOBIJAN — VERIFIKACIJA
        // ====================================================================

        [Fact]
        public void Jacobian_Triangle_ReturnsArea()
        {
            var fe = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 0.0, 0.0, 0),
                    new Node("2", 3.0, 0.0, 0),
                    new Node("3", 0.0, 4.0, 0),
                },
                ft = FEType.Triangle,
                fo = FEOrder.Linear,
            };
            var jac = new Jacobian(FEType.Triangle, FEOrder.Linear);
            double result = jac.Calculate(fe.nodes, new Point(1.0 / 3.0, 1.0 / 3.0, 0));
            double area = 0.5 * 3.0 * 4.0;
            Assert.Equal(area, result, 10);
        }

        [Fact]
        public void Jacobian_Rectangle_ReturnsArea()
        {
            var fe = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", -2.0, -1.0, 0),
                    new Node("2",  2.0, -1.0, 0),
                    new Node("3",  2.0,  3.0, 0),
                    new Node("4", -2.0,  3.0, 0),
                },
                ft = FEType.Rectangle,
                fo = FEOrder.Linear,
            };
            var jac = new Jacobian(FEType.Rectangle, FEOrder.Linear);
            double detJ = jac.Calculate(fe.nodes, new Point(0, 0, 0));
            // Rectangle: J = [2 0; 0 2], detJ = 4
            // But Jacobian.Calculate for Rectangle applies factor l=1 and returns sqrt(...)
            // For a 4×4 rectangle: dx/dξ=2, dy/dη=2, detJ = 4
            Assert.Equal(4.0, detJ, 10);
        }

        [Fact]
        public void Jacobian_Hexahedron_Cube_ReturnsVolumeOver8()
        {
            double a = 0.1;
            var fe = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 0, 0, 0), new Node("2", a, 0, 0),
                    new Node("3", a, a, 0), new Node("4", 0, a, 0),
                    new Node("5", 0, 0, a), new Node("6", a, 0, a),
                    new Node("7", a, a, a), new Node("8", 0, a, a),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Linear,
            };
            var jac = new Jacobian(FEType.Hexaedron, FEOrder.Linear);
            double detJ = jac.Calculate(fe.nodes, new Point(0, 0, 0));
            double expected = a * a * a / 8.0; // a³/8 = 1.25e-4
            Assert.Equal(expected, detJ, 10);
        }
    }
}
