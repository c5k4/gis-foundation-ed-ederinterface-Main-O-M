using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
//using PGE.ETGIS.DESKTOP.AutoupdatersAndValidationRules;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using ESRI.ArcGIS.Editor;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ArcMapCommands.DateYear
{
    public partial class DateYearUpdate : Form
    {
       // string rtextYear =string.Empty ;
       // int rnumYear = 0;
        //DateTime rDateInstalled ;
        int rtnumYear = 0;
        DateTime rtDateInstalled;
        bool dt_valuechanged = false;
        //bool _dt_valuechanged = false;
        //IEnumFeature enumFeatSelected = null;

        IApplication m_applicationCopy = null;

        public DateYearUpdate(IApplication m_application)
        {
            InitializeComponent();
             try
            {
                m_applicationCopy = m_application;
                //dateTimePicker1.Value = new DateTime(1900, 01, 01);
               // dateTimePicker1.MaxDate=DateTime.Now;
                dt_valuechanged = false;
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = " ";
                cYear.Text = string.Empty;
                
            }//dateTimePicker1.CustomFormat = " ";
            // dateTimePicker1.;
            catch (Exception ex)
            {
            }
        }

       
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
            DateTime cDate = dateTimePicker1.Value;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "MM/dd/yyyy";
            
            dt_valuechanged = true;
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void bClear_Click(object sender, EventArgs e)
        {
            dateTimePicker1.ResetText();
            dateTimePicker1.CustomFormat = " ";
            dt_valuechanged = false;

            cYear.Text = string.Empty;
            this.DialogResult = DialogResult.None;
          

        }

        private void cYear_TextChanged(object sender, EventArgs e)
        {
           
            if (System.Text.RegularExpressions.Regex.IsMatch(cYear.Text, " ^ [0-9]"))
            {
                if (cYear.Text.Length == 4)
                {
                    string textYear = cYear.Text;
                }
                else
                {
                    MessageBox.Show("Year Installed must be four digit number");
                }
            }
           

        }

       


        public void cApply_Click(object sender, EventArgs e)
        {
            string yearInstalled = cYear.Text.ToString();
            

                if (cYear.Text.ToString() == "" && dt_valuechanged == false)
                {
                    MessageBox.Show("Select Either Date Installed or Year Installed.");
                    dt_valuechanged = false;
                    this.DialogResult = DialogResult.None;
                }
                else if ((cYear.Text.ToString() != "" && dt_valuechanged == true))
                {
                    MessageBox.Show("Please choose either the Date Installed or Year Installed field. Do not populate both.");
                    dateTimePicker1.ResetText();
                    dateTimePicker1.CustomFormat = " ";
                    dt_valuechanged = false;

                    cYear.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(cYear.Text) && dt_valuechanged == true)
                    {
                        
                        rtDateInstalled = dateTimePicker1.Value;
                        rtDateInstalled = rtDateInstalled.Date;
                        updateDateYearInstalled(m_applicationCopy);
                        //this.Dispose();

                    }

                    else if (dt_valuechanged == false && cYear.Text.ToString() != "")
                    {
                        
                        if (cYear.Text.Length == 4)
                        {
                            if (Convert.ToInt64(cYear.Text.ToString()) > 1800)
                            {
                                rtnumYear = int.Parse(cYear.Text);
                                updateDateYearInstalled(m_applicationCopy);
                                //this.Dispose();
                            }
                            else
                            {
                                MessageBox.Show("Please enter valid Year Installed");
                            }
                        }
                        
                        else
                        {
                            MessageBox.Show("Year Installed must be four digit number");
                            this.DialogResult = DialogResult.None;
                        }
                    }

                }
            
           
            }
         

        public int yearInstalled
        {

            get { return rtnumYear; }

            set {rtnumYear=0;}
           
        }
        public bool Dt_valuechanged
        {
            get { return dt_valuechanged; }
           // set { _dt_valuechanged = dt_valuechanged; }
        }

      
        public DateTime dateInstalled
        {

            get { return rtDateInstalled; }
            //set { rDateInstalled = rtDateInstalled; }
        }

        private void cYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void updateDateYearInstalled(IApplication m_applicationCopy)
        {
            
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
             UID pID = new UIDClass();
            pID.Value = "esriEditor.Editor";
          //  IWorkspace loginWorkspace = GetWorkspace();
            IEditor editor = m_applicationCopy.FindExtensionByCLSID(pID) as IEditor;
            IWorkspaceEdit _wksp = editor.EditWorkspace as IWorkspaceEdit;
          //  IWorkspaceEdit _wksp = (IWorkspaceEdit)loginWorkspace;
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            try
            {// TODO: Add Command1.OnClick implementation

                IMxDocument MXDOC = m_applicationCopy.Document as IMxDocument;
                IMap pMap = MXDOC.FocusMap;
                
                IEnumFeature enumFeatSelected = pMap.FeatureSelection as IEnumFeature;
                
                    _wksp.StartEditOperation();
                    DateTime uDateInstalled = this.dateInstalled;
                    int uyearInstalled = this.yearInstalled;
                    if (uyearInstalled.Equals(0) && this.Dt_valuechanged== false)
                    {
                        //MessageBox.Show("Select Either Date Installed or Year Installed.");
                    }
                    else if (!uyearInstalled.Equals(0) && this.Dt_valuechanged == true)
                    {
                        //MessageBox.Show("Please choose either the Date Installed or Year Installed field. Do not populate both.");
                    }
                    else
                    {
                        try
                        {
                            if (this.Dt_valuechanged == false)
                            {
                               int uyearInstalled1 = this.yearInstalled;
                                AllSelectedFeatures(uDateInstalled, uyearInstalled1, this.Dt_valuechanged , m_applicationCopy);
                            }
                            else
                            {
                              int  uyearInstalled2 = uDateInstalled.Year;
                              AllSelectedFeatures(uDateInstalled, uyearInstalled2, this.Dt_valuechanged, m_applicationCopy);
                                //   string message = "update completed";
                                //   string title = "close window";
                            }

                          //  DialogResult nresult = MessageBox.Show(message, title);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            _wksp.AbortEditOperation();
                            System.Windows.Forms.Cursor.Current = Cursors.Default;
                        }
                    }
                    _wksp.StopEditOperation();

                }
                //selectedFeature();
                //result.Refresh();
                
               // _wksp.StopEditing(true);

            
            catch (Exception ex)
            {

                _wksp.AbortEditOperation();

                MessageBox.Show("Error during execution: " + ex.Message);
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
            finally
            {
                Marshal.ReleaseComObject(_wksp);
                immAutoupdater.AutoUpdaterMode = currentAUMode;
                _wksp.StopEditOperation();
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                //this.Dispose();
            }

        }
        
        public IMap map { get; set; }

        public void AllSelectedFeatures(DateTime pdate, int pyear, bool Dt_ValueChanged, IApplication m_applicationCopy)
        {
            IFeature feat;
            List<IRow> RelatedObject = new List<IRow>();
            IObjectClass objectClass = null;
            IRow pRow = null;
            IDataset pDataset = null;
            IMxDocument AMXDOC = m_applicationCopy.Document as IMxDocument;
            IMap pmap = AMXDOC.FocusMap;
            int iRelatedObjectCount = 0;
            IEnumFeature enumFeat = pmap.FeatureSelection as IEnumFeature;
            IEnumFeature enumFeatCopy = enumFeat;
            enumFeat.Reset();
            enumFeatCopy.Reset();

            int recUpdateCount = 0;
            int recRelUpdateCount = 0;

            //bool isAllSelFeat = true;

            try
            {
                bool recUpdateStatus = false;
                if (enumFeatCopy != null && enumFeatCopy.Next() != null)
                {
                    enumFeat.Reset();
                    enumFeatCopy.Reset();
                    

                    while ((feat = enumFeat.Next()) != null)
                    {
                        if (feat.FeatureType != esriFeatureType.esriFTAnnotation)
                        {
                            objectClass = feat.Class;
                            int index1 = objectClass.Fields.FindField("INSTALLJOBYEAR");
                            int index2 = objectClass.Fields.FindField("INSTALLATIONDATE");

                            if (ModelNameFacade.ContainsAllClassModelNames(objectClass, SchemaInfo.General.ClassModelNames.INSTALL_DATE))
                            {
                                try
                                {
                                    if (index1 != -1 && index2 != -1)
                                    {
                                        if (Dt_ValueChanged == true)
                                        {
                                            int fieldIndexDate = objectClass.Fields.FindField("INSTALLATIONDATE");
                                            //INSTALLJOBYEAR
                                            int fieldIndexYear = objectClass.Fields.FindField("INSTALLJOBYEAR");
                                            // DateTime cDate = DateTime.Parse(pdate);
                                            feat.set_Value(index2, pdate);
                                            feat.set_Value(index1, pyear);
                                            feat.Store();
                                            recUpdateCount++;
                                            recUpdateStatus = true;
                                        }

                                        else if (!pyear.Equals(""))
                                        {
                                            //int fieldIndexYear = objectClass.Fields.FindField("INSTALLJOBYEAR");

                                            feat.set_Value(index1, pyear);
                                            feat.Store();
                                            recUpdateCount++;
                                            recUpdateStatus = true;
                                        }
                                    }

                                    else if (index1 != -1 && index2 == -1)
                                    {
                                        //if (Dt_ValueChanged == true)
                                        //{


                                        // int fieldIndexDate = objectClass.Fields.FindField("INSTALLATIONDATE");
                                        //INSTALLJOBYEAR
                                        //int fieldIndexYear = objectClass.Fields.FindField("INSTALLJOBYEAR");
                                        // DateTime cDate = DateTime.Parse(pdate);
                                        //feat.set_Value(fieldIndexDate, pdate);
                                        feat.set_Value(index1, pyear);
                                        feat.Store();
                                        recUpdateCount++;
                                        recUpdateStatus = true;
                                        //}
                                    }

                                    else if (index1 == -1 && index2 != -1)
                                    {
                                        if (Dt_ValueChanged == true)
                                        {

                                            //int fieldIndexDate = objectClass.Fields.FindField("INSTALLATIONDATE");
                                            //INSTALLJOBYEAR
                                            //int fieldIndexYear = objectClass.Fields.FindField("INSTALLJOBYEAR");
                                            // DateTime cDate = DateTime.Parse(pdate);
                                            feat.set_Value(index2, pdate);
                                            feat.Store();
                                            recUpdateCount++;
                                            recUpdateStatus = true;
                                            // feat.set_Value(fieldIndexYear, pdate);
                                        }
                                    }

                                    RelatedObject = findMultipleRelatedFeaturewithClassName(feat, pdate, pyear);
                                    iRelatedObjectCount = RelatedObject.Count;


                                    for (int i = 0; i < iRelatedObjectCount; i++)
                                    {
                                        IObject relatedObject = RelatedObject[i] as IObject;
                                        // pRow = null;
                                        // pRow = RelatedObject[i];
                                        int rindex1 = relatedObject.Fields.FindField("INSTALLJOBYEAR");
                                        int rindex2 = relatedObject.Fields.FindField("INSTALLATIONDATE");
                                        if (rindex1 != -1 && rindex2 != -1)
                                        {
                                            if (Dt_ValueChanged == true)
                                            {
                                                //int fieldIndexDater = relatedObject.Fields.FindField("INSTALLATIONDATE");
                                                // int fieldIndexYearr = relatedObject.Fields.FindField("INSTALLJOBYEAR");
                                                relatedObject.set_Value(rindex2, pdate);
                                                relatedObject.set_Value(rindex1, pyear);

                                                relatedObject.Store();
                                                recRelUpdateCount++;
                                                recUpdateStatus = true;
                                            }

                                            else if (!pyear.Equals(""))
                                            {
                                                // int fieldIndexYearr = objectClass.Fields.FindField("INSTALLJOBYEAR");
                                                // RelatedObject[i].set_Value(fieldIndexDater, pdate);
                                                relatedObject.set_Value(rindex1, pyear);

                                                relatedObject.Store();
                                                recRelUpdateCount++;
                                                recUpdateStatus = true;
                                            }
                                        }
                                        else if (rindex1 != -1 && rindex2 == -1)
                                        {
                                            if (Dt_ValueChanged == true)
                                            {
                                                //int fieldIndexDater = RelatedObject[i].Fields.FindField("INSTALLATIONDATE");
                                                //int fieldIndexYearr = relatedObject.Fields.FindField("INSTALLJOBYEAR");
                                                // RelatedObject[i].set_Value(fieldIndexDater, pdate);
                                                relatedObject.set_Value(rindex1, pyear);

                                                relatedObject.Store();
                                                recRelUpdateCount++;
                                                recUpdateStatus = true;
                                            }

                                        }
                                        else if (rindex1 == -1 && rindex2 != -1)
                                        {
                                            if (Dt_ValueChanged == true)
                                            {
                                                // int fieldIndexDater = relatedObject.Fields.FindField("INSTALLATIONDATE");
                                                //int fieldIndexYearr = objectClass.Fields.FindField("INSTALLJOBYEAR");
                                                relatedObject.set_Value(rindex2, pdate);
                                                //relatedObject.set_Value(fieldIndexYearr, pdate);

                                                relatedObject.Store();
                                                recRelUpdateCount++;
                                                recUpdateStatus = true;
                                            }

                                        }

                                    }


                                    // feat.Store();

                                }
                                catch (Exception ex)
                                {
                                    // MessageBox.Show("Failed to Update: "+ feat.OID.ToString());
                                    //return false;
                                }
                            }

                        }

                    }
                    if (recUpdateStatus)
                    {
                        MessageBox.Show(recUpdateCount + " feature(s) and " + recRelUpdateCount + " related" + " record(s) updated successfully.");
                        
                        //dateTimePicker1.ResetText();
                        //dateTimePicker1.CustomFormat = " ";
                        ////dt_valuechanged = false;
                        //cYear.ResetText();
                        //this.Dispose();
                        this.yearInstalled = 0;
                        
                    }
                    else
                    {
                        MessageBox.Show("Records not updated successfully.");
                    }
                    
                }
                else
                {
                    MessageBox.Show("Please select layers to update.");
                    enumFeat.Reset();
                    enumFeatCopy.Reset();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception encountered while finding related features," + ex.Message.ToString() + " at " + ex.StackTrace);
                MessageBox.Show(ex.Message);
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }

            finally
            {
                Marshal.ReleaseComObject(enumFeat);
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
            // return feat;
        }


        public List<IRow> findMultipleRelatedFeaturewithClassName(IFeature pFeature, DateTime prdate, int pryear)
        {
            List<IRow> RelatedObject = new List<IRow>();
            IObjectClass objectClass = null;
            IObjectClass objectTable = null;
            IEnumRelationshipClass relClasses = null;
            IRelationshipClass relClass = null;
            ITable pTable_Rel = null;
            IDataset pDataset = null;
            string pTableName_Rel = null;
            //IField InstalledYear = ModelNameFacade.FieldFromModelName(pFeature.Class, SchemaInfo.Electric.FieldModelNames.InstallationDate);
            try
            {
                objectClass = pFeature.Class;
                relClasses = objectClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                relClasses.Reset();
                relClass = relClasses.Next();

                while (relClass != null)
                {
                    pTable_Rel = null;
                    pTableName_Rel = null;

                    if (relClass.DestinationClass is ITable)
                    {
                        objectTable = relClass.DestinationClass;
                        // pTable_Rel = relClass.DestinationClass as ITable;

                        // pDataset = (IDataset)pTable_Rel;
                        //pTableName_Rel = pDataset.Name;

                        if ((ModelNameFacade.ContainsAllClassModelNames(objectTable, SchemaInfo.General.ClassModelNames.INSTALL_DATE)) && (ModelNameFacade.ContainsAllClassModelNames(objectTable, SchemaInfo.General.ClassModelNames.REL_INST_DATE)))
                        {
                            // IRelationshipClass relClass2 = ModelNameFacade.RelationshipClassFromModelName(objectTable, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.INSTALL_DATE);
                            ISet relatedFeatures = relClass.GetObjectsRelatedToObject(pFeature);
                            relatedFeatures.Reset();
                            object pRelatedObj = relatedFeatures.Next();
                            while (pRelatedObj != null)
                            {

                                IRow pRelatedObjectRow = (IRow)pRelatedObj;

                                //int FieldIndex = pRelatedObjectRow.Fields.FindField("INSTALLATIONDATE");
                                // pRelatedObjectRow.set_Value(FieldIndex, prdate);
                                //pRelatedObjectRow.Store();
                                RelatedObject.Add(pRelatedObjectRow);


                                pRelatedObj = relatedFeatures.Next();

                            }
                        }

                    }

                    relClass = relClasses.Next();
                    //return RelatedObject;
                }


            }
            catch (Exception ex)
            {
                RelatedObject = null;
                Console.WriteLine("Unhandled exception encountered while finding related features," + ex.Message.ToString() + " at " + ex.StackTrace);
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
            return RelatedObject;
        }
        
    }
}
