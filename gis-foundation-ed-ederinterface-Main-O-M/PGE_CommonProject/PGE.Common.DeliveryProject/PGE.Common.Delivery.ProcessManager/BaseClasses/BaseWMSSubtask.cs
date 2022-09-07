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
    public class BaseWMSSubtask : BasePxSubtask
    {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public BaseWMSSubtask(string name) : base(name)
        {
            AddExtension(BasePxSubtask.WorkflowManagerExt);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets approved design regardless of the current state.
        /// </summary>
        /// <param name="workRequestNode">Parent Work Request Px Node.</param>
        /// <param name="numberOfDesigns">Number of Active designs associated to the work request.</param>
        /// <returns>Design Px Node, or null if there isn't an approved design associated with the work request.</returns>
        protected IMMPxNode GetApprovedDesignNode(IMMPxNode workRequestNode, out int numberOfDesigns)
        {
            IMMPxNode retVal = null;

            // find the design for this work request that is in the specified
            // check all the designs under this work request to see if any are in the specified state
            bool designExists = false;
            ID8List designList = (ID8List)workRequestNode;
            designList.Reset();
            IMMPxNode childDesignNode = null;
            int numberOfDNs = 0;
            if (designList.get_HasChildren(false))
            {
                childDesignNode = (IMMPxNode)designList.Next(false);
                while (childDesignNode != null)
                {
                    IMMWMSDesign wmsDN = (IMMWMSDesign)GetWmsNode(childDesignNode, BasePxSubtask.DesignNodeName);
                    IMMWMSPropertySet wmsPropSet = wmsDN.PropertySet;

                    if ((wmsPropSet.GetProperty("APPROVED_ID").ToString() != "") &&
                        wmsPropSet.GetProperty("APPROVED_DATE").ToString() != "")
                    {
                        designExists = true;
                        break;
                    }
                    childDesignNode = (IMMPxNode)designList.Next(false);

                    numberOfDNs++;
                }
            }
            if (designExists)
            {
                retVal = childDesignNode;
            }

            numberOfDesigns = numberOfDNs;

            return retVal;
        }

        /// <summary>
        /// Returns the approved design if there is one.
        /// </summary>
        /// <param name="designNode"></param>
        /// <returns></returns>
        protected IMMPxNode GetApprovedDesignNode(IMMPxNode designNode)
        {
            //get the WR node:  containedby
            IMMPxNode wrNode = ((ID8ListItem)designNode).ContainedBy as IMMPxNode;
            int numberOfDNs;
            return GetApprovedDesignNode(wrNode, out numberOfDNs);
        }

        #endregion
    }
}
