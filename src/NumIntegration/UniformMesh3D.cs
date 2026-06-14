namespace NumIntegration
{
    /// <summary>
    /// Klasa za formiranje 3D mreže konačnih elemenata (heksaedar, tetraedar)
    /// i sklapanje integrala po domenu.
    /// 
    /// Elementi se definišu eksterno (u testu), a mreža ih asemblira interno.
    /// Kasnije se može proširiti sa Assemble() za globalnu matricu krutosti.
    /// </summary>
    public class UniformMesh3D
    {
        private readonly FiniteElement[] _elements;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="elements">Niz 3D konačnih elemenata (FEType.Hexaedron ili FEType.Tetrahedron)</param>
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
        }

        /// <summary>
        /// Integrira zadati izraz po svim elementima mreže i vraća sumu.
        /// </summary>
        /// <param name="expression">Podintegralna funkcija (string za Flee parser)</param>
        /// <param name="dop">Stepen tačnosti integracije (podrazumijevano Cubic)</param>
        /// <returns>Ukupni integral po mreži</returns>
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
        /// Kreira uniformnu mrežu heksaedarskih elemenata nad pravougaonim domenom
        /// [xMin,xMax]×[yMin,yMax]×[zMin,zMax] sa nx×ny×nz podjela.
        /// Svi elementi su linearni 8-čvorni heksaedri.
        /// </summary>
        public static UniformMesh3D CreateUniformHexMesh(
            double xMin, double xMax, int nx,
            double yMin, double yMax, int ny,
            double zMin, double zMax, int nz,
            FEOrder order = FEOrder.Linear)
        {
            if (nx < 1 || ny < 1 || nz < 1)
                throw new ArgumentException("Broj podjela mora biti >= 1 u svakom pravcu.");

            double dx = (xMax - xMin) / nx;
            double dy = (yMax - yMin) / ny;
            double dz = (zMax - zMin) / nz;

            int totalElements = nx * ny * nz;
            var elements = new FiniteElement[totalElements];
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

                        elements[idx++] = new FiniteElement()
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
                    }
                }
            }

            return new UniformMesh3D(elements);
        }

        /// <summary>Broj konačnih elemenata u mreži.</summary>
        public int ElementCount => _elements.Length;

        /// <summary>Vraća element na zadatom indeksu.</summary>
        public FiniteElement this[int index] => _elements[index];
    }
}
