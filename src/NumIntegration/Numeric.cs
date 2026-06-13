

using Flee.PublicTypes;

namespace NumIntegration
{
    public class Numeric
    {
        private Register register;
        private ExpressionContext context;
        private IGenericExpression<double>? function = null;

        readonly FiniteElement _domain;

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
                function = context?.CompileGeneric<double>(strExpression);

                var gv = Gaussian.NullsAndWeights(_domain.ft, degreeofPrecision);
                
                if(gv is null || gv.gi is null || 
                    gv.gi.Length == 0 || gv.wi is null 
                    || gv.wi.Length == 0) throw new NullReferenceException(nameof(gv));

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
            catch (Exception)
            {
                
               throw;
            }
        }

    }    
}
