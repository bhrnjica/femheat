using System;
using System.Collections.Generic;
using System.Linq;

namespace NumIntegration
{
    /// <summary>
    /// Tip graničnog uslova na ivici 2D elementa.
    /// </summary>
    public enum BoundaryConditionType
    {
        /// <summary>Zadani toplotni tok q [W/m²] (Neumann)</summary>
        HeatFlux,
        /// <summary>Konvekcija α [W/(m²·°C)], ϑ∞ [°C] (Robin)</summary>
        Convection
    }

    /// <summary>
    /// Opis graničnog uslova na ivici konačnog elementa.
    /// </summary>
    public class EdgeBoundaryCondition
    {
        /// <summary>Indeks elementa u mreži (0-based).</summary>
        public int ElementIndex { get; set; }
        /// <summary>Lokalni indeks ivice (0, 1, 2 za trougao; 0, 1, 2, 3 za četvorougao).</summary>
        public int EdgeIndex { get; set; }
        /// <summary>Tip graničnog uslova.</summary>
        public BoundaryConditionType BCType { get; set; }
        /// <summary>Zadati toplotni tok q [W/m²] (za HeatFlux).</summary>
        public double HeatFlux { get; set; }
        /// <summary>Koeficijent konvekcije α [W/(m²·°C)] (za Convection).</summary>
        public double ConvectionCoeff { get; set; }
        /// <summary>Temperatura okoline ϑ∞ [°C] (za Convection).</summary>
        public double AmbientTemp { get; set; }
    }

    /// <summary>
    /// Klasa za formiranje 2D mreže trouglih i četvorouglih konačnih elemenata
    /// i sklapanje globalne matrice krutosti i vektora opterećenja za stacionarni
    /// prenos toplote kondukcijom.
    /// 
    /// Koristi izoparametarsku formulaciju sa Gaussovom numeričkom integracijom.
    /// Podržava linearne trougle i bilinearne četvorouglove.
    /// </summary>
    public class Mesh2D
    {
        private readonly FiniteElement[] _elements;
        private readonly double[] _conductivities;
        private readonly Dictionary<int, (double x, double y)> _nodes; // global node id → coords
        private readonly int _nodeCount;

        /// <summary>Debljina ploče d [m]. Podrazumevano: 1.0.
        /// Koristi se kada NodeThicknesses nije zadat.</summary>
        public double Thickness { get; set; } = 1.0;

        /// <summary>
        /// Opciona mapa debljina po globalnim čvorovima [m].
        /// Kada je zadana, debljina se interpolira u svakoj Gaussovoj tački
        /// koristeći iste bazne funkcije kao i za temperaturno polje.
        /// Kada nije zadana (null ili prazna), koristi se konstantna Thickness.
        /// </summary>
        public Dictionary<int, double> NodeThicknesses { get; set; } = null;

        /// <summary>Unutrašnji izvor toplote G [W/m³]. Podrazumevano: 0.</summary>
        public double HeatSource { get; set; } = 0.0;

        /// <summary>Lista graničnih uslova po ivicama elemenata.</summary>
        public List<EdgeBoundaryCondition> EdgeBCs { get; set; } = new();

        /// <summary>
        /// Da li je problem osno-simetričan (axisymmetric).
        /// Kada je true, svi zapreminski integrali se množe faktorom 2π·r,
        /// a površinski integrali po ivicama faktorom 2π·r, gde je
        /// r = x-koordinata (radijalna koordinata) u (r,z) ravni.
        /// </summary>
        public bool IsAxisymmetric { get; set; } = false;

        /// <summary>Ukupan broj čvorova u mreži.</summary>
        public int NodeCount => _nodeCount;

