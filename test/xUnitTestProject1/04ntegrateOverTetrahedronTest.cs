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
            var ni = new Numeric(cd);
            var result = ni.Integrate(fun);
            Assert.Equal(4.69444, result, 2);

            var result1 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(4.69444, result1, 5);

            var result2 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(4.69444, result2, 5);
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
            var ni = new Numeric(cd);
            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);
           
            Assert.Equal(0.74985164, result, 2);

            Assert.Equal(0.795894, result1, 5);

            Assert.Equal(0.792313, result2, 3);
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

            var ni = new Numeric(cd);
            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(0.37099833686332434, result, 2);

            Assert.Equal(0.4406867935, result1, 1);//****

            Assert.Equal(0.419673273704, result2, 3);
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

            var ni = new Numeric(cd);
            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(0.1428577142, result, 3);

            Assert.Equal(0.1428577142, result1, 4); 

            Assert.Equal(0.142852554690499, result2, 3);
        }

    }
}