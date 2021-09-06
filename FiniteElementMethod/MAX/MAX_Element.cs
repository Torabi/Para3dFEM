using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Controls;
using FiniteElementMethod.CrossSections;
using BriefFiniteElementNet.Sections;
using BriefFiniteElementNet.Materials;

namespace FiniteElementMethod
{
    public class MAX_Element : BarElement
    {


        //internal BarElement element1D;

        private Material mat;
        private CrossSection sec;

        //public Node StartNode
        //{
        //    get
        //    {
        //        //if (element1D is TrussElement2Node)
        //        //    return ((TrussElement2Node)element1D).StartNode;
        //        //else if (element1D is FrameElement2Node)
        //        //    return ((FrameElement2Node)element1D).StartNode;
        //        //else
        //        //    return null;
        //        element1D.StartNode;
        //    }
        //}
        //public Node EndNode
        //{
        //    get
        //    {
        //        if (element1D is TrussElement2Node)
        //            return ((TrussElement2Node)element1D).EndNode;
        //        else if (element1D is FrameElement2Node)
        //            return ((FrameElement2Node)element1D).EndNode;
        //        else
        //            return null;
        //    }
        //}

        /// <summary>
        /// Maximum internal forces in format of vector.
        /// in a jagged array[6][3] 
        /// first array [] 0:Torsion 1:Y_Moment 2:Z_Moment 3:Axial 4:YShear 5:ZShear
        /// second aray [][] : 0:positive 1:negative 2:Absolute
        /// </summary>
        public double[][] maximumForce;
        public double[][] maximumForceLocation;


        /// <summary>
        /// used for double integration
        /// </summary>
        //public List<Vector> FirstIntegration;
        //List<Vector> SecondIntegration;
        /// <summary>
        /// previous position on the beam 
        /// </summary>
        double x0;
        /// <summary>
        /// Previous moment
        /// </summary>
        Vector m0;
        /// <summary>
        /// Length of the beam;
        /// </summary>
        double Length;
        /// <summary>
        /// Maximum Tensile Stress
        /// </summary>
        public double TS;
        /// <summary>
        /// Maximum Compressive Stress
        /// </summary>
        public double CS;
        /// <summary>
        /// Maximum Bending Stress
        /// </summary>
        public double BS;
        /// <summary>
        /// Capacity factor
        /// </summary>
        public double Phi = 0.9;
        private int division;

        /// <summary>
        /// Reset maximum values
        /// </summary>
        public void Reset()
        {
            Length = GetElementLength();
            maximumForce = new double[][]{
            new double[]{0,0,0},new double[]{0,0,0},new double[]{0,0,0},
            new double[]{0,0,0},new double[]{0,0,0},new double[]{0,0,0}
            };
            maximumForceLocation = new double[][]{
            new double[]{0,0,0},new double[]{0,0,0},new double[]{0,0,0},
            new double[]{0,0,0},new double[]{0,0,0},new double[]{0,0,0}
            };
            //Torsion_Value = Y_Moment_Value = Z_Moment_Value = Axial_Force_Value = Y_Shear_Value = Z_Shear_Value = Vector.Zero;
            //Torsion_Location = Y_Moment_Location = Z_Moment_Location = Axial_Force_Location = Y_Shear_Location = Z_Shear_Location = Vector.Zero;
            x0 = division = 0;
            //FirstIntegration = new List<Vector>();
            //SecondIntegration = new List<Vector>();

        }
        public Material Mat
        {
            get { return mat; }
            set
            {                
                mat = value;
                var m = UniformIsotropicMaterial.CreateFromYoungShear(mat.E, mat.G);
                Material = m;
                
            }
        }

        public CrossSection Sec
        {
            get { return sec; }
            set
            {
                sec = value;
                var  s = new UniformGeometric1DSection(sec.Shape.ToArray());
                Section = s;
                
                
            }
        }
        
        public MAX_Element(MAX_Node a,MAX_Node b, string elementType, int index):base(a,b)
        {
            

            
            Behavior = (elementType == "Truss") ? BarElementBehaviours.Truss : BarElementBehaviours.FullFrame; 
            string lableChar = (elementType == "Truss") ? "T" : "F";
            Label = lableChar + (index + 1).ToString();
            Reset();
        }

     