        /// <summary>Ukupan broj elemenata u mreži.</summary>
        public int ElementCount => _elements.Length;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="elements">Niz konačnih elemenata (FEType.Triangle ili FEType.Rectangle)</param>
        /// <param name="conductivities">Koeficijenti toplotne provodnosti λ [W/(m·°C)] za svaki element</param>
        public Mesh2D(FiniteElement[] elements, double[] conductivities)
        {
            _elements = elements ?? throw new ArgumentNullException(nameof(elements));
            _conductivities = conductivities ?? throw new ArgumentNullException(nameof(conductivities));

            if (elements.Length == 0)
                throw new ArgumentException("Mreža mora imati barem jedan element.");
            if (conductivities.Length != elements.Length)
                throw new ArgumentException(
                    $"Broj koeficijenata provodnosti ({conductivities.Length}) "
                    + $"mora odgovarati broju elemenata ({elements.Length}).");

            // Validacija tipova elemenata
            for (int e = 0; e < elements.Length; e++)
            {
                var ft = elements[e].ft;
                if (ft != FEType.Triangle && ft != FEType.Rectangle)
                    throw new ArgumentException(
                        $"Element {e}: očekivan Triangle ili Rectangle, dobijen {ft}.");
                if (elements[e].fo != FEOrder.Linear)
                    throw new ArgumentException(
                        $"Element {e}: podržan samo Linear red, dobijen {elements[e].fo}.");
            }

            // Izgradnja globalne numeracije čvorova iz ID-eva čvorova
            _nodes = new Dictionary<int, (double x, double y)>();
            foreach (var fe in _elements)
            {
                foreach (var node in fe.nodes)
                {
                    int id = int.Parse(node.id);
                    if (!_nodes.ContainsKey(id))
                        _nodes[id] = (node.x, node.y);
                }
            }
            _nodeCount = _nodes.Count;
        }

        /// <summary>
        /// Vraća koordinate globalnog čvora.
        /// </summary>
        public (double x, double y) GetNodeCoord(int globalNodeId)
        {
            if (!_nodes.TryGetValue(globalNodeId, out var coord))
                throw new ArgumentException($"Čvor {globalNodeId} ne postoji u mreži.");
            return coord;
        }

        /// <summary>
        /// Sklapa globalnu matricu krutosti [K] i vektor opterećenja {F}.
        /// </summary>
        public (double[,] K, double[] F) Assemble()
        {
            int n = _nodeCount;
            double[,] K = new double[n, n];
            double[] F = new double[n];

            for (int e = 0; e < _elements.Length; e++)
            {
                var fe = _elements[e];
                double lambda = _conductivities[e];

                // Lokalni indeksi čvorova (globalni ID-evi)
                int[] localToGlobal = fe.nodes.Select(nd => int.Parse(nd.id)).ToArray();
                int m = localToGlobal.Length; // 3 za trougao, 4 za četvorougao

                // Izračunavanje matrice kondukcije (izoparametarski + Gauss)
                double[,] Ke = ComputeConductionMatrix(fe, lambda);

                // Sklapanje u globalnu matricu
                for (int a = 0; a < m; a++)
                {
                    int gi = localToGlobal[a] - 1; // 0-based indeks
                    for (int b = 0; b < m; b++)
                    {
                        int gj = localToGlobal[b] - 1;
                        K[gi, gj] += Ke[a, b];
                    }
                }

                // Vektor opterećenja od unutrašnjeg izvora toplote G
                if (HeatSource != 0.0)
                {
                    double[] fe_G = ComputeHeatSourceVector(fe);
                    for (int a = 0; a < m; a++)
                    {
                        int gi = localToGlobal[a] - 1;
                        F[gi] += fe_G[a];
                    }
                }
            }

            // Primjena graničnih uslova po ivicama
            foreach (var bc in EdgeBCs)
            {
                ApplyEdgeBC(K, F, bc);
            }

            return (K, F);
        }

