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
            var ni = new NumericalIntegration(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(result, 1063.125, 3);
            Assert.Equal(result1, 1063.125, 5);
            Assert.Equal(result2, 1063.125, 7);
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
            var ni = new NumericalIntegration(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(result, 609.1176125581478, 2);
            Assert.Equal(result1, 609.1176125581478, 3);
            Assert.Equal(result2, 609.1176125581478, 5);
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
            var ni = new NumericalIntegration(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(result, 77.06811471928427, 1);
            Assert.Equal(result1, 77.06811471928427, 3);
            Assert.Equal(result2, 77.06811471928427, 5);
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
            var ni = new NumericalIntegration(cd);

            var result = ni.Integrate(fun);
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            var result2 = ni.Integrate(fun, DOP.Cubic);

            Assert.Equal(result, 11.757822939049598, 1);
            Assert.Equal(result1, 45.04657880769588, 3);
            Assert.Equal(result2, 38.727493885388569, 5);
        }


    }
}