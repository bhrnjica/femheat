using NumIntegration;

namespace Book
{
    internal class Chapter4
    {
     
        //Krivolinijski integral-određivanje duzine luka parabole y=x^2, na intrvalu 0-2
        public static void Example1()
        {
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",0.0, 0.0),
                    new Node("2",2.0, 4.0),
                    new Node("3",1.0, 1.0),
                },
                ft = FEType.Line,
                fo = FEOrder.Quadratic,
            };

            //create coordinate and jacobian object
            var coor = new Coordinates(FEType.Line, FEOrder.Quadratic);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Quadratic);

            var numI = new NumericalIntegration(fe1);

            var value1 = numI.Integrate("1",DOP.Linear);
            var value2 = numI.Integrate("1", DOP.Quadratic);
            var value3 = numI.Integrate("1", DOP.Cubic);
            var value4 = numI.Integrate("1", DOP.Fourth);

            ///ispis rezultata
            Console.WriteLine($" a) - Duzina luka y=x^2 na intervalu (0,2) l={value1}, 2 gaussove tačke.");
            Console.WriteLine($" b) - Duzina luka y=x^2 na intervalu (0,2) l={value2}, 3 gaussove tačke.");
            Console.WriteLine($" c) - Duzina luka y=x^2 na intervalu (0,2) l={value3}, 4 gaussove tačke.");
            Console.WriteLine($" d) - Duzina luka y=x^2 na intervalu (0,2) l={value4}, 5 gaussovih tačaka.");

        }

        //Integracija u polarnim koordinatama. Određivanje momenta inercije četvrtine prstena
        public static void Example2()
        {
            //koordinate u polarnim 
            var fe1 = new FiniteElement()
            {
                nodes = new[]
                {
                    new Node("1",40.0, 0.0),
                    new Node("2",70.0, 0.0),
                    new Node("3",70.0, Math.PI/2.0),
                    new Node("4",40.0, Math.PI/2.0),
                },
                ft = FEType.Rectangle,
                fo = FEOrder.Linear,
            };

            //create coordinate and jacobian object
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //moment inercije u polarnim koordinatama
            var strInertia = "(Sin(y)*Sin(y))*x*x*x";

            var numI = new NumericalIntegration(fe1);

            var value1 = numI.Integrate(strInertia, DOP.Linear);
            var value2 = numI.Integrate(strInertia, DOP.Quadratic);
            var value3 = numI.Integrate(strInertia, DOP.Cubic);

            ///ispis rezultata (4211700 tacan rezultat)
            Console.WriteLine($" a) - Moment inercije četvrtine prstena I={value1}, 2 gaussove tačke.");
            Console.WriteLine($" b) - Moment inercije četvrtine prstena I={value2}, 3 gaussove tačke.");
            Console.WriteLine($" c) - Moment inercije četvrtine prstena I={value3}, 4 gaussove tačke.");

        }

        //Izračunavanje površinskog integrala
        public static void Example3()
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

            //create coordinate and jacobian object
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //moment inercije u polarnim koordinatama
            var strInertia = "1.0";

            var numI = new NumericalIntegration(fe1);

            var value1 = numI.Integrate(strInertia, DOP.Linear);
            var value2 = numI.Integrate(strInertia, DOP.Quadratic);
            var value3 = numI.Integrate(strInertia, DOP.Cubic);

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

            //create coordinate and jacobian object
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //moment inercije u polarnim koordinatama
            var strInertia = "1.0";

            var numI = new NumericalIntegration(fe1);

            var value1 = numI.Integrate(strInertia, DOP.Linear);
            var value2 = numI.Integrate(strInertia, DOP.Quadratic);
            var value3 = numI.Integrate(strInertia, DOP.Cubic);

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

            //create coordinate and jacobian object
            var coor = new Coordinates(FEType.Line, FEOrder.Linear);
            var jacobian = new Jacobian(FEType.Line, FEOrder.Linear);

            //moment inercije u polarnim koordinatama
            var strInertia = "6.0*x*y";

            var numI = new NumericalIntegration(fe1);

            var value1 = numI.Integrate(strInertia, DOP.Linear);
            var value2 = numI.Integrate(strInertia, DOP.Quadratic);
            var value3 = numI.Integrate(strInertia, DOP.Cubic);

            ///ispis rezultata (4 tacan rezultat)
            Console.WriteLine($" a) - Moment inercije četvrtine prstena I={value1}, 2 gaussove tačke.");
            Console.WriteLine($" b) - Moment inercije četvrtine prstena I={value2}, 3 gaussove tačke.");
            Console.WriteLine($" c) - Moment inercije četvrtine prstena I={value3}, 4 gaussove tačke.");
        }
    }
}
