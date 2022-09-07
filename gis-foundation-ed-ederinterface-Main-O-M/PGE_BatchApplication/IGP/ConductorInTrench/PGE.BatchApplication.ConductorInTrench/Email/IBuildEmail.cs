using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;

namespace PGE.BatchApplication.ConductorInTrench
{
    interface IBuildEmail
    {
        DataTable BuildEmails(string globalID, string DIVISION, DataTable table_PriUG);
    }
}
