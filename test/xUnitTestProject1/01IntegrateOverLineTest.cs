using NumIntegration;
using System;
using Xunit;

namespace xUnitTestProject1
{
    public class IntegrateOverLineTest
    {
        [Fact]
        public void Test02()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,0,0),
                    new Node("2",2.0,0,0),
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            //
            var fun = "1/(1+x^2)";
            var ni = new NumericalIntegration(cd);

            var result1 = ni.Integrate(fun, DOP.Linear);
            Assert.Equal(result1, 0.32175, 3);

            var result2 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result2, 0.32175, 3);

            var result3 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result3, 0.32175, 3);

        }

        [Fact]
        public void Test03()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0,0,0),
                    new Node("2",1.0,0,0),
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            //this will wont work: var fun = "e^(-x^2)";
            //skip minus on power
            //
            var fun = "1.0/e^(x^2)";
            var ni = new NumericalIntegration(cd);

            //correct: 0.746824

            var result1 = ni.Integrate(fun, DOP.Linear);
            Assert.Equal(result1, 0.746824, 3);

            var result2 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result2, 0.746824, 4);

            var result3 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result3, 0.746824, 5);

        }

        [Fact]
        public void Test04()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0,0,0),
                    new Node("2",Math.PI,0,0),
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };

            //
            var fun = "Sin(x)";
            var ni = new NumericalIntegration(cd);


            //correct: 2.0

            var result1 = ni.Integrate(fun, DOP.Linear);
            Assert.Equal(result1, 1.9358195746511364, 3);

            var result2 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result2, 2.0013889136077441, 3);

            var result3 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result3, 2.0, 4);

        }


    }
}