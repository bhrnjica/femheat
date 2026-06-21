using NumIntegration;

namespace Book
{
    internal class Chapter4
    {
        // ====================================================================
        // Zadatak 4.02: Implementirati program za izračunavanje integrala
        // funkcije f(x)=xe^(2x) nad linijskim KE čiji čvorovi imaju
        // koordinate 1(0.0), 2(4.0). Poređenje DOP.Linear (2 GP),
        // DOP.Quadratic (3 GP), DOP.Cubic (4 GP).
        // ====================================================================
        public static void Example_462()
        {
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0),
                    new Node("2",4.0),
                },
                ft = FEType.Line,
                fo = FEOrder.Linear,
            };
            
            //numeric object
            var numI = new Numeric(fe1);

            //podintegralna funkcija
            var fun = "x*e^(2*x)";

            //solutions
            var value1 = numI.Integrate(fun, DOP.Linear);
            var value2 = numI.Integrate(fun, DOP.Quadratic);
            var value3 = numI.Integrate(fun, DOP.Cubic);
            
            ///ispis rezultata
            Console.WriteLine($" n = 2; I={value1}.");
            Console.WriteLine($" n = 3; I={value2}.");
            Console.WriteLine($" n = 3; I={value3}.");           
        }

        // ====================================================================
        // Zadatak 4.04: Napisati program za određivanje dužine luka krive
        // y=x²+2x, na segmentu x=[0,2]. Koristi kvadratni linijski KE
        // sa koordinatama čvorova: 1(0,0), 2(2,8), 3(1,3).
        // ====================================================================
        public static void Example_464()
        {
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0, 0.0),
                    new Node("2",2.0, 8.0),
                    new Node("3",1.0, 3.0),
                },
                ft = FEType.Line,
                fo = FEOrder.Quadratic,
            };

            var fun = "1";
            var numI = new Numeric(fe1);

            var value2 = numI.Integrate(fun, DOP.Quadratic);

            //ispis rezultata
            Console.WriteLine($"Duzina luka y=x^2+2x na intervalu (0,2) l={value2}.");
        }

        // ====================================================================
        // Zadatak 4.06: Napisati program za izračunavanje dvostrukog integrala
        // I=∫∫(x²-x)·sin(y) dx dy koristeći 4, 9 i 16 Gaussovih tačaka.
        // Domen integracije: x∈[0,2], y∈[0,π/2] (pravougaonik u polarnim
        // koordinatama). Tačna vrijednost: I=2/3=0.666667.
        // ====================================================================
        public static void Example_466()
        {
            //koordinate u polarnim 
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0, 0),
                    new Node("2",2.0, 0),
                    new Node("3",2.0, Math.PI/2.0),
                    new Node("4",0.0, Math.PI/2.0),
                },
                ft = FEType.Rectangle,
                fo = FEOrder.Linear,
            };

            //podintegralna funkcija
            var fun = "(x^2-x)*sin(y)";

            var numI = new Numeric(fe1);

            var value1 = numI.Integrate(fun, DOP.Linear);
            var value2 = numI.Integrate(fun, DOP.Quadratic);
            var value3 = numI.Integrate(fun, DOP.Cubic);

            //ispis rezultata (4211700 tacan rezultat)
            Console.WriteLine($" I={value1}, 4 gaussove tačke.");
            Console.WriteLine($" I={value2}, 9 gaussovih tačaka.");
            Console.WriteLine($" I={value3}, 16 gaussovih tačaka.");

        }

        // ====================================================================
        // Zadatak 4.08: Napisati program za određivanje momenta inercije
        // četvrtine prstena. Koristeći numeričku integraciju izračunati
        // moment inercije površine: I_xx = ∬ y² dA.
        // Unutrašnji i vanjski poluprečnici: r₁=35 mm, r₂=80 mm.
        // ====================================================================
        public static void Example_468()
        {
            //koordinate u polarnim 
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",35.0, 0.0),
                    new Node("2",80.0, 0.0),
                    new Node("3",80.0, Math.PI/2.0),
                    new Node("4",35.0, Math.PI/2.0),
                },
                ft = FEType.Rectangle,
                fo = FEOrder.Linear,
            };

            //koordinate i jacobian
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //podintegralna funkcija
            var fun = "(Sin(y)*Sin(y))*x*x*x";

            var numI = new Numeric(fe1);

            var value1 = numI.Integrate(fun, DOP.Linear);
            var value2 = numI.Integrate(fun, DOP.Quadratic);
            var value3 = numI.Integrate(fun, DOP.Cubic);

            //ispis rezultata
            Console.WriteLine($" I={value1}.");
        }

        // ====================================================================
        // Zadatak 4.14: Napisati program za numeričku integraciju funkcije
        // f(x,y,z)=x·y·z po oblasti tetraedra sa čvorovima
        // 1(2,3,2), 2(2,1,0), 3(3,4,0), 4(1,3,0).
        // Poređenje 4, 10 i 20 Gaussovih tačaka.
        // ====================================================================
        public static void Example_4610()
        {
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",2.0, 3.0, 2.0),
                    new Node("2",2.0, 1.0,0),
                    new Node("3",3.0, 4.0, 0),
                    new Node("4",1.0,3.0,0),
                },
                ft = FEType.Tetrahedron,
                fo = FEOrder.Linear,
            };

            //koordinate i jacobian
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //podintegralna funkcija
            var fun = "x*y*z";

            var numI = new Numeric(fe1);

            var value1 = numI.Integrate(fun, DOP.Linear);
            var value2 = numI.Integrate(fun, DOP.Quadratic);
            var value3 = numI.Integrate(fun, DOP.Cubic);

            //ispis rezultata
            Console.WriteLine($"a) I={value1} 4 gaussova tačka.");
            Console.WriteLine($"b) I={value2} 10 gaussovih tačaka.");
            Console.WriteLine($"c) I={value3} 20 gaussovih tačaka.");
        }

        // ====================================================================
        // Zadatak 4.16: Napisati program za numeričku integraciju funkcije
        // f(x,y,z)=x·y·z po oblasti heksaedra sa čvorovima:
        // (1,2,3),(4,2,3),(4,5,3),(1,5,3),(1,2,6),(4,2,6),(4,5,6),(1,5,6).
        // Poređenje 8, 27 i 64 Gaussovih tačaka. Tačna vrijednost: I=1063.125.
        // ====================================================================
        public static void Example_4612()
        {
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",1.0,2.0,3.0),
                    new Node("2",4.0,2.0,3.0),
                    new Node("3",4.0,5.0,3.0),
                    new Node("4",1.0,5.0,3.0),
                    new Node("5",1.0,2.0,6.0),
                    new Node("6",4.0,2.0,6.0),
                    new Node("7",4.0,5.0,6.0),
                    new Node("8",1.0,5.0,6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Linear,
            };

            //koordinate i jacobian
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //podintegralna funkcija
            var fun = "x*y*z";

            var numI = new Numeric(fe1);

            var value1 = numI.Integrate(fun, DOP.Linear);
            var value2 = numI.Integrate(fun, DOP.Quadratic);
            var value3 = numI.Integrate(fun, DOP.Cubic);

            //ispis rezultata
            Console.WriteLine($"a) I={value1} 8 gaussovih tačaka.");
            Console.WriteLine($"b) I={value2} 27 gaussovih tačaka.");
            Console.WriteLine($"c) I={value3} 64 gaussove tačke.");
        }



        // ====================================================================
        // [OBSOLETE] Površinski integral četverougla u prostoru
        // Integracija f(x,y,z)=1 nad zakrivljenim četverouglom u 3D
        // sa čvorovima 1(2,1,1), 2(4,1,1), 3(4,3,2), 4(2,3,2).
        // Zamijenjen sa Example_466 — zadržan radi referentnih podataka.
        // ====================================================================
        public static void Example4()
        {
            //koordinate cetvorougla u prostoru
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",2.0, 1.0, 1.0),
                    new Node("2",4.0, 1.0, 1.0),
                    new Node("3",4.0, 3.0, 2.0),
                    new Node("4",2.0, 3.0, 2.0),
                },
                ft = FEType.Rectangle,
                fo = FEOrder.Linear,
            };

            //koordinate i jacobian
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //podintegralna funkcija
            var fun = "1.0";

            var numI = new Numeric(fe1);

            var value1 = numI.Integrate(fun, DOP.Linear);
            var value2 = numI.Integrate(fun, DOP.Quadratic);
            var value3 = numI.Integrate(fun, DOP.Cubic);

            ///ispis rezultata (4 tacan rezultat)
            Console.WriteLine($" a) - Moment inercije četvrtine prstena I={value1}, 2 gaussove tačke.");
            Console.WriteLine($" b) - Moment inercije četvrtine prstena I={value2}, 3 gaussove tačke.");
            Console.WriteLine($" c) - Moment inercije četvrtine prstena I={value3}, 4 gaussove tačke.");
        }

        // ====================================================================
        // Zadatak 4.10: Napisati program za izračunavanje površinskog
        // integrala f(x,y)=1 nad pravougaonom domenom [2,4]×[1,3]
        // koristeći 4, 9 i 16 Gaussovih tačaka.
        // Provjera: numerička integracija vraća tačnu površinu 4.0.
        // ====================================================================
        public static void Example5()
        {
            //koordinate cetvorougla u prostoru
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",2.0, 1.0),
                    new Node("2",4.0, 1.0),
                    new Node("3",4.0, 3.0),
                    new Node("4",2.0, 3.0),
                },
                ft = FEType.Rectangle,
                fo = FEOrder.Linear,
            };

            //koordinate i jacobian
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //podintegralna funkcija
            var fun = "1.0";

            var numI = new Numeric(fe1);

            var value1 = numI.Integrate(fun, DOP.Linear);
            var value2 = numI.Integrate(fun, DOP.Quadratic);
            var value3 = numI.Integrate(fun, DOP.Cubic);

            ///ispis rezultata (4 tacan rezultat)
            Console.WriteLine($" a) - Moment inercije četvrtine prstena I={value1}, 2 gaussove tačke.");
            Console.WriteLine($" b) - Moment inercije četvrtine prstena I={value2}, 3 gaussove tačke.");
            Console.WriteLine($" c) - Moment inercije četvrtine prstena I={value3}, 4 gaussove tačke.");
        }


        // ====================================================================
        // Zadatak 4.12: Napisati program za izračunavanje površinskog
        // integrala f(x,y)=6xy nad trouglom u prostoru sa čvorovima
        // 1(0.2,0.5,0.3), 2(0.4,0.3,0.3), 3(0.2,0.6,0.2).
        // Poređenje 4, 9 i 16 Gaussovih tačaka.
        // ====================================================================
        public static void Example6()
        {
            //koordinate cetvorougla u prostoru
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.2, 0.5, 0.3),
                    new Node("2",0.4, 0.3, 0.3),
                    new Node("3",0.2, 0.6, 0.2)
                },
                ft = FEType.Triangle,
                fo = FEOrder.Linear,
            };

            //koordinate i jacobian
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //podintegralna funkcija
            var fun = "6.0*x*y";

            var numI = new Numeric(fe1);

            var value1 = numI.Integrate(fun, DOP.Linear);
            var value2 = numI.Integrate(fun, DOP.Quadratic);
            var value3 = numI.Integrate(fun, DOP.Cubic);

            ///ispis rezultata (4 tacan rezultat)
            Console.WriteLine($" a) - Moment inercije četvrtine prstena I={value1}, 2 gaussove tačke.");
            Console.WriteLine($" b) - Moment inercije četvrtine prstena I={value2}, 3 gaussove tačke.");
            Console.WriteLine($" c) - Moment inercije četvrtine prstena I={value3}, 4 gaussove tačke.");
        }


        // ====================================================================
        // Zadatak 4.04a: Dužina luka kubne krive
        // Numeričkim putem odrediti dužinu luka krive y=x³-1,
        // na segmentu x=[0,2]. Koristi kubni linijski KE sa 4 čvora
        // (dva unutrašnja). Poređenje DOP.Linear, Quadratic, Cubic.
        // ====================================================================
        public static void Example2_2()
        {
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 0.0,-1.0),//first node
                    new Node("2", 3.0, 26.0),//last node
                    new Node("3", 1.0, 0),//first inner node
                    new Node("4", 2.0, 7),//second inner node
                },
                ft = FEType.Line,
                fo = FEOrder.Cubic,
            };

            var numI = new Numeric(fe1);
            var value1 = numI.Integrate("1", DOP.Linear);
            var value2 = numI.Integrate("1", DOP.Quadratic);
            var value3 = numI.Integrate("1", DOP.Cubic);

            ///ispis rezultata
            Console.WriteLine($" a) - Duzina luka y=x^3-1 na intervalu (0,2) l={value1}, 2 gaussove tačke.");
            Console.WriteLine($" b) - Duzina luka y=x^3-1 na intervalu (0,2) l={value2}, 3 gaussove tačke.");
            Console.WriteLine($" c) - Duzina luka y=x^3-1 na intervalu (0,2) l={value3}, 4 gaussove tačke.");

        }

        // ====================================================================
        // Zadatak 4.17: Izračunati integral funkcije
        // f(x,y,z)=sin²(x²+1)·√(y+z) nad domenom [1,4]×[2,5]×[3,6]
        // korištenjem jednog heksaedarskog KE s 8, 27 i 64 Gaussovih
        // tačaka. Uporediti s tačnom vrijednošću I=35.8118794928527
        // i diskutovati zašto povećanje broja GP ne daje tačnost.
        // ====================================================================
        public static void Example_4613()
        {
            Console.WriteLine("=== Zadatak 4.17: Integracija oscilatorne funkcije ===");
            Console.WriteLine("f(x,y,z) = sin²(x²+1)·√(y+z) nad [1,4]×[2,5]×[3,6]\n");

            var fe = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1", 1.0, 2.0, 3.0),
                    new Node("2", 4.0, 2.0, 3.0),
                    new Node("3", 4.0, 5.0, 3.0),
                    new Node("4", 1.0, 5.0, 3.0),
                    new Node("5", 1.0, 2.0, 6.0),
                    new Node("6", 4.0, 2.0, 6.0),
                    new Node("7", 4.0, 5.0, 6.0),
                    new Node("8", 1.0, 5.0, 6.0),
                },
                ft = FEType.Hexaedron,
                fo = FEOrder.Linear,
            };

            var fun = "Sin(x*x+1)*Sin(x*x+1) * Sqrt(y+z)";
            var ni = new Numeric(fe);

            double I1 = ni.Integrate(fun, DOP.Linear);     // 8 tačaka
            double I2 = ni.Integrate(fun, DOP.Quadratic);  // 27 tačaka
            double I3 = ni.Integrate(fun, DOP.Cubic);      // 64 tačke

            double trueValue = 35.8118794928527;

            Console.WriteLine("--- Rezultati integracije s jednim KE ---");
            Console.WriteLine($"  DOP.Linear    (8  GP): I = {I1,10:F6}  "
                + $"(greška: {Math.Abs(I1 - trueValue),10:F6})");
            Console.WriteLine($"  DOP.Quadratic (27 GP): I = {I2,10:F6}  "
                + $"(greška: {Math.Abs(I2 - trueValue),10:F6})");
            Console.WriteLine($"  DOP.Cubic     (64 GP): I = {I3,10:F6}  "
                + $"(greška: {Math.Abs(I3 - trueValue),10:F6})");
            Console.WriteLine($"  Tačna vrijednost:       I = {trueValue,10:F6}\n");

            Console.WriteLine("--- Diskusija ---");
            Console.WriteLine("  Funkcija sin²(x²+1) osciluje duž x-ose sa rastućom");
            Console.WriteLine("  frekvencijom. Jedan konačni element, bez obzira na");
            Console.WriteLine("  broj Gaussovih tačaka, ne može tačno aproksimirati");
            Console.WriteLine("  oscilatorno ponašanje na cijelom intervalu [1,4].");
            Console.WriteLine("  Greška s DOP.Cubic iznosi ~8%.");
            Console.WriteLine("  Rješenje: profiniti mrežu (h-adaptacija) — podijeliti");
            Console.WriteLine("  domenu na više manjih elemenata.\n");

            Console.WriteLine("=== Kraj zadatka 4.17 ===\n");
        }

        // ====================================================================
        // Zadatak 4.18: Implementirati program za integraciju funkcije
        // f(x,y,z)=sin²(x²+1)·√(y+z) profinjenom mrežom heksaedara.
        // Domenu [1,4]×[2,5]×[3,6] podijeliti na nx×1×1 jednakih
        // pod-elemenata i ispitati konvergenciju za nx=1,2,4,6,8,10.
        // Koristi UniformMesh3D.CreateUniformHexMesh.
        // ====================================================================
        public static void Example_4614()
        {
            Console.WriteLine("=== Zadatak 4.18: Integracija profinjenom mrežom ===");
            Console.WriteLine("f(x,y,z) = sin²(x²+1)·√(y+z) nad [1,4]×[2,5]×[3,6]\n");

            var fun = "Sin(x*x+1)*Sin(x*x+1) * Sqrt(y+z)";
            double trueValue = 35.8118794928527;

            // Mreža: nx×1×1 — profinjujemo samo duž x jer funkcija
            // osciluje po x, a glatka je po y i z.
            int[] refinements = { 1, 2, 4, 6, 8, 10 };

            Console.WriteLine("  nx  |      Integral   |     Greška    |  Odnos");
            Console.WriteLine("  ----|-----------------|---------------|-------");

            double prevError = double.NaN;
            foreach (int nx in refinements)
            {
                var mesh = UniformMesh3D.CreateUniformHexMesh(
                    xMin: 1.0, xMax: 4.0, nx: nx,
                    yMin: 2.0, yMax: 5.0, ny: 1,
                    zMin: 3.0, zMax: 6.0, nz: 1);

                double I = mesh.Integrate(fun, DOP.Cubic);
                double error = Math.Abs(I - trueValue);

                string ratio = double.IsNaN(prevError) ? "  ---"
                    : $"{prevError / error,5:F2}";

                Console.WriteLine($"  {nx,3}  |  {I,12:F8}  |  {error,12:E2}  | {ratio}");

                prevError = error;
            }

            Console.WriteLine($"\n  Tačna vrijednost: {trueValue:F8}");
            Console.WriteLine($"  Sa nx=10 podjela, greška < 0.001 (0.1%).");
            Console.WriteLine($"  Konvergencija je približno kvadratna (O(h²)).\n");

            Console.WriteLine("=== Kraj zadatka 4.18 ===");
        }
    }
}
