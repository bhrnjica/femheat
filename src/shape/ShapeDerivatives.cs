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

        //Tetrahedron Quadratic Finite Element 10 Nodes
        // L1=x, L2=y, L3=z, L4=1-x-y-z
        public static double p1NHQ1(Point p) => 4.0 * p.x - 1.0;
        public static double p2NHQ1(Point p) => 0;
        public static double p3NHQ1(Point p) => 0;
        public static double p1NHQ2(Point p) => 0;
        public static double p2NHQ2(Point p) => 4.0 * p.y - 1.0;
        public static double p3NHQ2(Point p) => 0;
        public static double p1NHQ3(Point p) => 0;
        public static double p2NHQ3(Point p) => 0;
        public static double p3NHQ3(Point p) => 4.0 * p.z - 1.0;
        public static double p1NHQ4(Point p) => 4.0 * (p.x + p.y + p.z) - 3.0;
        public static double p2NHQ4(Point p) => 4.0 * (p.x + p.y + p.z) - 3.0;
        public static double p3NHQ4(Point p) => 4.0 * (p.x + p.y + p.z) - 3.0;
        public static double p1NHQ5(Point p) => 4.0 * p.y;
        public static double p2NHQ5(Point p) => 4.0 * p.x;
        public static double p3NHQ5(Point p) => 0;
        public static double p1NHQ6(Point p) => 0;
        public static double p2NHQ6(Point p) => 4.0 * p.z;
        public static double p3NHQ6(Point p) => 4.0 * p.y;
        public static double p1NHQ7(Point p) => 4.0 * p.z;
        public static double p2NHQ7(Point p) => 0;
        public static double p3NHQ7(Point p) => 4.0 * p.x;
        public static double p1NHQ8(Point p) => 4.0 * (1.0 - 2.0 * p.x - p.y - p.z);
        public static double p2NHQ8(Point p) => -4.0 * p.x;
        public static double p3NHQ8(Point p) => -4.0 * p.x;
        public static double p1NHQ9(Point p) => -4.0 * p.y;
        public static double p2NHQ9(Point p) => 4.0 * (1.0 - p.x - 2.0 * p.y - p.z);
        public static double p3NHQ9(Point p) => -4.0 * p.y;
        public static double p1NHQ10(Point p) => -4.0 * p.z;
        public static double p2NHQ10(Point p) => -4.0 * p.z;
        public static double p3NHQ10(Point p) => 4.0 * (1.0 - p.x - p.y - 2.0 * p.z);

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

        // ===== Hexahedron Quadratic Serendipity (20 nodes) Derivatives =====
        // Corner derivative: dN/dξ = A/8 * (1+Bη)(1+Cζ) * (2Aξ + Bη + Cζ - 1)
        //                    dN/dη = B/8 * (1+Aξ)(1+Cζ) * (Aξ + 2Bη + Cζ - 1)
        //                    dN/dζ = C/8 * (1+Aξ)(1+Bη) * (Aξ + Bη + 2Cζ - 1)
        // where (A,B,C) = (±1,±1,±1)
        private static double dNXQ_corner_xi(double x, double y, double z, int A, int B, int C) =>
            A / 8.0 * (1.0 + B * y) * (1.0 + C * z) * (2.0 * A * x + B * y + C * z - 1.0);
        private static double dNXQ_corner_eta(double x, double y, double z, int A, int B, int C) =>
            B / 8.0 * (1.0 + A * x) * (1.0 + C * z) * (A * x + 2.0 * B * y + C * z - 1.0);
        private static double dNXQ_corner_zeta(double x, double y, double z, int A, int B, int C) =>
            C / 8.0 * (1.0 + A * x) * (1.0 + B * y) * (A * x + B * y + 2.0 * C * z - 1.0);

        // Midside ξ-edge (A=0): dN/dξ = -ξ/2*(1+Bη)(1+Cζ), dN/dη = B/4*(1-ξ²)(1+Cζ), dN/dζ = C/4*(1-ξ²)(1+Bη)
        private static double dNXQ_edge_xi_dxi(double x, double y, double z, int B, int C) =>
            -x / 2.0 * (1.0 + B * y) * (1.0 + C * z);
        private static double dNXQ_edge_xi_deta(double x, double y, double z, int B, int C) =>
            B / 4.0 * (1.0 - x * x) * (1.0 + C * z);
        private static double dNXQ_edge_xi_dzeta(double x, double y, double z, int B, int C) =>
            C / 4.0 * (1.0 - x * x) * (1.0 + B * y);

        // Midside η-edge (B=0): dN/dξ = A/4*(1-η²)(1+Cζ), dN/dη = -η/2*(1+Aξ)(1+Cζ), dN/dζ = C/4*(1+Aξ)(1-η²)
        private static double dNXQ_edge_eta_dxi(double x, double y, double z, int A, int C) =>
            A / 4.0 * (1.0 - y * y) * (1.0 + C * z);
        private static double dNXQ_edge_eta_deta(double x, double y, double z, int A, int C) =>
            -y / 2.0 * (1.0 + A * x) * (1.0 + C * z);
        private static double dNXQ_edge_eta_dzeta(double x, double y, double z, int A, int C) =>
            C / 4.0 * (1.0 + A * x) * (1.0 - y * y);

        // Midside ζ-edge (C=0): dN/dξ = A/4*(1+Bη)(1-ζ²), dN/dη = B/4*(1+Aξ)(1-ζ²), dN/dζ = -ζ/2*(1+Aξ)(1+Bη)
        private static double dNXQ_edge_zeta_dxi(double x, double y, double z, int A, int B) =>
            A / 4.0 * (1.0 + B * y) * (1.0 - z * z);
        private static double dNXQ_edge_zeta_deta(double x, double y, double z, int A, int B) =>
            B / 4.0 * (1.0 + A * x) * (1.0 - z * z);
        private static double dNXQ_edge_zeta_dzeta(double x, double y, double z, int A, int B) =>
            -z / 2.0 * (1.0 + A * x) * (1.0 + B * y);

        // Corner nodes 1-8
        public static double p1NXQ1(Point p) => dNXQ_corner_xi(p.x, p.y, p.z, -1, -1, -1);
        public static double p2NXQ1(Point p) => dNXQ_corner_eta(p.x, p.y, p.z, -1, -1, -1);
        public static double p3NXQ1(Point p) => dNXQ_corner_zeta(p.x, p.y, p.z, -1, -1, -1);
        public static double p1NXQ2(Point p) => dNXQ_corner_xi(p.x, p.y, p.z, +1, -1, -1);
        public static double p2NXQ2(Point p) => dNXQ_corner_eta(p.x, p.y, p.z, +1, -1, -1);
        public static double p3NXQ2(Point p) => dNXQ_corner_zeta(p.x, p.y, p.z, +1, -1, -1);
        public static double p1NXQ3(Point p) => dNXQ_corner_xi(p.x, p.y, p.z, +1, +1, -1);
        public static double p2NXQ3(Point p) => dNXQ_corner_eta(p.x, p.y, p.z, +1, +1, -1);
        public static double p3NXQ3(Point p) => dNXQ_corner_zeta(p.x, p.y, p.z, +1, +1, -1);
        public static double p1NXQ4(Point p) => dNXQ_corner_xi(p.x, p.y, p.z, -1, +1, -1);
        public static double p2NXQ4(Point p) => dNXQ_corner_eta(p.x, p.y, p.z, -1, +1, -1);
        public static double p3NXQ4(Point p) => dNXQ_corner_zeta(p.x, p.y, p.z, -1, +1, -1);
        public static double p1NXQ5(Point p) => dNXQ_corner_xi(p.x, p.y, p.z, -1, -1, +1);
        public static double p2NXQ5(Point p) => dNXQ_corner_eta(p.x, p.y, p.z, -1, -1, +1);
        public static double p3NXQ5(Point p) => dNXQ_corner_zeta(p.x, p.y, p.z, -1, -1, +1);
        public static double p1NXQ6(Point p) => dNXQ_corner_xi(p.x, p.y, p.z, +1, -1, +1);
        public static double p2NXQ6(Point p) => dNXQ_corner_eta(p.x, p.y, p.z, +1, -1, +1);
        public static double p3NXQ6(Point p) => dNXQ_corner_zeta(p.x, p.y, p.z, +1, -1, +1);
        public static double p1NXQ7(Point p) => dNXQ_corner_xi(p.x, p.y, p.z, +1, +1, +1);
        public static double p2NXQ7(Point p) => dNXQ_corner_eta(p.x, p.y, p.z, +1, +1, +1);
        public static double p3NXQ7(Point p) => dNXQ_corner_zeta(p.x, p.y, p.z, +1, +1, +1);
        public static double p1NXQ8(Point p) => dNXQ_corner_xi(p.x, p.y, p.z, -1, +1, +1);
        public static double p2NXQ8(Point p) => dNXQ_corner_eta(p.x, p.y, p.z, -1, +1, +1);
        public static double p3NXQ8(Point p) => dNXQ_corner_zeta(p.x, p.y, p.z, -1, +1, +1);

        // Midside nodes 9-20
        // 9: ξ-edge, η=-1, ζ=-1
        public static double p1NXQ9(Point p) => dNXQ_edge_xi_dxi(p.x, p.y, p.z, -1, -1);
        public static double p2NXQ9(Point p) => dNXQ_edge_xi_deta(p.x, p.y, p.z, -1, -1);
        public static double p3NXQ9(Point p) => dNXQ_edge_xi_dzeta(p.x, p.y, p.z, -1, -1);
        // 10: η-edge, ξ=+1, ζ=-1
        public static double p1NXQ10(Point p) => dNXQ_edge_eta_dxi(p.x, p.y, p.z, +1, -1);
        public static double p2NXQ10(Point p) => dNXQ_edge_eta_deta(p.x, p.y, p.z, +1, -1);
        public static double p3NXQ10(Point p) => dNXQ_edge_eta_dzeta(p.x, p.y, p.z, +1, -1);
        // 11: ξ-edge, η=+1, ζ=-1
        public static double p1NXQ11(Point p) => dNXQ_edge_xi_dxi(p.x, p.y, p.z, +1, -1);
        public static double p2NXQ11(Point p) => dNXQ_edge_xi_deta(p.x, p.y, p.z, +1, -1);
        public static double p3NXQ11(Point p) => dNXQ_edge_xi_dzeta(p.x, p.y, p.z, +1, -1);
        // 12: η-edge, ξ=-1, ζ=-1
        public static double p1NXQ12(Point p) => dNXQ_edge_eta_dxi(p.x, p.y, p.z, -1, -1);
        public static double p2NXQ12(Point p) => dNXQ_edge_eta_deta(p.x, p.y, p.z, -1, -1);
        public static double p3NXQ12(Point p) => dNXQ_edge_eta_dzeta(p.x, p.y, p.z, -1, -1);
        // 13: ζ-edge, ξ=-1, η=-1
        public static double p1NXQ13(Point p) => dNXQ_edge_zeta_dxi(p.x, p.y, p.z, -1, -1);
        public static double p2NXQ13(Point p) => dNXQ_edge_zeta_deta(p.x, p.y, p.z, -1, -1);
        public static double p3NXQ13(Point p) => dNXQ_edge_zeta_dzeta(p.x, p.y, p.z, -1, -1);
        // 14: ζ-edge, ξ=+1, η=-1
        public static double p1NXQ14(Point p) => dNXQ_edge_zeta_dxi(p.x, p.y, p.z, +1, -1);
        public static double p2NXQ14(Point p) => dNXQ_edge_zeta_deta(p.x, p.y, p.z, +1, -1);
        public static double p3NXQ14(Point p) => dNXQ_edge_zeta_dzeta(p.x, p.y, p.z, +1, -1);
        // 15: ζ-edge, ξ=+1, η=+1
        public static double p1NXQ15(Point p) => dNXQ_edge_zeta_dxi(p.x, p.y, p.z, +1, +1);
        public static double p2NXQ15(Point p) => dNXQ_edge_zeta_deta(p.x, p.y, p.z, +1, +1);
        public static double p3NXQ15(Point p) => dNXQ_edge_zeta_dzeta(p.x, p.y, p.z, +1, +1);
        // 16: ζ-edge, ξ=-1, η=+1
        public static double p1NXQ16(Point p) => dNXQ_edge_zeta_dxi(p.x, p.y, p.z, -1, +1);
        public static double p2NXQ16(Point p) => dNXQ_edge_zeta_deta(p.x, p.y, p.z, -1, +1);
        public static double p3NXQ16(Point p) => dNXQ_edge_zeta_dzeta(p.x, p.y, p.z, -1, +1);
        // 17: ξ-edge, η=-1, ζ=+1
        public static double p1NXQ17(Point p) => dNXQ_edge_xi_dxi(p.x, p.y, p.z, -1, +1);
        public static double p2NXQ17(Point p) => dNXQ_edge_xi_deta(p.x, p.y, p.z, -1, +1);
        public static double p3NXQ17(Point p) => dNXQ_edge_xi_dzeta(p.x, p.y, p.z, -1, +1);
        // 18: η-edge, ξ=+1, ζ=+1
        public static double p1NXQ18(Point p) => dNXQ_edge_eta_dxi(p.x, p.y, p.z, +1, +1);
        public static double p2NXQ18(Point p) => dNXQ_edge_eta_deta(p.x, p.y, p.z, +1, +1);
        public static double p3NXQ18(Point p) => dNXQ_edge_eta_dzeta(p.x, p.y, p.z, +1, +1);
        // 19: ξ-edge, η=+1, ζ=+1
        public static double p1NXQ19(Point p) => dNXQ_edge_xi_dxi(p.x, p.y, p.z, +1, +1);
        public static double p2NXQ19(Point p) => dNXQ_edge_xi_deta(p.x, p.y, p.z, +1, +1);
        public static double p3NXQ19(Point p) => dNXQ_edge_xi_dzeta(p.x, p.y, p.z, +1, +1);
        // 20: η-edge, ξ=-1, ζ=+1
        public static double p1NXQ20(Point p) => dNXQ_edge_eta_dxi(p.x, p.y, p.z, -1, +1);
        public static double p2NXQ20(Point p) => dNXQ_edge_eta_deta(p.x, p.y, p.z, -1, +1);
        public static double p3NXQ20(Point p) => dNXQ_edge_eta_dzeta(p.x, p.y, p.z, -1, +1);

        // ===== Hexahedron Cubic Lagrange (27 nodes) Derivatives =====
        // N_ijk(ξ,η,ζ) = Li(ξ) * Lj(η) * Lk(ζ)
        // dN/dξ = dLi/dξ * Lj(η) * Lk(ζ), etc.
        private static double dLi1(double xi) => xi - 0.5;       // d/dξ of ½ξ(ξ-1)
        private static double dLi2(double xi) => xi + 0.5;       // d/dξ of ½ξ(ξ+1)
        private static double dLi3(double xi) => -2.0 * xi;      // d/dξ of 1-ξ²
        private static double Li1(double xi) => 0.5 * xi * (xi - 1.0);
        private static double Li2(double xi) => 0.5 * xi * (xi + 1.0);
        private static double Li3(double xi) => 1.0 - xi * xi;

        private static double dNXC_dxi(double x, double y, double z, int i, int j, int k)
        {
            double dx = i switch { 1 => dLi1(x), 2 => dLi2(x), 3 => dLi3(x), _ => 0 };
            double ny = j switch { 1 => Li1(y), 2 => Li2(y), 3 => Li3(y), _ => 0 };
            double nz = k switch { 1 => Li1(z), 2 => Li2(z), 3 => Li3(z), _ => 0 };
            return dx * ny * nz;
        }
        private static double dNXC_deta(double x, double y, double z, int i, int j, int k)
        {
            double nx = i switch { 1 => Li1(x), 2 => Li2(x), 3 => Li3(x), _ => 0 };
            double dy = j switch { 1 => dLi1(y), 2 => dLi2(y), 3 => dLi3(y), _ => 0 };
            double nz = k switch { 1 => Li1(z), 2 => Li2(z), 3 => Li3(z), _ => 0 };
            return nx * dy * nz;
        }
        private static double dNXC_dzeta(double x, double y, double z, int i, int j, int k)
        {
            double nx = i switch { 1 => Li1(x), 2 => Li2(x), 3 => Li3(x), _ => 0 };
            double ny = j switch { 1 => Li1(y), 2 => Li2(y), 3 => Li3(y), _ => 0 };
            double dz = k switch { 1 => dLi1(z), 2 => dLi2(z), 3 => dLi3(z), _ => 0 };
            return nx * ny * dz;
        }

        // ζ=-1 layer (k=1): nodes 1-9
        public static double p1NXC1(Point p) => dNXC_dxi(p.x,p.y,p.z,1,1,1); public static double p2NXC1(Point p) => dNXC_deta(p.x,p.y,p.z,1,1,1); public static double p3NXC1(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,1,1);
        public static double p1NXC2(Point p) => dNXC_dxi(p.x,p.y,p.z,3,1,1); public static double p2NXC2(Point p) => dNXC_deta(p.x,p.y,p.z,3,1,1); public static double p3NXC2(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,1,1);
        public static double p1NXC3(Point p) => dNXC_dxi(p.x,p.y,p.z,2,1,1); public static double p2NXC3(Point p) => dNXC_deta(p.x,p.y,p.z,2,1,1); public static double p3NXC3(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,1,1);
        public static double p1NXC4(Point p) => dNXC_dxi(p.x,p.y,p.z,1,3,1); public static double p2NXC4(Point p) => dNXC_deta(p.x,p.y,p.z,1,3,1); public static double p3NXC4(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,3,1);
        public static double p1NXC5(Point p) => dNXC_dxi(p.x,p.y,p.z,3,3,1); public static double p2NXC5(Point p) => dNXC_deta(p.x,p.y,p.z,3,3,1); public static double p3NXC5(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,3,1);
        public static double p1NXC6(Point p) => dNXC_dxi(p.x,p.y,p.z,2,3,1); public static double p2NXC6(Point p) => dNXC_deta(p.x,p.y,p.z,2,3,1); public static double p3NXC6(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,3,1);
        public static double p1NXC7(Point p) => dNXC_dxi(p.x,p.y,p.z,1,2,1); public static double p2NXC7(Point p) => dNXC_deta(p.x,p.y,p.z,1,2,1); public static double p3NXC7(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,2,1);
        public static double p1NXC8(Point p) => dNXC_dxi(p.x,p.y,p.z,3,2,1); public static double p2NXC8(Point p) => dNXC_deta(p.x,p.y,p.z,3,2,1); public static double p3NXC8(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,2,1);
        public static double p1NXC9(Point p) => dNXC_dxi(p.x,p.y,p.z,2,2,1); public static double p2NXC9(Point p) => dNXC_deta(p.x,p.y,p.z,2,2,1); public static double p3NXC9(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,2,1);
        // ζ=0 layer (k=3): nodes 10-18
        public static double p1NXC10(Point p) => dNXC_dxi(p.x,p.y,p.z,1,1,3); public static double p2NXC10(Point p) => dNXC_deta(p.x,p.y,p.z,1,1,3); public static double p3NXC10(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,1,3);
        public static double p1NXC11(Point p) => dNXC_dxi(p.x,p.y,p.z,3,1,3); public static double p2NXC11(Point p) => dNXC_deta(p.x,p.y,p.z,3,1,3); public static double p3NXC11(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,1,3);
        public static double p1NXC12(Point p) => dNXC_dxi(p.x,p.y,p.z,2,1,3); public static double p2NXC12(Point p) => dNXC_deta(p.x,p.y,p.z,2,1,3); public static double p3NXC12(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,1,3);
        public static double p1NXC13(Point p) => dNXC_dxi(p.x,p.y,p.z,1,3,3); public static double p2NXC13(Point p) => dNXC_deta(p.x,p.y,p.z,1,3,3); public static double p3NXC13(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,3,3);
        public static double p1NXC14(Point p) => dNXC_dxi(p.x,p.y,p.z,3,3,3); public static double p2NXC14(Point p) => dNXC_deta(p.x,p.y,p.z,3,3,3); public static double p3NXC14(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,3,3);
        public static double p1NXC15(Point p) => dNXC_dxi(p.x,p.y,p.z,2,3,3); public static double p2NXC15(Point p) => dNXC_deta(p.x,p.y,p.z,2,3,3); public static double p3NXC15(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,3,3);
        public static double p1NXC16(Point p) => dNXC_dxi(p.x,p.y,p.z,1,2,3); public static double p2NXC16(Point p) => dNXC_deta(p.x,p.y,p.z,1,2,3); public static double p3NXC16(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,2,3);
        public static double p1NXC17(Point p) => dNXC_dxi(p.x,p.y,p.z,3,2,3); public static double p2NXC17(Point p) => dNXC_deta(p.x,p.y,p.z,3,2,3); public static double p3NXC17(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,2,3);
        public static double p1NXC18(Point p) => dNXC_dxi(p.x,p.y,p.z,2,2,3); public static double p2NXC18(Point p) => dNXC_deta(p.x,p.y,p.z,2,2,3); public static double p3NXC18(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,2,3);
        // ζ=+1 layer (k=2): nodes 19-27
        public static double p1NXC19(Point p) => dNXC_dxi(p.x,p.y,p.z,1,1,2); public static double p2NXC19(Point p) => dNXC_deta(p.x,p.y,p.z,1,1,2); public static double p3NXC19(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,1,2);
        public static double p1NXC20(Point p) => dNXC_dxi(p.x,p.y,p.z,3,1,2); public static double p2NXC20(Point p) => dNXC_deta(p.x,p.y,p.z,3,1,2); public static double p3NXC20(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,1,2);
        public static double p1NXC21(Point p) => dNXC_dxi(p.x,p.y,p.z,2,1,2); public static double p2NXC21(Point p) => dNXC_deta(p.x,p.y,p.z,2,1,2); public static double p3NXC21(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,1,2);
        public static double p1NXC22(Point p) => dNXC_dxi(p.x,p.y,p.z,1,3,2); public static double p2NXC22(Point p) => dNXC_deta(p.x,p.y,p.z,1,3,2); public static double p3NXC22(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,3,2);
        public static double p1NXC23(Point p) => dNXC_dxi(p.x,p.y,p.z,3,3,2); public static double p2NXC23(Point p) => dNXC_deta(p.x,p.y,p.z,3,3,2); public static double p3NXC23(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,3,2);
        public static double p1NXC24(Point p) => dNXC_dxi(p.x,p.y,p.z,2,3,2); public static double p2NXC24(Point p) => dNXC_deta(p.x,p.y,p.z,2,3,2); public static double p3NXC24(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,3,2);
        public static double p1NXC25(Point p) => dNXC_dxi(p.x,p.y,p.z,1,2,2); public static double p2NXC25(Point p) => dNXC_deta(p.x,p.y,p.z,1,2,2); public static double p3NXC25(Point p) => dNXC_dzeta(p.x,p.y,p.z,1,2,2);
        public static double p1NXC26(Point p) => dNXC_dxi(p.x,p.y,p.z,3,2,2); public static double p2NXC26(Point p) => dNXC_deta(p.x,p.y,p.z,3,2,2); public static double p3NXC26(Point p) => dNXC_dzeta(p.x,p.y,p.z,3,2,2);
        public static double p1NXC27(Point p) => dNXC_dxi(p.x,p.y,p.z,2,2,2); public static double p2NXC27(Point p) => dNXC_deta(p.x,p.y,p.z,2,2,2); public static double p3NXC27(Point p) => dNXC_dzeta(p.x,p.y,p.z,2,2,2);
    }
}
