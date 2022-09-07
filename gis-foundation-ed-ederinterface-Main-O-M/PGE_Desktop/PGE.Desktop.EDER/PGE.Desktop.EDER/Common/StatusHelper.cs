using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Desktop.EDER
{
    public static class StatusHelper
    {
        #region Private Variables
        private static readonly string _statusFieldModelName = SchemaInfo.Electric.FieldModelNames.Status;

        #endregion

        /// <summary>
        /// Checks to see if a line has a proposed status.
        /// </summary>
        /// <param name="line"> Feature</param>
        /// <returns> True if it's proposed or false if it's not.</returns>
        public static bool IsProposed(IObject feature)
        {
            bool isProposed = false;
            int status = feature.GetFieldValue(null, false, _statusFieldModelName).Convert(-1);
            if (status == 0)
            {
                isProposed = true;
            }

            return isProposed;

        }
        /// <summary>
        /// Checks to see if a feature has a Idle status.
        /// </summary>
        /// <param name="feature"> </param>
        /// <returns> True if it's Idle or false if it's not.</returns>
        public static bool IsIdle(IObject feature)
        {
            bool isIdle = false;
            int status = feature.GetFieldValue(null, false, _statusFieldModelName).Convert(-1);
            if (status == 30)
            {
                isIdle = true;
            }

            return isIdle;

        }

        /// <summary>
        /// Checks to see if a status value is Idle.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool IsIdle(int status)
        {
            bool isIdle = false;
            if (status == 30)
            {
                isIdle = true;
            }

            return isIdle;

        }

        /// <summary>
        /// Checks to see if a line has a Idle status.
        /// </summary>
        /// <param name="line"> Feature</param>
        /// <returns> True if it's Idle or false if it's not.</returns>
        public static bool IsInService(IObject feature)
        {
            bool isInService = false;
            int status = feature.GetFieldValue(null, false, _statusFieldModelName).Convert(-1);
            if (status == 5)
            {
                isInService = true;
            }

            return isInService;

        }
    }
}
