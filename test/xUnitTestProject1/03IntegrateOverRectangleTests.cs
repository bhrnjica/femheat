using GaussianIntegrationApp;
using NumIntegration;
using System;
using Xunit;

namespace xUnitTestProject1
{
    public class IntegrateOverRectangleTest
    {
        [Fact]
        public void Test02()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",-2.0,-1.0,0),
                    new Node("2",2.0,-1.0,0),
                    new Node("3",2.0,3.0,0),
                    new Node("3",-2.0,3.0,0),
                },
                ft= FEType.Rectangle,
                fo= FEOrder.Linear,
            };

            //
            var fun = "x+y";
            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);
            Assert.Equal(result, 16.0, 3) ;

            var result1 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result1, 16.0, 6);

            var result2 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result2, 16.0, 9);
        }

        [Fact]
        public void Test03()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,1.0,0),
                    new Node("2",5.0,1.0,0),
                    new Node("3",5.0,5.0,0),
                    new Node("3",1.0,5.0,0),
                },
                ft = FEType.Rectangle,
                fo = FEOrder.Linear,
            };

            //
            var fun = "x*y";
            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);
            Assert.Equal(result, 144.0, 3);

            var result1 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result1, 144.0, 6);

            var result2 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result2, 144.0, 9);
        }


        [Fact]
        public void Test04()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,1.0,0),
                    new Node("2",5.0,1.0,0),
                    new Node("3",5.0,5.0,0),
                    new Node("3",1.0,5.0,0),
                },
                ft = FEType.Rectangle,
                fo = FEOrder.Linear,
            };

            //correct:0.02597016274668535

            var fun = "Sin(x*y)/(e^(x+y))";
            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);
            Assert.Equal(result, -0.007506469163738325, 2);

            var result1 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result1, 0.022335128362979243, 2);

            var result2 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result2, 0.025970162746, 2);
        }


    }
}