        /// <summary>
        /// Računa matricu kondukcije elementa koristeći izoparametarsku 
        /// formulaciju i Gaussovu numeričku integraciju.
        /// </summary>
        private double[,] ComputeConductionMatrix(FiniteElement fe, double lambda)
        {
            int m = fe.nodes.Length;
            double[,] Ke = new double[m, m];

            // Gaussova integracija
            var gw = Gaussian.NullsAndWeights(fe.ft, DOP.Linear);
            int ng = gw.n;

            for (int g = 0; g < ng; g++)
            {
                // Gaussova tačka u prirodnim koordinatama
                double xi = gw.gi[g].x;
                double eta = gw.gi[g].y;
                double w = gw.wi[g];

                // Matrica derivacija baznih funkcija [∂N/∂ξ; ∂N/∂η] (2 × m)
                double[,] dN = GetShapeDerivatives(fe.ft, m, xi, eta);

                // Jakobijan: J = dN * coords  →  (2×2)
                double[,] J = ComputeJacobianMatrix(fe, dN);

                // Determinanta Jakobijana
                double detJ = J[0, 0] * J[1, 1] - J[0, 1] * J[1, 0];
                if (detJ <= 0)
                    throw new InvalidOperationException(
                        $"Jakobijan elementa je negativan ili nula (detJ={detJ}). "
                        + "Provjerite redoslijed čvorova (CCW).");

                // Inverzija: [J]⁻¹
                double[,] invJ = new double[,]
                {
                    {  J[1, 1] / detJ, -J[0, 1] / detJ },
                    { -J[1, 0] / detJ,  J[0, 0] / detJ }
                };

                // B = invJ * dN → (2 × m)
                double[,] B = Multiply(invJ, dN);

                // Interpolirana debljina u Gaussovoj tački
                double d_g = GetThicknessAtGaussPoint(fe, xi, eta);

                // Osno-simetrični faktor: 2π · r(ξ,η)
                double axiFactor = IsAxisymmetric
                    ? 2.0 * Math.PI * GetRadiusAtGaussPoint(fe, xi, eta)
                    : 1.0;

                // B^T B → (m × m), pomnoženo sa detJ, debljinom i težinom
                for (int a = 0; a < m; a++)
                {
                    for (int b = 0; b < m; b++)
                    {
                        double sum = 0;
                        for (int k = 0; k < 2; k++) // dve vrste B (∂/∂x, ∂/∂y)
                            sum += B[k, a] * B[k, b];
                        Ke[a, b] += lambda * d_g * sum * detJ * w * axiFactor;
                    }
                }
            }

            return Ke;
        }

        /// <summary>
        /// Računa Jakobijan matricu [J] = [∂x/∂ξ, ∂y/∂ξ; ∂x/∂η, ∂y/∂η]
        /// iz derivacija baznih funkcija i koordinata čvorova.
        /// </summary>
        private double[,] ComputeJacobianMatrix(FiniteElement fe, double[,] dN)
        {
            int m = fe.nodes.Length;
            double[,] J = new double[2, 2];

            for (int i = 0; i < m; i++)
            {
                J[0, 0] += dN[0, i] * fe.nodes[i].x; // ∂x/∂ξ
                J[0, 1] += dN[0, i] * fe.nodes[i].y; // ∂y/∂ξ
                J[1, 0] += dN[1, i] * fe.nodes[i].x; // ∂x/∂η
                J[1, 1] += dN[1, i] * fe.nodes[i].y; // ∂y/∂η
            }

            return J;
        }

        /// <summary>
        /// Vektor opterećenja od uniformnog izvora toplote G.
        /// f_G = G · d · ∫ N^T dΩ ≈ G · d · Σ w_g N(ξ_g,η_g)^T · det[J]
        /// </summary>
        private double[] ComputeHeatSourceVector(FiniteElement fe)
        {
            int m = fe.nodes.Length;
            double[] fe_vec = new double[m];

            var gw = Gaussian.NullsAndWeights(fe.ft, DOP.Linear);
            int ng = gw.n;

            for (int g = 0; g < ng; g++)
            {
                double xi = gw.gi[g].x;
                double eta = gw.gi[g].y;
                double w = gw.wi[g];

                var jac = new Jacobian(fe.ft, fe.fo);
                double detJ = jac.Calculate(fe.nodes, new Point(xi, eta, 0));

                // Vrijednosti baznih funkcija u Gaussovoj tački
                double[] N = GetShapeFunctions(fe.ft, m, xi, eta);

                // Interpolirana debljina u Gaussovoj tački
                double d_g = GetThicknessAtGaussPoint(fe, xi, eta);

                // Osno-simetrični faktor: 2π · r(ξ,η)
                double axiFactor = IsAxisymmetric
                    ? 2.0 * Math.PI * GetRadiusAtGaussPoint(fe, xi, eta)
                    : 1.0;

                for (int a = 0; a < m; a++)
                    fe_vec[a] += HeatSource * d_g * N[a] * detJ * w * axiFactor;
            }

            return fe_vec;
        }

