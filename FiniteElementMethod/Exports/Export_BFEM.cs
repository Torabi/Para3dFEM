using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet;
namespace FiniteElementMethod
{
    public partial class Exports
    {
        public static void Export_BFEM (FiniteElementMethod.FEM_MAX model, string filePath)
        {
            string log = "";            
            log += "using System;\r\n";
            log += "using System.Collections.Generic;\r\n";
            log += "using System.Linq;\r\n";
            log += "using System.Text;\r\n";
            log += "using BriefFiniteElementNet.Controls;\r\n";
            log += "using BriefFiniteElementNet.Elements;\r\n";
            log += "\r\n";
            log += "namespace BriefFiniteElementNet.CodeProjectExamples\r\n";
            log += "{\r\n"; // name space
            log+= "class Program\r\n";
            log += "{\r\n"; // class
            log += "[STAThread]\r\n";
            log += "static void Main(string[] args)\r\n";
            log += "{\r\n"; // main

            log += "// wait for user to press any key \r\n";
            log += "Console.ReadKey();\r\n"; // 
            log+= "// Initiating Model, Nodes and Members\r\n";
            log += "var model = new Model();\r\n";
            log += "// registering nodes \r\n";
            
            foreach (Node n in model.model.Nodes)
            {
               
                //var n2 = new Node(-1, 1, 0) { Label = "n2" }
                log += string.Format("var {0} = new Node({1}, {2}, {3}) ",n.Label, n.Location.X, n.Location.Y, n.Location.Z)+"{ Label = \""+n.Label+"\"};\r\n";

                log += n.Label + ".Constraints = new Constraint(";
                log += "DofConstraint."+n.Constraints.DX.ToString()+",DofConstraint."+n.Constraints.DY.ToString()+",DofConstraint."+n.Constraints.DZ.ToString()+",";
                log += "DofConstraint."+n.Constraints.RX.ToString()+",DofConstraint."+n.Constraints.RY.ToString()+",DofConstraint."+n.Constraints.RZ.ToString()+");\r\n";
                log += "model.Nodes.Add(" + n.Label + ");\r\n";
 
 
            }

            
            log += "// registering elements \r\n";

            int i = 0;
            List<int> alreadyMade = new List<int>();
            foreach (Element e in model.model.Elements)
            {
                //var e1 = new FrameElement2Node(n1, n5) { Label = "e1" };
                log += string.Format("var {0} = new FrameElement2Node({1}, {2}) ", e.Label, e.Nodes.First().Label, e.Nodes.Last().Label) + "{ Label = \"{" + e.Label + "}\" };\r\n";
                
                if (e is FrameElement2Node)
                {
                    FrameElement2Node f = e as FrameElement2Node;
                    if (e.Label.Substring(0,1)== "T")
                    {
                        // truss element
                        log += string.Format("{0}.A = {1};{0}.E = {2};\r\n", e.Label, f.A, f.E);
                    }
                    else
                    {
                        // frame element
                        log += string.Format("{0}.E = {1};{0}.G = {2};\r\n", e.Label, f.E, f.G);
                        int secId = model.sectionIds[i];
                        if (!alreadyMade.Contains(secId))
                        {
                            log += "var sec" + secId.ToString() + "= " + model.sections[secId].GetCString()+";\r\n";
                            alreadyMade.Add(secId);
                        }

                        log += e.Label+".Geometry = sec" + secId.ToString() + ";\r\n";
                    }
                }
                log += "model.Elements.Add(" + e.Label + ");\r\n";
                i++;
            }

            log += "// Solve model \r\n";
            log += " model.Solve();\r\n";

            log += "}\r\n"; // main
            log += "}\r\n"; // class
            log += "}\r\n"; // nameSpace

            System.IO.File.WriteAllText(@filePath, log);
        }
    }
}
