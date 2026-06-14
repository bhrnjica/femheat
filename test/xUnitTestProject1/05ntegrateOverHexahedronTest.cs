using NumIntegration;
using System;
using Xunit;

namespace xUnitTestProject1
{
    public class IntegrateOverHexahedronTest
    {
        [Fact]
        public void Test01()
        {

            //1063.13
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,2.0,3.0),
                    new Node("2",4.0,2.0,3.0),
                    new Node("3",4.0,5.0,3.0),
                    new Node("4",1.0,5.0,3.0),
                    new Node("5",1.0,2.0,6.0),
                    new Node("6",4.0,2.0,6.0),
                    new Node("7",4.0,5.0,6.0),
                    new Node("8",1.0,5.0,6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Linear,
            };

            //
            var fun = "x*y*z";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(1063.125, result, 3);
            Assert.Equal(1063.125, result1, 5);
            Assert.Equal(1063.125, result2, 7);
        }

        [Fact]
        public void Test02()
        {

            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,2.0,3.0),
                    new Node("2",4.0,2.0,3.0),
                    new Node("3",4.0,5.0,3.0),
                    new Node("4",1.0,5.0,3.0),
                    new Node("5",1.0,2.0,6.0),
                    new Node("6",4.0,2.0,6.0),
                    new Node("7",4.0,5.0,6.0),
                    new Node("8",1.0,5.0,6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Linear,
            };

