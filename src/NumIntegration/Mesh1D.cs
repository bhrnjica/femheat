namespace NumIntegration
{
    /// <summary>
    /// Klasa za formiranje 1D mreže konačnih elemenata i sklapanje
    /// globalne matrice krutosti i vektora opterećenja za stacionarni
    /// prenos toplote kroz ravni zid.
    /// 
    /// Podržava linearne i kvadratne linijske konačne elemente.
    /// Granični uvjeti: zadati toplotni tok (lijevo) i konvekcija (desno).
    /// </summary>
    public class Mesh1D
    {
        /// <summary>Zadati toplotni tok na lijevoj granici q [W/m²]</summary>
        public double LeftHeatFlux { get; set; }

        /// <summary>Koeficijent konvekcije na desnoj granici α [W/(m²·°C)]</summary>
        public double RightConvectionCoeff { get; set; }

        /// <summary>Temperatura okoline na desnoj strani ϑ∞ [°C]</summary>
        public double RightAmbientTemp { get; set; }

        private readonly FiniteElement[] _elements;
        private readonly double[] _conductivities;
        private readonly double[] _areas;
        private readonly int _nodeCount;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="elements">Niz konačnih elemenata (FEType.Line)</param>
        /// <param name="conductivities">Koeficijenti toplotne provodnosti λ [W/(m·°C)] za svaki element</param>
        /// <param name="areas">Površine poprečnog presjeka A [m²] za svaki element</param>
        public Mesh1D(FiniteElement[] elements, double[] conductivities, double[] areas)
        {
            _elements = elements ?? throw new ArgumentNullException(nameof(elements));
            _conductivities = conductivities ?? throw new ArgumentNullException(nameof(conductivities));
            _areas = areas ?? throw new ArgumentNullException(nameof(areas));

            if (elements.Length == 0)
                throw new ArgumentException("Mreža mora imati barem jedan element.");
            if (conductivities.Length != elements.Length)
                throw new ArgumentException(
                    $"Broj koeficijenata provodnosti ({conductivities.Length}) "
                    + $"mora odgovarati broju elemenata ({elements.Length}).");
            if (areas.Length != elements.Length)
                throw new ArgumentException(
                    $"Broj površina ({areas.Length}) "
                    + $"mora odgovarati broju elemenata ({elements.Length}).");

            // Validacija tipa elementa
            for (int e = 0; e < elements.Length; e++)
            {
                if (elements[e].ft != FEType.Line)
                    throw new ArgumentException(
                        $"Element {e}: očekivan FEType.Line, dobijen {elements[e].ft}.");
            }

            // Izračunavanje ukupnog broja čvorova
            _nodeCount = 0;
            for (int e = 0; e < elements.Length; e++)
            {
                int nodesPerElement = elements[e].nodes.Length;
                if (e == 0)
                    _nodeCount += nodesPerElement;
                else
                    // Svaki sljedeći element dijeli prvi čvor s prethodnim
                    _nodeCount += nodesPerElement - 1;
            }
        }

        /// <summary>
        /// Sklapa globalnu matricu krutosti [K] i vektor opterećenja {F}.
        /// </summary>
        public (double[,] K, double[] F) Assemble()
        {
            int n = _nodeCount;
            double[,] K = new double[n, n];
            double[] F = new double[n];

            int globalOffset = 0; // prvi globalni indeks tekućeg elementa

            for (int e = 0; e < _elements.Length; e++)
            {
                var fe = _elements[e];
                double lambda = _conductivities[e];
                double area = _areas[e];

                // Dužina elementa iz koordinata čvorova
                double l = fe.nodes[^1].x - fe.nodes[0].x;
                int nodesPerElem = fe.nodes.Length;

                // Lokalna matrica krutosti elementa
                double[,] Ke = ComputeElementStiffness(fe.fo, lambda, area, l);

                // Mapiranje lokalnih indeksa na globalne i superpozicija
                for (int a = 0; a < nodesPerElem; a++)
                {
                    int gi = globalOffset + a;
                    for (int b = 0; b < nodesPerElem; b++)
                    {
                        int gj = globalOffset + b;
                        K[gi, gj] += Ke[a, b];
                    }
                }

                // Ažuriranje offseta za sljedeći element
                // (dijelimo jedan čvor sa sljedećim elementom)
                globalOffset += nodesPerElem - 1;
            }

            // ---- Granični uvjeti ----

            // Lijevi granični uvjet: zadati toplotni tok q
            double A_left = _areas[0];
            F[0] += LeftHeatFlux * A_left;

            // Desni granični uvjet: konvekcija α, ϑ∞
            double A_right = _areas[^1];
            double alphaA = RightConvectionCoeff * A_right;
            K[n - 1, n - 1] += alphaA;
            F[n - 1] += alphaA * RightAmbientTemp;

            return (K, F);
        }

        /// <summary>
        /// Računa lokalnu matricu krutosti [K_e] za jedan 1D element.
        /// </summary>
        private static double[,] ComputeElementStiffness(
            FEOrder order, double lambda, double area, double l)
        {
            double coeff = lambda * area / l;

            return order switch
            {
                FEOrder.Linear => new double[,]
                {
                    {  coeff, -coeff },
                    { -coeff,  coeff },
                },

                FEOrder.Quadratic => new double[,]
                {
                    {  7.0 / 3.0 * coeff, -8.0 / 3.0 * coeff,  1.0 / 3.0 * coeff },
                    { -8.0 / 3.0 * coeff, 16.0 / 3.0 * coeff, -8.0 / 3.0 * coeff },
                    {  1.0 / 3.0 * coeff, -8.0 / 3.0 * coeff,  7.0 / 3.0 * coeff },
                },

                _ => throw new NotSupportedException(
                    $"FEOrder.{order} nije podržan u Mesh1D.")
            };
        }

        /// <summary>Broj čvorova u mreži.</summary>
        public int NodeCount => _nodeCount;

        /// <summary>Broj konačnih elemenata u mreži.</summary>
        public int ElementCount => _elements.Length;

        /// <summary>Vraća dužinu elementa na osnovu koordinata čvorova.</summary>
        public double GetElementLength(int index) =>
            _elements[index].nodes[^1].x - _elements[index].nodes[0].x;
    }
}
