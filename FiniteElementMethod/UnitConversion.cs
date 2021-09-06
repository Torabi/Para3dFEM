using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
namespace FiniteElementMethod
{
    public enum ElasticModulusUnit
    {
        PA = 1,
        MPA = 2,
        GPA = 3
    }
    public enum ForceUnit
    {
        N = 1,
        KN = 2
    }

    public enum AreaUnit
    {
        MM2 = 1,
        CM2 = 2,
        M2 = 3

    }
    public static class UnitConversion
    {
        public static string SysUnit;
        /// <summary>
        /// Converts forces from N to other units
        /// </summary>
        /// <param name="f">Force value in N</param>
        /// <param name="fu">Desired force unit</param>
        /// <returns>Converted force value</returns>
        public static Force ConvertOutputForce(Force f, ForceUnit fu)
        {
            switch (fu)
            {
                case ForceUnit.N:
                    return f;

                case ForceUnit.KN:
                    return f * (1.0 / 1000.0);

                default:
                    return f;

            }

        }
        /// <summary>
        /// Converts forces from N to other units
        /// </summary>
        /// <param name="f">Force value in N</param>
        /// <param name="fu">Desired force unit</param>
        /// <returns>Converted force value</returns>
        public static double ConvertOutputForce(double f, ForceUnit fu)
        {
            switch (fu)
            {
                case ForceUnit.N:
                    return f;

                case ForceUnit.KN:
                    return f * (1.0 / 1000.0);

                default:
                    return f;

            }

        }
        /// <summary>
        /// convert elastic modulus from other units to PA
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static double ConvertInputElasticModulus(double val, ElasticModulusUnit emu)
        {
            switch (emu)
            {
                case ElasticModulusUnit.GPA:
                    return  val / 1000000000.0;
                case ElasticModulusUnit.MPA:
                    return val / 1000000.0;
                case ElasticModulusUnit.PA:
                    return val;
                default :
                    return val;

            }
        }
        /// <summary>
        /// Convert the given force to N
        /// </summary>
        /// <param name="f">force value</param>
        /// <param name="fu">Unit of force value</param>
        /// <returns></returns>
        public static Force ConverInputForce(Force f, ForceUnit fu)
        {
            switch (fu)
            {
                case ForceUnit.N:
                    return f;

                case ForceUnit.KN:
                    return 1000 * f;

                default:
                    return f;

            }

        }
        /// <summary>
        /// Convert the given force to N
        /// </summary>
        /// <param name="f">force value</param>
        /// <param name="fu">Unit of force value</param>
        /// <returns></returns>
        public static double ConverInputForce(double f, ForceUnit fu)
        {
            switch (fu)
            {
                case ForceUnit.N:
                    return f;

                case ForceUnit.KN:
                    return 1000 * f;

                default:
                    return f;

            }

        }
        /// <summary>
        /// Convert given length to meters based on scene units in max
        /// </summary>
        
        /// <param name="x">length</param>
        /// <returns>length in meters</returns>
        public static double ConvertToMeters(double x)
        {
            switch (SysUnit.ToLower())
            {
                case "inches":
                    return (x * 0.0254);
                case "feet":
                    return (x * 0.3048);

                case "miles":
                    return (x * 1609.34);

                case "centimeters":
                    return (x * 0.01);

                case "meters":
                    return (x);

                case "kilometers":
                    return (x * 1000);
                case "millimeters":
                    return (x * 0.001);
                default:
                    return x;
            }
        }

        public static Vector ConvertToMeters(Vector x)
        {
            return ConvertToMeters(1.0) * x;
        }
        /// <summary>
        /// Convert given length to meters based on scene units in max
        /// and round the number off by 5 digits
        /// </summary>               
        /// <param name="x"></param>
        /// <returns>Rounded length in meters </returns>
        public static double RoundToMeters(double x)
        {
            return ConvertToMeters(Math.Round(x, 4));
        }

    }
}
