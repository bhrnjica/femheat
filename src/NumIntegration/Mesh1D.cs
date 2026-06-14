namespace NumIntegration
{
    /// <summary>
    /// Tip geometrije za 1D prenos toplote.
    /// </summary>
    public enum GeometryType
    {
        /// <summary>Ravni zid (konstantan poprečni presek A)</summary>
        PlaneWall,
        /// <summary>Cilindrični zid (radijalni prenos, L — dužina cevi)</summary>
        Cylindrical
    }

    /// <summary>
    /// Klasa za formiranje 1D mreže konačnih elemenata i sklapanje
    /// globalne matrice krutosti i vektora opterećenja za stacionarni
    /// prenos toplote kroz ravni ili cilindrični zid.
    /// 
    /// Podržava linearne, kvadratne i kubne linijske konačne elemente.
    /// Granični uvjeti: zadati toplotni tok (lijevo) i konvekcija (desno).
    /// 
    /// Promjenjivi poprečni presek: kada je <see cref="VariableCrossSection"/> = true,
    /// niz areas sadrži površine po čvorovima (dužine = NodeCount). Površina
    /// unutar elementa se interpolira izoparametarski: A(ξ) = A_i·N₁(ξ) + A_j·N₂(ξ).
    /// Efektivna površina za kondukciju je aritmetička sredina (A_i + A_j)/2,
    /// dok se za granične uslove koristi stvarna površina u čvoru.
    /// </summary>
    public class Mesh1D
    {
        /// <summary>Zadati toplotni tok na lijevoj granici q [W/m²]</summary>
        public double LeftHeatFlux { get; set; }

        /// <summary>Koeficijent konvekcije na desnoj granici α [W/(m²·°C)]</summary>
        public double RightConvectionCoeff { get; set; }

        /// <summary>Temperatura okoline na desnoj strani ϑ∞ [°C]</summary>
        public double RightAmbientTemp { get; set; }

        /// <summary>Tip geometrije (ravni ili cilindrični zid). Podrazumevano: PlaneWall.</summary>
        public GeometryType Geometry { get; set; } = GeometryType.PlaneWall;

        /// <summary>
        /// Dužina cilindra L [m]. Koristi se samo kada je Geometry = Cylindrical.
        /// Za ravni zid se zanemaruje.
        /// </summary>
        public double CylinderLength { get; set; } = 1.0;

        /// <summary>
        /// Da li je poprečni presek promjenjiv duž domene.
        /// Kada je true, areas sadrži površine po čvorovima (NodeCount elemenata).
        /// Kada je false (podrazumevano), areas sadrži jednu površinu po elementu.
        /// </summary>
        public bool VariableCrossSection { get; set; } = false;

        /// <summary>
        /// Unutrašnji izvor toplote G [W/m³]. Ako je 0 (podrazumevano), nema
        /// generacije toplote unutar domene.
        /// </summary>
        public double HeatSource { get; set; } = 0.0;

        private readonly FiniteElement[] _elements;
        private readonly double[] _conductivities;
        private readonly double[] _areas;
        private readonly int _nodeCount;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="elements">Niz konačnih elemenata (FEType.Line)</param>
        /// <param name="conductivities">Koeficijenti toplotne provodnosti λ [W/(m·°C)] za svaki element</param>
        /// <param name="areas">
        /// Površine poprečnog presjeka. Ako je VariableCrossSection = false (default),
        /// niz sadrži jednu površinu po elementu (konstantan presek unutar elementa).
        /// Ako je VariableCrossSection = true, niz sadrži površine po čvorovima
        /// (dužina = ukupan broj čvorova u mreži).
        /// </param>
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
                    _nodeCount += nodesPerElement - 1;
            }

            // Validacija dužine niza areas zavisi od VariableCrossSection
            // (provjerava se u Assemble(), jer se svojstvo može postaviti
            //  nakon konstruktora)
        }

        /// <summary>
        /// Sklapa globalnu matricu krutosti [K] i vektor opterećenja {F}.
        /// </summary>
        public (double[,] K, double[] F) Assemble()
        {
            // Validacija dužine niza areas
            if (VariableCrossSection)
            {
                if (_areas.Length != _nodeCount)
                    throw new ArgumentException(
                        $"Kod promjenjivog preseka (VariableCrossSection=true), "
                        + $"broj površina ({_areas.Length}) mora odgovarati "
                        + $"broju čvorova ({_nodeCount}).");
            }
            else
            {
                if (_areas.Length != _elements.Length)
                    throw new ArgumentException(
                        $"Broj površina ({_areas.Length}) mora odgovarati "
                        + $"broju elemenata ({_elements.Length}). "
                        + $"Za promjenjivi presek postavite VariableCrossSection=true.");
            }

            int n = _nodeCount;
            double[,] K = new double[n, n];
            double[] F = new double[n];

            int globalOffset = 0; // prvi globalni indeks tekućeg elementa

            for (int e = 0; e < _elements.Length; e++)
            {
                var fe = _elements[e];
                double lambda = _conductivities[e];

                // Dužina elementa iz koordinata čvorova
                double r_i = fe.nodes[0].x;
                double r_j = fe.nodes[^1].x;
                double l = r_j - r_i;
                int nodesPerElem = fe.nodes.Length;

                // Efektivna površina za kondukciju
                double A_eff = Geometry switch
                {
                    GeometryType.Cylindrical => 2 * Math.PI * CylinderLength * (r_i + r_j) / 2.0,
                    _ => VariableCrossSection
                        ? (_areas[globalOffset] + _areas[globalOffset + nodesPerElem - 1]) / 2.0
                        : _areas[e]
                };

                // Lokalna matrica krutosti elementa
                double[,] Ke = ComputeElementStiffness(fe.fo, lambda, A_eff, l);

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

                // Vektor opterećenja od unutrašnjeg izvora toplote G
                if (HeatSource != 0.0 && fe.fo == FEOrder.Linear)
                {
                    // Za linearni KE: {f_G} = G·A_eff·l/2 · {1, 1}^T  (jed. 05-37)
                    // Za cilindrični: A_eff je već 2πL·r̄, pa je rezultat isti
                    double fe_G = HeatSource * A_eff * l / 2.0;
                    F[globalOffset] += fe_G;
                    F[globalOffset + nodesPerElem - 1] += fe_G;
                }

                // Ažuriranje offseta za sljedeći element
                globalOffset += nodesPerElem - 1;
            }

            // ---- Granični uvjeti ----

            // Lijevi granični uvjet: zadati toplotni tok q
            double A_left = Geometry == GeometryType.Cylindrical
                ? 2 * Math.PI * _elements[0].nodes[0].x * CylinderLength
                : VariableCrossSection ? _areas[0] : _areas[0];
            F[0] += LeftHeatFlux * A_left;

            // Desni granični uvjet: konvekcija α, ϑ∞
            double A_right = Geometry == GeometryType.Cylindrical
                ? 2 * Math.PI * _elements[^1].nodes[^1].x * CylinderLength
                : VariableCrossSection ? _areas[^1] : _areas[^1];
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

                FEOrder.Cubic => new double[,]
                {
                    {  148.0 / 40.0 * coeff, -189.0 / 40.0 * coeff,   54.0 / 40.0 * coeff,  -13.0 / 40.0 * coeff },
                    { -189.0 / 40.0 * coeff,  432.0 / 40.0 * coeff, -297.0 / 40.0 * coeff,   54.0 / 40.0 * coeff },
                    {   54.0 / 40.0 * coeff, -297.0 / 40.0 * coeff,  432.0 / 40.0 * coeff, -189.0 / 40.0 * coeff },
                    {  -13.0 / 40.0 * coeff,   54.0 / 40.0 * coeff, -189.0 / 40.0 * coeff,  148.0 / 40.0 * coeff },
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
