using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
namespace FiniteElementMethod.CrossSections
{
    public class I_Section : Standard_Section
    {
        public I_Section(double h, double w, double tf, double tw)
            : base(h, w, tf, tw)
        {
            SectionType = Section_Type.I;
            Shape = SectionGenerator.GetISetion(h, w, tf, tw);
        }
        /*
        public override void FindSectionClass()
        {
            if (dir == LoadDirection.Y) // flange
                {
                    La = W / (2 * TF);
                    Lp = 0.38 * Math.Sqrt(Mat.E / Mat.Fy);
                    Lr = 0.83 * Math.Sqrt(Mat.E/(Mat.Fy-10));
                }
                else // web
                {
                    La = H / TW;
                    Lp = 3.76 * Math.Sqrt(Mat.E/Mat.Fy);
                    Lr = 5.70 * Math.Sqrt(Mat.E / Mat.Fy);
                }
                if (La <= Lp)
                    SectionClass =  Section_Class.Copmact;
                if (La <= Lr)
                    SectionClass =  Section_Class.NonCompact;
                else
                    SectionClass =  Section_Class.Slender;
        }
         * */
        /*
        public override double TorsionalConstant()
        {
            double d = H - TF;
            return (2*W*Math.Pow(TF,3)+d*Math.Pow(TW,3))/3.0;
        }
         */ 
    }
}
