using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.AUConveyor.Processing
{
    internal struct ProcessOIDInfo
    {
        internal int OID;
        internal int ClassID;

        internal ProcessOIDInfo(int oid, int classid)
        {
            OID = oid;
            ClassID = classid;
        }
    }
}
