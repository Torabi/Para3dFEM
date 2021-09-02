/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
namespace FiniteElementMethod
{
    public class MAX_UniformLoad1D : UniformLoad1D
    {
        public string UniqueID;
        public override bool Equals(object obj)
        {
            if (!(obj is MAX_UniformLoad1D))
                return false;
            MAX_UniformLoad1D l = obj as MAX_UniformLoad1D;
            return (UniqueID == l.UniqueID);
        }
        public MAX_UniformLoad1D (double f,LoadDirection d,CoordinationSystem cs,string uniqueID):base(f,d,cs)
        {
            UniqueID = uniqueID;
        }
    }
    public class MAX_ConcentratedLoad1D : ConcentratedLoad1D
    {
        public string UniqueID;
        public MAX_ConcentratedLoad1D(Force f,double d,CoordinationSystem cs,string uniqueID):base( f, d, cs)
        {
            UniqueID = uniqueID;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is MAX_ConcentratedLoad1D))
                return false;
            MAX_ConcentratedLoad1D l = obj as MAX_ConcentratedLoad1D;
            return (UniqueID == l.UniqueID);
        }
    }
    public class MAX_NodalLoad
    {
        public string UniqueID;
        public NodalLoad NL;
        public MAX_NodalLoad(NodalLoad nl,string uniqueID)
        {
            NL = nl;
            UniqueID = uniqueID;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is MAX_NodalLoad))
                return false;
            MAX_NodalLoad l = obj as MAX_NodalLoad;
            return (UniqueID == l.UniqueID);
        }
    }
    
}
*/