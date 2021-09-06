using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using BriefFiniteElementNet.Controls;
//using BriefFiniteElementNet.Elements;
//using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Loads;
using FiniteElementMethod.CrossSections;
using BriefFiniteElementNet.Validation;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Mathh;

namespace FiniteElementMethod
{
    public class FEM_MAX
    {
        public  Model model;
        public static double epsilon = 0.000001;
        private string sysUnit = "Meters";
        public bool Solved = false;
        public static LoadCase SelfLoad = new LoadCase("SelfLoad", LoadType.Dead);
        public static LoadCase DeadLoad = new LoadCase("DeadLoad", LoadType.Dead);
        public static LoadCombination LoadCombin;
        
        internal List<CrossSection> Sections;
        internal List<Material> Materials;
        //internal Dictionary<int, MAX_Element> _elements;

      
        public string SystemUnit
        {
            get
            {
                return sysUnit;
            }
            set
            {
                UnitConversion.SysUnit= sysUnit = value;
                epsilon = UnitConversion.ConvertToMeters(0.001);
            }

        }
        private MAX_Node currentNode;
        private MAX_Element currentElement;
        /*
        enum ForceUnit
        {
            N = 1,
            KN = 2
        }
         * */
       
        /*
        
        private AreaUnit AU = AreaUnit.MM2;
        private ElasticModulusUnit EMU = ElasticModulusUnit.GPA;
        private ForceUnit FU = ForceUnit.KN;

        public void SetAreaUnit(int x) { AU = (AreaUnit)x; }
        public void SetElasticModulusUnit(int x) { EMU = (ElasticModulusUnit)x; }
        public void SetForceUnit(int x) { FU = (ForceUnit)x; }
        */
        /// <summary>
        /// reset the cross section and material database
        /// </summary>
        public void INIT()
        {
            Sections = new List<CrossSection>();
            Materials = new List<Material>();
          
        }
       
        // Initiating Model, Nodes and Members
        public FEM_MAX()
        {
            model = new BriefFiniteElementNet.Model();
            //_elements = new Dictionary<int, MAX_Element>();
            LoadCombin = new LoadCombination();
            LoadCombin.Add(DeadLoad, 1);
            LoadCombin.Add(SelfLoad, 1);
            INIT();
       
        }
        #region Node methods
        public int GetNodeCount()
        {
            return model.Nodes.Count;
        }
        /// <summary>
        /// Add a new node and return the index of node 
        /// </summary>
        /// <param name="x">x position </param>
        /// <param name="y">y position </param>
        /// <param name="z">z position </param>
        /// <returns> if positive the node is a newly created , if negetive node is existing </returns>
        public int AddNode (double x,double y,double z)
        {
            MAX_Node n = new MAX_Node(UnitConversion.RoundToMeters(x), UnitConversion.RoundToMeters(y), UnitConversion.RoundToMeters(z));
                        
            int index = 0;
            foreach(Node item in model.Nodes)
            {
                double dist = (item.Location - n.Location).Length;
                if (dist < epsilon)
                {                    
                    return -1*(index+1);
                }
                index += 1;
            }
            int i = model.Nodes.Count;
            n.Label = "N"+(i+1).ToString();
      
       
            model.Nodes.Add(n);            
            return (i+1);                        
        }
        /// <summary>
        /// set the current node by given node index 
        /// when current node is set any changes in position and nodal load
        /// will effect only this node
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>