            //609.118
            var fun = "(x*x+1) * Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(609.1176125581478, result, 2);
            Assert.Equal(609.1176125581478, result1, 3);
            Assert.Equal(609.1176125581478, result2, 5);
        }

        [Fact]
        public void Test03()
        {

            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,2.0,3.0),
                    new Node("2",4.0,2.0,3.0),
                    new Node("3",4.0,5.0,3.0),
                    new Node("4",1.0,5.0,3.0),
                    new Node("5",1.0,2.0,6.0),
                    new Node("6",4.0,2.0,6.0),
                    new Node("7",4.0,5.0,6.0),
                    new Node("8",1.0,5.0,6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Linear,
            };

            //77.06811471928427
            var fun = "(x*x+1) / Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(77.06811471928427, result, 1);
            Assert.Equal(77.06811471928427, result1, 3);
            Assert.Equal(77.06811471928427, result2, 5);
        }

        [Fact]
        public void Test04()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,2.0,3.0),
                    new Node("2",4.0,2.0,3.0),
                    new Node("3",4.0,5.0,3.0),
                    new Node("4",1.0,5.0,3.0),
                    new Node("5",1.0,2.0,6.0),
                    new Node("6",4.0,2.0,6.0),
                    new Node("7",4.0,5.0,6.0),
                    new Node("8",1.0,5.0,6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Linear,
            };

            //35.8118794928527 - correct value
            var fun = "Sin(x*x+1)*Sin(x*x+1) * Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(11.757822939049598, result, 1);
            Assert.Equal(45.04657880769588, result1, 3);
            Assert.Equal(38.727493885388569, result2, 5);
        }

        // ========== Quadratic (20-node serendipity) tests ==========

        [Fact]
        public void Test01_Quadratic()
        {
            // 20-node serendipity hexahedron: 8 corners + 12 mid-edge nodes
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    // corners 1-8
                    new Node("1", 1.0, 2.0, 3.0),
                    new Node("2", 4.0, 2.0, 3.0),
                    new Node("3", 4.0, 5.0, 3.0),
                    new Node("4", 1.0, 5.0, 3.0),
                    new Node("5", 1.0, 2.0, 6.0),
                    new Node("6", 4.0, 2.0, 6.0),
                    new Node("7", 4.0, 5.0, 6.0),
                    new Node("8", 1.0, 5.0, 6.0),
                    // mid-edge nodes 9-20
                    new Node("9",  2.5, 2.0, 3.0), // edge 1-2
                    new Node("10", 4.0, 3.5, 3.0), // edge 2-3
                    new Node("11", 2.5, 5.0, 3.0), // edge 3-4
                    new Node("12", 1.0, 3.5, 3.0), // edge 4-1
                    new Node("13", 1.0, 2.0, 4.5), // edge 1-5
                    new Node("14", 4.0, 2.0, 4.5), // edge 2-6
                    new Node("15", 4.0, 5.0, 4.5), // edge 3-7
                    new Node("16", 1.0, 5.0, 4.5), // edge 4-8
                    new Node("17", 2.5, 2.0, 6.0), // edge 5-6
                    new Node("18", 4.0, 3.5, 6.0), // edge 6-7
                    new Node("19", 2.5, 5.0, 6.0), // edge 7-8
                    new Node("20", 1.0, 3.5, 6.0), // edge 8-5
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Quadratic,
            };

            var fun = "x*y*z";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(1063.125, result, 3);
            Assert.Equal(1063.125, result1, 5);
            Assert.Equal(1063.125, result2, 7);
        }

        [Fact]
        public void Test02_Quadratic()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 1.0, 2.0, 3.0), new Node("2", 4.0, 2.0, 3.0),
                    new Node("3", 4.0, 5.0, 3.0), new Node("4", 1.0, 5.0, 3.0),
                    new Node("5", 1.0, 2.0, 6.0), new Node("6", 4.0, 2.0, 6.0),
                    new Node("7", 4.0, 5.0, 6.0), new Node("8", 1.0, 5.0, 6.0),
                    new Node("9",  2.5, 2.0, 3.0), new Node("10", 4.0, 3.5, 3.0),
                    new Node("11", 2.5, 5.0, 3.0), new Node("12", 1.0, 3.5, 3.0),
                    new Node("13", 1.0, 2.0, 4.5), new Node("14", 4.0, 2.0, 4.5),
                    new Node("15", 4.0, 5.0, 4.5), new Node("16", 1.0, 5.0, 4.5),
                    new Node("17", 2.5, 2.0, 6.0), new Node("18", 4.0, 3.5, 6.0),
                    new Node("19", 2.5, 5.0, 6.0), new Node("20", 1.0, 3.5, 6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Quadratic,
            };

            var fun = "(x*x+1) * Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(609.1176125581478, result, 2);
            Assert.Equal(609.1176125581478, result1, 3);
            Assert.Equal(609.1176125581478, result2, 5);
        }

        [Fact]
        public void Test03_Quadratic()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 1.0, 2.0, 3.0), new Node("2", 4.0, 2.0, 3.0),
                    new Node("3", 4.0, 5.0, 3.0), new Node("4", 1.0, 5.0, 3.0),
                    new Node("5", 1.0, 2.0, 6.0), new Node("6", 4.0, 2.0, 6.0),
                    new Node("7", 4.0, 5.0, 6.0), new Node("8", 1.0, 5.0, 6.0),
                    new Node("9",  2.5, 2.0, 3.0), new Node("10", 4.0, 3.5, 3.0),
                    new Node("11", 2.5, 5.0, 3.0), new Node("12", 1.0, 3.5, 3.0),
                    new Node("13", 1.0, 2.0, 4.5), new Node("14", 4.0, 2.0, 4.5),
                    new Node("15", 4.0, 5.0, 4.5), new Node("16", 1.0, 5.0, 4.5),
                    new Node("17", 2.5, 2.0, 6.0), new Node("18", 4.0, 3.5, 6.0),
                    new Node("19", 2.5, 5.0, 6.0), new Node("20", 1.0, 3.5, 6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Quadratic,
            };

            var fun = "(x*x+1) / Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(77.06811471928427, result, 1);
            Assert.Equal(77.06811471928427, result1, 3);
            Assert.Equal(77.06811471928427, result2, 5);
        }

        [Fact]
        public void Test04_Quadratic()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 1.0, 2.0, 3.0), new Node("2", 4.0, 2.0, 3.0),
                    new Node("3", 4.0, 5.0, 3.0), new Node("4", 1.0, 5.0, 3.0),
                    new Node("5", 1.0, 2.0, 6.0), new Node("6", 4.0, 2.0, 6.0),
                    new Node("7", 4.0, 5.0, 6.0), new Node("8", 1.0, 5.0, 6.0),
                    new Node("9",  2.5, 2.0, 3.0), new Node("10", 4.0, 3.5, 3.0),
                    new Node("11", 2.5, 5.0, 3.0), new Node("12", 1.0, 3.5, 3.0),
                    new Node("13", 1.0, 2.0, 4.5), new Node("14", 4.0, 2.0, 4.5),
                    new Node("15", 4.0, 5.0, 4.5), new Node("16", 1.0, 5.0, 4.5),
                    new Node("17", 2.5, 2.0, 6.0), new Node("18", 4.0, 3.5, 6.0),
                    new Node("19", 2.5, 5.0, 6.0), new Node("20", 1.0, 3.5, 6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Quadratic,
            };

            //35.8118794928527 - correct value
            var fun = "Sin(x*x+1)*Sin(x*x+1) * Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            // Same as linear: quadratic mapping on straight edges = linear mapping
            Assert.Equal(11.757822939049598, result, 1);
            Assert.Equal(45.04657880769588, result1, 3);
            Assert.Equal(38.727493885388569, result2, 5);
        }

        // ========== Cubic (27-node Lagrange) tests ==========

        private static Node[] Nodes27()
        {
            // 27-node Lagrange hexahedron: tensor product 3×3×3
            // Node ordering: (k*9 + j*3 + i + 1),  k=ζ, j=η, i=ξ
            // ζ∈{-1,0,+1}→z∈{3, 4.5, 6}
            // η∈{-1,0,+1}→y∈{2, 3.5, 5}
            // ξ∈{-1,0,+1}→x∈{1, 2.5, 4}
            return new[]
            {
                // ζ=-1 (z=3)
                new Node("1",  1.0, 2.0, 3.0), new Node("2",  2.5, 2.0, 3.0), new Node("3",  4.0, 2.0, 3.0),
                new Node("4",  1.0, 3.5, 3.0), new Node("5",  2.5, 3.5, 3.0), new Node("6",  4.0, 3.5, 3.0),
                new Node("7",  1.0, 5.0, 3.0), new Node("8",  2.5, 5.0, 3.0), new Node("9",  4.0, 5.0, 3.0),
                // ζ=0 (z=4.5)
                new Node("10", 1.0, 2.0, 4.5), new Node("11", 2.5, 2.0, 4.5), new Node("12", 4.0, 2.0, 4.5),
                new Node("13", 1.0, 3.5, 4.5), new Node("14", 2.5, 3.5, 4.5), new Node("15", 4.0, 3.5, 4.5),
                new Node("16", 1.0, 5.0, 4.5), new Node("17", 2.5, 5.0, 4.5), new Node("18", 4.0, 5.0, 4.5),
                // ζ=+1 (z=6)
                new Node("19", 1.0, 2.0, 6.0), new Node("20", 2.5, 2.0, 6.0), new Node("21", 4.0, 2.0, 6.0),
                new Node("22", 1.0, 3.5, 6.0), new Node("23", 2.5, 3.5, 6.0), new Node("24", 4.0, 3.5, 6.0),
                new Node("25", 1.0, 5.0, 6.0), new Node("26", 2.5, 5.0, 6.0), new Node("27", 4.0, 5.0, 6.0),
            };
        }

        [Fact]
        public void Test01_Cubic()
        {
            var cd = new FiniteElement()
            {
                nodes = Nodes27(),
                ft = FEType.Hexaedron,
                fo = FEOrder.Cubic,
            };

            var fun = "x*y*z";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(1063.125, result, 3);
            Assert.Equal(1063.125, result1, 5);
            Assert.Equal(1063.125, result2, 7);
        }

        [Fact]
        public void Test02_Cubic()
        {
            var cd = new FiniteElement()
            {
                nodes = Nodes27(),
                ft = FEType.Hexaedron,
                fo = FEOrder.Cubic,
            };

            var fun = "(x*x+1) * Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(609.1176125581478, result, 2);
            Assert.Equal(609.1176125581478, result1, 3);
            Assert.Equal(609.1176125581478, result2, 5);
        }

        [Fact]
        public void Test03_Cubic()
        {
            var cd = new FiniteElement()
            {
                nodes = Nodes27(),
                ft = FEType.Hexaedron,
                fo = FEOrder.Cubic,
            };

            var fun = "(x*x+1) / Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(77.06811471928427, result, 1);
            Assert.Equal(77.06811471928427, result1, 3);
            Assert.Equal(77.06811471928427, result2, 5);
        }

        [Fact]
        public void Test04_Cubic()
        {
            var cd = new FiniteElement()
            {
                nodes = Nodes27(),
                ft = FEType.Hexaedron,
                fo = FEOrder.Cubic,
            };
            //35.8118794928527 - correct value
            var fun = "Sin(x*x+1)*Sin(x*x+1) * Sqrt(y+z)";
            var ni = new Numeric(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            // Same as linear/quadratic for rectilinear geometry with mid-nodes at midpoints
            Assert.Equal(11.757822939049598, result, 1);
            Assert.Equal(45.04657880769588, result1, 3);
            Assert.Equal(38.727493885388569, result2, 5);
        }

        // ========== Mesh refinement test ==========

        [Fact]
        public void Test04_MeshRefined()
        {
            // True analytical value: 35.8118794928527
            // Function sin²(x²+1)·√(y+z) oscillates in x → we refine mesh in x-direction
            var fun = "Sin(x*x+1)*Sin(x*x+1) * Sqrt(y+z)";
            double trueValue = 35.8118794928527;

            // Single element mesh: huge error (~8%)
            var mesh1 = UniformMesh3D.CreateUniformHexMesh(
                xMin: 1.0, xMax: 4.0, nx: 1,
                yMin: 2.0, yMax: 5.0, ny: 1,
                zMin: 3.0, zMax: 6.0, nz: 1);
            double singleResult = mesh1.Integrate(fun, DOP.Cubic);
            double singleError = Math.Abs(singleResult - trueValue);
            Assert.True(singleError > 0.1, $"Single element error {singleError:E2} should be large");

            // nx=6 with DOP.Cubic → error ≈ 1.3e-2 (< 2e-2)  ✔ e-2 accuracy
            var mesh6 = UniformMesh3D.CreateUniformHexMesh(
                xMin: 1.0, xMax: 4.0, nx: 6,
                yMin: 2.0, yMax: 5.0, ny: 1,
                zMin: 3.0, zMax: 6.0, nz: 1);
            double result6 = mesh6.Integrate(fun, DOP.Cubic);
            double error6 = Math.Abs(result6 - trueValue);
            Assert.True(error6 < 0.02, $"nx=6 error {error6:E2} exceeds 2e-2");

            // nx=8 with DOP.Cubic → error ≈ 3.1e-3 (< 5e-3)
            var mesh8 = UniformMesh3D.CreateUniformHexMesh(
                xMin: 1.0, xMax: 4.0, nx: 8,
                yMin: 2.0, yMax: 5.0, ny: 1,
                zMin: 3.0, zMax: 6.0, nz: 1);
            double result8 = mesh8.Integrate(fun, DOP.Cubic);
            double error8 = Math.Abs(result8 - trueValue);
            Assert.True(error8 < 0.005, $"nx=8 error {error8:E2} exceeds 5e-3");

            // nx=10 with DOP.Cubic → error ≈ 3.4e-4 (< 1e-3)
            var mesh10 = UniformMesh3D.CreateUniformHexMesh(
                xMin: 1.0, xMax: 4.0, nx: 10,
                yMin: 2.0, yMax: 5.0, ny: 1,
                zMin: 3.0, zMax: 6.0, nz: 1);
            double result10 = mesh10.Integrate(fun, DOP.Cubic);
            double error10 = Math.Abs(result10 - trueValue);
            Assert.True(error10 < 0.001, $"nx=10 error {error10:E2} exceeds 1e-3");
        }


    }
}