        /// <summary>
        /// Primjenjuje granični uslov na ivici elementa.
        /// Za konvekciju: K_α += α·d ∫ N N^T ds, f_α += α·ϑ∞·d ∫ N ds
        /// Za toplotni tok: f_q += -q·d ∫ N ds
        /// Integracija duž ivice koristi 1D Gaussovo pravilo.
        /// </summary>
        private void ApplyEdgeBC(double[,] K, double[] F, EdgeBoundaryCondition bc)
        {
            var fe = _elements[bc.ElementIndex];
            int[] localToGlobal = fe.nodes.Select(nd => int.Parse(nd.id)).ToArray();
            int m = localToGlobal.Length;

            // Lokalni čvorovi koji pripadaju ivici
            var edgeNodes = GetEdgeLocalNodes(fe.ft, bc.EdgeIndex);
            int nEdge = edgeNodes.Length;

            // Dužina ivice
            double edgeLength = ComputeEdgeLength(fe, bc.EdgeIndex);

            // 1D Gauss duž ivice (2 tačke za linearnu ivicu)
            var gw1D = Gaussian.NullsAndWeights(FEType.Line, DOP.Linear);
            int ng1 = gw1D.n;

            // Parametrizacija ivice: ξ ∈ [-1, 1] duž ivice
            for (int g = 0; g < ng1; g++)
            {
                double t = gw1D.gi[g].x; // prirodna koordinata duž ivice [-1, 1]
                double w = gw1D.wi[g];

                // Mapiranje t → prirodne koordinate elementa (ξ, η)
                (double xi, double eta) = MapEdgeToNatural(fe.ft, bc.EdgeIndex, t);

                // Bazne funkcije u tački na ivici
                double[] N = GetShapeFunctions(fe.ft, m, xi, eta);

                // Jakobijan ivice: ds = (L/2) dt, gdje je L dužina ivice
                double ds = edgeLength / 2.0 * w;

                // Interpolirana debljina duž ivice
                double d_edge = GetThicknessOnEdge(fe, bc.EdgeIndex, t);

                // Osno-simetrični faktor: 2π · r(t) duž ivice
                double axiFactor = IsAxisymmetric
                    ? 2.0 * Math.PI * GetRadiusOnEdge(fe, bc.EdgeIndex, t)
                    : 1.0;

                if (bc.BCType == BoundaryConditionType.Convection)
                {
                    double alpha = bc.ConvectionCoeff;
                    double theta_inf = bc.AmbientTemp;

                    // K_α += α·d·ds · N N^T  (× 2πr za axisymmetric)
                    for (int a = 0; a < m; a++)
                    {
                        int gi = localToGlobal[a] - 1;
                        for (int b = 0; b < m; b++)
                        {
                            int gj = localToGlobal[b] - 1;
                            K[gi, gj] += alpha * d_edge * N[a] * N[b] * ds * axiFactor;
                        }
                        // f_α += α·ϑ∞·d·ds · N  (× 2πr za axisymmetric)
                        F[gi] += alpha * theta_inf * d_edge * N[a] * ds * axiFactor;
                    }
                }
                else if (bc.BCType == BoundaryConditionType.HeatFlux)
                {
                    double q = bc.HeatFlux;

                    for (int a = 0; a < m; a++)
                    {
                        int gi = localToGlobal[a] - 1;
                        // f_q = -q·d·ds · N  (× 2πr za axisymmetric)
                        // Konvencija: HeatFlux = q_n = -λ ∂ϑ/∂n (toplotni tok
                        // u smjeru vanjske normale, pozitivan kada napušta domen).
                        // Za toplotu koja ulazi u domen, zadati negativnu vrijednost.
                        F[gi] += -q * d_edge * N[a] * ds * axiFactor;
                    }
                }
            }
        }

        // ==================== POMOĆNE FUNKCIJE ====================

