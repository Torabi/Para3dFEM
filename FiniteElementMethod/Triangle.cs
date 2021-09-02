using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
namespace FiniteElementMethod
{
    public class Triangle
    {
        //public PointYZ[] Points;
        public PointYZ A,B,C;
        public double Area,Product,Iy,Iz;
        public PointYZ Center;
        //private Triangle[] vertical;
        private Triangle[] horizontal;
        
        private void Initialize()
        {
            Area = area();
            Center = Centroid();
            Product = product();
            List<PointYZ> p = new List<PointYZ> { A, B, C };

            p.Sort(delegate(PointYZ x, PointYZ y)
            {
                if (x.Z < y.Z)
                    return -1;
                else
                    return 1;
            });
            if (p[0].Z != p[1].Z && p[1].Z != p[2].Z)
            {
                // the base of triangle is not horizontal so we divide it in to two triangle horizontaly
                Triangle t1, t2;
                DivideHorizontaly(p, out t1, out t2);
                Iy = t1.Iy + Math.Pow((Center.Z - t1.Center.Z), 2) * t1.Area +
                    t2.Iy + Math.Pow((Center.Z - t2.Center.Z), 2) * t2.Area;
                Iz = t1.Iz + Math.Pow((Center.Y - t1.Center.Y), 2) * t1.Area +
                    t2.Iz + Math.Pow((Center.Y - t2.Center.Y), 2) * t2.Area;

            }
            else // the base of triangle is horizontal
            {
                
                PointYZ p1, p2, p3;
                if (p[0].Z == p[1].Z)
                {
                    p3 = p[2];
                    if (p[0].Y<p[1].Y)
                    {
                        p1 = p[0]; p2 = p[1]; 
                    }
                    else
                    {
                        p1 = p[1]; p2 = p[0]; 
                    }
                }else{
                    // p[1].Z=p[2].Z
                     p3 = p[0];
                    if(p[1].Y<p[2].Y){
                        p1 = p[1]; p2 = p[2];
                    }
                    else
                    {
                        p1 = p[2]; p2 = p[1];
                    }
                }
                double b = Extensions.Distance(p1,p2);
                double h = Extensions.Distance(p3, p1, p2);
                double a = p3.Y - p1.Y;

                Iy = CaluculateMomentOfInertiaY(h, b);                
                Iz = CaluculateMomentOfInertiaZ(h, b, a);
            }
        }
        
        public Triangle(PointYZ[] points)
        {
            A = points[0];
            B = points[1];
            C = points[2];
            Initialize();
        }
        public Triangle (PointYZ a,PointYZ b,PointYZ c)
        {
            if (a == b || b == c || c == a)
                throw new System.ArgumentException("Vertices cannot overlap");
            A = a; B = b; C = c;
            Initialize();
        }
        private PointYZ Centroid ()
        {
            double y = (A.Y + B.Y + C.Y) / 3.0;
            double z = (A.Z + B.Z + C.Z) / 3.0;
            return new PointYZ(y, z);
        }
        
        private double area ()
        {
           
            return Math.Abs(A.Y*(B.Z-C.Z)+B.Y*(C.Z-A.Z)+C.Y*(A.Z-B.Z))/2.0;
        }
        private double product()
        {            
            return Center.Y*Center.Z*Area;
        }
        /// <summary>
        /// Divides the triangle into two triangles (t1 & t2) by passing a horizontal line from a point 
        /// </summary>
        /// <param name="points"> points of triangle ( in Z order) </param>
        /// <param name="t1"> first triangle </param>
        /// <param name="t2"> second triangle</param>
        /// <returns></returns>
        private void DivideHorizontaly(List<PointYZ> points, out Triangle t1,out Triangle t2)
        {
            PointYZ intersection = Extensions.IntersectXY(points[0], points[2], points[1], new PointYZ(points[1].Y+1, points[1].Z));
            t1 = new Triangle(points[1],points[0],intersection);
            t2 = new Triangle(points[1],intersection,points[2]);                
        }
        /*
        private Triangle[] DivideVerticaly(List<PointYZ> points)
        {
            PointYZ intersection = Extensions.IntersectXY(points[0], points[2], points[1], new PointYZ(points[1].Y, points[1].Z + 1));
            return new Triangle[2] { new Triangle(points[0], points[1], intersection), new Triangle(points[1], intersection, points[2]) };
        }
        */
        /// <summary>
        /// calculate the momenat of inertial about axis Y using height and base value
        /// http://www.efunda.com/math/areas/triangle.cfm
        /// </summary>
        /// <param name="h">Height</param>
        /// <param name="b">Base</param>
        /// <returns>Iy</returns>
        private double CaluculateMomentOfInertiaY(double h,double b)
        {
            return b * Math.Pow(h, 3) / 36.0;
        }

        private double CaluculateMomentOfInertiaZ(double h,double b,double a)
        {
            return (h*Math.Pow(b,3)-b*b*h*a+b*h*a*a) / 36.0;
        }
        
    }
}
