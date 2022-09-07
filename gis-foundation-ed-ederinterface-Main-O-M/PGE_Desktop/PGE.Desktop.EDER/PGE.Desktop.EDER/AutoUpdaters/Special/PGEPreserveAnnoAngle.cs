using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Desktop.EDER.ArcMapCommands;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("3e439476-983d-4616-afae-937f6806e13a")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.PGEPreserveAnnoAngle")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEPreserveAnnoAngle : BaseSpecialAU
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.SpecialAutoUpdateStrategy.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.SpecialAutoUpdateStrategy.Unregister(regKey);
        }

        #endregion

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static bool rotateAnno = false;
        private static bool askedUser = false;

        public PGEPreserveAnnoAngle()
            : base("PGE Preserve Anno Angle")
        {

        }

        #region Override Methods
        /// <summary>
        /// Enable the AU on if it is on update and it is an annotation feature class
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns>True if condition satisfied else False.</returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            if (objectClass is IFeatureClass)
            {
                IFeatureClass featClass = objectClass as IFeatureClass;

                //Enable if Feature Event type is Update and the feature class is an annotation feature class
                if (featClass.FeatureType == esriFeatureType.esriFTAnnotation && eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Execute the AU while working with ArcMap Application only.
        /// </summary>
        /// <param name="eAUMode">The AU application mode.</param>
        /// <returns>True if condition satisfied else false.</returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Execute Annotation Horizontal Alignment AU
        /// </summary>
        /// <param name="obj">The Object being Updated.</param>
        /// <param name="eAUMode">The AU Mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            mmAutoUpdaterMode currentAUMode = eAUMode;
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);

            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            try
            {
                if (PGE_RotateTool.RotateInProgress)
                {
                    if (!askedUser)
                    {
                        DialogResult result = MessageBox.Show("Rotate annotation features as well?", "Rotate Annotation", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes) { rotateAnno = true; }
                        askedUser = true;
                    }
                    if (!rotateAnno)
                    {
                        //Because these are annotation feature classes, the "Angle" field is always named that so we don't
                        //need to do anything with model names in this case.

                        int angleFieldIndex;
                        IRowChanges rowChanges = pObject as IRowChanges;

                        angleFieldIndex = pObject.Fields.FindField("Angle");
                        double origAngle; //Double.Parse(rowChanges.get_OriginalValue(angleFieldIndex).ToString());

                        if (Double.TryParse(rowChanges.get_OriginalValue(angleFieldIndex).ToString(), out origAngle))
                        {
                            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

                            pObject.Store();

                            double newAngle = Double.Parse(pObject.get_Value(angleFieldIndex).ToString());
                            double adjustmentAngle = origAngle - newAngle;

                            //Get the relationshipclass so we can get the object this is related to so we can use it as the anchor point for rotating
                            IEnumRelationshipClass relClassEnum = pObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                            relClassEnum.Reset();
                            IRelationshipClass relClass = relClassEnum.Next();
                            if (relClass != null)
                            {
                                ISet objectSet = relClass.GetObjectsRelatedToObject(pObject);
                                objectSet.Reset();
                                object obj = objectSet.Next();
                                IFeature relatedFeat = obj as IFeature;
                                if (relatedFeat.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                                {
                                    IPoint anchorPoint = relatedFeat.Shape as IPoint;
                                    ApplyAngleRotation(pObject as IFeature, adjustmentAngle, anchorPoint);
                                }
                            }

                            //Weird Work around --  had to set value and store it twice
                            pObject.set_Value(angleFieldIndex, origAngle);
                            pObject.Store();

                            pObject.set_Value(angleFieldIndex, origAngle);
                            pObject.Store();
                        }
                        else
                        {
                            _logger.Debug("Cannot revert annotation rotation for " + pObject.Class.AliasName + " [OID: " + pObject.OID + "]. It has NULL 'ANGLE' or 'SHAPE' attribute value");
                            //MessageBox.Show("Cannot revert annotation rotation for " + pObject.Class.AliasName + " [OID: " + pObject.OID + "]. It has NULL 'ANGLE' or 'SHAPE' attribute value", "Rotate Annotation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error preserving annotation angle during rotate: " + e.Message);
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }

        }
        #endregion Override Methods

        /// <summary>
        /// Applies the specified angle to the given feature using the specified anchor point
        /// </summary>
        /// <param name="feature">IFeature to apply angle rotation to</param>
        /// <param name="angle">Angle to rotate by</param>
        /// <param name="aboutPoint">IPoint to rotate about</param>
        private void ApplyAngleRotation(IFeature feature, double angle, IPoint aboutPoint)
        {
            IAnnotationFeature2 annoFeat2 = feature as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;

            IGeometry annoGeom = annoElement.Geometry;

            ITransform2D transform2D = annoGeom as ITransform2D;
            double radianAngle = (Math.PI / 180.0) * angle;
            transform2D.Rotate(aboutPoint, radianAngle);

            annoElement.Geometry = transform2D as IGeometry;

            ITextElement textElement = annoElement as ITextElement;

            annoFeat2.Annotation = textElement as IElement;
        }

        public static bool SetAnnoAngle
        {
            set
            {
                rotateAnno = value;
                askedUser = false;
            }
        }

    }
}
