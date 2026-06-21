namespace NumIntegration
{
    /// <summary>
    /// Tip graničnog uslova na površi 3D elementa.
    /// </summary>
    public enum FaceBoundaryConditionType
    {
        /// <summary>Zadani toplotni tok q [W/m²] (Neumann)</summary>
        HeatFlux,
        /// <summary>Konvekcija α [W/(m²·°C)], ϑ∞ [°C] (Robin)</summary>
        Convection
    }

    /// <summary>
    /// Opis graničnog uslova na površi heksaedarskog konačnog elementa.
    /// </summary>
    public class FaceBoundaryCondition
    {
        /// <summary>Indeks elementa u mreži (0-based).</summary>
        public int ElementIndex { get; set; }
        /// <summary>Indeks strane (0–5). 
        /// 0:ζ=−1(dno), 1:ζ=+1(vrh), 2:η=−1, 3:η=+1, 4:ξ=−1, 5:ξ=+1.</summary>
        public int FaceIndex { get; set; }
        /// <summary>Tip graničnog uslova.</summary>
        public FaceBoundaryConditionType BCType { get; set; }
        /// <summary>Zadati toplotni tok q [W/m²] (za HeatFlux).</summary>
        public double HeatFlux { get; set; }
        /// <summary>Koeficijent konvekcije α [W/(m²·°C)] (za Convection).</summary>
        public double ConvectionCoeff { get; set; }
        /// <summary>Temperatura okoline ϑ∞ [°C] (za Convection).</summary>
        public double AmbientTemp { get; set; }
    }

    /// <summary>
    /// Klasa za formiranje 3D mreže konačnih elemenata (heksaedar, tetraedar),
    /// sklapanje integrala po domenu i sklapanje globalne matrice krutosti
    /// i vektora opterećenja za stacionarni prenos toplote kondukcijom.
    /// 
    /// Koristi izoparametarsku formulaciju sa Gaussovom numeričkom integracijom.
    /// Podržava linearne heksaedre i tetraedre.
    /// </summary>
    public class UniformMesh3D
    {
        private readonly FiniteElement[] _elements;
        private readonly double[] _conductivities;
        private readonly Dictionary<int, (double x, double y, double z)> _nodes;
        private readonly int _nodeCount;

        /// <summary>Unutrašnji izvor toplote G [W/m³]. Podrazumevano: 0.</summary>
        public double HeatSource { get; set; } = 0.0;

        /// <summary>Lista graničnih uslova po površima elemenata.</summary>
        public List<FaceBoundaryCondition> FaceBCs { get; set; } = new();

        /// <summary>Ukupan broj čvorova u mreži.</summary>
        public int NodeCount => _nodeCount;

        /// <summary>Ukupan broj elemenata u mreži.</summary>
        public int ElementCount => _elements.Length;

        /// <summary>
        /// Konstruktor sa provodnostima po elementu.
        /// </summary>
        public UniformMesh3D(FiniteElement[] elements, double[] conductivities)
        {
            _elements = elements ?? throw new ArgumentNullException(nameof(elements));
            _conductivities = conductivities ?? throw new ArgumentNullException(nameof(conductivities));

            if (elements.Length == 0)
                throw new ArgumentException("Mreža mora imati barem jedan element.");
            if (conductivities.Length != elements.Length)
                throw new ArgumentException(
                    $"Broj koeficijenata provodnosti ({conductivities.Length}) "
                    + $"mora odgovarati broju elemenata ({elements.Length}).");

            for (int e = 0; e < elements.Length; e++)
            {
                var ft = elements[e].ft;
                if (ft != FEType.Hexaedron && ft != FEType.Tetrahedron)
                    throw new ArgumentException(
                        $"Element {e}: očekivan 3D element (Hexaedron/Tetrahedron), dobijen {ft}.");
                if (elements[e].fo != FEOrder.Linear)
                    throw new ArgumentException(
                        $"Element {e}: podržan samo Linear red, dobijen {elements[e].fo}.");
            }

            // Izgradnja globalne numeracije čvorova
            _nodes = new Dictionary<int, (double x, double y, double z)>();
            foreach (var fe in _elements)
            {
                foreach (var node in fe.nodes)
                {
                    int id = int.Parse(node.id);
                    if (!_nodes.ContainsKey(id))
                        _nodes[id] = (node.x, node.y, node.z);
                }
            }
            _nodeCount = _nodes.Count;
        }

        /// <summary>
        /// Konstruktor bez provodnosti (za backward kompatibilnost s Integrate).
        /// </summary>
        public UniformMesh3D(FiniteElement[] elements)
        {
            _elements = elements ?? throw new ArgumentNullException(nameof(elements));

            if (elements.Length == 0)
                throw new ArgumentException("Mreža mora imati barem jedan element.");

            for (int e = 0; e < elements.Length; e++)
            {
                if (elements[e].ft != FEType.Hexaedron && elements[e].ft != FEType.Tetrahedron)
                    throw new ArgumentException(
                        $"Element {e}: očekivan 3D element (Hexaedron/Tetrahedron), dobijen {elements[e].ft}.");
            }

            _conductivities = new double[elements.Length]; // nulto
            _nodes = new Dictionary<int, (double, double, double)>();
            foreach (var fe in _elements)
            {
                foreach (var node in fe.nodes)
                {
                    int id = int.Parse(node.id);
                    if (!_nodes.ContainsKey(id))
                        _nodes[id] = (node.x, node.y, node.z);
                }
            }
            _nodeCount = _nodes.Count;
        }

        /// <summary>
        /// Integrira zadati izraz po svim elementima mreže i vraća sumu.
        /// </summary>
        public double Integrate(string expression, DOP dop = DOP.Cubic)
        {
            double total = 0.0;
            for (int e = 0; e < _elements.Length; e++)
            {
                var num = new Numeric(_elements[e]);
                total += num.Integrate(expression, dop);
            }
            return total;
        }

        /// <summary>
        /// Sklapa globalnu matricu krutosti [K] i vektor opterećenja {F}
        /// za stacionarni 3D problem provođenja toplote.
        /// </summary>
        public (double[,] K, double[] F) Assemble()
        {
            int n = _nodeCount;
            double[,] K = new double[n, n];
            double[] F = new double[n];

            var gw = Gaussian.NullsAndWeights(FEType.Hexaedron, DOP.Linear);
            var jac = new Jacobian(FEType.Hexaedron, FEOrder.Linear);

            for (int e = 0; e < _elements.Length; e++)
            {
                var fe = _elements[e];
                double lambda = _conductivities[e];
                int[] idx = fe.nodes.Select(nd => int.Parse(nd.id) - 1).ToArray();
                int m = fe.nodes.Length;

                // --- Matrica kondukcije ---
                double[,] Ke = new double[m, m];
                for (int g = 0; g < gw.n; g++)
                {
                    double xi = gw.gi[g].x, eta = gw.gi[g].y, zeta = gw.gi[g].z;
                    double w = gw.wi[g];

                    double[,] dN = ComputeHexDerivatives(xi, eta, zeta);
                    double[,] J = ComputeJacobian3D(fe, dN);
                    double detJ = jac.Calculate(fe.nodes, new Point(xi, eta, zeta));
                    double[,] invJ = Inverse3x3(J, detJ);
                    double[,] B = Multiply3x3_3x8(invJ, dN);

                    double factor = lambda * detJ * w;
                    for (int ia = 0; ia < m; ia++)
                        for (int ib = 0; ib < m; ib++)
                        {
                            double sum = B[0, ia] * B[0, ib]
                                       + B[1, ia] * B[1, ib]
                                       + B[2, ia] * B[2, ib];
                            Ke[ia, ib] += factor * sum;
                        }
                }

                // Sklapanje Ke u globalnu matricu
                for (int ia = 0; ia < m; ia++)
                    for (int ib = 0; ib < m; ib++)
                        K[idx[ia], idx[ib]] += Ke[ia, ib];

                // --- Vektor unutrašnjeg izvora toplote ---
                if (HeatSource != 0.0)
                {
                    for (int g = 0; g < gw.n; g++)
                    {
                        double xi = gw.gi[g].x, eta = gw.gi[g].y, zeta = gw.gi[g].z;
                        double w = gw.wi[g];
                        double[] N = GetHexShapeFunctions(xi, eta, zeta);
                        double detJ = jac.Calculate(fe.nodes, new Point(xi, eta, zeta));

                        for (int ia = 0; ia < m; ia++)
                            F[idx[ia]] += HeatSource * N[ia] * detJ * w;
                    }
                }
            }

            // --- Granični uslovi po površima ---
            foreach (var bc in FaceBCs)
                ApplyFaceBC(K, F, bc);

            return (K, F);
        }

        // ==================== POMOĆNE METODE ZA SKLAPANJE ====================

        /// <summary>Derivacije baznih funkcija linearnog heksaedra (3×8).</summary>
        private static double[,] ComputeHexDerivatives(double xi, double eta, double zeta)
        {
            int[] sx = { -1, 1, 1, -1, -1, 1, 1, -1 };
            int[] sy = { -1, -1, 1, 1, -1, -1, 1, 1 };
            int[] sz = { -1, -1, -1, -1, 1, 1, 1, 1 };

            double[,] dN = new double[3, 8];
            for (int i = 0; i < 8; i++)
            {
                dN[0, i] = 0.125 * sx[i] * (1 + sy[i] * eta) * (1 + sz[i] * zeta);
                dN[1, i] = 0.125 * sy[i] * (1 + sx[i] * xi) * (1 + sz[i] * zeta);
                dN[2, i] = 0.125 * sz[i] * (1 + sx[i] * xi) * (1 + sy[i] * eta);
            }
            return dN;
        }

        /// <summary>Bazne funkcije linearnog heksaedra (8).</summary>
        private static double[] GetHexShapeFunctions(double xi, double eta, double zeta)
        {
            return new double[]
            {
                0.125 * (1 - xi) * (1 - eta) * (1 - zeta),
                0.125 * (1 + xi) * (1 - eta) * (1 - zeta),
                0.125 * (1 + xi) * (1 + eta) * (1 - zeta),
                0.125 * (1 - xi) * (1 + eta) * (1 - zeta),
                0.125 * (1 - xi) * (1 - eta) * (1 + zeta),
                0.125 * (1 + xi) * (1 - eta) * (1 + zeta),
                0.125 * (1 + xi) * (1 + eta) * (1 + zeta),
                0.125 * (1 - xi) * (1 + eta) * (1 + zeta),
            };
        }

        /// <summary>Jakobijan 3×3 iz dN i koordinata čvorova.</summary>
        private static double[,] ComputeJacobian3D(FiniteElement fe, double[,] dN)
        {
            double[,] J = new double[3, 3];
            for (int i = 0; i < fe.nodes.Length; i++)
            {
                J[0, 0] += dN[0, i] * fe.nodes[i].x;
                J[0, 1] += dN[0, i] * fe.nodes[i].y;
                J[0, 2] += dN[0, i] * fe.nodes[i].z;
                J[1, 0] += dN[1, i] * fe.nodes[i].x;
                J[1, 1] += dN[1, i] * fe.nodes[i].y;
                J[1, 2] += dN[1, i] * fe.nodes[i].z;
                J[2, 0] += dN[2, i] * fe.nodes[i].x;
                J[2, 1] += dN[2, i] * fe.nodes[i].y;
                J[2, 2] += dN[2, i] * fe.nodes[i].z;
            }
            return J;
        }

        /// <summary>Inverzija 3×3 matrice.</summary>
        private static double[,] Inverse3x3(double[,] m, double det)
        {
            double invDet = 1.0 / det;
            return new double[,]
            {
                { (m[1,1]*m[2,2] - m[1,2]*m[2,1]) * invDet,
                  (m[0,2]*m[2,1] - m[0,1]*m[2,2]) * invDet,
                  (m[0,1]*m[1,2] - m[0,2]*m[1,1]) * invDet },
                { (m[1,2]*m[2,0] - m[1,0]*m[2,2]) * invDet,
                  (m[0,0]*m[2,2] - m[0,2]*m[2,0]) * invDet,
                  (m[0,2]*m[1,0] - m[0,0]*m[1,2]) * invDet },
                { (m[1,0]*m[2,1] - m[1,1]*m[2,0]) * invDet,
                  (m[0,1]*m[2,0] - m[0,0]*m[2,1]) * invDet,
                  (m[0,0]*m[1,1] - m[0,1]*m[1,0]) * invDet }
            };
        }

        /// <summary>Množenje 3×3 · 3×8 = 3×8.</summary>
        private static double[,] Multiply3x3_3x8(double[,] A, double[,] B)
        {
            double[,] C = new double[3, 8];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 8; j++)
                    for (int k = 0; k < 3; k++)
                        C[i, j] += A[i, k] * B[k, j];
            return C;
        }

        /// <summary>Primjena graničnog uslova na površi heksaedra.</summary>
        private void ApplyFaceBC(double[,] K, double[] F, FaceBoundaryCondition bc)
        {
            var fe = _elements[bc.ElementIndex];
            if (fe.ft != FEType.Hexaedron)
                throw new NotSupportedException("FaceBC podržan samo za heksaedre.");

            int[] localToGlobal = fe.nodes.Select(nd => int.Parse(nd.id) - 1).ToArray();

            // Lokalni čvorovi koji pripadaju površi (4 čvora)
            int[] faceNodes = GetFaceLocalNodes(bc.FaceIndex);

            // Koja je prirodna koordinata fiksirana na ±1
            var (fixedName, fixedVal) = GetFaceFixedCoord(bc.FaceIndex);

            // Površina strane u fizičkim koordinatama
            double faceArea = ComputeFaceArea(fe, bc.FaceIndex);

            // 2D Gauss po površi (2×2 za linearnu)
            var gw2D = Gaussian.NullsAndWeights(FEType.Rectangle, DOP.Linear);

            for (int g = 0; g < gw2D.n; g++)
            {
                double u = gw2D.gi[g].x;  // ξ ili η
                double v = gw2D.gi[g].y;  // η ili ζ
                double w = gw2D.wi[g];

                // Rekonstrukcija 3D prirodnih koordinata na površi
                // u,v su dvije varijabilne koordinate (iz 2D Gaussa)
                (double xi, double eta, double zeta) = fixedName switch
                {
                    "xi"   => (fixedVal, u, v),
                    "eta"  => (u, fixedVal, v),
                    "zeta" => (u, v, fixedVal),
                    _ => throw new InvalidOperationException()
                };

                // Bazne funkcije na površi
                double[] N = GetHexShapeFunctions(xi, eta, zeta);

                // dS = (površina/4) · w  (Jakobijan za ravan kvadrat 2×2 → fizički)
                double ds = faceArea / 4.0 * w;

                if (bc.BCType == FaceBoundaryConditionType.Convection)
                {
                    double alpha = bc.ConvectionCoeff;
                    double theta_inf = bc.AmbientTemp;

                    for (int ia = 0; ia < 4; ia++)
                    {
                        int gi = localToGlobal[faceNodes[ia]];
                        for (int ib = 0; ib < 4; ib++)
                        {
                            int gj = localToGlobal[faceNodes[ib]];
                            K[gi, gj] += alpha * N[faceNodes[ia]] * N[faceNodes[ib]] * ds;
                        }
                        F[gi] += alpha * theta_inf * N[faceNodes[ia]] * ds;
                    }
                }
                else if (bc.BCType == FaceBoundaryConditionType.HeatFlux)
                {
                    double q = bc.HeatFlux;
                    for (int ia = 0; ia < 4; ia++)
                    {
                        int gi = localToGlobal[faceNodes[ia]];
                        F[gi] += -q * N[faceNodes[ia]] * ds;
                    }
                }
            }
        }

        /// <summary>Lokalni indeksi čvorova na svakoj od 6 strana heksaedra.</summary>
        private static int[] GetFaceLocalNodes(int faceIdx)
        {
            return faceIdx switch
            {
                0 => new[] { 0, 3, 2, 1 },  // ζ=−1 (dno)
                1 => new[] { 4, 5, 6, 7 },  // ζ=+1 (vrh)
                2 => new[] { 0, 1, 5, 4 },  // η=−1
                3 => new[] { 3, 2, 6, 7 },  // η=+1
                4 => new[] { 0, 4, 7, 3 },  // ξ=−1
                5 => new[] { 1, 2, 6, 5 },  // ξ=+1
                _ => throw new ArgumentException($"Neispravan indeks strane: {faceIdx}")
            };
        }

        /// <summary>
        /// Vraća ime fiksirane prirodne koordinate i njenu vrijednost (±1) za datu stranu.
        /// Preostale dvije koordinate variraju u [-1,1] i preslikavaju se na (u,v) Gaussa.
        /// </summary>
        private static (string fixedName, double fixedVal)
            GetFaceFixedCoord(int faceIdx)
        {
            return faceIdx switch
            {
                0 => ("zeta", -1.0),  // dno: ζ=−1
                1 => ("zeta", +1.0),  // vrh: ζ=+1
                2 => ("eta",  -1.0),  // prednja: η=−1
                3 => ("eta",  +1.0),  // zadnja: η=+1
                4 => ("xi",   -1.0),  // lijeva: ξ=−1
                5 => ("xi",   +1.0),  // desna: ξ=+1
                _ => throw new ArgumentException($"Neispravan indeks strane: {faceIdx}")
            };
        }

        /// <summary>Približna površina strane heksaedra u fizičkim koordinatama.</summary>
        private static double ComputeFaceArea(FiniteElement fe, int faceIdx)
        {
            var nodes = GetFaceLocalNodes(faceIdx);
            var p0 = fe.nodes[nodes[0]];
            var p1 = fe.nodes[nodes[1]];
            var p2 = fe.nodes[nodes[2]];
            var p3 = fe.nodes[nodes[3]];

            // Podjela četvorougla na dva trougla
            double AreaTriangle(double x1, double y1, double z1,
                                double x2, double y2, double z2,
                                double x3, double y3, double z3)
            {
                double ax = x2 - x1, ay = y2 - y1, az = z2 - z1;
                double bx = x3 - x1, by = y3 - y1, bz = z3 - z1;
                double cx = ay * bz - az * by;
                double cy = az * bx - ax * bz;
                double cz = ax * by - ay * bx;
                return 0.5 * Math.Sqrt(cx * cx + cy * cy + cz * cz);
            }

            return AreaTriangle(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z, p2.x, p2.y, p2.z)
                 + AreaTriangle(p0.x, p0.y, p0.z, p2.x, p2.y, p2.z, p3.x, p3.y, p3.z);
        }

        // ==================== STATIČKE FABRIKE ====================

        /// <summary>
        /// Kreira uniformnu mrežu heksaedarskih elemenata nad pravougaonim domenom.
        /// Vraća UniformMesh3D sa zadatom provodnošću za sve elemente.
        /// </summary>
        public static UniformMesh3D CreateUniformHexMesh(
            double xMin, double xMax, int nx,
            double yMin, double yMax, int ny,
            double zMin, double zMax, int nz,
            double conductivity = 0.0,
            FEOrder order = FEOrder.Linear)
        {
            if (nx < 1 || ny < 1 || nz < 1)
                throw new ArgumentException("Broj podjela mora biti >= 1 u svakom pravcu.");

            double dx = (xMax - xMin) / nx;
            double dy = (yMax - yMin) / ny;
            double dz = (zMax - zMin) / nz;

            int totalElements = nx * ny * nz;
            var elements = new FiniteElement[totalElements];
            var conductivities = new double[totalElements];
            int idx = 0;

            for (int iz = 0; iz < nz; iz++)
            {
                double z1 = zMin + iz * dz;
                double z2 = z1 + dz;
                for (int iy = 0; iy < ny; iy++)
                {
                    double y1 = yMin + iy * dy;
                    double y2 = y1 + dy;
                    for (int ix = 0; ix < nx; ix++)
                    {
                        double x1 = xMin + ix * dx;
                        double x2 = x1 + dx;

                        elements[idx] = new FiniteElement()
                        {
                            nodes = new[]
                            {
                                new Node("1", x1, y1, z1),
                                new Node("2", x2, y1, z1),
                                new Node("3", x2, y2, z1),
                                new Node("4", x1, y2, z1),
                                new Node("5", x1, y1, z2),
                                new Node("6", x2, y1, z2),
                                new Node("7", x2, y2, z2),
                                new Node("8", x1, y2, z2),
                            },
                            ft = FEType.Hexaedron,
                            fo = order,
                        };
                        conductivities[idx] = conductivity;
                        idx++;
                    }
                }
            }

            return new UniformMesh3D(elements, conductivities);
        }

        /// <summary>Broj konačnih elemenata u mreži.</summary>
        public int ElementsCount => _elements.Length;

        /// <summary>Vraća element na zadatom indeksu.</summary>
        public FiniteElement this[int index] => _elements[index];
    }
}
