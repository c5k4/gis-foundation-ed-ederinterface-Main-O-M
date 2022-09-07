using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Desktop;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;



namespace CreateConduitAnno
{
    // Class to pass info from form.
    public static class Form_Status
    {
        public static bool OK_Pressed = false;
        public static bool Cancel_Pressed = false;
    }

    public class CreateConduitAnnoTool
    {
        #region Local Variables
        //TextWriterTraceListener m_Logger = null;
        System.IO.StreamWriter m_Logger = null;
        IApplication m_app;
        IMxDocument m_pMxDoc;
        IEditor3 m_editor = null;
        IWorkspace m_workspace = null;
        IFeatureWorkspace m_featureworkspace = null;
        frmCreateConduitAnno m_dialog;
        #endregion

        public CreateConduitAnnoTool(frmCreateConduitAnno dialog)
        {
            m_dialog = dialog;
            m_dialog.Text = m_dialog.Text + " v0.0.0";
            m_app = (IApplication)ArcMap.ThisApplication;
            m_pMxDoc = (IMxDocument)ArcMap.Application.Document;
            //
            // TODO: Set the pathing for the logger or use the listener. See region Local Variables also.
            //m_Logger = new TextWriterTraceListener("CreateConduitAnno.log", "CreateConduitAnno");
            //m_Logger = new System.IO.StreamWriter("c:\\projects\\pge\\reports\\CreateConduitAnno.txt");
            m_Logger = new System.IO.StreamWriter("c:\\temp\\CreateConduitAnno.txt");
            // 
            m_editor = m_app.FindExtensionByName("ESRI Object Editor") as IEditor3;
            m_workspace = m_editor.EditWorkspace;
            m_featureworkspace = (IFeatureWorkspace)m_workspace;
        }

        #region Public Properties
        /// <summary>
        /// TODO: These properties need to be read from a configuration file.
        /// </summary>
        public string anno_copy_attributes
        {
            // Removed OVERRIDE (Bombed Map. I think this is a system setting of some sort.)
            get { return "FEATUREID,ZORDER,STATUS,ELEMENT,FONTNAME,FONTSIZE,BOLD,ITALIC,UNDERLINE,VERTICALALIGNMENT,HORIZONTALALIGNMENT,XOFFSET,YOFFSET,ANGLE,FONTLEADING,WORDSPACING,CHARACTERWIDTH,CHARACTERSPACING,FLIPANGLE,FEATURECONVERSIONID,CONVERSIONWORKPACKAGE"; }
        }
        private int _anno_class_id_to_copy;
        public int anno_class_id_to_copy
        {
            get {return _anno_class_id_to_copy; }
            set { _anno_class_id_to_copy = value; }
        }
        private int _anno_class_id;
        public int anno_class_id
        {
            get { return _anno_class_id; }
            set { _anno_class_id = value; }
        }
        private int _anno_symbol_id;
        public int anno_symbol_id
        {
            get { return _anno_symbol_id; }
            set { _anno_symbol_id = value; }
        }
        private string _anno_cursor_table_name;
        public string anno_cursor_table_name
        {
            get {return _anno_cursor_table_name; }
            set { _anno_cursor_table_name = value; }
        }
        private string _conductor_feature_cursor_table_name;
        public string conductor_feature_cursor_table_name
        {
            get { return _conductor_feature_cursor_table_name; }
            set { _conductor_feature_cursor_table_name = value;}
        }
        private string _anno_feature_class_alias;
        public string anno_feature_class_alias
        {
            get { return _anno_feature_class_alias; }
            set { _anno_feature_class_alias = value; }
        }
        private string _conductor_feature_class_alias;
        public string conductor_feature_class_alias
        {
            get { return _conductor_feature_class_alias; }
            set { _conductor_feature_class_alias = value; }
        }
        private string _conductor_labeltext_attrib;
        public string conductor_labeltext_attrib
        {
            get { return _conductor_labeltext_attrib; }
            set { _conductor_labeltext_attrib = value; }
        }

        #endregion

