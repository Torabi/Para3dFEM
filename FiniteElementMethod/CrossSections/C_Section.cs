﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
namespace FiniteElementMethod.CrossSections
{
    public class C_Section : Standard_Section
    {
        public C_Section(double h, double w, double tf, double tw)
            : base(h, w, tf, tw)
        {
            SectionType = Section_Type.C;
            Shape = new PointYZ[9]
                {
                    new PointYZ(-w/2,-h/2),
                    new PointYZ(-w/2,h/2),
                    new PointYZ(w/2,h/2),
                    new PointYZ(w/2,h/2-tf),
                    new PointYZ(-w/2+tw,h/2-tf),
                    new PointYZ(-w/2+tw,-h/2+tf),
                    new PointYZ(w/2,-h/2+tf),
                    new PointYZ(w/2,-h/2),
                    new PointYZ(-w/2,-h/2),
                };
        }
        
    }
}