        /// <summary>
        /// Update the maximum values
        /// </summary>
        /// <param name="input"></param>
        /// <param name="x"></param>
        /// <param name="output"></param>
        /// <param name="location"></param>
        internal void FindMaximum (double input,double x,ref double[] output, ref double[] location)
        {
            if (input>0)
            {
                //check for positive maximum
                if (input > output[0])
                {
                    output[0] = input;
                    location[0] = x;
                }
            }
            else
            {
                // check for negative maximum
                if (input < output[1])
                {
                    output[1] = input;
                    location[1] = x;
                }
            }
            // check for absolute maximum 
            if (Math.Abs(input) > output[2])
            {
                output[2] = Math.Abs(input);
                location[2] = x;
            }
        }
        /// <summary>
        /// Return the internal force of the element at given point from the first node
        /// </summary>
        /// <param name="x">Distance from the first node</param>
        /// <param name="forceUnit">output unit</param>
        /// <returns>internal force</returns>
        public Force GetFrameInternalForce (double x,int forceUnit)
        {
            
            var l = 2*x-1;
            if (l <= -1)
                l += 1e-9;
            else if (l>=1)
                l -= 1e-9;
            Force tmp = GetExactInternalForceAt(l,FEM_MAX.LoadCombin);


            FindMaximum(tmp.Mx, l, ref maximumForce[0], ref maximumForceLocation[0]);
            FindMaximum(tmp.My, l, ref maximumForce[1], ref maximumForceLocation[1]);
            FindMaximum(tmp.Mz, l, ref maximumForce[2], ref maximumForceLocation[2]);
            FindMaximum(tmp.Fx, l, ref maximumForce[3], ref maximumForceLocation[3]);
            FindMaximum(tmp.Fy, l, ref maximumForce[4], ref maximumForceLocation[4]);
            FindMaximum(tmp.Fz, l, ref maximumForce[5], ref maximumForceLocation[5]);

            // // save the first moment value in m0
            //if (division < 1)
            //{
                
            //    FirstIntegration.Add(tmp.Moments);
            //    SecondIntegration.Add(Vector.Zero);
            //}
            //else
            //{
            //    var i1 = FirstIntegration[division-1]+0.5*(x-x0)*(tmp.Moments+m0);
            //    FirstIntegration.Add(i1);
            //    var i2 = SecondIntegration[division - 1] + 0.5*(x - x0) * (FirstIntegration[division - 1] + i1);//(x-x0)*FirstIntegration[division-1]+.25*Math.Pow((x-x0),2)*(tmp.Moments+m0);
            //    SecondIntegration.Add(i2);
            //}
            //m0 = tmp.Moments;
            division++;
            return UnitConversion.ConvertOutputForce(tmp, (ForceUnit)forceUnit);
        }
       
        
        /// <summary>
        /// finds the maximum stress along the member
        /// </summary>

