using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumIntegration
{
    public class Shape
    {
        //Line Linear Finite Element
        public static double NLL1(Point p) => 0.5 * (1.0 - p.x);
        public static double NLL2(Point p) => 0.5 * (1.0 + p.x);

        //Line Quadratic Finite Element
        public static double NLQ1(Point p) => 0.5 * p.x * (p.x - 1.0);
        public static double NLQ2(Point p) => 0.5 * p.x * (1.0 + p.x);
        public static double NLQ3(Point p) => 1.0 - p.x * p.x;

        //Line Cubic Finite Element
        public static double NLC1(Point p) => 0.5625 * (p.x * p.x - 1.0 / 9.0) * (1.0 - p.x);
        public static double NLC2(Point p) => 0.5625 * (p.x * p.x - 1.0 / 9.0) * (1.0 + p.x);
        public static double NLC3(Point p) => 1.6875 * (p.x * p.x - 1.0) * (p.x - 1.0 / 9.0);
        public static double NLC4(Point p) => 1.6875 * (1.0 - p.x * p.x) * (p.x + 1.0 / 9.0);


        //Triangle Linear Finite element
        public static double NTL1(Point p) => p.x;
        public static double NTL2(Point p) => p.y;
        public static double NTL3(Point p) => 1.0 - p.x - p.y;

        //Triangle Quadratic Finite Element 6 Nodes
        public static double NTQ1(Point p) => (1.0 - p.x - p.y) * (1.0 - 2.0 * p.x - 2.0 * p.y);
        public static double NTQ2(Point p) => p.x * (2.0 * p.x - 1.0);
        public static double NTQ3(Point p) => p.y * (2.0 * p.y - 1.0);
        public static double NTQ4(Point p) => 4.0 * p.x * (1.0 - p.x - p.y);
        public static double NTQ5(Point p) => 4.0 * p.x * p.y;
        public static double NTQ6(Point p) => 4.0 * p.y * (1.0 - p.x - p.y);

        //Triangle Cubic Finite Element 10 Nodes
        public static double NTC1(Point p) =>   0.5 * (1.0 - 3.0 * p.x - 3.0 * p.y) * (2.0 - 3.0 * p.x - 3.0 * p.y) * (1.0 - p.x - p.y);
        public static double NTC2(Point p) =>   4.5 * p.x * (p.x - 1.0/ 3.0) * (p.x - 2.0 / 3.0);
        public static double NTC3(Point p) =>   4.5 * p.y * (p.y - 1.0 / 3.0) * (p.y - 2.0 / 3.0);
        public static double NTC4(Point p) =>   4.5 * p.x * (2.0 - 3.0 * p.x - 3.0 * p.y) * (1.0 - p.x - p.y);
        public static double NTC5(Point p) =>   4.5 * p.x * p.y * (3.0 * p.x - 1.0);
        public static double NTC6(Point p) =>   4.5 * p.y * (3.0 * p.y - 1.0) * (1.0 - p.x - p.y);
        public static double NTC7(Point p) =>   4.5 * p.x * (3.0 * p.x - 1.0) * (1.0 - p.x - p.y);
        public static double NTC8(Point p) =>   4.5 * p.x * p.y * (3.0 * p.y - 1.0);
        public static double NTC9(Point p) =>   4.5 * p.y * (2.0 - 3.0 * p.x - 3.0 * p.y) * (1.0 - p.x - p.y);
        public static double NTC10(Point p) => 27.0 * p.x * p.y * (1.0 - p.x - p.y);

        //Rectangle Linear Finite element shape function and its derivatives
        public static double NRL1(Point p) => 0.25 * (1.0 - p.x) * (1.0 - p.y);
        public static double NRL2(Point p) => 0.25 * (1.0 + p.x) * (1.0 - p.y);
        public static double NRL3(Point p) => 0.25 * (1.0 + p.x) * (1.0 + p.y);
        public static double NRL4(Point p) => 0.25 * (1.0 - p.x) * (1.0 + p.y);


        //Rectangle Quadratic Finite element 8 -nodes
        public static double NRBQ1(Point p) => 0.25 * (1.0 - p.x) * (1.0 - p.y) * (-p.x - p.y - 1.0);
        public static double NRBQ2(Point p) => 0.25 * (1.0 + p.x) * (1.0 - p.y) * (p.x - p.y - 1.0);
        public static double NRBQ3(Point p) => 0.25 * (1.0 + p.x) * (1.0 + p.y) * (p.x + p.y - 1.0);
        public static double NRBQ4(Point p) => 0.25 * (1.0 - p.x) * (1.0 + p.y) * (-p.x - p.y - 1.0);
        public static double NRBQ5(Point p) => 0.5 * (1.0 - p.x * p.x) * (1.0 - p.y);
        public static double NRBQ6(Point p) => 0.5 * (1.0 + p.x) * (1.0 - p.y * p.y);
        public static double NRBQ7(Point p) => 0.5 * (1.0 - p.x * p.x) * (1.0 + p.y);
        public static double NRBQ8(Point p) => 0.5 * (1.0 - p.x) * (1.0 - p.y * p.y);

        //Rectangle Quadratic Finite element 9 -nodes
        public static double NRQ1(Point p) => 0.25 * p.x * (p.x - 1.0) * p.y * (p.y - 1.0);
        public static double NRQ2(Point p) => 0.25 * p.x * (p.x + 1.0) * p.y * (p.y - 1.0);
        public static double NRQ3(Point p) => 0.25 * p.x * (p.x + 1.0) * p.y * (p.y + 1.0);
        public static double NRQ4(Point p) => 0.25 * p.x * (p.x - 1.0) * p.y * (p.y + 1.0);
        public static double NRQ5(Point p) => -0.5 * (p.x + 1.0) * (p.x - 1.0) * p.y * (p.y - 1.0);
        public static double NRQ6(Point p) => -0.5 * p.x * (1.0 + p.x) * (1.0 + p.y) * (p.y - 1.0);
        public static double NRQ7(Point p) => -0.5 * (p.x - 1.0) * (1.0 + p.x) * p.y * (1.0 + p.y);
        public static double NRQ8(Point p) => -0.5 * (p.x - 1.0) * p.x * (p.y - 1.0) * (1.0 + p.y);
        public static double NRQ9(Point p) => (1.0 - p.x * p.x) * (1.0 - p.y * p.y);


        //Tetrahedron Linear Finite Element 
        public static double NHL1(Point p) => p.x;
        public static double NHL2(Point p) => p.y;
        public static double NHL3(Point p) => p.z;
        public static double NHL4(Point p) => 1.0 - p.x - p.y - p.z;

        //Hexahedron Linear Finite Element
        public static double NXL1(Point p) => 0.125 * (1.0 - p.x) * (1.0 - p.y) * (1.0 - p.z);
        public static double NXL2(Point p) => 0.125 * (1.0 + p.x) * (1.0 - p.y) * (1.0 - p.z);
        public static double NXL3(Point p) => 0.125 * (1.0 + p.x) * (1.0 + p.y) * (1.0 - p.z);
        public static double NXL4(Point p) => 0.125 * (1.0 - p.x) * (1.0 + p.y) * (1.0 - p.z);
        public static double NXL5(Point p) => 0.125 * (1.0 - p.x) * (1.0 - p.y) * (1.0 + p.z);
        public static double NXL6(Point p) => 0.125 * (1.0 + p.x) * (1.0 - p.y) * (1.0 + p.z);
        public static double NXL7(Point p) => 0.125 * (1.0 + p.x) * (1.0 + p.y) * (1.0 + p.z);
        public static double NXL8(Point p) => 0.125 * (1.0 - p.x) * (1.0 + p.y) * (1.0 + p.z);

    }
}
