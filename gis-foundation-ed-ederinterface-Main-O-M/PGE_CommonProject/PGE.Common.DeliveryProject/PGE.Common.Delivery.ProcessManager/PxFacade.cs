using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop.Process;

namespace PGE.Common.Delivery.Process
{
    /// <summary>
    /// 
    /// </summary>
    public class PxFacade
    {
        #region ProgIds
        /// <summary>
        /// 
        /// </summary>
        public const string PxSDEVersionclassProgId = "mmPxBaseUI.PxSDEVersion";
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pxApp"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetConfigValue(IMMPxApplication pxApp,string config)
        {
            IMMPxHelper2 pxHelper = (IMMPxHelper2)pxApp.Helper;
            return pxHelper.GetConfigValue(config);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pxNd"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static IMMPxTask RetrievePxTask(IMMPxNode pxNd, string taskName)
        {
            IMMPxTask pxTsk = null;

            if (pxNd != null)
            {
                pxTsk = ((IMMPxNode3)pxNd).GetTaskByName(taskName);
            }

            return pxTsk;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="nodeType"></param>
        /// <param name="pxApplication"></param>
        /// <returns></returns>
        public static IMMPxState GetNodeStateFromName(string stateName, int nodeType, IMMPxApplication pxApplication)
        {
            IMMPxState retVal = null;
            IMMEnumPxState states = pxApplication.States;
            states.Reset();
            IMMPxState pxState = states.Next();
            while (pxState != null)
            {
                if ((pxState.NodeType == nodeType) && (pxState.Name.ToUpper() == stateName.ToUpper()))
                {
                    retVal = pxState;
                    break;
                }
            }
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="nodeType"></param>
        /// <param name="pxApp"></param>
        /// <returns></returns>
        public static IMMPxState GetNodeStateByID(int state, int nodeType, IMMPxApplication pxApp)
        {
            IMMPxState retVal = null;
            IMMEnumPxState states = pxApp.States;
            states.Reset();
            IMMPxState pxState = states.Next();
            while (pxState != null)
            {
                if ((pxState.NodeType == nodeType) && (pxState.State == state))
                {
                    retVal = pxState;
                    break;
                }
            }
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pxNd"></param>
        /// <param name="taskName"></param>
        /// <param name="pxApp"></param>
        /// <returns></returns>
        public static bool ExecuteTask(IMMPxNode pxNd, string taskName, IMMPxApplication pxApp)
        {
            bool retVal = false;
            bool unLocked = false;
            IMMPxTask pxTask = RetrievePxTask(pxNd, taskName);

            if (pxTask == null)
            {
                return false;
            }
            if (!pxTask.get_Enabled(pxNd))
            {
                return false;
            }
            IMMPxApplicationEx5 pxApp5 = pxApp as IMMPxApplicationEx5;
            if (pxNd.Locked)
            {
                pxApp5.SetNodeLock(pxNd, false);
                unLocked = true;
            }
            retVal = pxTask.Execute(pxNd);
            if (unLocked) pxApp5.SetNodeLock(pxNd, true);
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pxNd"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static bool ExecuteTask(IMMPxNode pxNd, string taskName)
        {
            IMMPxTask pxTask = RetrievePxTask(pxNd, taskName);

            if (pxTask == null)
            {
                return false;
            }
            if (!pxTask.get_Enabled(pxNd))
            {
                return false;
            }
            return pxTask.Execute(pxNd);
        }

    }
}
