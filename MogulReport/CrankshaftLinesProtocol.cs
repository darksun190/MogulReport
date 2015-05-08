using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPInterface;

namespace MogulReport
{
    class CrankshaftLinesProtocol : Protocol
    {
        private List<Line> lines = new List<Line>();
        public CrankshaftLinesProtocol(List<Feature> eles)
        {
            foreach (var p in eles)
            {
                lines.Add((Line)p);
            }
            

        }
    }
}
