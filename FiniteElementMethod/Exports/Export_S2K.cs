using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;

using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Loads;
using CSparse;
using FiniteElementMethod.CrossSections;
using BriefFiniteElementNet;

namespace FiniteElementMethod
{
    public partial class Exports
    {
        public static string GetSectionInfo (CrossSection s)
        {
            /*
             * for known sections
             *  SectionName=FSEC1   Material=A992Fy50   Shape="I/Wide Flange"   t3=0.3048   t2=0.127   tf=0.009652   tw=0.00635   t2b=0.127   tfb=0.009652   Area=0.0042645076   TorsConst=9.65117678053953E-08   I33=6.5724174702235E-05 _
        I22=3.30125717301008E-06   I23=0   AS2=0.00193548   AS3=2.04300666666667E-03   S33=4.3126098885981E-04   S22=5.19883019371667E-05   Z33=4.911874950424E-04   Z22=0.000080716532115   R33=0.124144683132414   R22=2.78230817990979E-02 _
        ConcCol=No   ConcBeam=No   Color=Yellow   TotalWt=5908.52453895998   TotalMass=602.501799682785   FromFile=No   AMod=1   A2Mod=1   A3Mod=1   JMod=1   I2Mod=1   I3Mod=1   MMod=1   WMod=1   Notes="Added 2/6/2016 10:51:29 PM"
             * 
             * */

            /*
             * for customshapes
             *  SectionName=FSEC2   Material=4000Psi   Shape="SD Section"   Area=0.46875   TorsConst=2.97116891403892E-02   I33=0.0216796875   I22=1.64713541666667E-02   I23=-0.00361328125   AS2=0.429568890003405   AS3=0.417778255173891 _
        S33=0.05419921875   S22=3.80108173076923E-02   Z33=8.70174919521986E-02   Z22=7.51953125000077E-02   R33=0.215058131676066   R22=0.1874536979867   ConcCol=No   ConcBeam=No   Color=White   TotalWt=198813.838636565 _
        TotalMass=20273.3685471594   FromFile=No   AMod=1   A2Mod=1   A3Mod=1   JMod=1   I2Mod=1   I3Mod=1   MMod=1   WMod=1   Notes="Added 2/25/2016 9:37:34 PM"
             * */

        
            
            
            //Triangulation2D.Triangulate(new Polygon(Geom), out productOfInertia);
            /*
            string result = "\t"+"SectionName=SEC" + ID.ToString() + "\t";
            result += "Material=" + Mat.Name+Mat.ID.ToString()+"\t";
            result += "Shape=" + "\"" + Name+ID.ToString() + "\""+"\t";
            result += "t3=" + W.ToString() + "\t" + "t2=" + H.ToString() + "\t" + "tf=" + TF.ToString() + "\t" + "tw=" + TW.ToString() + "\t"+"t2b="+TW.ToString()+"\t"+"tfb="+TF.ToString()+"\t";
            result += string.Format("Area={0}   TorsConst={1}   I33={2}\t", gp[3], 11, gp[0])+"\t";
            result += string.Format("I22={0}   I23=0   AS2={1}   AS3={2}   S33={3}   S22={4}   Z33={5}   Z22={6}   R33={7}   R22={8}    ",gp[1],gp[4],gp[5],0,0,0,0,0,0) + "\t";
            result += string.Format("ConcCol=No   ConcBeam=No   Color=Yellow   TotalWt={0}   TotalMass={1}   FromFile=No   AMod=1   A2Mod=1   A3Mod=1   JMod=1   I2Mod=1   I3Mod=1   MMod=1   WMod=1   Notes=\"Added 2/6/2016 10:51:29 PM\"",Mat.W,Mat.W/9.8)+"\r\n";
            */
            
            string result = "";
            result += "\t" + string.Format("SectionName=SEC{0}   Material={1}{2}   Shape=\"SD Section\"", s.ID, s.Mat.Name, s.Mat.ID);
            result += "\t" + string.Format("Area={0}   TorsConst={1}   I33={2}   I22={3}   I23={4}   AS2={5}   AS3={6}", s.Area,0, s.Iy,s.Iz, s.Iyz, s.Sy, s.Sz);
            result += "\t" + string.Format("S33={0}   S22={1}   Z33={2}   Z22={3}   R33={4}   R22={5}   ConcCol=No   ConcBeam=No   Color=White   TotalWt={6}",0,0,0,0,0,0,s.Mat.W);
            result += "\t"+ string.Format("TotalMass={0}   FromFile=No   AMod=1   A2Mod=1   A3Mod=1   JMod=1   I2Mod=1   I3Mod=1   MMod=1   WMod=1   Notes=\"Added {1}\"",s.Mat.W/9.8,0);
            return result;
        }
        
