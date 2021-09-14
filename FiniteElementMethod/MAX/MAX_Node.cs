using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
namespace FiniteElementMethod
{
 
    public class MAX_Node : Node
    {
        List<string> LoadUniqueIds;
        internal int Index2;
        public MAX_Node(double x, double y, double z)
            : base(x, y, z)
        {
            LoadUniqueIds = new List<string>();
        }
        public Force GetInternalForce(int forceUnit)
        {
            Force tmp = GetSupportReaction(FEM_MAX.LoadCombin);
            return UnitConversion.ConvertOutputForce(tmp, (ForceUnit)forceUnit);
        }

        public Displacement GetDefaultNodalDisplacement()
        {
            return GetNodalDisplacement(FEM_MAX.LoadCombin);
        }
      
    }
 
    
}
