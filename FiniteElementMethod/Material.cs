using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
namespace FiniteElementMethod
{
    public class Material : IEquatable<Material>
    {
        public string Name;

        public string Category;
        /// <summary>
        /// Unique ID
        /// </summary>
        public int ID=-1;
        
        /// <summary>
        /// Elastic Modulus
        /// </summary>
        public double E;
        /// <summary>
        /// Shear Modulus
        /// </summary>
        public double G;
        /// <summary>
        /// Weight per volume
        /// </summary>
        public double W; // weight
        /// <summary>
        /// Ultimate Tensile Strength
        /// </summary>
        public double Fu;
        /// <summary>
        /// Minimum Compressive Yield Strength 
        /// </summary>
        public double Fcy;
        /// <summary>
        /// Minimum Tension Yield Strength 
        /// </summary>
        public double Fty;

        /// <summary>
        /// Create a new material
        /// </summary>
        /// <param name="category">Category of material (Steel/Wood or Concrete) </param>
        /// <param name="name">Unique name</param>
        /// <param name="e">Modulus of elastisity </param>
        /// <param name="g">Shear Modulus</param>
        /// <param name="eUnit"> Units of modulus</param>
        /// <param name="w">Weight per unit</param>
        /// <param name="Wunit">Weight unit</param>
        /// <param name="fyc">Compressive yeild stress</param>
        /// <param name="fyt">Tensile yeild stress</param>
        /// <param name="fyUnit">Yeild stress unit </param>
        public Material(string category,string name,double e,double g, ElasticModulusUnit eUnit,double w,ForceUnit Wunit,double fyc,double fyt,ForceUnit fyUnit)
        {
            Name = name;
            Category = category;
            E = UnitConversion.ConvertInputElasticModulus(e, eUnit);
            G = UnitConversion.ConvertInputElasticModulus(g, eUnit);
            W = UnitConversion.ConverInputForce(w, Wunit);
            Fcy = UnitConversion.ConverInputForce(fyc, fyUnit);
            Fty = UnitConversion.ConverInputForce(fyt, fyUnit);
            
        }
        public void SetMaterial(BarElement f)
        {
            var m= new UniformIsotropicMaterial();
            m.YoungModulus = E;
            
            f.Material = m;
        }

        public override bool Equals(object obj)
        {
            if (obj is Material)
                return Equals(obj as Material);
            else
                return false;
        }
        public bool Equals(Material m)
        {
            if (m == null) return false;
            return (m.Name == Name&&m.Category == Category && m.E==E && m.G==G &&m.Fcy==Fcy &&m.Fty==Fty);
        }
        public override int GetHashCode()
        {
            return ID;
        } 
       public static bool operator == (Material m1, Material m2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(m1, m2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)m1 == null) || ((object)m2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (m1.Equals(m2));
        }
       public static bool operator !=(Material a, Material b)
       {
           return !(a == b);
       }
        private  string GetColor ()
       {
            switch (Category.ToLower())
            {
                case "steel":
                    return "Magenta";
                case "concrete" :
                    return "Yellow";
                default:
                    return "Red";

            }
       }
       internal string GetGeneralInfo()
       {
            /*
             *  Material=4000Psi   Type=Concrete   SymType=Isotropic   TempDepend=No   Color=Yellow   Notes="Customary f'c 4000 psi 2/14/2016 6:29:58 PM"
                Material=A992Fy50   Type=Steel   SymType=Isotropic   TempDepend=No   Color=Magenta   Notes="ASTM A992 Grade 50 2/14/2016 6:29:58 PM"
             * */
           string result = "\t";
           result += string.Format("Material={0}   Type={1}   SymType=Isotropic   TempDepend=No   Color={2}   Notes=\"Created by Para 3d\"", Name+ID.ToString(), Category,GetColor()) + "\r\n";

           return result;
       }
        internal double GetPoissonRatio ()
       {
           return (E - 2 * G) / (2 * G);
       }

        internal string GetMechanicalInfo()
       {
           /*
            *  Material=4000Psi   UnitWeight=23563.1216161855   UnitMass=2402.76960558926   E1=24855578060.0518   G12=10356490858.3549   U12=0.2   A1=0.0000099
      Material=A992Fy50   UnitWeight=76972.8639422648   UnitMass=7849.04737995992   E1=199947978795.958   G12=76903068767.676   U12=0.3   A1=0.0000117
 
            * */
           string result = "\t";
           result += string.Format("Material={0}   UnitWeight={1}   UnitMass={2}   E1={3}   G12={4}   U12={5}   A1=0.0000099", Name + ID.ToString(), W, W / 9.80665002864, E, G, GetPoissonRatio()) + "\r\n";
           return result;
       }
    }
}
