using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
namespace FiniteElementMethod.CrossSections
{
    public enum Section_Type
    {
        R = 1,                                        
        I = 2,
        L = 3,
        C = 4,
        T = 5
    }

    public enum Section_Class
    {
        Copmact = 1,
        NonCompact = 2,
        Slender = 3

    }
    public abstract class Standard_Section : CrossSection
    {

        
        /// <summary>
        /// Width
        /// </summary>
        public double W;
        /// <summary>
        /// Height
        /// </summary>
        public double H;
        /// <summary>
        /// Thickness of the flange
        /// </summary>
        public double TF;
        /// <summary>
        /// Thickness of the web
        /// </summary>
        public double TW;

        private double[] data;
        public Standard_Section(double h, double w, double tf, double tw)
        {
            W = w; H = h; TF = tf; TW = tw;
            data = new double[6];
        }
         
        
         
        
        public override bool Equals(object obj)
        {
            if (!(obj is Standard_Section))
                return false;
            Standard_Section s = obj as Standard_Section;
            return (s.SectionType==SectionType && s.H == H && s.W == W && s.TF == TF && s.TW == TW && s.Mat==Mat);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Create different standard sections by string name value
        /// </summary>
        /// <param name="name">type of section : b(Rectangle),i,l,t,c</param>
        /// <param name="h">height</param>
        /// <param name="w">Width</param>
        /// <param name="tf">flange thickness</param>
        /// <param name="tw">web thickness</param>
        /// <param name="mat">Material of the section</param>
        /// <returns>The section object of the desired class</returns>
        public static CrossSection CreateSection(string name, double h, double w, double tf, double tw)
        {           
            switch (name.ToLower())
            {                 
                case "b":
                    return new SolidRectangle_Section(h, w);
                 
                case "i":
                    return new I_Section(w, h, tf, tw);                
                default:
                    return null;
            }                
        }
        /*
        public Section_Class GetSectionClass(LoadDirection dir )
        {
            double La, Lp, Lr; 
            if (SectionType == Section_Type.I)
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
                    return Section_Class.Copmact;
                if (La <= Lr)
                    return Section_Class.NonCompact;
                else
                    return Section_Class.Slender;
            }
            else
            {
                return Section_Class.Slender;
            }
        }
         * */
        
    }
}