        public bool Run()
        {

            // If the anno engine being used in the map is needed, check its name.
            //IMap this_map = m_pMxDoc.Maps.get_Item(0);
            //IAnnotateMap annotateMap = this_map.AnnotationEngine;
            //MessageBox.Show( annotateMap.Name);

            WriteLogEntry("Start");

            // Set defaults.
            ClassSetPropertyValues();

            // Must be in a version and be editing. The method displays the messages to the user if not.
            if (VerifyEditVersion() == false)
            {
                CloseFiles();
                return false;
            }

            // Show the form to set the properties.
            FormLoadDefaultValues();
            m_dialog.ShowDialog();
            if (Form_Status.Cancel_Pressed == true)
            {
                WriteLogEntry("***************\r\nUser cancelled operation.\r\n***************");
                WriteLogEntry("End");
                CloseFiles();
                return false;
            }
            FormSetPropertyValues();

            // Nice to know if the classes are in the map.
            #region
            IFeatureClass fc_anno = GetFeatureClassFrLayer(anno_feature_class_alias);
            if (fc_anno == null)
            {
                MessageBox.Show("Annotation class " + anno_feature_class_alias + " not found in map. Quitting");
                WriteLogEntry("Annotation class " + anno_feature_class_alias + " not found in map. Quitting");
                CloseFiles();
                return false;
            }
            IFeatureClass fc = GetFeatureClassFrLayer(conductor_feature_class_alias);
            if (fc == null)
            {
                MessageBox.Show("Feature class " + conductor_feature_class_alias + " not found in map. Quitting");
                WriteLogEntry("Feature class " + conductor_feature_class_alias + " not found in map. Quitting");
                CloseFiles();
                return false;
            }
            #endregion

            // Create the query definition and get a cursor of the results.
            IQueryDef qd = CreateQueryDef();
            ICursor cursor;
            try
            {
                cursor = qd.Evaluate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("EXCEPTION::Run Method\n" + ex.Message + "\n" + ex.StackTrace);
                WriteLogEntry(ex.Message + "\r\n" + ex.StackTrace);
                CloseFiles();
                return false;
            }

            // Process the rows in the cursor.
            ProcessCursor(cursor, fc_anno);

            // Close up the shop.
            WriteLogEntry("End");
            CloseFiles();
            return true;
        }

        #region ArcMap processing methods

