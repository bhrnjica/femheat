using Ciloci.Flee;

namespace NumIntegration
{
    public class Numeric
    {
        private Register register = null;
        private ExpressionContext context = null;
        private IGenericExpression<double> function = null;

        readonly FiniteElement _domain;
        readonly string _strExpr;

        public Numeric(FiniteElement domain)
        {
            _domain = domain;

            register = new Register();
            context = new ExpressionContext(register);
            context.Imports.AddType(typeof(Math));

        }

        public double Integrate(string strExpression, DOP degreeofPrecision= DOP.Linear)
        {
            try
            {
                function = context.CompileGeneric<double>(strExpression);

                Gaussians gv = Gaussians.NullsAndWeights(_domain.ft, degreeofPrecision);

                var jac = new Jacobian(_domain.ft, _domain.fo);
                var coord = new Coordinates(_domain.ft, _domain.fo);

                double result = 0;

                for(int i=0; i < gv.n; i++)
                {
                    var pt = new Point(gv.gi[i].x, gv.gi[i].y, gv.gi[i].z);

                    var j = jac.Calculate(_domain.nodes, pt);
                    
                    (double x, double y, double z) = coord.Transform(_domain.nodes, pt);

                    var wi = gv.wi[i];

                    register.x = x;
                    register.y = y;
                    register.z = z;

                    var f = function.Evaluate();

                    result += j * wi * f;

                }

                return result;

            }
            catch (Exception ex)
            {
                
               throw;
            }
        }

    }    
}
