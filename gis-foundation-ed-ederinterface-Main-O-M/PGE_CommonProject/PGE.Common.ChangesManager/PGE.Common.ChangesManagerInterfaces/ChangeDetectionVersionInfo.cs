using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PGE.Common.ChangesManagerShared
{
    public class ChangeDetectionVersionInfo
    {
        //V3SF - CD API (EDGISREARC-1452) - Added [START]
        public string VersionTable;
        public string VersionTableConnStr;
        public string VersionWhereClause;
        public int RecordCount;
        public DateTime fromDateTime;
        public DateTime toDateTime;
        public bool UpdateEXESumm = true;
        //V3SF - CD API (EDGISREARC-1452) - Added [END]

        public string VersionLow;
        public string VersionHigh;

        public string VersionSource
        {
            get
            {
                return VersionHigh;
            }
        }

        public string VersionTarget
        {
            get
            {
                return VersionLow;
            }
        }

        public string VersionTemp;

        public string WriteVersionTemp;

        public ChangeDetectionVersionInfo(string versionLow, string versionHigh, string versionTemp)
        {
            VersionHigh = versionHigh;
            VersionLow = versionLow;
            VersionTemp = versionTemp;
        }

        /// <summary>
        ///V3SF - CD API (EDGISREARC-1452) - Added
        /// </summary>
        /// <param name="versionLow">version Name</param>
        /// <param name="versionHigh"></param>
        /// <param name="versionTemp"></param>
        /// <param name="versionTable">GDBM Version Difference Table Name</param>
        public ChangeDetectionVersionInfo(string versionLow, string versionHigh, string versionTemp,string versionTable,string versionWhereClause)
        {
            VersionHigh = versionHigh;
            VersionLow = versionLow;
            VersionTemp = versionTemp;
            VersionTable = versionTable;
            VersionWhereClause = versionWhereClause;
        }

        /// <summary>
        ///V3SF - CD API (EDGISREARC-1452) - Added
        /// </summary>
        /// <param name="versionLow">version Name</param>
        /// <param name="versionHigh"></param>
        /// <param name="versionTemp"></param>
        /// <param name="versionTable">GDBM Version Difference Table Name</param>
        /// <param name="versionTableConnStr">Version Difference Table Connection String</param>
        public ChangeDetectionVersionInfo(string versionLow, string versionHigh, string versionTemp, string versionTable,string versionTableConnStr,string versionWhereClause)
        {
            VersionHigh = versionHigh;
            VersionLow = versionLow;
            VersionTemp = versionTemp;
            VersionTable = versionTable;
            VersionTableConnStr = PGE_DBPasswordManagement.ReadEncryption.GetConnectionStr(versionTableConnStr);
            VersionWhereClause = versionWhereClause;
        }

        /// <summary>
        ///V3SF - CD API (EDGISREARC-1452) - Added
        /// </summary>
        /// <param name="versionLow">version Name</param>
        /// <param name="versionHigh"></param>
        /// <param name="versionTemp"></param>
        /// <param name="versionTable">GDBM Version Difference Table Name</param>
        /// <param name="versionTableConnStr">Version Difference Table Connection String</param>
        public ChangeDetectionVersionInfo(string versionLow, string versionHigh, string versionTemp, string versionTable, string versionTableConnStr, string versionWhereClause, int recordCount)
        {
            VersionHigh = versionHigh;
            VersionLow = versionLow;
            VersionTemp = versionTemp;
            VersionTable = versionTable;
            VersionTableConnStr = PGE_DBPasswordManagement.ReadEncryption.GetConnectionStr(versionTableConnStr);
            VersionWhereClause = versionWhereClause;
            RecordCount = recordCount;
        }

        /// <summary>
        ///V3SF - CD API (EDGISREARC-1452) - Added
        /// </summary>
        /// <param name="versionLow">version Name</param>
        /// <param name="versionHigh"></param>
        /// <param name="versionTemp"></param>
        /// <param name="versionTable">GDBM Version Difference Table Name</param>
        /// <param name="versionTableConnStr">Version Difference Table Connection String</param>
        public ChangeDetectionVersionInfo(string versionLow, string versionHigh, string versionTemp, string versionTable, string versionTableConnStr, string versionWhereClause, int recordCount, string startDate, string endDate, bool UpdateEXESumm)
        {
            VersionHigh = versionHigh;
            VersionLow = versionLow;
            VersionTemp = versionTemp;
            VersionTable = versionTable;
            VersionTableConnStr = PGE_DBPasswordManagement.ReadEncryption.GetConnectionStr(versionTableConnStr);
            VersionWhereClause = versionWhereClause;
            RecordCount = recordCount;

            fromDateTime = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
            toDateTime = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);

            this.UpdateEXESumm = UpdateEXESumm;

        }
    }
}