        public void CalculateInternalForces ()
        {
             

            for (int i = 0; i < 6; i++)
            {
                Func<double, double> f;
                // create the delegate based on user choice 
                switch (i)
                {
                    case 0: // Torsion
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Mx; };
                        break;
                    case 1: // Y_Moment
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).My; };
                        break;
                    case 2: // Z_Moment
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Mz; };
                        break;
                    case 3: // Axial force
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Fx; };
                        break;
                    case 4: // Y shear
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Fy; };
                        break;
                    case 5: // Z shear
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Fz; };
                        break;
                    default:
                        f = delegate(double x) { return 0; };
                        break;

                }
                for (int j = 0; j < 2; j++)
                {
                    double left = maximumForceLocation[i][j] - Length / division;
                    double right = left + 2 * Length / division;
                    double val = maximumForce[i][j];
                    if (left < 0)
                        left = 0;
                    if (right > Length)
                        right = Length;
                    if (j == 0)
                    {
                        maximumForce[i][j] = GoldenSearch.FindExtremum(f, left, right, GoldenSearch.Extermum.Maximum);
                        
                    }
                    else
                    {
                        maximumForce[i][j] = GoldenSearch.FindExtremum(f, left, right, GoldenSearch.Extermum.Minimum);
                        
                    }
                }
                if (Math.Abs(maximumForce[i][0]) > Math.Abs(maximumForce[i][1]))
                {
                    maximumForce[i][2] = Math.Abs(maximumForce[i][0]);
                }
                else
                {
                    maximumForce[i][2] = Math.Abs(maximumForce[i][1]);
                }

            }
            // finding the maximum compressive stress
            var CSY = Math.Max(CalculateStress(true, maximumForce[1][0], LoadDirection.Y), CalculateStress(true, maximumForce[1][1], LoadDirection.Y)); // check the compressive stress in Y direction
            var CSZ = Math.Max(CalculateStress(true, maximumForce[2][0], LoadDirection.Z), CalculateStress(true, maximumForce[2][1], LoadDirection.Z)); // check the compressive stress in Z direction
            CS = Math.Max(CSY, CSZ);
            // finding the maximum tensile stress
            var TSY = Math.Max(CalculateStress(false, maximumForce[1][0], LoadDirection.Y), CalculateStress(false, maximumForce[1][1], LoadDirection.Y)); // check the tensile stress in Y direction
            var TSZ = Math.Max(CalculateStress(false, maximumForce[2][0], LoadDirection.Z), CalculateStress(false, maximumForce[2][1], LoadDirection.Z)); // check the tensile stress in Z direction
            TS = Math.Max(TSY, TSZ);
            BS = Math.Max(CS, TS);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceIndex">one based index</param>
        /// <param name="sign">one based index</param>
        /// <param name="forceUnit"></param>
        /// <returns></returns>
        public double GetMaximumInternalForce (int forceIndex,int sign, int forceUnit)
        {
            
            return UnitConversion.ConvertOutputForce(maximumForce[forceIndex-1][sign-1],(ForceUnit)forceUnit);
        }

        public Vector GetInternalDisplacement(double xi)
        {
            Displacement d= GetInternalDisplacementAt(xi, FEM_MAX.LoadCombin);
            Vector dis = new Vector(d.DX, d.DY , d.DZ );
            return UnitConversion.ConvertToMeters(dis);
        }

        /*
        /// <summary>
        /// Calculate the actual maxium force and return all values in a jagged array[6][3]
        /// </summary>
        /// <param name="forceUnit">Convert the result based on given force unit</param>
        /// <returns>all maximum values </returns>
        public double[][] GetMaximumInternalForce(int forceUnit)
        {
            double[][] result = maximumForce;
            
            
            for (int i = 0; i < 6;i++ )
            {
                Func<double, double> f;
                // create the delegate based on user choice 
                switch (i)
                {
                    case 0: // Torsion
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Mx; };
                        break;
                    case 1: // Y_Moment
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).My; };
                        break;
                    case 2: // Z_Moment
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Mz; };
                        break;
                    case 3: // Axial force
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Fx; };
                        break;
                    case 4: // Y shear
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Fy; };
                        break;
                    case 5: // Z shear
                        f = delegate(double x) { return GetInternalForceAt(x, FEM_MAX.LoadCombin).Fz; };
                        break;
                    default:
                        f = delegate(double x) { return 0; };
                        break;

                }
                for (int j = 0; j < 2; j++)
                {                    
                    double left = maximumForceLocation[i][j] - Length / division;
                    double right = left + 2 * l / division;
                    double val = maximumForce[i][j];
                    if (left < 0)
                        left = 0;
                    if (right > Length)
                        right = l;
                    if (j == 0)
                    {
                        maximumForce[i][j] = GoldenSearch.FindExtremum(f, left, right, GoldenSearch.Extermum.Maximum);
                        result[i][0] = Extensions.ConvertOutputForce(maximumForce[i][j], (ForceUnit)forceUnit);
                    }
                    else
                    {
                        maximumForce[i][j] =  GoldenSearch.FindExtremum(f, left, right, GoldenSearch.Extermum.Minimum);
                        result[i][1] = Extensions.ConvertOutputForce(maximumForce[i][j], (ForceUnit)forceUnit);
                    }
                }
                if (Math.Abs(result[i][0]) > Math.Abs(result[i][1]))
                {
                    result[i][2] = Math.Abs(result[i][0]);
                }
                else
                {
                    result[i][2] = Math.Abs(result[i][1]);
                }

            }
            // finding the maximum compressive stress
            var CSY = Math.Max(CalculateStress(true, result[1][0], LoadDirection.Y), CalculateStress(true, result[1][1], LoadDirection.Y)); // check the compressive stress in Y direction
            var CSZ = Math.Max(CalculateStress(true, result[2][0], LoadDirection.Z), CalculateStress(true, result[2][1], LoadDirection.Z)); // check the compressive stress in Z direction
            CS = Math.Max(CSY, CSZ);
            // finding the maximum tensile stress
            var TSY = Math.Max(CalculateStress(false, result[1][0], LoadDirection.Y), CalculateStress(false, result[1][1], LoadDirection.Y)); // check the tensile stress in Y direction
            var TSZ = Math.Max(CalculateStress(false, result[2][0], LoadDirection.Z), CalculateStress(false, result[2][1], LoadDirection.Z)); // check the tensile stress in Z direction
            TS = Math.Max(TSY, TSZ);
            BS = Math.Max(CS, TS);
            return result;


        }
         * */
        /// <summary>
        /// calculte compressive stress 
        /// </summary>
        /// <param name="stressType"> true for compressive false for tensile </param>
        /// <param name="moment"> maximum signed moment </param>
        /// <param name="direction">Z or Y</param>
        /// <returns></returns>
        private double CalculateStress (bool stressType, double moment, LoadDirection direction)
        {
            switch (direction) 
            {
                case (LoadDirection.Z) :
                if ((moment > 0) == stressType)                     
                    return Math.Abs(moment * Sec.Czt / Sec.Iz);                
                else
                    return Math.Abs(moment * Sec.Czb / Sec.Iz);
            
                case (LoadDirection.Y):
                if ((moment > 0) == stressType)
                    return Math.Abs(moment * Sec.Cyr / Sec.Iy);
                else
                    return Math.Abs(moment * Sec.Cyl / Sec.Iy);
                default :
                return 0;
            }

        }

       
        public double GetTrussInternalForce(int forceUnit)
        {
            
            Force tmp = GetInternalForceAt(0,FEM_MAX.LoadCombin);
            return UnitConversion.ConvertOutputForce(tmp, (ForceUnit)forceUnit).Fx;
        }
        /// <summary>
        /// Adding self load the the element
        /// </summary>
        public void AddSelfLoad()
        {
            //double f = (element1D is TrussElement2Node) ? ((TrussElement2Node)element1D).A * Mat.W: ((FrameElement2Node)element1D).A * Mat.W; // calculte the weight per meter
            //if (f != 0)
            //{
            //var _l = new UniformLoad(-f, LoadDirection.Z, CoordinationSystem.Global,FEM_MAX.SelfLoad);
            double f = Mat.W * Sec.Area;
            if (Behavior == BarElementBehaviours.Truss)
            {
                // in case of the truss we transfer the loads to the nodes 
                var nodalLoad = new NodalLoad(new Force(0, 0, -f / 2,0,0,0), FEM_MAX.SelfLoad);
                StartNode.Loads.Add(nodalLoad);
                EndNode.Loads.Add(nodalLoad);

            }
            else
            {
                var _l = new UniformLoad(FEM_MAX.SelfLoad, Vector.K, -f, CoordinationSystem.Global);
                Loads.Add(_l);
            }
            //}
        }
        /// <summary>
        /// Returns true if element is cantileaver 
        /// </summary>
        /// <returns></returns>
        internal bool IsCantilever()
        {
            return (StartReleaseCondition ==  Constraints.Released  || StartNode.Constraints == Constraints.Released);
        }
        /// <summary>
        /// retunrs the effective length of a compressive member in given direction
        /// </summary>
        /// <param name="dir"> Y or Z</param>
        /// <returns></returns>
        internal double GetEffectiveLength (LoadDirection dir)
        {
            int tmp = 0;
            if (dir == LoadDirection.Y)
            {
                if (StartNode.Constraints.DY == DofConstraint.Fixed)
                    tmp += 100;
                if (EndNode.Constraints.DY == DofConstraint.Fixed)
                    tmp += 100;
                if (StartNode.Constraints.RZ == DofConstraint.Fixed)
                    tmp += 10;
                if (EndNode.Constraints.RZ == DofConstraint.Fixed)
                    tmp += 10;
            }
            else
            {
                if (StartNode.Constraints.DZ == DofConstraint.Fixed)
                    tmp += 100;
                if (EndNode.Constraints.DZ == DofConstraint.Fixed)
                    tmp += 100;
                if (StartNode.Constraints.RY == DofConstraint.Fixed)
                    tmp += 10;
                if (EndNode.Constraints.RY == DofConstraint.Fixed)
                    tmp += 10;
            }
            switch (tmp)
            {
                case (220):
                    return 0.7*Length;
                case (210):
                    return 0.85*Length;
                case (200):
                    return 1.0*Length;
                case(120):
                    return 1.2*Length;
                case (110):
                    return 2.2*Length;
                default:
                    return 1*Length;
            }
        }

        internal double SlendernessRatio ()
        {
            if (Sec.Ky < Sec.Kz)
                return (GetEffectiveLength(LoadDirection.Y) / Sec.Ky);
            else
                return (GetEffectiveLength(LoadDirection.Z) / Sec.Kz);
        }
        internal double CriticalBucklingLoad()
        {
            double r = SlendernessRatio();
            return Math.Pow(Math.PI, 2) * Mat.E * Sec.Area / Math.Pow(r, 2);
        }
        internal double BucklingLoad()
        {
            if (Sec.Iy<Sec.Iz)
            {
                return Math.Pow(Math.PI,2)*Mat.E*Sec.Iy/Math.Pow(GetEffectiveLength(LoadDirection.Y),2);
            }else{
                return Math.Pow(Math.PI,2)*Mat.E*Sec.Iz/Math.Pow(GetEffectiveLength(LoadDirection.Z),2);
            }
            
        }

        internal double GetElementLength()
        {
            return (StartNode.Location - EndNode.Location).Length;
        }
    }
}