        /// <summary>
        /// This is the main method for adding annotations.
        /// </summary>
        public bool ProcessCursor(ICursor cursor, IFeatureClass fc_anno)
        {
            // Attributes to populate
            string[] arr_anno_copy_attributes = this.anno_copy_attributes.Split(',');
            // Set some key attribute indexes.
            int idx_shape = cursor.FindField(this.anno_cursor_table_name + "SHAPE");
            //int idx_textstring = cursor.FindField(this.anno_cursor_table_name + "TEXTSTRING4");
            int idx_labeltext = cursor.FindField(this.conductor_labeltext_attrib);
            //int idx_ele = cursor.FindField(this.anno_cursor_table_name + "ELEMENT");
            //
            // Status and progress bar. First, get an estimated count of annos to set the progress bar.
            IStatusBar statusBar = m_app.StatusBar;
            statusBar.set_Message(2, "Estimating annotation count...");
            long est_ct = GetAnnotationCount(fc_anno);
            IStepProgressor progBar;
            progBar = statusBar.ProgressBar;
            progBar.Position = 0;
            statusBar.ShowProgressBar("Create Conduit Annotation processing " + fc_anno.AliasName, 0, (int)est_ct, 1, true);
            //
            // Create the feature buffer
            IFeatureBuffer featureBuffer = fc_anno.CreateFeatureBuffer();
            //comReleaser.ManageLifetime(featureBuffer);
            // Create an insert cursor.
            IFeatureCursor insertCursor = fc_anno.Insert(true);
            //comReleaser.ManageLifetime(insertCursor);

            // Check what attribute names the cursor is using.
            for (int i = 0; i <= cursor.Fields.FieldCount - 1; i++)
            {
                IField fld = cursor.Fields.get_Field(i);
                WriteLogEntry(i.ToString() + "." + fld.Name);
            }
            m_Logger.Flush();
            
            // Some local vars
            string attrib_name = "";
            int bufferFieldIdx = 0;
            int rowFieldIdx = 0;
            IPolygon poly = null;
            int ct = 0;
            IRow row = cursor.NextRow();
            int new_OID = 0;
            IFeature fea = null;

            m_editor.StartOperation();
            while (row != null)
            {
                WriteLogEntry("----------------------");
                // Very basic required log entries. Comment in the ones below for more detail.
                int idx = cursor.FindField(this.anno_cursor_table_name + "OBJECTID");
                WriteLogEntry("Anno OID " + Convert.ToString(row.get_Value(idx)));
                idx = cursor.FindField(this.anno_cursor_table_name + "FEATUREID");
                WriteLogEntry("Conductor OID " + Convert.ToString(row.get_Value(idx)));
                idx = cursor.FindField(this.conductor_feature_cursor_table_name + "GLOBALID");
                WriteLogEntry("Conductor GUID " + Convert.ToString(row.get_Value(idx)));

                // Set some key attributes first. These are not copied from the original anno.
                attrib_name = "ANNOTATIONCLASSID";
                bufferFieldIdx = fc_anno.FindField(attrib_name);
                featureBuffer.set_Value(bufferFieldIdx, this.anno_class_id);
                //WriteLogEntry(Convert.ToString(attrib_name + " idx=" + bufferFieldIdx + " val=" + this.anno_class_id.ToString()));
                //
                attrib_name = "TEXTSTRING";
                bufferFieldIdx = fc_anno.FindField(attrib_name);
                featureBuffer.set_Value(bufferFieldIdx, row.get_Value(idx_labeltext));
                //WriteLogEntry(Convert.ToString(attrib_name + " idx=" + bufferFieldIdx + " cnd val=" + Convert.ToString(row.get_Value(idx_labeltext))));
                //
                attrib_name = "SYMBOLID";
                bufferFieldIdx = fc_anno.FindField(attrib_name);
                featureBuffer.set_Value(bufferFieldIdx, this.anno_symbol_id);
                //WriteLogEntry(attrib_name + " idx=" + Convert.ToString(bufferFieldIdx));
                //
                poly = (IPolygon)row.get_Value(idx_shape);
                featureBuffer.Shape = poly;

                //catch (Exception ex)
                //{
                //    MessageBox.Show("EXCEPTION::ProcessCursor Method\n" + ex.Message + "\n" + ex.StackTrace);
                //    WriteLogEntry(ex.Message + "\n" + ex.StackTrace);
                //    CloseFiles();
                //    return false;
                //}

                // The rest of the attributes are copied verbatim using the array of attribute names.
                for (int i = 0; i < arr_anno_copy_attributes.Count(); ++i)
                {
                    attrib_name = arr_anno_copy_attributes[i];
                    bufferFieldIdx = fc_anno.FindField(attrib_name);
                    rowFieldIdx = cursor.FindField(this.anno_cursor_table_name + attrib_name);
                    if (!DBNull.Value.Equals(row.get_Value(rowFieldIdx)))
                    {
                        //WriteLogEntry(Convert.ToString(attrib_name + " idx=" + bufferFieldIdx + " row val=" + Convert.ToString(row.get_Value(rowFieldIdx))));
                        featureBuffer.set_Value(bufferFieldIdx, row.get_Value(rowFieldIdx));
                    }
                    else
                    {
                        //WriteLogEntry(Convert.ToString(attrib_name + " idx=" + bufferFieldIdx + " row val= NULL"));
                    }
                }

                // Now, insert the new anno. Then retrieve it and set the annotation element text.
                new_OID = (int)insertCursor.InsertFeature(featureBuffer);
                WriteLogEntry("NEW ANNO OID " + new_OID.ToString());
                fea = fc_anno.GetFeature(new_OID);
                IAnnotationFeature afea = (IAnnotationFeature)fea;
                ITextElement textElement = (ITextElement)afea.Annotation;
                textElement.Text = Convert.ToString(row.get_Value(idx_labeltext));
                afea.Annotation = (IElement)textElement;
                (afea as IFeature).Store();
                //WriteLogEntry("STORED AS ANNOTATION FEATURE");
                //
                ct++;
                row = cursor.NextRow();
                statusBar.StepProgressBar();
                Application.DoEvents();
                statusBar.set_Message(2, ct.ToString());
                // For quicky testing...
                //if (ct == 50)
                //    break;
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(cursor);
            statusBar.HideProgressBar();

            // Now flush the insert buffer
            insertCursor.Flush();
            m_editor.StopOperation("Done.");

            System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureBuffer);
            
            MessageBox.Show(ct.ToString());
            WriteLogEntry("\r\nTotal annotations added = " + ct.ToString());
            return true;
        }

