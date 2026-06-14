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
        public static double NRL1(Point p) => 0.25 * (1.0 - p.x - p.y + p.x * p.y);
        public static double NRL2(Point p) => 0.25 * (1.0 + p.x - p.y - p.x * p.y);
        public static double NRL3(Point p) => 0.25 * (1.0 + p.x + p.y + p.x * p.y);
        public static double NRL4(Point p) => 0.25 * (1.0 - p.x + p.y - p.x * p.y);


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

        //Tetrahedron Quadratic Finite Element 10 Nodes
        // L1=x, L2=y, L3=z, L4=1-x-y-z  (volume/barycentric coordinates)
        public static double NHQ1(Point p) => (2.0 * p.x - 1.0) * p.x;
        public static double NHQ2(Point p) => (2.0 * p.y - 1.0) * p.y;
        public static double NHQ3(Point p) => (2.0 * p.z - 1.0) * p.z;
        public static double NHQ4(Point p) { double L4 = 1.0 - p.x - p.y - p.z; return (2.0 * L4 - 1.0) * L4; }
        public static double NHQ5(Point p) => 4.0 * p.x * p.y;
        public static double NHQ6(Point p) => 4.0 * p.y * p.z;
        public static double NHQ7(Point p) => 4.0 * p.z * p.x;
        public static double NHQ8(Point p) => 4.0 * p.x * (1.0 - p.x - p.y - p.z);
        public static double NHQ9(Point p) => 4.0 * p.y * (1.0 - p.x - p.y - p.z);
        public static double NHQ10(Point p) => 4.0 * p.z * (1.0 - p.x - p.y - p.z);

        //Hexahedron Linear Finite Element (8 nodes)
        public static double NXL1(Point p) => 0.125 * (1.0 - p.x) * (1.0 - p.y) * (1.0 - p.z);
        public static double NXL2(Point p) => 0.125 * (1.0 + p.x) * (1.0 - p.y) * (1.0 - p.z);
        public static double NXL3(Point p) => 0.125 * (1.0 + p.x) * (1.0 + p.y) * (1.0 - p.z);
        public static double NXL4(Point p) => 0.125 * (1.0 - p.x) * (1.0 + p.y) * (1.0 - p.z);
        public static double NXL5(Point p) => 0.125 * (1.0 - p.x) * (1.0 - p.y) * (1.0 + p.z);
        public static double NXL6(Point p) => 0.125 * (1.0 + p.x) * (1.0 - p.y) * (1.0 + p.z);
        public static double NXL7(Point p) => 0.125 * (1.0 + p.x) * (1.0 + p.y) * (1.0 + p.z);
        public static double NXL8(Point p) => 0.125 * (1.0 - p.x) * (1.0 + p.y) * (1.0 + p.z);

        // Hexahedron Quadratic Serendipity Finite Element (20 nodes)
        // Corners: nodes 1-8,  Midside: nodes 9-20
        // Corner formula: N_i = 1/8*(1+ξ_i·ξ)(1+η_i·η)(1+ζ_i·ζ)*(ξ_i·ξ + η_i·η + ζ_i·ζ - 2)
        // Midside ξ-edge: N_i = 1/4*(1-ξ²)(1+η_i·η)(1+ζ_i·ζ)
        // Midside η-edge: N_i = 1/4*(1+ξ_i·ξ)(1-η²)(1+ζ_i·ζ)
        // Midside ζ-edge: N_i = 1/4*(1+ξ_i·ξ)(1+η_i·η)(1-ζ²)

        private static double NXQ_corner(double x, double y, double z, int sx, int sy, int sz) =>
            0.125 * (1.0 + sx * x) * (1.0 + sy * y) * (1.0 + sz * z) * (sx * x + sy * y + sz * z - 2.0);
        private static double NXQ_edge_xi(double x, double y, double z, int sy, int sz) =>
            0.25 * (1.0 - x * x) * (1.0 + sy * y) * (1.0 + sz * z);
        private static double NXQ_edge_eta(double x, double y, double z, int sx, int sz) =>
            0.25 * (1.0 + sx * x) * (1.0 - y * y) * (1.0 + sz * z);
        private static double NXQ_edge_zeta(double x, double y, double z, int sx, int sy) =>
            0.25 * (1.0 + sx * x) * (1.0 + sy * y) * (1.0 - z * z);

        // Corners (ξ_i,η_i,ζ_i)
        public static double NXQ1(Point p) => NXQ_corner(p.x, p.y, p.z, -1, -1, -1);
        public static double NXQ2(Point p) => NXQ_corner(p.x, p.y, p.z, +1, -1, -1);
        public static double NXQ3(Point p) => NXQ_corner(p.x, p.y, p.z, +1, +1, -1);
        public static double NXQ4(Point p) => NXQ_corner(p.x, p.y, p.z, -1, +1, -1);
        public static double NXQ5(Point p) => NXQ_corner(p.x, p.y, p.z, -1, -1, +1);
        public static double NXQ6(Point p) => NXQ_corner(p.x, p.y, p.z, +1, -1, +1);
        public static double NXQ7(Point p) => NXQ_corner(p.x, p.y, p.z, +1, +1, +1);
        public static double NXQ8(Point p) => NXQ_corner(p.x, p.y, p.z, -1, +1, +1);
        // Midside nodes (ξ=0 edges: 9,11,17,19; η=0 edges: 10,12,18,20; ζ=0 edges: 13,14,15,16)
        public static double NXQ9(Point p)  => NXQ_edge_xi(p.x, p.y, p.z, -1, -1);    // edge 1-2
        public static double NXQ10(Point p) => NXQ_edge_eta(p.x, p.y, p.z, +1, -1);    // edge 2-3
        public static double NXQ11(Point p) => NXQ_edge_xi(p.x, p.y, p.z, +1, -1);    // edge 3-4
        public static double NXQ12(Point p) => NXQ_edge_eta(p.x, p.y, p.z, -1, -1);    // edge 4-1
        public static double NXQ13(Point p) => NXQ_edge_zeta(p.x, p.y, p.z, -1, -1);   // edge 1-5
        public static double NXQ14(Point p) => NXQ_edge_zeta(p.x, p.y, p.z, +1, -1);   // edge 2-6
        public static double NXQ15(Point p) => NXQ_edge_zeta(p.x, p.y, p.z, +1, +1);   // edge 3-7
        public static double NXQ16(Point p) => NXQ_edge_zeta(p.x, p.y, p.z, -1, +1);   // edge 4-8
        public static double NXQ17(Point p) => NXQ_edge_xi(p.x, p.y, p.z, -1, +1);    // edge 5-6
        public static double NXQ18(Point p) => NXQ_edge_eta(p.x, p.y, p.z, +1, +1);    // edge 6-7
        public static double NXQ19(Point p) => NXQ_edge_xi(p.x, p.y, p.z, +1, +1);    // edge 7-8
        public static double NXQ20(Point p) => NXQ_edge_eta(p.x, p.y, p.z, -1, +1);    // edge 8-5

        // Hexahedron Cubic Lagrange Finite Element (27 nodes, 3×3×3 tensor product)
        // 1D quadratic shape functions: L1(ξ)=½ξ(ξ-1), L2(ξ)=½ξ(ξ+1), L3(ξ)=1-ξ²
        // Node ordering: ξ fast, η medium, ζ slow (k*9 + j*3 + i + 1)
        private static double Li1(double xi) => 0.5 * xi * (xi - 1.0);  // node at ξ=-1
        private static double Li2(double xi) => 0.5 * xi * (xi + 1.0);  // node at ξ=+1
        private static double Li3(double xi) => 1.0 - xi * xi;           // node at ξ=0

        private static double NXC_ijk(double x, double y, double z, int i, int j, int k)
        {
            double nx = i switch { 1 => Li1(x), 2 => Li2(x), 3 => Li3(x), _ => 0 };
            double ny = j switch { 1 => Li1(y), 2 => Li2(y), 3 => Li3(y), _ => 0 };
            double nz = k switch { 1 => Li1(z), 2 => Li2(z), 3 => Li3(z), _ => 0 };
            return nx * ny * nz;
        }

        // ζ=-1 layer (k=1): nodes 1-9
        public static double NXC1(Point p)  => NXC_ijk(p.x, p.y, p.z, 1, 1, 1);
        public static double NXC2(Point p)  => NXC_ijk(p.x, p.y, p.z, 3, 1, 1);
        public static double NXC3(Point p)  => NXC_ijk(p.x, p.y, p.z, 2, 1, 1);
        public static double NXC4(Point p)  => NXC_ijk(p.x, p.y, p.z, 1, 3, 1);
        public static double NXC5(Point p)  => NXC_ijk(p.x, p.y, p.z, 3, 3, 1);
        public static double NXC6(Point p)  => NXC_ijk(p.x, p.y, p.z, 2, 3, 1);
        public static double NXC7(Point p)  => NXC_ijk(p.x, p.y, p.z, 1, 2, 1);
        public static double NXC8(Point p)  => NXC_ijk(p.x, p.y, p.z, 3, 2, 1);
        public static double NXC9(Point p)  => NXC_ijk(p.x, p.y, p.z, 2, 2, 1);
        // ζ=0 layer (k=3): nodes 10-18
        public static double NXC10(Point p) => NXC_ijk(p.x, p.y, p.z, 1, 1, 3);
        public static double NXC11(Point p) => NXC_ijk(p.x, p.y, p.z, 3, 1, 3);
        public static double NXC12(Point p) => NXC_ijk(p.x, p.y, p.z, 2, 1, 3);
        public static double NXC13(Point p) => NXC_ijk(p.x, p.y, p.z, 1, 3, 3);
        public static double NXC14(Point p) => NXC_ijk(p.x, p.y, p.z, 3, 3, 3);
        public static double NXC15(Point p) => NXC_ijk(p.x, p.y, p.z, 2, 3, 3);
        public static double NXC16(Point p) => NXC_ijk(p.x, p.y, p.z, 1, 2, 3);
        public static double NXC17(Point p) => NXC_ijk(p.x, p.y, p.z, 3, 2, 3);
        public static double NXC18(Point p) => NXC_ijk(p.x, p.y, p.z, 2, 2, 3);
        // ζ=+1 layer (k=2): nodes 19-27
        public static double NXC19(Point p) => NXC_ijk(p.x, p.y, p.z, 1, 1, 2);
        public static double NXC20(Point p) => NXC_ijk(p.x, p.y, p.z, 3, 1, 2);
        public static double NXC21(Point p) => NXC_ijk(p.x, p.y, p.z, 2, 1, 2);
        public static double NXC22(Point p) => NXC_ijk(p.x, p.y, p.z, 1, 3, 2);
        public static double NXC23(Point p) => NXC_ijk(p.x, p.y, p.z, 3, 3, 2);
        public static double NXC24(Point p) => NXC_ijk(p.x, p.y, p.z, 2, 3, 2);
        public static double NXC25(Point p) => NXC_ijk(p.x, p.y, p.z, 1, 2, 2);
        public static double NXC26(Point p) => NXC_ijk(p.x, p.y, p.z, 3, 2, 2);
        public static double NXC27(Point p) => NXC_ijk(p.x, p.y, p.z, 2, 2, 2);

    }
}
