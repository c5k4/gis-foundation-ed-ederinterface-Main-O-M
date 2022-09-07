using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Common.ChangesManagerShared
{
    public class FeatureClassChangeRestrictions
    {
        public string FeatureClassName { get; set; }
        public IList<string> AttributesRestricted { get; set; }
    }
}