        /// <summary>
        /// This method creates the query def that joins the annotation to the conductor.
        /// </summary>
        public IQueryDef CreateQueryDef()
        {
            // Caution: The order of the .Tables matter. They somehow drive the join which drives the result set. Attributes like
            // SHAPE can be returned but it's not a legitimate com object.
            //
            WriteLogEntry("CreateQueryDef:");
            //
            // Hardcoded example.
            //IQueryDef qd = m_featureworkspace.CreateQueryDef();
            //qd.Tables = "EDGIS.PriUGConductorAnno, EDGIS.PriUGConductor";
            //qd.SubFields = "EDGIS.PriUGConductor.LABELTEXT4,EDGIS.PriUGConductorAnno.*";
            //qd.WhereClause = "(EDGIS.PriUGConductor.OBJECTID = EDGIS.PriUGConductorAnno.FEATUREID)";
            //qd.WhereClause = qd.WhereClause + " and (EDGIS.PriUGConductorAnno.TEXTSTRING Is Not Null and EDGIS.PriUGConductorAnno.SHAPE Is Not Null)";
            //qd.WhereClause = qd.WhereClause + " and (EDGIS.PriUGConductorAnno.ANNOTATIONCLASSID = 0)";
            //WriteLogEntry(qd.Tables);
            //WriteLogEntry(qd.SubFields);
            //WriteLogEntry(qd.WhereClause);
            //return qd;
            
            IQueryDef qd = m_featureworkspace.CreateQueryDef();
            string tables = anno_cursor_table_name.TrimEnd('.') + "," + conductor_feature_cursor_table_name.TrimEnd('.');
            WriteLogEntry(tables);
            string subfields = conductor_labeltext_attrib + "," + conductor_feature_cursor_table_name + "GLOBALID," + anno_cursor_table_name + "*";
            WriteLogEntry(subfields);
            string whereclause = "(" + conductor_feature_cursor_table_name + "OBJECTID = " + anno_cursor_table_name + "FEATUREID)";
            whereclause = whereclause + " and (" + anno_cursor_table_name + "TEXTSTRING Is Not Null and " + anno_cursor_table_name + "SHAPE Is Not Null)";
            whereclause = whereclause + " and (" + anno_cursor_table_name + "ANNOTATIONCLASSID = " + anno_class_id_to_copy.ToString() + ")";
            WriteLogEntry(whereclause);
            qd.Tables = tables;
            qd.SubFields = subfields;
            qd.WhereClause = whereclause;
            return qd;
        }

        #endregion

        #region ArcMap utility methods - version checking, getting feature classes, etc
        
