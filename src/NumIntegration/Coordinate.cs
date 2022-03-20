using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumIntegration
{
    public class Coordinates
    {
        readonly List<Func<Point, double>> _shapeFunctions;
        readonly FEType _ft;
        readonly FEOrder _fo;

        public Coordinates(FEType ft, FEOrder fo)
        {
            _ft = ft;
            _fo = fo;
            _shapeFunctions = getShapeFunctions();
        }
        public Point Transform(Node[] nodes, Point pt)
        {
            //get appropriate Shape functions
            var shape = _shapeFunctions;

            (double x, double y, double z) = (0, 0, 0);

            if (_ft == FEType.Line)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    x += nodes[i].x * shape[i](pt);
                }
            }

            else if (_ft == FEType.Rectangle || _ft == FEType.Triangle)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    x += nodes[i].x * shape[i](pt);
                    y += nodes[i].y * shape[i](pt);
                }
            }

            else if (_ft == FEType.Tetrahedron || _ft == FEType.Hexaedron)
            {
                for (int i = 0; i < nodes.Length; i++)
                {

                    x += nodes[i].x * shape[i](pt);
                    y += nodes[i].y * shape[i](pt);
                    z += nodes[i].z * shape[i](pt);
                }
            }

            else
                throw new NotSupportedException();


            return new Point(x, y, z);
        }

        internal List<Func<Point, double>> getShapeFunctions()
        {
            var shape = new List<Func<Point, double>>();

            if (_ft == FEType.Line)
            {
                if (_fo == FEOrder.Linear)
                {
                    shape.Add(Shape.NLL1);
                    shape.Add(Shape.NLL2);
                }
                if (_fo == FEOrder.Quadratic)
                {
                    shape.Add(Shape.NLQ1);
                    shape.Add(Shape.NLQ2);
                    shape.Add(Shape.NLQ3);
                }
                if (_fo == FEOrder.Cubic)
                {
                    shape.Add(Shape.NLC1);
                    shape.Add(Shape.NLC2);
                    shape.Add(Shape.NLC3);
                    shape.Add(Shape.NLC4);

                }

                return shape;
            }
            if (_ft == FEType.Rectangle || _ft == FEType.Triangle)
            {
                if (_ft == FEType.Triangle && _fo == FEOrder.Linear)
                {
                    shape.Add(Shape.NTL1);
                    shape.Add(Shape.NTL2);
                    shape.Add(Shape.NTL3);

                }
                if (_ft == FEType.Triangle && _fo == FEOrder.Quadratic)
                {
                    shape.Add(Shape.NTQ1);
                    shape.Add(Shape.NTQ2);
                    shape.Add(Shape.NTQ3);
                    shape.Add(Shape.NTQ4);
                    shape.Add(Shape.NTQ5);
                    shape.Add(Shape.NTQ6);

                }
                if (_ft == FEType.Triangle && _fo == FEOrder.Cubic)
                {
                    return null;
                }
                if (_ft == FEType.Rectangle && _fo == FEOrder.Linear)
                {
                    shape.Add(Shape.NRL1);
                    shape.Add(Shape.NRL2);
                    shape.Add(Shape.NRL3);
                    shape.Add(Shape.NRL4);
                }
                if (_ft == FEType.Rectangle && _fo == FEOrder.Quadratic)
                {
                    shape.Add(Shape.NRBQ1);
                    shape.Add(Shape.NRBQ2);
                    shape.Add(Shape.NRBQ3);
                    shape.Add(Shape.NRBQ4);
                    shape.Add(Shape.NRBQ5);
                    shape.Add(Shape.NRBQ6);
                    shape.Add(Shape.NRBQ7);
                    shape.Add(Shape.NRBQ8);
                }
               
                return shape;
            }

            if (_ft == FEType.Tetrahedron || _ft == FEType.Hexaedron)
            {
                if (_ft == FEType.Tetrahedron && _fo == FEOrder.Linear)
                {
                    shape.Add(Shape.NHL1);
                    shape.Add(Shape.NHL2);
                    shape.Add(Shape.NHL3);
                    shape.Add(Shape.NHL4);


                }
                if (_ft == FEType.Hexaedron && _fo == FEOrder.Linear)
                {
                    shape.Add(Shape.NXL1);
                    shape.Add(Shape.NXL2);
                    shape.Add(Shape.NXL3);
                    shape.Add(Shape.NXL4);
                    shape.Add(Shape.NXL5);
                    shape.Add(Shape.NXL6);
                    shape.Add(Shape.NXL7);
                    shape.Add(Shape.NXL8);

                }

                return shape;
            }

            return null;
        }
    }
}
