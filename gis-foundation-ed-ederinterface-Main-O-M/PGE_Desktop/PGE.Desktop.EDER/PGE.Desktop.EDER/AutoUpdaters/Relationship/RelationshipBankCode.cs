using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using System.Linq;

namespace PGE.Desktop.EDER.AutoUpdaters.Relationship
{
    [ComVisible(true)]
    [Guid("c43fd49a-641a-42a9-945b-6620ecdf0ff3")]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Relationship.RelationshipBankCode")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class RelationshipBankCode : BaseRelationshipAU
    {
        /// <summary>
        /// Relationship Bankcode - private members 
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public static string[] _modelNames = { SchemaInfo.Electric.ClassModelNames.StepDown, SchemaInfo.Electric.ClassModelNames.PGETransformer, SchemaInfo.Electric.ClassModelNames.VoltageRegulator };


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipBankCode"/> class.
        /// </summary>
        public RelationshipBankCode()
            : base("PGE Relationship BankCode AU")
        {
        }

        #endregion

        /// <summary>
        /// Internal Enabled event for the given relationship.
        /// </summary>
        /// <param name="relClass">The relationship class.</param>
        /// <param name="originClass">The origin class.</param>
        /// <param name="destClass">The destination class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns>
        /// 	<c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c>
        /// </returns>
        /// <remarks>This method will be called from IMMRelationshipAUStrategyEx::get_Enabled
        /// and is wrapped within the exception handling for that method.</remarks>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            return ModelNameFacade.ContainsClassModelName(originClass, _modelNames)
                && ModelNameFacade.ContainsClassModelName(destClass, SchemaInfo.Electric.ClassModelNames.PGEUnitTable);
        }

        /// <summary>
        /// Internal Execute event for the given relationship
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="auMode">The au mode.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <remarks>This method will be called from IMMRelationshipAUStrategy::Execute
        /// and is wrapped within the exception handling for that method.</remarks>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {

            try
            {
                

                // code for Relationship  create bank code value as per sequence 
                if (relationship == null ||
                                    relationship.OriginObject == null ||
                                    relationship.DestinationObject == null ||
                                    auMode == mmAutoUpdaterMode.mmAUMNoEvents ||
                                    auMode == mmAutoUpdaterMode.mmAUMNotSet)
                {
                    return;
                }
                // get parent and child Objects from IRelationship 
                IObject prntObj = relationship.OriginObject;
                IObject childObj = relationship.DestinationObject;

                if (prntObj != null)
                {
                    // getting IRelationshipClass from Relationship
                    IRelationshipClass relationshipClass = relationship.RelationshipClass;
                    //  from IRelationClass getting  related objectset
                    ISet objSet = relationshipClass.GetObjectsRelatedToObject(prntObj);
                    objSet.Reset();
                    int bankCodeFldIndx = -1;
                    // check object set not null
                    if (objSet != null)
                    {
                        // get Bankcoe index from relationship class and Filed Model name "PGE_BANKCODE"
                        bankCodeFldIndx = ModelNameFacade.FieldIndexFromModelName(relationshipClass.DestinationClass, SchemaInfo.Electric.FieldModelNames.BankCode);

                        if (bankCodeFldIndx != -1)
                        {
                            bool isDelete = eEvent == mmEditEvent.mmEventRelationshipDeleted;
                            int relatedObjCount = 0;
                            List<IObject> objList = new List<IObject>();
                            List<IObject> errorList = new List<IObject>();
                            List<int> bankcodeValues = new List<int>();
                            do
                            {
                                IObject objRelation = objSet.Next() as IObject;
                                if (objRelation == null)
                                {
                                    break;
                                }
                                if (isDelete)
                                {
                                    if (objRelation == relationship.DestinationObject) continue;
                                }
                                relatedObjCount++;
                                object statusVal = objRelation.get_Value(bankCodeFldIndx);
                                if (statusVal != null && !(statusVal is DBNull))
                                {
                                    int bankcodevalue = -1;
                                    // checking bankcode value is integer or not 
                                    bool valid = int.TryParse(statusVal.ToString(), out bankcodevalue);
                                    if (valid)
                                    {
                                        if (!bankcodeValues.Contains(bankcodevalue))
                                        {
                                            // adding to bankcode values list
                                            bankcodeValues.Add(bankcodevalue);
                                            objList.Add(objRelation);
                                        }
                                        else
                                        {
                                            //duplicate value exists in this object - change its value later
                                            errorList.Add(objRelation);
                                        }
                                    }
                                    else
                                    {
                                        //non-numeric values
                                        errorList.Add(objRelation);
                                    }
                                }
                                else
                                {
                                    //null 
                                    errorList.Add(objRelation);
                                }
                            } while (true);

                            if (relatedObjCount > 0)
                            {
                                for (int count = 1; count <= relatedObjCount; count++)
                                {
                                    //Check if count value is available in any Object
                                    if (objList.Find(x => int.Parse(x.get_Value(bankCodeFldIndx).ToString()) == count) == null)
                                    {
                                        //Put the count in error list
                                        if (errorList.Count > 0)
                                        {
                                            errorList[0].set_Value(bankCodeFldIndx, count);
                                            try
                                            {
                                                errorList[0].Store();
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.Debug("RelationshipBankcode Error - " + ex.Message + ex.StackTrace);
                                            }
                                            errorList.RemoveAt(0);
                                        }
                                        else
                                        {
                                            if (objList.Count > 0)
                                            {
                                                objList[0].set_Value(bankCodeFldIndx, count);
                                                try
                                                {
                                                    objList[0].Store();
                                                }
                                                catch (Exception ex)
                                                {
                                                    _logger.Debug("RelationshipBankcode Error - " + ex.Message + ex.StackTrace);
                                                }
                                                objList.RemoveAt(0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        IObject obFind = objList.Find(x => int.Parse(x.get_Value(bankCodeFldIndx).ToString()) == count);
                                        if (obFind != null)
                                        {
                                            objList.Remove(obFind);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("RelationshipBankcode Error - "+ ex.Message+ex.StackTrace);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }
        }
    }
}
