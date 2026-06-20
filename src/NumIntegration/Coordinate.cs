using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumIntegration
{
    public class Coordinates
    {
        readonly List<Func<Point, double>>? _shapeFunctions;
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
            if(shape is null)
                throw new NullReferenceException(nameof(shape));
                
            (double x, double y, double z) = (0, 0, 0);

            if (_ft == FEType.Line)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    x += nodes[i].x * shape[i](pt);
                    y += nodes[i].y * shape[i](pt);
                    z += nodes[i].z * shape[i](pt);
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

        internal List<Func<Point, double>>? getShapeFunctions()
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
                if (_ft == FEType.Tetrahedron && _fo == FEOrder.Quadratic)
                {
                    shape.Add(Shape.NHQ1);
                    shape.Add(Shape.NHQ2);
                    shape.Add(Shape.NHQ3);
                    shape.Add(Shape.NHQ4);
                    shape.Add(Shape.NHQ5);
                    shape.Add(Shape.NHQ6);
                    shape.Add(Shape.NHQ7);
                    shape.Add(Shape.NHQ8);
                    shape.Add(Shape.NHQ9);
                    shape.Add(Shape.NHQ10);
                }
                if (_ft == FEType.Tetrahedron && _fo == FEOrder.Cubic)
                {
                    return null; // 20-node cubic tetrahedron not implemented yet
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
                if (_ft == FEType.Hexaedron && _fo == FEOrder.Quadratic)
                {
                    shape.Add(Shape.NXQ1);
                    shape.Add(Shape.NXQ2);
                    shape.Add(Shape.NXQ3);
                    shape.Add(Shape.NXQ4);
                    shape.Add(Shape.NXQ5);
                    shape.Add(Shape.NXQ6);
                    shape.Add(Shape.NXQ7);
                    shape.Add(Shape.NXQ8);
                    shape.Add(Shape.NXQ9);
                    shape.Add(Shape.NXQ10);
                    shape.Add(Shape.NXQ11);
                    shape.Add(Shape.NXQ12);
                    shape.Add(Shape.NXQ13);
                    shape.Add(Shape.NXQ14);
                    shape.Add(Shape.NXQ15);
                    shape.Add(Shape.NXQ16);
                    shape.Add(Shape.NXQ17);
                    shape.Add(Shape.NXQ18);
                    shape.Add(Shape.NXQ19);
                    shape.Add(Shape.NXQ20);
                }
                if (_ft == FEType.Hexaedron && _fo == FEOrder.Cubic)
                {
                    shape.Add(Shape.NXC1);
                    shape.Add(Shape.NXC2);
                    shape.Add(Shape.NXC3);
                    shape.Add(Shape.NXC4);
                    shape.Add(Shape.NXC5);
                    shape.Add(Shape.NXC6);
                    shape.Add(Shape.NXC7);
                    shape.Add(Shape.NXC8);
                    shape.Add(Shape.NXC9);
                    shape.Add(Shape.NXC10);
                    shape.Add(Shape.NXC11);
                    shape.Add(Shape.NXC12);
                    shape.Add(Shape.NXC13);
                    shape.Add(Shape.NXC14);
                    shape.Add(Shape.NXC15);
                    shape.Add(Shape.NXC16);
                    shape.Add(Shape.NXC17);
                    shape.Add(Shape.NXC18);
                    shape.Add(Shape.NXC19);
                    shape.Add(Shape.NXC20);
                    shape.Add(Shape.NXC21);
                    shape.Add(Shape.NXC22);
                    shape.Add(Shape.NXC23);
                    shape.Add(Shape.NXC24);
                    shape.Add(Shape.NXC25);
                    shape.Add(Shape.NXC26);
                    shape.Add(Shape.NXC27);
                }

                return shape;
            }

            return null;
        }
    }
}
