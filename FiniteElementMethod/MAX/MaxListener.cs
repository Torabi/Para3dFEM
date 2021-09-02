using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Common;
namespace FiniteElementMethod
{
    public class MAXListener : ITraceListener
    {
        List<TraceRecord> TraceRecords = new List<TraceRecord>();
        public System.Collections.ObjectModel.ReadOnlyCollection<TraceRecord> Records
        {
            get { return new System.Collections.ObjectModel.ReadOnlyCollection<TraceRecord>(this.TraceRecords); }
        }

        public void Write(TraceRecord record)
        {
            this.TraceRecords.Add(record);
        }




    }
        
}
