using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.DirectoryServices;
using System.Web.Hosting;

namespace SettingsApp.Common
{
    public class Security
    {
        public static bool IsInAdminGroup
        {
            get { return isInAdminGroup(); }
           
        }
        //New Group-Circuit Load-INC000004403314
        public static bool IsInAdminGroup_CircuitLoad
        {
            get { return isInAdminGroup_CircuitLoad(); }

        }
        private static bool isInAdminGroup_CircuitLoad()
        {
            if (Constants.prefix.ToUpper() == "DEV")
                return true;
            else
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(Common.Constants.Admin_Group_Name_Electric_Operations);
            }
        }
        //******************ENOS2EDGIS Start************************
        /// <summary>
        /// ENOS2EDGIS, to check whether an engineer is part of super user engineer group
        /// </summary>
        public static bool IsInSuperUserGroup
        {
            get { return isInSuperUserGroup(); }

        }

        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        public static bool IsSuperUserActive { get; set; }
        //*******************ENOS2EDGIS End************************

        private static bool isInAdminGroup()
        {
            if (Constants.prefix.ToUpper() == "DEV")
                return true;
            else
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(Common.Constants.Admin_Group_Name);
            }
        }

        //******************ENOS2EDGIS Start************************
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <returns></returns>
        private static bool isInSuperUserGroup()
        {
            if (Constants.prefix.ToUpper() == "DEV")
                return true;
            else
            {
                //ENOS2EDGIS - only checking for super user group and not for admin group

                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(Common.Constants.Super_User_Group_Name);
            }
        }
        //*******************ENOS2EDGIS End*****************************
        public static string CurrentUserName
        {
            get 
            {
                //WindowsIdentity identity = WindowsIdentity.GetCurrent();
                //return identity.Name;
                string retVal = string.Empty;
                WindowsPrincipal p = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                using (HostingEnvironment.Impersonate())
                {
                    UserPrincipal userPrincipal = null;
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Constants.ADDomain))
                    {
                        userPrincipal = UserPrincipal.FindByIdentity(pc, p.Identity.Name);
                    }
                    retVal = userPrincipal.DisplayName;
                }
                return retVal;
            }
        }

        public static string CurrentUser
        {
            get
            {
                string retVal = string.Empty;

                WindowsPrincipal p = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                using (HostingEnvironment.Impersonate())
                {
                    UserPrincipal userPrincipal = null;
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Constants.ADDomain))
                    {
                        userPrincipal = UserPrincipal.FindByIdentity(pc, p.Identity.Name);
                    }
                    //ENOS2EDGIS Start, incase if userprinciple is null, take user name from Environment
                    //retVal = userPrincipal.SamAccountName;
                    retVal = userPrincipal != null ? userPrincipal.SamAccountName : Environment.UserName;
                    //****************************ENOS2EDGIS End************************
                }
                return retVal;
            }
        }
    }
}
