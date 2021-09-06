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
        
    }
}
