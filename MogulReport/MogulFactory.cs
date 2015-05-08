using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPInterface;

namespace MogulReport
{
    /// <summary>
    /// factory class to get a report instance
    /// </summary>
    class MogulFactory
    {
        public static Protocol createProtocol(List<Feature> eles)
        {
            var types = eles.Select(n => n.geoType).ToList();
            var circle_types = eles.Where(n => n.geoType == FeatureType.Circle).ToList();
            var line_types = eles.Where(n => n.geoType == FeatureType.Line).ToList();

            if(circle_types.Count == types.Count)
            {
                return new CrankshaftCirclesProtocol(eles);
            }
            if(line_types.Count == types.Count)
            {
                return new CrankshaftLinesProtocol(eles);
            }

            throw new NotImplementedException();
             
        }
        
    }
}
