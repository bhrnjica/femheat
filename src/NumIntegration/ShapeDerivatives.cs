using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumIntegration
{
    public class ShapeDerivatives : Shape
    {
        //Line Linear Finite Element
        public static double dNLL1(Point p) => -0.5;
        public static double dNLL2(Point p) => 0.5;

        //Line Quadratic Finite Element
        public static double dNLQ1(Point p) => p.x - 0.5;
        public static double dNLQ3(Point p) => -2.0 * p.x;
        public static double dNLQ2(Point p) => p.x + 0.5;

        //Line Cubic Finite Element
        public static double dNLC1(Point p) => 0.0625 * (1.0 + 18.0 * p.x - 27.0 * p.x * p.x);
        public static double dNLC2(Point p) => 0.0625 * (-1.0 + 18.0 * p.x + 27.0 * p.x * p.x);
        public static double dNLC3(Point p) => 0.5625 * (-3.0 - 2.0 * p.x + 9.0 * p.x * p.x);
        public static double dNLC4(Point p) => -0.5625 * (-3.0 + 2.0 * p.x + 9.0 * p.x * p.x);

        //Triangle Linear Finite element
        public static double p1NTL1(Point p) => 1.0;
        public static double p2NTL1(Point p) => 0;
        public static double p1NTL2(Point p) => 0;
        public static double p2NTL2(Point p) => 1.0;
        public static double p1NTL3(Point p) => -1.0;
        public static double p2NTL3(Point p) => -1.0;

        //Triangle Quadratic Finite Element 6 Nodes
        public static double p1NTQ1(Point p) => -3.0 + 4.0 * p.x + 4.0 * p.y;
        public static double p2NTQ1(Point p) => -3.0 + 4.0 * p.x + 4.0 * p.y;
        public static double p1NTQ2(Point p) => -1.0 + 4.0 * p.x;
        public static double p2NTQ2(Point p) => 0;
        public static double p1NTQ3(Point p) => 0;
        public static double p2NTQ3(Point p) => -1.0 + 4.0 * p.y;
        public static double p1NTQ4(Point p) => -4.0 * (-1.0 + 2.0 * p.x + p.y);
        public static double p2NTQ4(Point p) => -4.0 * p.x;
        public static double p1NTQ5(Point p) => 4.0 * p.y;
        public static double p2NTQ5(Point p) => 4.0 * p.x;
        public static double p1NTQ6(Point p) => -4.0 * p.y;
        public static double p2NTQ6(Point p) => -4.0 * (-1.0 + p.x + 2.0 * p.y);

        //Triangle Cubic Finite Element 10 Nodes


        //Rectangle Linear Finite element shape function and its derivatives
        public static double p1NRL1(Point p) => 0.25 * (-1.0 + p.y);
        public static double p2NRL1(Point p) => 0.25 * (-1.0 + p.x);
        public static double p1NRL2(Point p) => 0.25 * (1.0 - p.y);
        public static double p2NRL2(Point p) => 0.25 * (-1.0 - p.x);
        public static double p1NRL3(Point p) => 0.25 * (1.0 + p.y);
        public static double p2NRL3(Point p) => 0.25 * (1.0 + p.x);
        public static double p1NRL4(Point p) => 0.25 * (-1.0- p.y);
        public static double p2NRL4(Point p) => 0.25 * (1.0 - p.x);

        //Rectangle Quadratic Finite element 8 -nodes
        public static double p1NRBQ1(Point p) => 0.25 * (-1.0 + p.y) * (p.y + 2.0 * p.x);
        public static double p2NRBQ1(Point p) => 0.25 * (-1.0 + p.x) * (2.0 * p.y + p.x);
        public static double p1NRBQ2(Point p) => 0.25 * (-1.0 + p.y) * (p.y - 2.0 * p.x);
        public static double p2NRBQ2(Point p) => 0.25 * (1.0 + p.x) * (2.0 * p.y - p.x);
        public static double p1NRBQ3(Point p) => 0.25 * (1.0 + p.y) * (p.y + 2.0 * p.x);
        public static double p2NRBQ3(Point p) => 0.25 * (1.0 + p.x) * (2.0 * p.y + p.x);
        public static double p1NRBQ4(Point p) => 0.25 * (1.0 + p.y) * (p.y + 2.0 * p.x);
        public static double p2NRBQ4(Point p) => 0.25 * (-1.0 + p.x) * (2.0 + 2.0 * p.y + p.x);
        public static double p1NRBQ5(Point p) => (-1.0 + p.y) * p.x;
        public static double p2NRBQ5(Point p) => 0.5 * (-1.0 + p.x * p.x);
        public static double p1NRBQ6(Point p) => 0.5 * (1.0 - p.y * p.y);
        public static double p2NRBQ6(Point p) => -p.y * (1.0 + p.x);
        public static double p1NRBQ7(Point p) => (-1.0 + p.y) * p.x;
        public static double p2NRBQ7(Point p) => 0.5 * (1.0 - p.x * p.x);
        public static double p1NRBQ8(Point p) => 0.5 * (-1.0 + p.y * p.y);
        public static double p2NRBQ8(Point p) => p.y * (-1.0 + p.x);

        //Rectangle Quadratic Finite element 9 -nodes


        //Tetrahedron Linear Finite Element 
        public static double p1NHL1(Point p) => 1.0;
        public static double p2NHL1(Point p) => 0;
        public static double p3NHL1(Point p) => 0;
        public static double p1NHL2(Point p) => 0;
        public static double p2NHL2(Point p) => 1.0;
        public static double p3NHL2(Point p) => 0;
        public static double p1NHL3(Point p) => 0;
        public static double p2NHL3(Point p) => 0;
        public static double p3NHL3(Point p) => 1.0;
        public static double p1NHL4(Point p) => -1.0;
        public static double p2NHL4(Point p) => -1.0;
        public static double p3NHL4(Point p) => -1.0;

        //Hexahedron Linear Finite Element
        public static double p1NXL1(Point p) => -0.125 * (-1.0 + p.y) * (-1.0 + p.z);
        public static double p2NXL1(Point p) => -0.125 * (-1.0 + p.x) * (-1.0 + p.z);
        public static double p3NXL1(Point p) => -0.125 * (-1.0 + p.x) * (-1.0 + p.y);
        public static double p1NXL2(Point p) => 0.125 * (-1.0 + p.y) * (-1.0 + p.z);
        public static double p2NXL2(Point p) => 0.125 * (1.0 + p.x) * (-1.0 + p.z);
        public static double p3NXL2(Point p) => 0.125 * (1.0 + p.x) * (-1.0 + p.y);
        public static double p1NXL3(Point p) => -0.125 * (1.0 + p.y) * (-1.0 + p.z);
        public static double p2NXL3(Point p) => -0.125 * (1.0 + p.x) * (-1.0 + p.z);
        public static double p3NXL3(Point p) => -0.125 * (1.0 + p.x) * (1.0 + p.y);
        public static double p1NXL4(Point p) => 0.125 * (1.0 + p.y) * (-1.0 + p.z);
        public static double p2NXL4(Point p) => 0.125 * (-1.0 + p.x) * (-1.0 + p.z);
        public static double p3NXL4(Point p) => 0.125 * (-1.0 + p.x) * (1.0 + p.y);
        public static double p1NXL5(Point p) => 0.125 * (-1.0 + p.y) * (1.0 + p.z);
        public static double p2NXL5(Point p) => 0.125 * (-1.0 + p.x) * (1.0 + p.z);
        public static double p3NXL5(Point p) => 0.125 * (-1.0 + p.x) * (-1.0 + p.y);
        public static double p1NXL6(Point p) => -0.125 * (-1.0 + p.y) * (1.0 + p.z);
        public static double p2NXL6(Point p) => -0.125 * (1.0 + p.x) * (1.0 + p.z);
        public static double p3NXL6(Point p) => -0.125 * (1.0 + p.x) * (-1.0 + p.y);
        public static double p1NXL7(Point p) => 0.125 * (1.0 + p.y) * (1.0 + p.z);
        public static double p2NXL7(Point p) => 0.125 * (1.0 + p.x) * (1.0 + p.z);
        public static double p3NXL7(Point p) => 0.125 * (1.0 + p.x) * (1.0 + p.y);
        public static double p1NXL8(Point p) => -0.125 * (1.0 + p.y) * (1.0 + p.z);
        public static double p2NXL8(Point p) => -0.125 * (-1.0 + p.x) * (1.0 + p.z);
        public static double p3NXL8(Point p) => -0.125 * (-1.0 + p.x) * (1.0 + p.y);
    }
}
