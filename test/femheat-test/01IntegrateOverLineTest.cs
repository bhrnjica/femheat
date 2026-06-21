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
            var ni = new Numeric(cd);

            var result1 = ni.Integrate(fun, DOP.Linear);
            Assert.Equal(0.32175, result1, 3);

            var result2 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(0.32175, result2, 3);

            var result3 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(0.32175, result3, 3);

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
            var ni = new Numeric(cd);

            //correct: 0.746824

            var result1 = ni.Integrate(fun, DOP.Linear);
            Assert.Equal(0.746824, result1, 3);

            var result2 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(0.746824, result2, 4);

            var result3 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(0.746824, result3, 5);

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
            var ni = new Numeric(cd);


            //correct: 2.0

            var result1 = ni.Integrate(fun, DOP.Linear);
            Assert.Equal(1.9358195746511364, result1, 3);

            var result2 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(2.0013889136077441, result2, 3);

            var result3 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(2.0, result3, 4);

        }


    }
}