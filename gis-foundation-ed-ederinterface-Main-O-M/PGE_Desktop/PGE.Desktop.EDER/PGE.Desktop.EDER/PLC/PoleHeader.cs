using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PGE.Desktop.EDER.PLC
{
    [Guid("35c7e4a5-7462-42f0-af2f-7ef60bd862d6")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.PLC.PoleHeader")]
    public class PoleHeader
    {
        public string Source { get; set; }
        public string TrackingID { get; set; }
        public string Timestamp { get; set; }
    }
}
