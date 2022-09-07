using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PGE.BatchApplication.ROBCApp
{
    public class BaseViewModel:INotifyPropertyChanged
    {
        protected Action<string, string> popup = (Action<string, string>)((msg, capt) => System.Windows.MessageBox.Show(msg, capt));
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