        /// <summary>
        /// Računa dužinu ivice iz koordinata čvorova.
        /// </summary>
        private double ComputeEdgeLength(FiniteElement fe, int edgeIdx)
        {
            var edge = GetEdgeLocalNodes(fe.ft, edgeIdx);
            var p1 = fe.nodes[edge[0]];
            var p2 = fe.nodes[edge[1]];
            double dx = p2.x - p1.x;
            double dy = p2.y - p1.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Vraća lokalne indekse čvorova koji pripadaju zadatoj ivici.
        /// Za trougao: ivica 0=(0,1), 1=(1,2), 2=(2,0).
        /// Za četvorougao: ivica 0=(0,1), 1=(1,2), 2=(2,3), 3=(3,0).
        /// </summary>
        private int[] GetEdgeLocalNodes(FEType ft, int edgeIdx)
        {
            return ft switch
            {
                FEType.Triangle => edgeIdx switch
                {
                    0 => new[] { 0, 1 },
                    1 => new[] { 1, 2 },
                    2 => new[] { 2, 0 },
                    _ => throw new ArgumentException($"Neispravan indeks ivice trougla: {edgeIdx}")
                },
                FEType.Rectangle => edgeIdx switch
                {
                    0 => new[] { 0, 1 },
                    1 => new[] { 1, 2 },
                    2 => new[] { 2, 3 },
                    3 => new[] { 3, 0 },
                    _ => throw new ArgumentException($"Neispravan indeks ivice četvorougla: {edgeIdx}")
                },
                _ => throw new NotSupportedException($"Tip elementa {ft} nije podržan.")
            };
        }

        /// <summary>
        /// Mapira parametar t ∈ [-1, 1] duž ivice na prirodne koordinate (ξ, η) elementa.
        /// </summary>
        private (double xi, double eta) MapEdgeToNatural(FEType ft, int edgeIdx, double t)
        {
            return ft switch
            {
                FEType.Triangle => edgeIdx switch
                {
                    // ivica 0 (0→1): (1,0) → (0,1)
                    0 => ((1 - t) / 2.0, (1 + t) / 2.0),
                    // ivica 1 (1→2): (0,1) → (0,0)
                    1 => (0.0, (1 - t) / 2.0),
                    // ivica 2 (2→0): (0,0) → (1,0)
                    2 => ((1 + t) / 2.0, 0.0),
                    _ => throw new ArgumentException($"Ivica {edgeIdx} nije validna za trougao.")
                },
                FEType.Rectangle => edgeIdx switch
                {
                    // ivica 0: η=-1, ξ=t
                    0 => (t, -1.0),
                    // ivica 1: ξ=1, η=t
                    1 => (1.0, t),
                    // ivica 2: η=1, ξ=-t  (obrnuti smjer za CCW)
                    2 => (-t, 1.0),
                    // ivica 3: ξ=-1, η=-t
                    3 => (-1.0, -t),
                    _ => throw new ArgumentException($"Ivica {edgeIdx} nije validna za četvorougao.")
                },
                _ => throw new NotSupportedException($"Tip elementa {ft} nije podržan.")
            };
        }

        /// <summary>
        /// Vrijednosti baznih funkcija u tački (ξ, η).
        /// </summary>
        private double[] GetShapeFunctions(FEType ft, int m, double xi, double eta)
        {
            return ft switch
            {
                FEType.Triangle => new double[]
                {
                    xi,           // N1
                    eta,          // N2
                    1 - xi - eta  // N3
                },
                FEType.Rectangle => new double[]
                {
                    0.25 * (1 - xi) * (1 - eta),  // N1
                    0.25 * (1 + xi) * (1 - eta),  // N2
                    0.25 * (1 + xi) * (1 + eta),  // N3
                    0.25 * (1 - xi) * (1 + eta)   // N4
                },
                _ => throw new NotSupportedException($"Tip {ft} nije podržan.")
            };
        }

        /// <summary>
        /// Derivacije baznih funkcija po prirodnim koordinatama.
        /// Vraća matricu 2×m: [∂N₁/∂ξ ... ∂Nₘ/∂ξ; ∂N₁/∂η ... ∂Nₘ/∂η]
        /// </summary>
        private double[,] GetShapeDerivatives(FEType ft, int m, double xi, double eta)
        {
            double[,] dN = new double[2, m];

            switch (ft)
            {
                case FEType.Triangle:
                    dN[0, 0] = 1;   dN[1, 0] = 0;    // N1 = ξ
                    dN[0, 1] = 0;   dN[1, 1] = 1;    // N2 = η
                    dN[0, 2] = -1;  dN[1, 2] = -1;   // N3 = 1-ξ-η
                    break;

                case FEType.Rectangle:
                    dN[0, 0] = -0.25 * (1 - eta); dN[1, 0] = -0.25 * (1 - xi);   // N1
                    dN[0, 1] =  0.25 * (1 - eta); dN[1, 1] = -0.25 * (1 + xi);   // N2
                    dN[0, 2] =  0.25 * (1 + eta); dN[1, 2] =  0.25 * (1 + xi);   // N3
                    dN[0, 3] = -0.25 * (1 + eta); dN[1, 3] =  0.25 * (1 - xi);   // N4
                    break;
            }

            return dN;
        }

        /// <summary>
        /// Množenje matrica: A[p×q] × B[q×r] = C[p×r]
        /// </summary>
        private double[,] Multiply(double[,] A, double[,] B)
        {
            int p = A.GetLength(0), q = A.GetLength(1), r = B.GetLength(1);
            if (q != B.GetLength(0))
                throw new ArgumentException("Neusaglašene dimenzije matrica za množenje.");

            double[,] C = new double[p, r];
            for (int i = 0; i < p; i++)
                for (int j = 0; j < r; j++)
                    for (int k = 0; k < q; k++)
                        C[i, j] += A[i, k] * B[k, j];
            return C;
        }

        // ==================== PODRŠKA ZA PROMJENJIVU DEBLJINU ====================

        /// <summary>
        /// Vraća debljinu ploče u Gaussovoj tački (ξ,η) unutar elementa.
        /// Ako NodeThicknesses nije zadan, koristi se konstanta Thickness.
        /// Inače se debljina interpolira iz čvornih vrijednosti:
        ///   d(ξ,η) = Σ N_i(ξ,η) · d_i
        /// </summary>
        private double GetThicknessAtGaussPoint(FiniteElement fe, double xi, double eta)
        {
            if (NodeThicknesses == null || NodeThicknesses.Count == 0)
                return Thickness;

            int m = fe.nodes.Length;
            double[] N = GetShapeFunctions(fe.ft, m, xi, eta);
            double d = 0;
            for (int i = 0; i < m; i++)
            {
                int globalId = int.Parse(fe.nodes[i].id);
                if (!NodeThicknesses.TryGetValue(globalId, out double di))
                    throw new ArgumentException(
                        $"Debljina za čvor {globalId} nije zadata u NodeThicknesses.");
                d += N[i] * di;
            }
            return d;
        }

        /// <summary>
        /// Vraća debljinu ploče duž ivice elementa u tački parametra t ∈ [-1,1].
        /// Koristi mapiranje ivica → prirodne koordinate, pa interpolira debljinu.
        /// </summary>
        private double GetThicknessOnEdge(FiniteElement fe, int edgeIdx, double t)
        {
            if (NodeThicknesses == null || NodeThicknesses.Count == 0)
                return Thickness;

            (double xi, double eta) = MapEdgeToNatural(fe.ft, edgeIdx, t);
            return GetThicknessAtGaussPoint(fe, xi, eta);
        }

        // ==================== PODRŠKA ZA OSNO-SIMETRIČNE PROBLEME ====================

        /// <summary>
        /// Vraća radijalnu koordinatu r u Gaussovoj tački (ξ,η) unutar elementa.
        /// r(ξ,η) = Σ N_i(ξ,η) · x_i, gde je x_i = r-koordinata čvora i.
        /// Koristi se samo kada je IsAxisymmetric = true.
        /// </summary>
        private double GetRadiusAtGaussPoint(FiniteElement fe, double xi, double eta)
        {
            int m = fe.nodes.Length;
            double[] N = GetShapeFunctions(fe.ft, m, xi, eta);
            double r = 0;
            for (int i = 0; i < m; i++)
                r += N[i] * fe.nodes[i].x; // x-koordinata = radijus r
            return r;
        }

        /// <summary>
        /// Vraća radijalnu koordinatu r duž ivice elementa u tački t ∈ [-1,1].
        /// r(t) = Σ N_i(t) · r_i, gde su r_i x-koordinate čvorova ivice.
        /// Koristi se samo kada je IsAxisymmetric = true.
        /// </summary>
        private double GetRadiusOnEdge(FiniteElement fe, int edgeIdx, double t)
        {
            // Linearna interpolacija duž ivice: r(t) = N1(t)·r1 + N2(t)·r2
            double N1 = 0.5 * (1.0 - t);
            double N2 = 0.5 * (1.0 + t);

            var edgeNodes = GetEdgeLocalNodes(fe.ft, edgeIdx);
            double r1 = fe.nodes[edgeNodes[0]].x;
            double r2 = fe.nodes[edgeNodes[1]].x;

            return N1 * r1 + N2 * r2;
        }
    }
}
