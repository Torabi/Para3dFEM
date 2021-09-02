using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteElementMethod
{
    public static class GoldenSearch
    {
        public enum Extermum { Maximum,Minimum}
        private static double phi = (Math.Sqrt(5) - 1) / 2;
        public static double tol = 0.000001;
        public static double FindExtremum(Func<double, double> f, double min, double max,Extermum extermum)
        {

            double b = max;
            double a = min;
            double l = b - a;
            double c = b - phi * l;
            double d = a + phi * l;
            
            while (Math.Abs(c - d) > tol)
            {
                double fc = f(c);
                double fd = f(d);
                if ((extermum == Extermum.Minimum)?(fc < fd ):(fc > fd ))
                {
                    b = d; d = c;
                    fd = fc;
                    fc = f(c);
                    c = b - phi * (b - a);
                }
                else
                {
                    a = c; c = d;
                    fc = fd; fd = f(d);
                    d = a + phi * (b - a);
                }
            }
            double fa = f(a);
            double fb = f(b);
            if (extermum == Extermum.Minimum)
                return (fa > fb) ? fb : fa;
            else
                return (fa > fb) ? fa : fb;
            //return f(0.5 * (a + b));
        }
    }
}
