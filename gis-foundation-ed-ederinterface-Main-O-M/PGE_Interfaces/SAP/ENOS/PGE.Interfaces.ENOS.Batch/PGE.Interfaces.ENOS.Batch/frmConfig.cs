using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase; 

namespace PGE.Interface.ENOS.Batch
{
    public partial class frmConfig : Form
    {
        private ENOSConfig _pConfig = null; 

        public frmConfig(ENOSConfig pConfig)
        {
            _pConfig = pConfig; 
            InitializeComponent();

            try
            {
                ENOSEncryption pEncrypt = new ENOSEncryption();
                txtEditUser.Text = _pConfig.EditUser.ToUpper();
                txtEditUserPassword.Text = pEncrypt.Decrypt(_pConfig.EditUserPassword);
                txtPostUser.Text = _pConfig.PostUser.ToUpper();
                txtPostUserPassword.Text = pEncrypt.Decrypt(_pConfig.PostUserPassword);
                txtService.Text = _pConfig.Service; 
                numMaxArchiveAge.Value = _pConfig.MaxArchiveAge;
            }
            catch
            {
                //ignore 
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                //Persist each setting to the xml file 
                ENOSEncryption pEncrypt = new ENOSEncryption(); 
                string installDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                string configFilename = installDirectory + "\\" + "App.config";
                if (!File.Exists(configFilename))
                {
                    MessageBox.Show("Error loading configuration file!");
                    return; 
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilename);
            
                //XPath query for config settings 
                string xPath = "//configuration/appSettings/add";
                XmlNodeList pNodeList = xmlDoc.SelectNodes(xPath);
                XmlNode pXMLNode = null; 

                for (int i = 0; i < pNodeList.Count; i++)
                {
                    pXMLNode = pNodeList[i];
                    if (pXMLNode != null)
                    {
                        switch (pXMLNode.Attributes[0].Value)
                        {

                            case "MAX_ARCHIVE_AGE":
                                pXMLNode.Attributes["value"].Value = numMaxArchiveAge.Value.ToString();
                                break;
                            case "EDIT_USER":
                                pXMLNode.Attributes["value"].Value = txtEditUser.Text;
                                break;
                            case "EDIT_USER_PASSWORD":
                                pXMLNode.Attributes["value"].Value = pEncrypt.Encrypt(txtEditUserPassword.Text);
                                break;
                            case "POST_USER":
                                pXMLNode.Attributes["value"].Value = txtPostUser.Text;
                                break;
                            case "POST_USER_PASSWORD":
                                pXMLNode.Attributes["value"].Value = pEncrypt.Encrypt(txtPostUserPassword.Text);
                                break;
                            case "SERVICE":
                                pXMLNode.Attributes["value"].Value = txtService.Text;
                                break;
                            default:
                                //Do nothing;
                                break;
                        }
                    }
                }

                //Save the xml document 
                xmlDoc.Save(configFilename);
                MessageBox.Show("Saved completed successfully!", "Save ENOS Configuration File");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in btnOk_Click: " + ex.Message);
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            ENOSHelper pENOSHelper = null; 
            try
            {
                pENOSHelper = new ENOSHelper(-1, _pConfig);
                ENOSEncryption pEncrypt = new ENOSEncryption();

                //Try the edit user 
                try
                {                    
                    IWorkspace pWS = pENOSHelper.GetWorkspace(
                            ENOSCommon.DEFAULT_VERSION,
                            _pConfig.ServicePrefix + txtService.Text,
                            txtEditUser.Text,
                            pEncrypt.Decrypt(txtEditUserPassword.Text));
                }
                catch
                {
                    MessageBox.Show("Edit user connection details failed", "Test Failed");
                    return;
                }

                //Try the post user 
                try
                {
                    IWorkspace pWS = pENOSHelper.GetWorkspace(
                            ENOSCommon.DEFAULT_VERSION,
                            _pConfig.ServicePrefix + txtService.Text,
                            txtPostUser.Text,
                            pEncrypt.Decrypt(txtPostUserPassword.Text));
                }
                catch
                {
                    MessageBox.Show("Post user connection details failed", "Test Failed");
                    return;
                }

                MessageBox.Show("Success!", "Test ENOS Connection Details");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in btnTest_Click: " + ex.Message);
            }
            finally
            {
                if (pENOSHelper != null) 
                    pENOSHelper.ReleaseResources(); 
            }
        }
    }
}