        /// <summary>
        /// Method to get an estimated count of annotations to copy.
        /// </summary>
        public long GetAnnotationCount(IFeatureClass fc_anno)
        {
            long ret = 0;
            string where_clause = "ANNOTATIONCLASSID = " + anno_class_id_to_copy.ToString();
            IQueryFilter qf = new QueryFilter();
            qf.WhereClause = where_clause;
            WriteLogEntry("\r\nQuery for estimating = " + qf.WhereClause + "\r\n" + "Layer = " + fc_anno.AliasName);
            ret = fc_anno.FeatureCount(qf);
            WriteLogEntry("Annotation features queried = " + Convert.ToString(ret) + "\r\n");
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool VerifyEditVersion()
        {
            if (m_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show("You must be editing to use this tool.");
                return false;
            }
            else
            {
                if (m_editor.EditWorkspace.Type != esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    MessageBox.Show("Edit session must be in the SDE workspace.");
                    return false;
                }
                else
                {
                    //

                    IVersion version = (IVersion)m_workspace;
                    if (version.HasParent() != true)
                    {
                        MessageBox.Show("You must be in an SDE Version or Session to use this tool.");
                        return false;
                    }
                }
            }
            return true;
        }

        ///-------------------------------
        /// <summary>
        /// Get a feature class object from a map layer
        /// layer_name - Alias for the feature class
        /// </summary>
        /// ------------------------------
        public IFeatureClass GetFeatureClassFrLayer(string layer_name)
        {
            //
            IMap this_map = m_pMxDoc.Maps.get_Item(0);
            IFeatureClass this_FC = null;
            IFeatureLayer this_FL = null;
            try
            {
                IEnumLayer enumLayer = this_map.Layers;
                ILayer layer = enumLayer.Next();
                while (layer != null)
                {
                    if (layer is IFeatureLayer)
                    {
                        this_FL = (IFeatureLayer)layer;
                        this_FC = this_FL.FeatureClass;
                        //IDataset dataset = (IDataset)layer;
                        //MessageBox.Show(this_FL.Name + "\r\n" + dataset.Name + "\r\n" + layer.Name + "\r\n" + this_FC.AliasName);
                        if (this_FC.AliasName == layer_name)
                        {
                            this_FC = this_FL.FeatureClass;
                            return this_FC;
                        }
                    }
                    layer = enumLayer.Next();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("EXCEPTION::GetFeatureClassFrLayer\n" + ex.Message + "\n" + ex.StackTrace);
            }
            //
            return null;
        }


        #endregion

        #region Methods for interacting with the form. Also sets the class properties with default values.

        ///-------------------------------
        /// <summary>
        /// Set the class properties to display in the form.
        /// </summary>
        /// ------------------------------
        public bool FormLoadDefaultValues()
        {
            m_dialog.txtAnno_class_id_to_copy.Text = this.anno_class_id_to_copy.ToString();
            m_dialog.txtAnno_class_id.Text = this.anno_class_id.ToString();
            m_dialog.txtAnno_symbol_id.Text = this.anno_symbol_id.ToString();
            m_dialog.txtAnno_cursor_table_name.Text = this.anno_cursor_table_name.ToString();
            m_dialog.txtConductor_feature_cursor_table_name.Text = this.conductor_feature_cursor_table_name.ToString();
            m_dialog.txtAnno_feature_class_alias.Text = this.anno_feature_class_alias.ToString();
            m_dialog.txtConductor_feature_class_alias.Text = this.conductor_feature_class_alias.ToString();
            m_dialog.txtConductor_labeltext_attrib.Text=this.conductor_labeltext_attrib.ToString();
            return true;
        }
        ///-------------------------------
        /// <summary>
        /// Set the class properties to display in the form.
        /// </summary>
        /// ------------------------------
        public bool FormSetPropertyValues()
        {
            this.anno_class_id_to_copy = Convert.ToInt32(m_dialog.txtAnno_class_id_to_copy.Text);
            this.anno_class_id = Convert.ToInt32(m_dialog.txtAnno_class_id.Text);
            this.anno_symbol_id = Convert.ToInt32(m_dialog.txtAnno_symbol_id.Text);
            this.anno_cursor_table_name = m_dialog.txtAnno_cursor_table_name.Text;
            this.conductor_feature_cursor_table_name = m_dialog.txtConductor_feature_cursor_table_name.Text;
            this.anno_feature_class_alias = m_dialog.txtAnno_feature_class_alias.Text;
            this.conductor_feature_class_alias = m_dialog.txtConductor_feature_class_alias.Text;
            this.conductor_labeltext_attrib = m_dialog.txtConductor_labeltext_attrib.Text;
            return true;
        }
        ///-------------------------------
        /// <summary>
        /// Set the class properties to default values - UG Primary anno.
        /// </summary>
        /// ------------------------------
        public bool ClassSetPropertyValues()
        {
            this.anno_class_id_to_copy = 0;
            this.anno_class_id = 2;
            this.anno_symbol_id = 12;
            this.anno_cursor_table_name = "EDGIS.PriUGConductorAnno.";
            this.conductor_feature_cursor_table_name = "EDGIS.PriUGConductor.";
            this.anno_feature_class_alias = "EDGIS.PriUGConductorAnno";
            this.conductor_feature_class_alias = "Primary Underground Conductor";
            this.conductor_labeltext_attrib = "EDGIS.PriUGConductor.LABELTEXT4";
            return true;
        }
        #endregion

        #region Utility methods - log file
        ///-------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// ------------------------------
        public void WriteLogEntry(string entry)
        {
            switch (entry)
            {
                case "Start":
                    m_Logger.WriteLine("Start time " + DateTime.Now);
                    m_Logger.WriteLine("-----------------------------");
                    m_Logger.Flush();
                    break;

                case "End":
                    m_Logger.WriteLine("-----------------------------");
                    m_Logger.WriteLine("End time " + DateTime.Now);
                    m_Logger.Flush();
                    break;

                default:
                    m_Logger.WriteLine(entry);
                    m_Logger.Flush();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CloseFiles()
        {
            try
            {
                m_Logger.Flush();
                m_Logger.Close();
            }
            catch
            { }
            return true;
        }

        #endregion

    }//
}//
