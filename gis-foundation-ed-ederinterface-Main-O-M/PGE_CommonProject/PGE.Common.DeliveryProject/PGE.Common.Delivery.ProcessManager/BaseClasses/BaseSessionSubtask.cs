using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Miner.Interop;
using Miner.Interop.Process;

namespace PGE.Common.Delivery.Process.BaseClasses
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseSessionSubtask : BasePxSubtask
    {

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public BaseSessionSubtask(string name) : base(name)
        {
            AddExtension(BasePxSubtask.SessionManagerExt); 
        }

        #endregion

        #region Protected Methods


        #endregion

    }
}
