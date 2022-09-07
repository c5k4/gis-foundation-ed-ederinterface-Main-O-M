using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace PGE.Interfaces.SettingsEmailNotification
{
    interface IBuildEmail
    {
        List<Email> BuildEmails();
    }
}
