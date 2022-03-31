using NumIntegration;
using System;
using Xunit;

namespace xUnitTestProject1
{
    public class IntegrateOverTetrahedronTest
    {
        [Fact]
        public void Test01()
        {

            //4.69444
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",2.0,3.0,2.0),
                    new Node("2",2.0,1.0,0),
                    new Node("3",3.0,4.0,0),
                    new Node("4",1.0,3.0,0),
                },
                ft = FEType.Tetrahedron,
                fo = FEOrder.Linear,
            };

            //
            var fun = "x*y*z";
            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);
            Assert.Equal(result, 4.69444, 2);

            var result1 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result1, 4.69444, 5);

            var result2 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result2, 4.69444, 5);
        }

        [Fact]
        public void Test02()
        {

            //0.792313
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",2.0,3.0,2.0),
                    new Node("2",2.0,1.0,0),
                    new Node("3",3.0,4.0,0),
                    new Node("4",1.0,3.0,0),
                },
                ft = FEType.Tetrahedron,
                fo = FEOrder.Linear,
            };

            //
            var fun = "Sin(2.0* x ) * Cos(y +z )";
            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);
           
            Assert.Equal(result, 0.74985164, 2);

            Assert.Equal(result1, 0.795894, 5);

            Assert.Equal(result2, 0.792313, 3);
        }

        [Fact]
        public void Test03()
        {

            //0.4406867935
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0,0,1.0),
                    new Node("2",0,0,0),
                    new Node("3",1.0,0,0),
                    new Node("4",0,1.0,0),
                },
                ft = FEType.Tetrahedron,
                fo = FEOrder.Linear,
            };

            //
            var fun = "1.0/(Sqrt((1.0-x-y)*(1.0-x-y) + z*z))";

            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(result, 0.37099833686332434, 2);

            Assert.Equal(result1, 0.4406867935, 1);//****

            Assert.Equal(result2, 0.419673273704, 3);
        }

        [Fact]
        public void Test04()
        {

            //0.1428577142
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0,0,1.0),
                    new Node("2",0,0,0),
                    new Node("3",1.0,0,0),
                    new Node("4",0,1.0,0),
                },
                ft = FEType.Tetrahedron,
                fo = FEOrder.Linear,
            };

            //
            var fun = "Sqrt(x+y+z)";

            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(result, 0.1428577142, 3);

            Assert.Equal(result1, 0.1428577142, 4); 

            Assert.Equal(result2, 0.142852554690499, 3);
        }

    }
}