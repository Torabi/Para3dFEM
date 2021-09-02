using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;

using System.Linq;
using BriefFiniteElementNet.Elements;

namespace FiniteElementMethod.CrossSections
{
    /// <summary>
    /// Contains geometrical data of a cross section
    /// </summary>
    public abstract class CrossSection : IEquatable<CrossSection>
    {

        #region General properties
        /// <summary>
        /// the index in the table
        /// </summary>
        public int ID = -1;
        /// <summary>
        /// Type of section
        /// </summary>
        public Section_Type SectionType;
        /// <summary>
        /// Shape of cross section
        /// </summary>        
        public PointYZ[] Shape;
        /// <summary>
        /// Material of the section
        /// </summary>
        public Material Mat;



        #endregion
        #region Geometrical Properties
        /// <summary>
        /// Location of Centroid
        /// </summary>
        public PointYZ Centroid;
        /// <summary>
        /// Cross section area
        /// </summary>
        public double Area;
        /// <summary>
        /// Moment of inertial about axis Y
        /// </summary>
        public double Iy;
        /// <summary>
        /// Moment of inertial about axis Z
        /// </summary>
        public double Iz;
        /// <summary>
        /// Product of inertia about axis YZ
        /// </summary>
        public double Iyz;
        /*
        /// <summary>
        /// Torsional constant
        /// </summary>
        public double J;
         */ 
        /// <summary>
        /// Polar Moment Of Inertia
        /// </summary>
        public double Jo;
        /// <summary>
        /// Section modulus about y axis
        /// </summary>
        public double Sy;
        /// <summary>
        /// Section modulus about z axis
        /// </summary>
        public double Sz;
        /// <summary>
        /// Radius of Gyration about Y axis 
        /// </summary>
        public double Ky;
        /// <summary>
        /// Radius of Gyration about Z axis 
        /// </summary>
        public double Kz;
        /// <summary>
        /// Distance from the furthest left point to the centroid in Y axis 
        /// </summary>
        public double Cyl;
        /// <summary>
        /// Distance from the furthest right point to the centroid in Y axis 
        /// </summary>
        public double Cyr;
        /// <summary>
        /// Distance from the furthest top point to the centroid in Z axis 
        /// </summary>
        public double Czt;
        /// <summary>
        /// Distance from the furthest bottom point to the centroid in Z axis 
        /// </summary>
        public double Czb;
        /// <summary>
        /// maximum distance from centroid in Y axis
        /// </summary>
        public double Ymax;
        /// <summary>
        /// maximum distance from centroid in Z axis
        /// </summary>
        public double Zmax;
        
        /// <summary>
        /// Defines the section type : Slender/noncompact / compact
        /// </summary>
        public Section_Class SectionClass;

#endregion 

        #region abstract methods

        /// <summary>
        /// Calculate torsional constant
        /// </summary>
        /// <returns>J</returns>
        //public abstract double TorsionalConstant();
        
        /// <summary>
        /// Calulate polar moment of inertia
        /// </summary>
        /// <returns>Jo</returns>
        /*
        public virtual double PolarMomentOfInertia()
        {
            return Iy + Iz;
        }
        */
        //public abstract double SectionModulusY();


        //public abstract double SectionModulusZ();
       
        public virtual void FindSectionClass()
        {
            SectionClass = Section_Class.Slender;
        }

        #endregion
        
        internal void FindBondingBox()
        {
            var p = from point in Shape select point.Y;
            p.OrderBy(x=>x);
            Cyl = Math.Abs(p.First() - Centroid.Y);
            Cyr = Math.Abs(p.Last() - Centroid.Y);
            Ymax = Math.Max(Cyl, Cyr);
            p = from point in Shape select point.Z;
            p.OrderBy(x => x);
            Czb = Math.Abs(p.First() - Centroid.Z);
            Czt = Math.Abs(p.Last() - Centroid.Z);
            Zmax = Math.Max(Czb, Czt);

        }
        public void CalculateGeometricalProperties()
        {
            Centroid = new PointYZ(0, 0);
            Area = Iy = Iz = Iyz = 0;

            FindBondingBox();
            //Triangulation2D.Triangulate(new Polygon(Shape),this);
            // from wikipedia https://en.wikipedia.org/wiki/Second_moment_of_area
            for (int i = 0; i < Shape.Length - 1; i++)
            {
                double xi = Shape[i].Y;
                double yi = Shape[i].Z;
                double xj = Shape[i + 1].Y;
                double yj = Shape[i + 1].Z;
                double ai = (xi * yj - xj * yi);
                Area += ai;
                Iy += (yi * yi + yi * yj + yj * yj) * ai / 12.0;
                Iz += (xi * xi + xi * xj + xj * xj) * ai / 12.0;
                Iyz += (xi * yj + 2 * xi * yi + 2 * xj * yj + xj * yi) * ai / 24;
            }
            Area = .5 * Math.Abs(Area);
            Iy = Math.Abs(Iy);
            Iz = Math.Abs(Iz);

        }



        public string Log ()
        {
            string result = "Section properties \r\n";

            for (int i = 0; i < Shape.Length; i++)
                result += string.Format("Point {0} : [{1},{2}]\r\n", i, Shape[i].Y,Shape[i].Z);

            result += string.Format("Area = {0}\r\n", Area);
            result += string.Format("Moment of inertia about Y = {0}\r\n", Iy);
            result += string.Format("Moment of inertia about Z = {0}\r\n", Iz);
            result += string.Format("Product of inertia about axis YZ = {0}\r\n", Iyz);
            //result += string.Format("Torsional constant = {0}\r\n", J);
            result += string.Format("Polar Moment Of Inertia = {0}\r\n", Jo);
            result += string.Format("Section modulus about y axis\r\n = {0}\r\n", Sy);
            result += string.Format("Section modulus about z axis = {0}\r\n", Sz);
            result += string.Format("ky = {0},kz = {1}\r\n", Ky,Kz);
            //result += string.Format("center = {0},{1}\r\n", Centroid.Y,Centroid.Z);
            result += string.Format("Top = {0}, bottom = {1}, right = {2}, left = {3}\r\n", Czt,Czb,Cyr,Cyl);
            
            return result;

        }
        
        public virtual void Initialize()
        {
            //J = TorsionalConstant();
            //Jo = PolarMomentOfInertia();
            CalculateGeometricalProperties();
            //var s = _1DCrossSectionGeometricProperties.Calculate(Shape.ToArray(), true);
            
            Jo = Iy + Iz; // polar moment of inertia
            Ky = Math.Sqrt(Iy / Area); 
            Kz = Math.Sqrt(Iz / Area);
            Sy = Iy / Zmax;
            Sz = Iz / Ymax;
            
            System.IO.File.WriteAllText("D:\\log.txt", Log());
        }
        #region IEquatable methods


        /// <summary>
        /// Two section are equal if they are same material and same geomerty
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>        
        
        public override bool Equals(object obj)
        {
            
            if (obj == null)
                return false;
            if (!(obj is CrossSection))
                return false;
            CrossSection cs = obj as CrossSection;
            return this.Equals(cs);
        }
        public override int GetHashCode()
        {
            return ID;
        }


        public bool Equals(CrossSection other)
        {
            
            if (other.Mat != this.Mat)
                return false;
            if (other.Shape.Length != this.Shape.Length)
                return false;

            bool result = true;
            int i = 0;
            while (i < other.Shape.Length && result)
            {
                result = (other.Shape[i] == this.Shape[i]);
                i++;
            }
            return result;
        }
        #endregion
    }
   
   
    
     
}
