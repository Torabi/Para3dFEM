using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BriefFiniteElementNet.Controls;
namespace FiniteElementMethod
{

    public enum DesignMethod
    {
        ASD = 1,
        LRFD = 2
    }
    /// <summary>
    /// This class provides with method for checking the structural memebers 
    /// </summary>
    /// 

    public static class DesignCheck
    {

        //public static DesignMethod DM;   
        
      
        /// <summary>
        /// nominal strength in LRFD
        /// </summary>
        public static double Fn;

        /// <summary>
        /// Safety factor
        /// </summary>
        public static double Sf=1.67;
        

        /// <summary>
        /// check tesile bensing stress against the yeild stress
        /// </summary>
        /// <param name="element"></param>
        /// <returns> if greater than one it fails</returns>
        /// 
        public static double CheckTensileBendingStress (MAX_Element element,double phi)
        {         
            return (phi * element.TS / element.Mat.Fty);            
        }
        /// <summary>
        /// Check the compressive bending stress against the yeild stress
        /// </summary>
        /// <param name="element"></param>
        /// <param name="phi"></param>
        /// <returns></returns>

        public static double CheckCompressiveBendingStress(MAX_Element element,double phi)
        {
            return (phi * element.CS / element.Mat.Fcy);            
        }

        /// <summary>
        /// Check tensile stress
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static double CheckTensileStress (MAX_Element element,double phi)
        {            
            double fc = Math.Abs(element.maximumForce[3][1]); // maximum negative axial force                          
            // calculate the nominam compressive force ( fy*A)
            double fn = element.Mat.Fty * element.Sec.Area;
            return (phi * fc) / fn;
        }
        /*
        /// <summary>
        /// Checking the nominal section capasity (Ns)
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static double CheckNominalSectionCompressive (MAX_Element element)
        {
            double Nx, phi, Ns;
            double kf, An, Fy,Ae,Ag;
            Ag = element.Sec.Area;
            Fy = element.Mat.Fy;
            An = Ag; // this is not exact because we are using gross area instead of net area
            kf = Ae / Ag;
            Ns = kf * An * Fy;

            return (Nx / (phi * Ns));
        }
         * */
        /// <summary>
        /// Returns the compressive preformance of the member
        /// </summary>
        /// <param name="element">Element</param>
        /// <param name="phi"> Compressive safty factor</param>
        /// <returns></returns>
        public static double CheckCompressiveStress (MAX_Element element, double phi)
        {

            // first find the maximum compressive axial force
            double fc = Math.Abs(element.maximumForce[3][0]);
            // calculate the nominam compressive force ( fy*A)
            double fn = element.Mat.Fcy * element.Sec.Area;
            return (phi * fc) / fn;
        }
        /// <summary>
        /// Returns the performance value of the memebr in global buckling
        /// </summary>
        /// <param name="element">Element</param>
        /// <param name="phi">Safty factor</param>
        /// <returns></returns>
        public static double CheckBuckling (MAX_Element element, double phi)
        {
            // first find the maximum compressive axial force
            double fc = Math.Abs(element.maximumForce[3][0]);
            // calculate the critical buckling force ( fy*A)
            double fn = element.BucklingLoad();
            return (phi * fc) / fn;
        }


        


        /*
        public static double CheckBendingStress (MAX_Element element)
        {
            return 0;
        }
        */
        

        /// <summary>
        /// lateral-torsional buckling modification when both ends of the segment are braced
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static double Cb(MAX_Element element)
        {
            if (element.IsCantilever())
                return 1;
            double Mmax,Ma,Mb,Mc;
            if (element.maximumForce[1][2]>element.maximumForce[2][2]) {
                Mmax = element.maximumForce[1][2];
                Ma = Math.Abs(element.GetInternalForceAt(0.25*element.GetElementLength()).My);
                Mb = Math.Abs(element.GetInternalForceAt(0.5*element.GetElementLength()).My);
                Mc = Math.Abs(element.GetInternalForceAt(0.75*element.GetElementLength()).My);
            }else{
                Mmax = element.maximumForce[2][2];
                Ma = Math.Abs(element.GetInternalForceAt(0.25*element.GetElementLength()).Mz);
                Mb = Math.Abs(element.GetInternalForceAt(0.5*element.GetElementLength()).Mz);
                Mc = Math.Abs(element.GetInternalForceAt(0.75*element.GetElementLength()).Mz);
            }
            return ((12.5*Mmax)/(2.5*Mmax+3*Ma+4*Mb+3*Mc));
        }

    }
}
