using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using Telvent.Delivery.Framework;
using Miner;

namespace IBM.PGE.SettingsEmailNotification
{
    /// <summary>
    /// Class used to read Registry information for the ArcFM custom Login and Session Manager Logins
    /// Reads Registry information stored under User Registry Setting and Local Machine Registry.
    /// HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection is hte path for User Settings
    /// HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin for System settings
    /// HKCU\Software\Miner and Miner\Process Framework\Database Properties
    /// </summary>
    public class SMTPHelper
    {
        private static IMMRegistry _reg = new MMRegistry();


        #region System Level
        #region Mail Server
        /// <summary>
        /// SMTPServer string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        public static string SMTPServer
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("SMTPServer", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = Telvent.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("SMTPServer", EncryptionFacade.Encrypt(value));
            }
        }
        /// <summary>
        /// MailFromUser string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        public static string MailFromUser
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("MailFromUser", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = Telvent.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("MailFromUser", EncryptionFacade.Encrypt(value));
            }
        }
        /// <summary>
        /// EmailDomain string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        public static string EmailDomain
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("EmailDomain", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = Telvent.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("EmailDomain", EncryptionFacade.Encrypt(value));
            }
        }
        #endregion
        #endregion


        #region Private Methods
        /// <summary>
        /// Given a Name of the Registry key and type to return will read he registry and return the value stored in teh registry.
        /// If the registry key is not found will return the Default value for the Type value sent
        /// </summary>
        /// <typeparam name="T">Type to return the value in</typeparam>
        /// <param name="name">Name of the Registry</param>
        /// <param name="defaultValue">Default value to pass if the value is not found in the registry</param>
        /// <returns></returns>
        private static T ReadRegistry<T>(string name, T defaultValue)
        {
            T retVal = default(T);
            retVal = _reg.Read(name, defaultValue) != null ? (T)_reg.Read(name, defaultValue) : defaultValue;
            return retVal;
        }
        #endregion
    }
}