        public static string GetSectionDesignerProperties(CrossSection s)
        {
            /*
             * SectionName=FSEC1   DesignType="No Check/Design"   DsgnOrChck=Check   BaseMat=A992Fy500   IncludeVStr=No   nTotalShp=1   nIWideFlng=0   nChannel=0   nTee=0   nAngle=0   nDblAngle=0   nBoxTube=0   nPipe=0   nPlate=0   nSolidRect=0 _
        nSolidCirc=0   nSolidSeg=0   nSolidSect=0   nPolygon=1   nReinfSing=0   nReinfLine=0   nReinfRect=0   nReinfCirc=0   nRefLine=0   nRefCirc=0   nCaltransSq=0   nCaltransCr=0   nCaltransHx=0   nCaltransOc=0
             */

            string tmp = "";
            tmp+= "\t"+ string.Format("SectionName=SEC{0}   DesignType=\"No Check/Design\"   DsgnOrChck=Check   BaseMat={1}   IncludeVStr=No   nTotalShp=1   nIWideFlng=0   nChannel=0   nTee=0   nAngle=0   nDblAngle=0   nBoxTube=0   nPipe=0   nPlate=0   nSolidRect=0",s.ID,s.Mat);
            tmp += "\t" + string.Format("nSolidCirc=0   nSolidSeg=0   nSolidSect=0   nPolygon={0}   nReinfSing=0   nReinfLine=0   nReinfRect=0   nReinfCirc=0   nRefLine=0   nRefCirc=0   nCaltransSq=0   nCaltransCr=0   nCaltransHx=0   nCaltransOc=0",1);
            return (tmp+"\r\n");
        }

