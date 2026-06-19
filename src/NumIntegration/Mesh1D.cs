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

        // ---------- Svojstva za analizu rebara (fin analysis) ----------

        /// <summary>
        /// Da li je aktivna analiza rebra (fin). Kada je true, površinska
        /// konvekcija se dodaje duž svakog elementa preko perimetara.
        /// </summary>
        public bool IsFin { get; set; } = false;

        /// <summary>
        /// Koeficijent površinske konvekcije α [W/(m²·°C)] duž rebra.
        /// Koristi se samo kada je IsFin = true.
        /// </summary>
        public double SurfaceConvectionCoeff { get; set; }

        /// <summary>
        /// Temperatura okoline za površinsku konvekciju ϑ∞ [°C].
        /// Koristi se samo kada je IsFin = true.
        /// </summary>
        public double SurfaceAmbientTemp { get; set; }

        /// <summary>
        /// Obimi (perimetri) po čvorovima O [m]. Dužina niza mora 
        /// odgovarati ukupnom broju čvorova (NodeCount).
        /// Koristi se samo kada je IsFin = true.
        /// </summary>
        public double[]? Perimeters { get; set; }

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

            // Validacija fin parametara
            if (IsFin && Perimeters == null)
                throw new ArgumentException(
                    "Kod analize rebra (IsFin=true), Perimeters mora biti zadat.");
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
                double[,] Ke;
                if (VariableCrossSection && fe.fo == FEOrder.Quadratic)
                {
                    // Za kvadratni KE sa promjenjivim presjekom koristi se
                    // numerička integracija (Gauss 2 tačke) jer A(ξ) nije
                    // konstanto, pa se ne može izvući ispred integrala.
                    Ke = ComputeQuadraticVariableStiffness(lambda, l, globalOffset, nodesPerElem);
                }
                else
                {
                    Ke = ComputeElementStiffness(fe.fo, lambda, A_eff, l);
                }

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

                // Površinska konvekcija duž rebra (fin analysis)
                if (IsFin)
                {
                    if (Perimeters == null || Perimeters.Length != _nodeCount)
                        throw new ArgumentException(
                            $"Kod analize rebra (IsFin=true), Perimeters mora imati "
                            + $"{_nodeCount} elemenata (po jedan za svaki čvor).");

                    double O_i = Perimeters[globalOffset];
                    double O_j = Perimeters[globalOffset + nodesPerElem - 1];
                    double alpha = SurfaceConvectionCoeff;
                    double theta_inf = SurfaceAmbientTemp;

                    if (fe.fo == FEOrder.Linear)
                    {
                        // Matrica površinske konvekcije (jed. 05-104):
                        // Kα = (α·l/12) · [[3Oᵢ+Oⱼ, Oᵢ+Oⱼ], [Oᵢ+Oⱼ, Oᵢ+3Oⱼ]]
                        double cf = alpha * l / 12.0;
                        K[globalOffset, globalOffset] += cf * (3 * O_i + O_j);
                        K[globalOffset, globalOffset + 1] += cf * (O_i + O_j);
                        K[globalOffset + 1, globalOffset] += cf * (O_i + O_j);
                        K[globalOffset + 1, globalOffset + 1] += cf * (O_i + 3 * O_j);

                        // Vektor opterećenja od površinske konvekcije (jed. 05-106):
                        // fα = (α·ϑ∞·l/6) · {2Oᵢ+Oⱼ, Oᵢ+2Oⱼ}
                        double lf = alpha * theta_inf * l / 6.0;
                        F[globalOffset] += lf * (2 * O_i + O_j);
                        F[globalOffset + 1] += lf * (O_i + 2 * O_j);
                    }
                    else if (fe.fo == FEOrder.Quadratic)
                    {
                        // Za kvadratni element: numerička integracija sa 3 tačke
                        double[] xi_g = { -Math.Sqrt(0.6), 0.0, Math.Sqrt(0.6) };
                        double[] w_g = { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };
                        double J = l / 2.0;

                        for (int gp = 0; gp < 3; gp++)
                        {
                            double xi = xi_g[gp];
                            double wg = w_g[gp];

                            // Bazne funkcije kvadratnog elementa
                            double N1 = xi * (xi - 1) / 2.0;
                            double N2 = 1 - xi * xi;
                            double N3 = xi * (xi + 1) / 2.0;

                            // Interpolacija obima: O(ξ) = N₁Oᵢ + N₂Oₘ + N₃Oⱼ
                            // Za quadratic, nodesPerElem=3, srednji čvor je globalOffset+1
                            double O_m = Perimeters[globalOffset + 1];
                            double O_xi = N1 * O_i + N2 * O_m + N3 * O_j;

                            // Matrica: Kα += α·O(ξ)·NᵀN·J·w_g
                            double c = alpha * O_xi * J * wg;
                            K[globalOffset, globalOffset] += c * N1 * N1;
                            K[globalOffset, globalOffset + 1] += c * N1 * N2;
                            K[globalOffset, globalOffset + 2] += c * N1 * N3;
                            K[globalOffset + 1, globalOffset] += c * N2 * N1;
                            K[globalOffset + 1, globalOffset + 1] += c * N2 * N2;
                            K[globalOffset + 1, globalOffset + 2] += c * N2 * N3;
                            K[globalOffset + 2, globalOffset] += c * N3 * N1;
                            K[globalOffset + 2, globalOffset + 1] += c * N3 * N2;
                            K[globalOffset + 2, globalOffset + 2] += c * N3 * N3;

                            // Vektor: Fα += α·ϑ∞·O(ξ)·Nᵀ·J·w_g
                            double cf2 = alpha * theta_inf * O_xi * J * wg;
                            F[globalOffset] += cf2 * N1;
                            F[globalOffset + 1] += cf2 * N2;
                            F[globalOffset + 2] += cf2 * N3;
                        }
                    }
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

        /// <summary>
        /// Računa matricu kondukcije za kvadratni KE sa promjenjivim poprečnim
        /// presekom koristeći Gaussovu integraciju sa 2 tačke.
        /// A(ξ) se interpolira izoparametarski iz vrijednosti u čvorovima.
        /// </summary>
        private double[,] ComputeQuadraticVariableStiffness(
            double lambda, double l, int globalOffset, int nodesPerElem)
        {
            // Gauss 2 tačke: ±1/√3, težine w=1
            double[] xi_g = { -0.577350269189626, 0.577350269189626 };
            double[] w_g = { 1.0, 1.0 };
            double J = l / 2.0;

            double[,] Ke = new double[nodesPerElem, nodesPerElem];

            for (int gp = 0; gp < 2; gp++)
            {
                double xi = xi_g[gp];
                double wg = w_g[gp];

                // Bazne funkcije kvadratnog elementa
                double N1 = xi * (xi - 1) / 2.0;
                double N2 = 1 - xi * xi;
                double N3 = xi * (xi + 1) / 2.0;

                // Interpolacija površine: A(ξ) = N₁Aᵢ + N₂Aₘ + N₃Aₖ
                double A_i = _areas[globalOffset];
                double A_m = _areas[globalOffset + 1];
                double A_k = _areas[globalOffset + 2];
                double A_xi = N1 * A_i + N2 * A_m + N3 * A_k;

                // B-matrica: B = (2/l) * [dN₁/dξ, dN₂/dξ, dN₃/dξ]
                double B1 = (2.0 / l) * (xi - 0.5);
                double B2 = (2.0 / l) * (-2.0 * xi);
                double B3 = (2.0 / l) * (xi + 0.5);

                // Doprinos: K += λ·A(ξ)·BᵀB·J·w_g
                double coeff = lambda * A_xi * J * wg;

                Ke[0, 0] += coeff * B1 * B1;
                Ke[0, 1] += coeff * B1 * B2;
                Ke[0, 2] += coeff * B1 * B3;
                Ke[1, 0] += coeff * B2 * B1;
                Ke[1, 1] += coeff * B2 * B2;
                Ke[1, 2] += coeff * B2 * B3;
                Ke[2, 0] += coeff * B3 * B1;
                Ke[2, 1] += coeff * B3 * B2;
                Ke[2, 2] += coeff * B3 * B3;
            }

            return Ke;
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
