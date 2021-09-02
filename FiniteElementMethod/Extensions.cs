using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;
//using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet;
namespace FiniteElementMethod
{
    
    
    public static class Extensions
        {
             
            
            
            public static PointYZ IntersectXY (PointYZ p1,PointYZ p2,PointYZ p3,PointYZ p4)
            {
                double denom = (p1.Y-p2.Y)*(p3.Z-p4.Z)-(p1.Z-p2.Z)*(p3.Y-p4.Y);
                double p12 = (p1.Y*p2.Z-p1.Z*p2.Y);
                double p34 = (p3.Y*p4.Z-p3.Z*p4.Y);
                
                return new PointYZ(

                    (p12*(p3.Y-p4.Y)-(p1.Y-p2.Y)*p34)/denom,
                    (p12*(p3.Z-p4.Z)-(p1.Z-p2.Z)*p34)/denom
                    );
                
                
            }
            public static double Distance (PointYZ a,PointYZ b)
            {


            return Math.Sqrt(Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
            }
        /// <summary>
        /// distance of the point p0 from the line p1-p2
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
            public static double Distance(PointYZ p0, PointYZ p1, PointYZ p2)
            {
                double denom = Math.Sqrt(Math.Pow(p1.Y-p2.Y,2)+Math.Pow(p1.Z-p2.Z,2));
                return Math.Abs((p2.Y-p1.Y)*(p1.Z-p0.Z)-(p1.Y-p0.Y)*(p2.Z- p1.Z)) / denom;
            }

        }
  
 
}
