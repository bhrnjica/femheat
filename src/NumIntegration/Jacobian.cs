using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumIntegration
{
    public class Jacobian
    {
        readonly List<Func<Point, double>> _shapeDerivatives;
        readonly FEType _ft;
        readonly FEOrder _fo;
        public Jacobian(FEType ft, FEOrder fo)
        {
            _ft = ft;   
            _fo = fo;   
            _shapeDerivatives = getShapeDerivatives();
        }

    public double Calculate(Node[] nodes, Point pt)
    {
    var sDer = _shapeDerivatives;
            
    if (_ft == FEType.Line)
    {
        double jx = 0;
        double jy = 0;
        double jz = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            jx += nodes[i].x * sDer[i](pt);
            jy += nodes[i].y * sDer[i](pt);
            jz += nodes[i].z * sDer[i](pt);
        }
        //
        var j = Math.Sqrt(jx*jx + jy*jy + jz*jz); 
        return j;
    }
    else if (_ft == FEType.Triangle || _ft == FEType.Rectangle)
    {
        double l = FEType.Triangle == _ft ? 0.5 : 1;

        //i 
        double ai = 0, bi = 0, ci = 0, di = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            ai += nodes[i].y * sDer[i * 2](pt);
            bi += nodes[i].z * sDer[i * 2](pt);
            ci += nodes[i].y * sDer[i * 2 + 1](pt);
            di += nodes[i].z * sDer[i * 2 + 1](pt);
        }
        var d1 = (ai * di - bi * ci);
        //j 
        double aj = 0, bj = 0, cj = 0, dj = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            aj += nodes[i].x * sDer[i * 2](pt);
            bj += nodes[i].z * sDer[i * 2](pt);
            cj += nodes[i].x * sDer[i * 2 + 1](pt);
            dj += nodes[i].z * sDer[i * 2 + 1](pt);
        }
        var d2 = (aj * dj - bj * cj);
        // 
        double ak = 0, bk = 0, ck = 0, dk = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            ak += nodes[i].x * sDer[i * 2](pt);
            bk += nodes[i].y * sDer[i * 2](pt);
            ck += nodes[i].x * sDer[i * 2 + 1](pt);
            dk += nodes[i].y * sDer[i * 2 + 1](pt);
        }
        var d3 = (ak * dk - bk * ck);
        //
        return l * Math.Sqrt(d1 * d1 + d2 * d2 + d3 * d3);

    }
    else if (_ft == FEType.Tetrahedron || _ft == FEType.Hexaedron)
    {
        double l = FEType.Tetrahedron == _ft ? 1.0/6.0 : 1;
        var j = new double[3,3];
        var index = 0;
        for (int i = 0; i < nodes.Length; i++)
        {

            j[0,0] += nodes[i].x * sDer[index](pt);
            j[0,1] += nodes[i].y * sDer[index](pt);
            j[0,2] += nodes[i].z * sDer[index](pt);
                    
            index++;
                    
            j[1,0] += nodes[i].x * sDer[index](pt);
            j[1,1] += nodes[i].y * sDer[index](pt);
            j[1,2] += nodes[i].z * sDer[index](pt);

            index++;

            j[2,0] += nodes[i].x * sDer[index](pt);
            j[2,1] += nodes[i].y * sDer[index](pt);
            j[2,2] += nodes[i].z * sDer[index](pt);

            index++;
        }
        var det = 0.0;
        for (int i = 0; i < 3; i++)
            det += (j[0, i] * (j[1, (i + 1) % 3] * j[2, (i + 2) % 3] - j[1, (i + 2) % 3] * j[2, (i + 1) % 3]));
        return det * l;
    }
    else
        throw new NotSupportedException();  
    }

        public double Calculate2(Node[] nodes, Point pt)
        {
            var shape = _shapeDerivatives;

            if (_ft == FEType.Line)
            {
                double jx = 0;
                double jy = 0;
                double jz = 0;
                for (int i = 0; i < nodes.Length; i++)
                {
                    jx += nodes[i].x * shape[i](pt);
                    jy += nodes[i].y * shape[i](pt);
                    jz += nodes[i].z * shape[i](pt);
                }
                //
                var j = Math.Sqrt(jx * jx + jy * jy + jz * jz);
                return j;
            }
            else if (_ft == FEType.Triangle || _ft == FEType.Rectangle)
            {
                double l = FEType.Triangle == _ft ? 0.5 : 1;

                //i 
                double ai = 0, bi = 0, ci = 0, di = 0;
                for (int i = 0; i < nodes.Length; i++)
                {
                    ai += nodes[i].y * shape[i * 2](pt);
                    bi += nodes[i].z * shape[i * 2](pt);
                    ci += nodes[i].y * shape[i * 2 + 1](pt);
                    di += nodes[i].z * shape[i * 2 + 1](pt);
                }
                var d1 = (ai * di - bi * ci);
                //j 
                double aj = 0, bj = 0, cj = 0, dj = 0;
                for (int i = 0; i < nodes.Length; i++)
                {
                    aj += nodes[i].x * shape[i * 2](pt);
                    bj += nodes[i].z * shape[i * 2](pt);
                    cj += nodes[i].x * shape[i * 2 + 1](pt);
                    dj += nodes[i].z * shape[i * 2 + 1](pt);
                }
                var d2 = (aj * dj - bj * cj);
                // 
                double ak = 0, bk = 0, ck = 0, dk = 0;
                for (int i = 0; i < nodes.Length; i++)
                {
                    ak += nodes[i].x * shape[i * 2](pt);
                    bk += nodes[i].y * shape[i * 2](pt);
                    ck += nodes[i].x * shape[i * 2 + 1](pt);
                    dk += nodes[i].y * shape[i * 2 + 1](pt);
                }
                var d3 = (ak * dk - bk * ck);
                //
                return l * Math.Sqrt(d1*d1+d2*d2+d3*d3);

            }
            else if (_ft == FEType.Tetrahedron || _ft == FEType.Hexaedron)
            {
                double l = FEType.Tetrahedron == _ft ? 1.0 / 6.0 : 1;
                var j = new double[3, 3];
                var index = 0;
                for (int i = 0; i < nodes.Length; i++)
                {

                    j[0, 0] += nodes[i].x * shape[index](pt);//p1 derivation
                    j[0, 1] += nodes[i].y * shape[index](pt);//p1 derivation
                    j[0, 2] += nodes[i].z * shape[index](pt);//p1 derivation

                    index++;

                    j[1, 0] += nodes[i].x * shape[index](pt);//p2 derivation
                    j[1, 1] += nodes[i].y * shape[index](pt);//p2 derivation
                    j[1, 2] += nodes[i].z * shape[index](pt);//p2 derivation

                    index++;

                    j[2, 0] += nodes[i].x * shape[index](pt);//p3 derivation
                    j[2, 1] += nodes[i].y * shape[index](pt);//p3 derivation
                    j[2, 2] += nodes[i].z * shape[index](pt);//p3 derivation

                    index++;
                }
                var det = 0.0;
                for (int i = 0; i < 3; i++)
                    det += (j[0, i] * (j[1, (i + 1) % 3] * j[2, (i + 2) % 3] - j[1, (i + 2) % 3] * j[2, (i + 1) % 3]));
                return det * l;
            }
            else
                throw new NotSupportedException();



        }

        internal List<Func<Point, double>> getShapeDerivatives()
        {
            var shape = new List<Func<Point, double>>();

            if (_ft == FEType.Line)
            {
                if (_fo == FEOrder.Linear)
                {
                    shape.Add(ShapeDerivatives.dNLL1);
                    shape.Add(ShapeDerivatives.dNLL2);
                }
                if (_fo == FEOrder.Quadratic)
                {
                    shape.Add(ShapeDerivatives.dNLQ1);
                    shape.Add(ShapeDerivatives.dNLQ2);
                    shape.Add(ShapeDerivatives.dNLQ3);
                }
                if (_fo == FEOrder.Cubic)
                {
                    shape.Add(ShapeDerivatives.dNLC1);
                    shape.Add(ShapeDerivatives.dNLC2);
                    shape.Add(ShapeDerivatives.dNLC3);
                    shape.Add(ShapeDerivatives.dNLC4);
                }

                return shape;
            }

            if (_ft == FEType.Triangle || _ft == FEType.Rectangle)
            {
                if (_ft == FEType.Triangle && _fo == FEOrder.Linear)
                {
                    shape.Add(ShapeDerivatives.p1NTL1);
                    shape.Add(ShapeDerivatives.p2NTL1);
                    shape.Add(ShapeDerivatives.p1NTL2);
                    shape.Add(ShapeDerivatives.p2NTL2);
                    shape.Add(ShapeDerivatives.p1NTL3);
                    shape.Add(ShapeDerivatives.p2NTL3);
                }
                if (_ft == FEType.Triangle && _fo == FEOrder.Quadratic)
                {
                    shape.Add(ShapeDerivatives.p1NTQ1);
                    shape.Add(ShapeDerivatives.p2NTQ1);
                    shape.Add(ShapeDerivatives.p1NTQ2);
                    shape.Add(ShapeDerivatives.p2NTQ2);
                    shape.Add(ShapeDerivatives.p1NTQ3);
                    shape.Add(ShapeDerivatives.p2NTQ3);
                    shape.Add(ShapeDerivatives.p1NTQ4);
                    shape.Add(ShapeDerivatives.p2NTQ4);
                    shape.Add(ShapeDerivatives.p1NTQ5);
                    shape.Add(ShapeDerivatives.p2NTQ5);
                    shape.Add(ShapeDerivatives.p1NTQ6);
                    shape.Add(ShapeDerivatives.p2NTQ6);

                }
                if (_ft == FEType.Triangle && _fo == FEOrder.Cubic)
                {
                    return null;
                }
                if (_ft == FEType.Rectangle && _fo == FEOrder.Linear)
                {
                    shape.Add(ShapeDerivatives.p1NRL1);
                    shape.Add(ShapeDerivatives.p2NRL1);
                    shape.Add(ShapeDerivatives.p1NRL2);
                    shape.Add(ShapeDerivatives.p2NRL2);
                    shape.Add(ShapeDerivatives.p1NRL3);
                    shape.Add(ShapeDerivatives.p2NRL3);
                    shape.Add(ShapeDerivatives.p1NRL4);
                    shape.Add(ShapeDerivatives.p2NRL4);
                }
                if (_ft == FEType.Rectangle && _fo == FEOrder.Quadratic)
                {
                    shape.Add(ShapeDerivatives.p1NRBQ1);
                    shape.Add(ShapeDerivatives.p2NRBQ1);
                    shape.Add(ShapeDerivatives.p1NRBQ2);
                    shape.Add(ShapeDerivatives.p2NRBQ2);
                    shape.Add(ShapeDerivatives.p1NRBQ3);
                    shape.Add(ShapeDerivatives.p2NRBQ3);
                    shape.Add(ShapeDerivatives.p1NRBQ4);
                    shape.Add(ShapeDerivatives.p2NRBQ4);
                    shape.Add(ShapeDerivatives.p1NRBQ5);
                    shape.Add(ShapeDerivatives.p2NRBQ5);
                    shape.Add(ShapeDerivatives.p1NRBQ6);
                    shape.Add(ShapeDerivatives.p2NRBQ6);
                    shape.Add(ShapeDerivatives.p1NRBQ7);
                    shape.Add(ShapeDerivatives.p2NRBQ7);
                    shape.Add(ShapeDerivatives.p1NRBQ8);
                    shape.Add(ShapeDerivatives.p2NRBQ8);
                }
                //if (_ft == FEType.Rectangle && _fo == FEOrder.Quadratic && nodes.Length == 9)
                //{
                //    return null;
                //}

                return shape;
            }

            if (_ft == FEType.Tetrahedron || _ft == FEType.Hexaedron)
            {
                if (_ft == FEType.Tetrahedron && _fo == FEOrder.Linear)
                {
                    shape.Add(ShapeDerivatives.p1NHL1);
                    shape.Add(ShapeDerivatives.p2NHL1);
                    shape.Add(ShapeDerivatives.p3NHL1);
                    shape.Add(ShapeDerivatives.p1NHL2);
                    shape.Add(ShapeDerivatives.p2NHL2);
                    shape.Add(ShapeDerivatives.p3NHL2);
                    shape.Add(ShapeDerivatives.p1NHL3);
                    shape.Add(ShapeDerivatives.p2NHL3);
                    shape.Add(ShapeDerivatives.p3NHL3);
                    shape.Add(ShapeDerivatives.p1NHL4);
                    shape.Add(ShapeDerivatives.p2NHL4);
                    shape.Add(ShapeDerivatives.p3NHL4);

                }
                if (_ft == FEType.Hexaedron && _fo == FEOrder.Linear)
                {
                    shape.Add(ShapeDerivatives.p1NXL1);
                    shape.Add(ShapeDerivatives.p2NXL1);
                    shape.Add(ShapeDerivatives.p3NXL1);
                    shape.Add(ShapeDerivatives.p1NXL2);
                    shape.Add(ShapeDerivatives.p2NXL2);
                    shape.Add(ShapeDerivatives.p3NXL2);
                    shape.Add(ShapeDerivatives.p1NXL3);
                    shape.Add(ShapeDerivatives.p2NXL3);
                    shape.Add(ShapeDerivatives.p3NXL3);
                    shape.Add(ShapeDerivatives.p1NXL4);
                    shape.Add(ShapeDerivatives.p2NXL4);
                    shape.Add(ShapeDerivatives.p3NXL4);
                    shape.Add(ShapeDerivatives.p1NXL5);
                    shape.Add(ShapeDerivatives.p2NXL5);
                    shape.Add(ShapeDerivatives.p3NXL5);
                    shape.Add(ShapeDerivatives.p1NXL6);
                    shape.Add(ShapeDerivatives.p2NXL6);
                    shape.Add(ShapeDerivatives.p3NXL6);
                    shape.Add(ShapeDerivatives.p1NXL7);
                    shape.Add(ShapeDerivatives.p2NXL7);
                    shape.Add(ShapeDerivatives.p3NXL7);
                    shape.Add(ShapeDerivatives.p1NXL8);
                    shape.Add(ShapeDerivatives.p2NXL8);
                    shape.Add(ShapeDerivatives.p3NXL8);

                }

                return shape;
            }

            return null;
        }
    }
}