        public static string GetShapePolyGon(CrossSection s)
        {
            /*
            SectionName=FSEC1   ShapeName=Polygon2   X=-7.87654146552086E-02   Y=0.103315711021423   Radius=0   ShapeMat=4000Psi   ZOrder=1   FillColor=Green   Reinforcing=No
   SectionName=FSEC1   ShapeName=Polygon2   X=7.67195969820023E-02   Y=0.105361565947533   Radius=0 
            */                        
            string tmp = "";
          
            tmp += "\t" + string.Format("SectionName=SEC{0}   ShapeName={1}{0}   X={2}   Y={3}   Radius=0   ShapeMat={5}{5}   ZOrder=1   FillColor=Green   Reinforcing=No\r\n",s.ID,s.SectionType,s.Shape[0].Y,s.Shape[0].Z,s.Mat.Name,s.Mat.ID);
            for (int i = 1; i < s.Shape.Count-1;i++)
                tmp += "\t" + string.Format("SectionName=SEC{0}   ShapeName={1}{0}   X={2}   Y={3}   Radius=0 ", s.ID, s.SectionType, s.Shape[i].Y, s.Shape[i].Z)+"\r\n";
            return (tmp + "\r\n");
        }
        public static void Export_S2K(FiniteElementMethod.FEM_MAX model, string filePath)
        {
            string log = "";            
            /*
            TABLE:  "PROGRAM CONTROL"
                ProgramName=SAP2000   Version=16.0.0   ProgLevel=Advanced   LicenseOS=No   LicenseSC=No   LicenseHT=No   CurrUnits="N, m, C"   SteelCode="AISC 360-10"   ConcCode="ACI 318-11"   AlumCode="AA-ASD 2000"   ColdCode=AISI-ASD96   RegenHinge=Yes
            */
            log += "TABLE:  \"PROGRAM CONTROL\"\r\n";
            log += "\tProgramName=SAP2000   Version=16.0.0   ProgLevel=Advanced   LicenseOS=No   LicenseSC=No   LicenseHT=No   CurrUnits=\"N, m, C\"   SteelCode=\"AISC 360-10\"   ConcCode=\"ACI 318-11\"   AlumCode=\"AA-ASD 2000\"   ColdCode=AISI-ASD96   RegenHinge=Yes";
            log += "\r\n";
            string joinCoords = "";
            string joinRestraints = "";
            foreach (Node n in model.model.Nodes)
            {
                string xstr = n.Location.X.ToString();
                string ystr = n.Location.Y.ToString();
                string zstr = n.Location.Z.ToString();
                
                joinCoords += "\t" + "Joint=" + (n.Label) + "\t" + "CoordSys=GLOBAL   CoordType=Cartesian   ";
                joinCoords += "XorR=" + xstr + "\t" + "Y=" + ystr + "\t" + "Z=" + zstr + "\t";
                joinCoords += "SpecialJt=No" + "\t";
                joinCoords += "GlobalX=" + xstr + "\t" + "GlobalY=" + ystr + "\t" + "GlobalZ=" + zstr + "\r\n";

                string u1 = (n.Constraints.DX== DofConstraint.Fixed)?"Yes":"No";
                string u2 = (n.Constraints.DY== DofConstraint.Fixed)?"Yes":"No";
                string u3 = (n.Constraints.DZ== DofConstraint.Fixed)?"Yes":"No";
                string r1 = (n.Constraints.RX== DofConstraint.Fixed)?"Yes":"No";
                string r2 = (n.Constraints.RY== DofConstraint.Fixed)?"Yes":"No";
                string r3 = (n.Constraints.RZ== DofConstraint.Fixed)?"Yes":"No";
                joinRestraints += "\t" + "Joint=" + (n.Label)+"\t";
                joinRestraints += "U1=" + u1 + "\t" + "U2=" + u2 + "\t" + "U3=" + u3 + "\t" + "R1=" + r1 + "\t" + "R2=" + r2 + "\t" + "R3=" + r3 + "\r\n";
            }

            string frame_connectivity = "";
            string frame_sections = "";
            string frame_loads_point = "";
            string frame_loads_distributed = "";
            int i = 0;
            foreach (BarElement _e in model.model.Elements)
            {
                /* 
                 Frame=1   JointI=1   JointJ=2   IsCurved=No   Length=288   CentroidX=288   CentroidY=0   CentroidZ=144
                 */
                MAX_Element _me = model._elements[i];   
                Point center = Point.MidPoint(_e.Nodes.Last().Location, _e.Nodes.First().Location);
                double length = _e.GetElementLength();
                frame_connectivity += "\t" + "Frame=" + _e.Label+"\t";
                frame_connectivity += "JointI=" + _e.Nodes.First().Label + "\t" + "JointJ=" + _e.Nodes.Last().Label + "\t";
                frame_connectivity += "IsCurved=No" + "\t" + "Length=" + length.ToString() + "\t";
                frame_connectivity += "CentroidX=" + center.X.ToString() + "\t" + "CentroidY=" + center.Y.ToString() + "\t" + "CentroidZ=" + center.Z.ToString() + "\r\n";
                //frame_connectivity += "IsCurved=No" + "\t" + "Length=" + _e.GetElementLength().ToString() + "\t";
                if (_e is FrameElement2Node)
                {
                    
                    FrameElement2Node _f = _e as FrameElement2Node;
                    /*
                     *   Frame=1   SectionType="I/Wide Flange"   AutoSelect=N.A.   AnalSect=FSEC1   DesignSect=FSEC1   MatProp=Default
                    */
                    
                    frame_sections += "\t"+string.Format("Frame={0}   SectionType=\"{1}\"   AutoSelect=N.A.   AnalSect=SEC{2}   DesignSect=SEC{2}   MatProp=Default",_e.Label, _me.Sec.SectionType, _me.Sec.ID) + "\r\n";
                    
                    foreach (Load l in _e.Loads)
                    {
                        /*
                        * TABLE:  "FRAME LOADS - POINT"
                            Frame=1   LoadPat=DEAD   CoordSys=GLOBAL   Type=Force   Dir=Gravity   DistType=RelDist   RelDist=8.33333333333333E-02   AbsDist=0.5   Force=200
                        */
                        if (l is ConcentratedLoad)
                        {
                            ConcentratedLoad pl = l as ConcentratedLoad;
                            //string cs = (pl.CoordinationSystem == CoordinationSystem.Global) ? "GLOBAL" : "LOCAL";
                            string cs = pl.CoordinationSystem.ToString().ToUpper();
                            string dir = (cs == "GLOBAL") ? pl.Direction.ToString() : (((int)pl.Direction+1).ToString());
                            double ad = pl.DistanseFromStartNode;
                            double rd = ad / length;
                            frame_loads_point += "\t"+string.Format("Frame={0}   LoadPat=DEAD   CoordSys={1}   Type=Force   Dir={2}   DistType=RelDist   RelDist={3}   AbsDist={4}   Force={5}\r\n", _e.Label,cs,dir,rd,ad,pl.Force);
                        }
                        
                        /*
                        TABLE:  "FRAME LOADS - DISTRIBUTED"
                        Frame=1   LoadPat=DEAD   CoordSys=GLOBAL   Type=Force   Dir=Gravity   DistType=RelDist   RelDistA=0   RelDistB=1   AbsDistA=0   AbsDistB=6   FOverLA=3445   FOverLB=3445 
                        */
                        if (l is UniformLoad)
                        {
                            UniformLoad1D ul = l as UniformLoad1D;
                            if (ul.Case.CaseName != "SelfLoad") // ignore the self load
                            {
                                string cs = ul.CoordinationSystem.ToString().ToUpper();
                                string dir = (cs=="GLOBAL")?ul.Direction.ToString():(((int)ul.Direction+1).ToString());
                                frame_loads_distributed += "\t" + string.Format("Frame={0}   LoadPat=DEAD   CoordSys={1}   Type=Force   Dir={2}   DistType=RelDist   RelDistA=0   RelDistB=1   AbsDistA=0   AbsDistB={3}   FOverLA={4}   FOverLB={4} \r\n", _e.Label, cs, dir, length, ul.Magnitude);
                            }
                        }
                    }
                }
                i++;

            }
            /*
             * 
             * TABLE:  "MATERIAL PROPERTIES 01 - GENERAL"
   Material=4000Psi   Type=Concrete   SymType=Isotropic   TempDepend=No   Color=Yellow   Notes="Customary f'c 4000 psi 2/14/2016 6:29:58 PM"
   Material=A992Fy50   Type=Steel   SymType=Isotropic   TempDepend=No   Color=Magenta   Notes="ASTM A992 Grade 50 2/14/2016 6:29:58 PM"
             * */
            log += "TABLE:  \"MATERIAL PROPERTIES 01 - GENERAL\"\r\n";
            foreach (Material m in model.Materials)
            {
                log+= m.GetGeneralInfo()+"\r\n";
            }
            log += "TABLE:  \"MATERIAL PROPERTIES 02 - BASIC MECHANICAL PROPERTIES\"\r\n";
            foreach (Material m in model.Materials)
            {
                log+= m.GetMechanicalInfo()+"\r\n";
            }
            log += "TABLE:  \"FRAME SECTION PROPERTIES 01 - GENERAL\"\r\n" ;
            string sectionDesigner = "";
            string shapePolyGon = "";
           
            foreach (CrossSection sec in model.Sections)
            {
                log+= GetSectionInfo(sec)+"\r\n";
                sectionDesigner += GetSectionDesignerProperties(sec);
                shapePolyGon += GetShapePolyGon(sec);
            }
           
            log += "\r\n";
            /*
             * TABLE:  "SECTION DESIGNER PROPERTIES 01 - GENERAL" 
             * */
            log += "TABLE:  \"SECTION DESIGNER PROPERTIES 01 - GENERAL\"\r\n";
            log += sectionDesigner;
            log += "\r\n";
            /*
             * TABLE:  "SECTION DESIGNER PROPERTIES 16 - SHAPE POLYGON"
             * */
            log += "TABLE:  \"SECTION DESIGNER PROPERTIES 16 - SHAPE POLYGON\"\r\n";
            log += shapePolyGon;
            log += "\r\n";



            /*
             * exporting joints
             * TABLE:  "JOINT COORDINATES"            
             * */
            log += "TABLE:  \"JOINT COORDINATES\"\r\n" + joinCoords;
            log += "\r\n";
            /*
             * TABLE:  "CONNECTIVITY - FRAME"
             */
            log += "TABLE:  \"CONNECTIVITY - FRAME\"\r\n" + frame_connectivity;
            log += "\r\n";
            /* 
             * TABLE:  "JOINT RESTRAINT ASSIGNMENTS"  
             * */
            log += "TABLE:  \"JOINT RESTRAINT ASSIGNMENTS\"\r\n" + joinRestraints;
            log += "\r\n";
            /*
            TABLE:  "FRAME SECTION ASSIGNMENTS"
 
            */
            log += "TABLE:  \"FRAME SECTION ASSIGNMENTS\"\r\n" + frame_sections;
            log += "\r\n";
            /* TABLE:  "FRAME LOADS - POINT"*/
            if (frame_loads_point.Length > 0)
            {
                log += " TABLE:  \"FRAME LOADS - POINT\"\r\n" + frame_loads_point;
                log += "\r\n";
            }
            /* TABLE:  "FRAME LOADS - DISTRIBUTED"*/
            if (frame_loads_distributed.Length > 0)
            {             
                log += " TABLE:  \"FRAME LOADS - DISTRIBUTED\"\r\n" + frame_loads_distributed;
                log += "\r\n";
            }
            log += "END TABLE DATA";
            System.IO.File.WriteAllText(@filePath, log);
 
        }
    }
}
