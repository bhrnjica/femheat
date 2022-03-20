namespace NumIntegration
{
    public enum FEType
    {
        None,
        Line,
        Triangle,
        Rectangle,
        Tetrahedron,
        Hexaedron,
    }
     
    public enum FEOrder
    {
        None,
        Linear,
        Quadratic,
        Cubic,
        Fourth
    }
    public enum DOP
    {
        None,
        Linear,
        Quadratic,
        Cubic,
        Fourth
    }
    public readonly record struct Node(string id, double x, double y=0, double z=0);
    public readonly record struct FiniteElement(FEType ft, FEOrder fo, Node[] nodes);
    public readonly record struct GPoint(string id, double x, double y, double z);
    public readonly record struct Point(double x, double y=0, double z=0);

    /// <summary>
    /// Helper class for Expression evaluator
    /// </summary>
    public class Register
    {
        public Register()
        {
            //e = Convert.Todouble(Math.E);
            pi = Math.PI;
        }

        //constants
        //public double e { get; set; }
        public double pi { get; set; }

        //vars
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

}