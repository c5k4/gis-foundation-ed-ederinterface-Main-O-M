using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Desktop.EDER.UFM
{
    #region FilteredVault class

    public static class VaultFilter
    {
        // Store the result for other folks to consume
        private static string _filteredVault = string.Empty;

        #region Property accessors

        public static string FilteredVault
        {
            get { return _filteredVault; }
            set { _filteredVault = value; }
        }

        #endregion
    }

    #endregion
}
