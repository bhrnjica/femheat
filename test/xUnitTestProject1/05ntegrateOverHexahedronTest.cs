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


    }
}