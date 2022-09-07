#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace Telvent.PGE.Test.Data
{
    public class TestFields:IFields
    {
        #region IFields Members

        public int FieldCount
        {
            get { return 1; }
        }

        public int FindField(string Name)
        {
            return 0;
        }

        public int FindFieldByAliasName(string Name)
        {
            throw new NotImplementedException();
        }

        public IField get_Field(int Index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