        public bool SetCurrentNode (int nodeIndex)
        {
            if (nodeIndex >= 0 && nodeIndex < model.Nodes.Count)
            {
                currentNode = model.Nodes[nodeIndex] as MAX_Node;
                currentNode.Loads.Clear();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// set the position of current node (see SetCurrentNode)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetNodePosition ( double x,double y,double z)
        {
            currentNode.Location = new Point(UnitConversion.RoundToMeters(x), UnitConversion.RoundToMeters(y), UnitConversion.RoundToMeters(z));             
        }
        /// <summary>
        /// sets the constraints of a node
        /// </summary>
        /// <param name="nodeIndex"> the index of the node </param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        /// <param name="rz"></param>
        public void SetNodeConstraints(bool dx, bool dy, bool dz, bool rx, bool ry, bool rz)
        {

            currentNode.Constraints = new Constraint(
                    dx ? DofConstraint.Fixed : DofConstraint.Released,
                    dy ? DofConstraint.Fixed : DofConstraint.Released,
                    dz ? DofConstraint.Fixed : DofConstraint.Released,
                    rx ? DofConstraint.Fixed : DofConstraint.Released,
                    ry ? DofConstraint.Fixed : DofConstraint.Released,
                    rz ? DofConstraint.Fixed : DofConstraint.Released
                    );
            
        }
       
        /// <summary>
        /// Add nodal node to a node
        /// </summary>
        /// <param name="nodeIndex"> index of node in model </param>
        /// <param name="loud unit"> 1: N 2 : KN </param>
        /// <param name="fx"></param>
        /// <param name="fy"></param>
        /// <param name="fz"></param>
        /// <param name="mx"></param>
        /// <param name="my"></param>
        /// <param name="mz"></param>
        public void AddNodalLoad(double fx, double fy, double fz, double mx, double my, double mz, int load_unit,string uniqueID)
        {
                   
            Force f = new Force(fx, fy, fz, mx, my, mz);
            NodalLoad nl = new NodalLoad(UnitConversion.ConverInputForce(f, (ForceUnit)load_unit),DeadLoad);
            currentNode.Loads.Add(nl);
        
        }

        public MAX_Node GetNode (int index)
        {
            return model.Nodes[index] as MAX_Node;
        }
                       
        #endregion 
        #region Element Methods
        public int GetElementCount()
        {
            return model.Elements.Count;
        }
        public bool SetCurrentElement(int index)
        {
            if (index >= 0 && index < model.Elements.Count)
            {
                currentElement =  model.Elements[index] as MAX_Element;
                currentElement.Loads.Clear();

                return true;
            }
            else
                return false;
        }
        public MAX_Element GetElement(int index)
        {
            return (model.Elements[index] as MAX_Element);
        }
        #endregion
        #region Truss methods
        /*
        /// <summary>
       /// Add a truss element into the model
       /// </summary>
       /// <param name="node1">first node of the element</param>
        /// <param name="node2">second node of the element</param>
        /// <param name="E">elastic modulus</param>
        /// <param name="emu">unit of E 1:GFA 2:PA</param>
       /// <param name="A">Cross section Area</param>
       /// <param name="au">Area unit</param>
        public void AddTrussElement(int node1,int node2,int emu, double A,int au)
        {
            var n1 = model.Nodes[node1];
            var n2 = model.Nodes[node2];
            if (n1 == null || n2 == null)
                return ;
            int c = model.Elements.Count;
            var _e = new TrussElement2Node(n1, n2) { Label = "E" + (c + 1).ToString() };
            if (model.Elements.Contains(_e))
                return ;            
            
            switch ((AreaUnit)au)
            {
                case  AreaUnit.MM2:
                    _e.A = A / 1000000.0;
                    break;
                case  AreaUnit.CM2:
                    _e.A = A / 10000;
                    break;
                case  AreaUnit.M2:
                    _e.A = A;
                    break;
            }
            model.Elements.Add(_e);
            

        }
         * */
        /// <summary>
        /// retrive the truss 
        /// </summary>
        /// <param name="elementIndex"></param>
        /// <param name="forceUnit"></param>
        /// <returns></returns>
        /*
        public double GetTrussInternalForce(int elementIndex, int forceUnit)
        {
            var _e = model.Elements[elementIndex] as FrameElement2Node;
            if (_e == null)
                return 0;
            Force tmp = _e.GetInternalForceAt(0);
            return ConverOutputForce(tmp, (ForceUnit)forceUnit).Fx;
        }
         * */
        
      
        
        #endregion
        #region Frame Methods
        /// <summary>
        /// Add a frame element into the model
        /// </summary>
        /// <param name="node1">first node of the element</param>
        /// <param name="node2">second node of the element</param>
    
        public void AddFrameElement(int node1, int node2,string elementType)
        {
            var n1 = model.Nodes[node1] as MAX_Node;
            var n2 = model.Nodes[node2] as MAX_Node;
            if (n1 == null || n2 == null)
                return;
            int c = model.Elements.Count;

            var _e = new MAX_Element(n1, n2, elementType, c + 1);// { Label = lableChar + (c + 1).ToString() };
            //_e.ElementType = (elementType == "Truss") ? ElementType.TrussElement2Noded : ElementType.FrameElement2Node;
            if (model.Elements.Contains(_e))
                return;
            
            model.Elements.Add(_e);
            //_elements.Add(model.Elements.Count - 1, _e);

        }

       
        /// <summary>
        /// Set the physical properties of the currentElement
        /// </summary>
        /// <param name="E"></param>
        /// <param name="Eunit"></param>
        /// <param name="A"></param>
        /// <param name="AUnit"></param>
        public void SetElementMaterial(string category,string name,double E,double G,int Eunit,double W,int Wunit, double fyc,double fyt,int fyunit )
        {

            var m = new Material(category,name,E, G, (ElasticModulusUnit)Eunit,W,(ForceUnit)Wunit,fyc,fyt,(ForceUnit) fyunit );
            
            int existingMaterial = Materials.FindIndex(item => item.Equals(m));
            if (existingMaterial == -1)
            {
                Materials.Add(m);
                m.ID = Materials.Count - 1;
            }
            else
            {
                m = Materials[existingMaterial];
            }
            currentElement.Mat = m;
        
        }
        /// <summary>
        /// adding self-weight to the currentelement
        /// </summary>
  
        public void AddSelfLoad()
        {
            currentElement.AddSelfLoad();           
        }
        /// <summary>
        /// add uniform load on the frame element
        /// </summary>
        /// <param name="fx">X component of the forece</param>
        /// <param name="fy">Y component of the forece</param>
        /// <param name="fz">Z component of the forece</param>
        /// <param name="load_unit">Load unit 1:N 2:KN </param>
        /// <param name="coordSys">coordinate system 1:Global 2:Local </param>
        public void AddUnifromLoad( double fx, double fy, double fz, int load_unit,int coordSys)
        {

            var coord = (coordSys == 1) ? CoordinationSystem.Global : CoordinationSystem.Local;
            if (fx != 0)
            {
                var _l = new UniformLoad( DeadLoad, Vector.I ,UnitConversion.ConverInputForce(fx, (ForceUnit)load_unit), coord);
                currentElement.Loads.Add(_l);
              
            }
            if (fy != 0)
            {
                var _l = new UniformLoad(DeadLoad, Vector.J, UnitConversion.ConverInputForce(fy, (ForceUnit)load_unit), coord);
                currentElement.Loads.Add(_l);
               
            }
            if (fz != 0)
            {
                var _l = new UniformLoad(DeadLoad, Vector.K, UnitConversion.ConverInputForce(fz, (ForceUnit)load_unit), coord);
                currentElement.Loads.Add(_l);
               
            }

        }
        public void AddConcentratedLoad (double fx,double fy,double fz,double mx,double my,double mz,double dist,int load_unit)
        {

            //IsoPoint p =  new IsoPoint(dist);
            //var _l = new ConcentratedLoad(UnitConversion.ConverInputForce(new Force(fx, fy, fz, mx, my, mz),(ForceUnit)load_unit), p, CoordinationSystem.Global);                        
            //currentElement.Loads.Add(_l);

            //var coord = (coordSys == 1) ? CoordinationSystem.Global : CoordinationSystem.Local;
            if (fx != 0)
            {
                var _l = new PartialNonUniformLoad();
                _l.Case = DeadLoad;
                _l.CoordinationSystem = CoordinationSystem.Global;
                _l.Direction = new Vector(1, 0, 0);
                _l.StartLocation = new IsoPoint( dist - 0.001);
                _l.EndLocation = new IsoPoint(dist + 0.001);
                _l.SeverityFunction = new SingleVariablePolynomial(fx);                
                currentElement.Loads.Add(_l);

            }
            if (fy != 0)
            {
                var _l = new PartialNonUniformLoad();
                _l.Case = DeadLoad;
                _l.CoordinationSystem = CoordinationSystem.Global;
                _l.Direction = new Vector(0, 1, 0);
                _l.StartLocation = new IsoPoint(dist - 0.001);
                _l.EndLocation = new IsoPoint(dist + 0.001);
                _l.SeverityFunction = new SingleVariablePolynomial(fy);
                currentElement.Loads.Add(_l);

            }
            if (fz != 0)
            {
                var _l = new PartialNonUniformLoad();
                _l.Case = DeadLoad;
                _l.CoordinationSystem = CoordinationSystem.Global;
                _l.Direction = new Vector(0, 0, 1);
                _l.StartLocation = new IsoPoint(dist - 0.001);
                _l.EndLocation = new IsoPoint(dist + 0.001);
                _l.SeverityFunction = new SingleVariablePolynomial(fz);
                currentElement.Loads.Add(_l);

            }

        }
      /// <summary>
      /// set the element cross section 
      /// </summary>
      /// <param name="w">Width of the section</param>
      /// <param name="h">Height of the section </param>
      /// <param name="tf">Flange thickness </param>
      /// <param name="tw">Web thickness </param>
      /// <param name="section">section type (B,I,L,..) </param>
        public void SetSection (float w, float h, float tf, float tw, float rotationDegree,string section)
        {
        
            double W = UnitConversion.ConvertToMeters(System.Convert.ToDouble(w) );
            double H = UnitConversion.ConvertToMeters(System.Convert.ToDouble(h));
            double TF = UnitConversion.ConvertToMeters(System.Convert.ToDouble(tf));
            double TW = UnitConversion.ConvertToMeters(System.Convert.ToDouble(tw));

            //Section s = new Section(section, H, W, TF, TW);
            CrossSection s = Standard_Section.CreateSection(section, H, W, TF, TW,currentElement.Mat);
            //s.Mat = currentElement.Mat;
            int existingSectionId = Sections.FindIndex(item=>item.Equals(s));
            if (existingSectionId==-1 )
            {
                Sections.Add(s);
                s.ID = Sections.Count - 1;
                s.Initialize();
            }
            else
            {
                s = Sections[existingSectionId];
            }
            currentElement.Sec = s; // this will update the Area and the geometry property of the frameelement 
            currentElement.WebRotation =   rotationDegree ;
        }
       /*
        public Force GetFrameInternalForce(int elementIndex, int forceUnit, double x)
        {
            
            var _e = currentElement as FrameElement2Node;            
            
            if (_e == null)
                return new Force(0,0,0,0,0,0);
            Force tmp = _e.GetInternalForceAt(x * _e.GetElementLength());
            return ConverOutputForce(tmp, (ForceUnit)forceUnit);
        }
        * */
        
        #endregion
        #region Count Methods
      
       
        #endregion
        #region Conversion methods
        
       
       
        #endregion 
        
       
    
        /// <summary>
        /// solves the model
        /// </summary>
        /// <param name="solverType"></param>
        public void Solve(int solverType)
        {
            /*
            string log = "";
           // char[] seperator = new char[]{}
            log += "Nodes \r\n";
            foreach (Node n in model.Nodes)
            {
                log += n.Label + " : @ " + n.Location.ToString()+" "+n.Constraints.ToString()+"\r\n";
            }
            foreach (Element _e in model.Elements)
            {
                log += _e.Label + " : @ " + _e.Nodes[0].Label+ "--"+_e.Nodes[1].Label+"\r\n";
                foreach (Load _l in _e.Loads)
                {
                    log += "     " + _l.ToString()+"\r\n";
                }
                log += "Length = "+_e.GetElementLength()+"\r\n";
                if ( _e is FrameElement2Node)
                    log += "Section properties "+((_e as FrameElement2Node).Geometry.GetSectionGeometricalProperties()[3].ToString())+"\r\n";
            }            
            System.IO.File.WriteAllText(@"D:\femLog.txt", log);
             */
            Solved = false;
     
            model.Trace.Listeners.Clear();
         
            MAXListener maxlistener = new MAXListener();
            model.Trace.Listeners.Add(maxlistener);

            PosdefChecker.CheckModel(model,DeadLoad);
            

            new ModelWarningChecker().CheckModel(model);
            //model.PrecheckForErrors();

            if (maxlistener.Records.Count > 0)
                return;
            switch (solverType)
            {
                case 1:// default
                    model.Solve(SelfLoad,DeadLoad);
                    break;
                    
                case 2: // CholeskyDecomposition
                    model.Solve(BuiltInSolverType.CholeskyDecomposition);
                    break;
                case 3: //ConjugateGradient
                    model.Solve(BuiltInSolverType.ConjugateGradient);
                    break;               
            }
             
            Solved = true;
        }

     
 
        
      
       
    }
}
