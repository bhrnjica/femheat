using GaussianIntegrationApp;
using NumIntegration;
using System;
using Xunit;

namespace xUnitTestProject1
{
    public class IntegrateOverTriangleTest
    {
        [Fact]
        public void Test02()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,4.0,0),
                    new Node("2",4.0,3.0,0),
                    new Node("3",4.0,5.0,0),
                },
                ft= FEType.Triangle,
                fo= FEOrder.Linear,
            };

            //
            var fun = "x*y";
            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);

            Assert.Equal(result, 35.9999, 3) ;
        }

        [Fact]
        public void Test03()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0,0.0,0),
                    new Node("2",1.0,0.0,0),
                    new Node("3",0.0,1.0,0),
                },
                ft = FEType.Triangle,
                fo = FEOrder.Linear,
            };

            //
            var fun = "x*Sqrt(1.0 -y)";


            var ni = new NumericalIntegration(cd);

            var result = ni.Integrate(fun);
            //0.14282513652961193
            //0.142857144893951

            Assert.Equal(result, 0.142857144893951, 3);

            //0.14284844932147606
            //0.142857144893951
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result1, 0.142857144893951, 3);


            //0.14282513652961193
            //0.14285561297943772
            var result2 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result2, 0.142857144893951, 4);
        }

        [Fact]
        public void Test04()
        {
            var cd = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0,0.0,0),
                    new Node("2",Math.Sqrt(3),1,0),
                    new Node("3",0,2,0),
                },
                ft = FEType.Triangle,
                fo = FEOrder.Linear,
            };

            //e^(-1*x^2/2)
            var fun = "e^(x+y)";// "Power(e,x+y)";
            var ni = new NumericalIntegration(cd);
            var result = ni.Integrate(fun);

            Assert.Equal(result, 9.7, 1);

            //9.76313939042275
            //9.762716330962764
            var result1 = ni.Integrate(fun, DOP.Quadratic);
            Assert.Equal(result1, 9.76313939042275, 3);

            //9.76313939042275
            //9.7631365022414958
            var result2 = ni.Integrate(fun, DOP.Cubic);
            Assert.Equal(result2, 9.76313939042275, 5);
        }
    }
}