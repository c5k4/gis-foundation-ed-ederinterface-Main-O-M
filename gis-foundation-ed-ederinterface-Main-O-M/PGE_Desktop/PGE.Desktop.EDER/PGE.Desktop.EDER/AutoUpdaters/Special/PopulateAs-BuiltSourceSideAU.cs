// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Editing-General - EDER Component Specification
// Shaikh Rizuan 10/01/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Collections.Generic;
using PGE.Common.Delivery.Diagnostics;


namespace PGE.Desktop.EDER.AutoUpdaters.Special
{


    /// <summary>
    ///  used populate the As-Built SourceSide field on the Voltage Regulator featureclass.
    /// </summary>
    [ComVisible(true)]
    [Guid("5E8F3005-13B4-4226-90F4-5610D2A2093B")]
    [ProgId("PGE.Desktop.EDER.PopulateAs_BuiltSourceSideAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PopulateAs_BuiltSourceSideAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

         /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 

        public PopulateAs_BuiltSourceSideAU() : base("PGE Populate As-Built SourceSide AU")
        {
        }

        #region Base Special AU Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.StoreOriginal);
                _logger.Debug("Class model name: " + SchemaInfo.Electric.ClassModelNames.StoreOriginal + "Found -" + enabled);
            }

            return enabled;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == mmAutoUpdaterMode.mmAUMFeederManager || eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

         /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            //var resolver = new ModelNameResolver();
            IFeature feat = obj as IFeature;
            List<IFeature> edgeFeature=new List<IFeature>();
            //IFeature edgeFeature = null;
            IObjectClass objClass = obj.Class;
            IObjectClass edgeClass = null;
            bool fireAU = true;
            object originalSourceFldValu = null;
            int originalSourceFldIndex = ModelNameFacade.FieldIndexFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.OriginalSourceSide);
            if (originalSourceFldIndex != -1)
            {
                // Check for event Update/Create
                if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    fireAU = false;
                    originalSourceFldValu = obj.get_Value(originalSourceFldIndex);

                    //Check for field value
                    if (originalSourceFldValu == System.DBNull.Value && string.IsNullOrEmpty(originalSourceFldValu.ToString()))
                    {
                        fireAU = true;
                    }
                }

                if (fireAU)//if true
                {

                    //Get the first edge from the upstream trace
                    edgeFeature = TraceFacade.GetFirstUpstreamEdges(feat);
                   // edgeFeature = TraceFacade.GetFirstUpstreamEdge(pFeature);

                    //If feature is not null                 
                    if(edgeFeature!=null)

                    {

                        if (edgeFeature[0] != null)
                        {
                            edgeClass = edgeFeature[0].Class;
                            IClassEx edgeClassEx = (IClassEx)edgeClass;


                            if (edgeClassEx == null || edgeClassEx.HasGlobalID == false)//if global id field not present
                            {
                                _logger.Error(edgeFeature[0].Class.AliasName + " does not have GlobalID field.");
                            }
                            //get index from field name
                            int guidIndx = edgeFeature[0].Fields.FindField(edgeClassEx.GlobalIDFieldName);
                            //get field value
                            object guidValue = edgeFeature[0].get_Value(guidIndx);
                            //set field value
                            if (guidValue != System.DBNull.Value)
                            {
                                obj.set_Value(originalSourceFldIndex, guidValue);
                            }
                            else
                            {
                                _logger.Debug(edgeClassEx.GlobalIDFieldName + " is <Null>.");
                            }
                            

                        }
             

                    }
                    else //if feature null
                    {
                         
                        _logger.Error("Edge Feature is null or empty");
                    }
                }

            }
            else
            {
                _logger.Warn("verify that Model name : "+ SchemaInfo.Electric.FieldModelNames.OriginalSourceSide+" has not assigned properly.");
            }
        }
    }
    }
        #endregion