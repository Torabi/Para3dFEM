using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
namespace FiniteElementMethod.CrossSections
{
    public class SolidRectangle_Section : Standard_Section
    {
        public SolidRectangle_Section(double h, double w)
            : base(h, w, 0, 0)
        {
            SectionType = Section_Type.I;
            Shape = SectionGenerator.GetRectangularSection(h, w);
        }
        /*
        public override double TorsionalConstant()
        {
            double d = H - TF;
            return (2*W*Math.Pow(TF,3)+d*Math.Pow(TW,3))/3.0;
        }
         * */
    }
}
