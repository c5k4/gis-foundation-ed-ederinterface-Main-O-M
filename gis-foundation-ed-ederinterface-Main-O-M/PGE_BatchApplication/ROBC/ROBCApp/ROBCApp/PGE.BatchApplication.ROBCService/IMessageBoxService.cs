using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ROBCService
{
    public interface IMessageBoxService
    {
        void ShowNotification(string message);
        bool GetYesNoResponse(string message);
    }
}
