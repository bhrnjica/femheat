using NumIntegration;

namespace Book
{
    internal class Chapter4
    {
        //Numerička integracija nad linijskim konačnim elementom
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

        //Krivolinijski integral-određivanje duzine luka parabole y=x^2+2x, na intrvalu 0-2
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

        //Integracija dvostrukog integrala
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

        //Odredjivanje momenta inercije četvrtine prstena
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



        //obsolite
        //Izračunavanje površinskog integrala
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

        //Izračunavanje površinskog integrala
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


        //Izračunavanje površinskog integrala trougla
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
    }
}
