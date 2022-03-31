using NumIntegration;
using System;
using Xunit;

namespace xUnitTestProject1
{
    public class BookExamples
    {
        [Fact]
        public void Chapter03()
        {
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0),
                    new Node("2",2.0),
                    new Node("3",1.0),
                },
                ft = FEType.Line,
                fo = FEOrder.Quadratic,
            };
            var fe2 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0),
                    new Node("2",2.0),
                    new Node("3",2.0/3.0),
                },
                ft = FEType.Line,
                fo = FEOrder.Quadratic,
            };

            //create coordinate and jacobian object
            var coor = new Coordinates(FEType.Line, FEOrder.Quadratic);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Quadratic);

            //evaluacijske tačke
            var xi1 = new Point(-0.75);
            var xi2 = new Point(0.55);

            //evaluacija koordinata
            var xa1 = coor.Transform(fe1.nodes,xi1);
            var xa2 = coor.Transform(fe1.nodes, xi2);
            //evaluacija Jacobiana
            var ja1 = jacobian.Calculate(fe1.nodes, xi1);
            var ja2 = jacobian.Calculate(fe1.nodes, xi2);

            //evaluacija koordinata
            var xb1 = coor.Transform(fe2.nodes, xi1);
            var xb2 = coor.Transform(fe2.nodes, xi2);
            //evaluacija Jacobiana
            var jb1 = jacobian.Calculate(fe2.nodes, xi1);
            var jb2 = jacobian.Calculate(fe2.nodes, xi2);

            ///ispis rezultata
            Console.WriteLine($" a) - Evaluacija koordinata x(xi1)={xa1}, x(xi2)={xa2}");
            Console.WriteLine($" a) - Evaluacija Jacobiana  J(xi1)={ja1}, J(xi2)={ja2}");

            Console.WriteLine($" b) - Evaluacija koordinata x(xi1)={xb1}, x(xi2)={xb2}");
            Console.WriteLine($" b) - Evaluacija Jacobiana  J(xi1)={jb1}, J(xi2)={jb2}");


        }
    }

       
}