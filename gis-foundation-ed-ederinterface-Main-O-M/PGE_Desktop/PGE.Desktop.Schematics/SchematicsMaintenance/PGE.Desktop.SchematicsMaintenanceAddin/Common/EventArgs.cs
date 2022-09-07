using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Desktop.SchematicsMaintenance.Common
{
    public class EventArgs<T> : System.EventArgs
    {
        public T AssociatedData { get; private set; }
        public EventArgs(T associatedData)
        {
            this.AssociatedData = associatedData;
        }
    }
}
