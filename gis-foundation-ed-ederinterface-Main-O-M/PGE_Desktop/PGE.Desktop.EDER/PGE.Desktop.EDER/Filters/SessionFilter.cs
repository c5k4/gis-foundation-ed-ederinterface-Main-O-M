using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Interop.Process;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Process;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Systems;

namespace PGE.Desktop.EDER.Filters
{
    [ComVisible(true)]
    [Guid("79BB6D15-BD1C-4679-8091-21B31F237357")]
    [ProgId("PGE.Desktop.EDER.Filters.SessionFilter")]
    [ComponentCategory(ComCategory.MMFilter)]
    public class SessionFilter : PGE.Common.Delivery.Process.BaseClasses.BasePxFilter
    {
        public SessionFilter()
            : base("PM Search", "PGE.Desktop.EDER.Filters.SessionFilter", 4,
                "PGESessionFilter", "PGE Session Filter", "Session Manager", "MMSessionManager")
        {

        }

        public override ID8ListItem Execute()
        {
            //April 2019 release - Session Manager - provide option to filter sessions by session name
            IMMPxNodeEdit nodeEdit = new MMPxNodeListClass();
            SessionFilterForm filterForm = new SessionFilterForm(_PxApp);

            DialogResult result = filterForm.ShowDialog();

            //This is to build the parent node, doesn't matter if we have the child node, let's build the parent node first.
            //The first node has to be the custom node
            int nodeTypeID = _PxApp.Helper.GetNodeTypeID("PGESessionFilter");
            if (nodeTypeID == 0) return null;
            ((IMMPxNodeEdit2)nodeEdit).IsPxTopLevel = true;
            nodeEdit.Initialize(nodeTypeID, "PGESessionFilter", 0);
            ((IMMPxNodeEdit)nodeEdit).DisplayName = "PM Searched Sessions";
            ((IMMPxApplicationEx)_PxApp).HydrateNodeFromDB((IMMPxNode)nodeEdit);
            
            if (result == DialogResult.OK)
            {
                try
                {
                    // Assign the builder to the top level node & build.
                    //Start building the child nodes.
                    IMMDynamicList dynList = (IMMDynamicList)nodeEdit;
                    dynList.BuildObject = new FilterListBuilder(_PxApp, filterForm);
                    dynList.Build(false);
                }
                catch (Exception Ex)
                {
                }
            }

            //int nodeTypeID = _PxApp.Helper.GetNodeTypeID("PGESessionFilter");
            //if (nodeTypeID == 0) return null;
            //((IMMPxNodeEdit2)nodeEdit).IsPxTopLevel = true;
            //nodeEdit.Initialize(nodeTypeID, "PGESessionFilter", 0);
            //((IMMPxNodeEdit)nodeEdit).DisplayName = "Filtered Sessions";
            //((IMMPxApplicationEx)_PxApp).HydrateNodeFromDB((IMMPxNode)nodeEdit);
           
            return (ID8ListItem)nodeEdit;
        }

        private class FilterListBuilder : IMMListBuilder
        {
            private IMMPxApplication _PxApp;           
            string _sessionName;           
            bool _treeLoaded = false;
            
            public FilterListBuilder(IMMPxApplication pxApp, SessionFilterForm filterForm)
            {
                _PxApp = pxApp;

                //let's set the values from the form
                SetValuesFromForm(filterForm);
            }

            public void BuildList(Miner.Interop.ID8List pList)
            {
                ///If the list is already built, do not re-build the child list else will be in some infinite build tree
                ///Not sure if setting this boolean is the best solution to stop the tree from branching out when hitting 
                ///the plus sign, but this seems to do the trick for now, if there is a better solution, please apply.
                //
                if (_treeLoaded) return;

                try
                {
                    int nodeTypeID = _PxApp.Helper.GetNodeTypeID("Session");
                    if (nodeTypeID == 0) return;

                    ADODB.Connection connection = _PxApp.Connection;
                    DBFacade DBFacade = new DBFacade(connection);

                    //Let's build the where clause based on the data from the form.
                    String sql = BuildWhereClause();

                    DataTable table = DBFacade.Fill(sql, "SelectedSessions");
                    if (table!= null && table.Rows.Count != 0 )
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            //Adding the child node to the parent node, every node must be initialized and hydrated in order
                            //to be displayed in the parent node. 
                            int nodeID = TypeCastFacade.Cast(row["SESSION_ID"], 0);
                            string sessionName = row["SESSION_NAME"].ToString();

                            //The build needs to use the "Session" Node type, not the custom node defined above.
                            IMMPxNodeEdit nodeEdit = new MMPxNodeListClass();
                            //((IMMPxNodeEdit2)nodeEdit).IsPxTopLevel = false;
                            nodeEdit.Initialize(nodeTypeID, "Session", nodeID);
                            //nodeEdit.DisplayName = string.Format("{0} - {1}", nodeID, sessionName);
                            nodeEdit.DisplayName = sessionName; //string.Format("{1}", sessionName);

                            IMMPxNode node = (IMMPxNode)nodeEdit;
                            ((IMMPxApplicationEx)_PxApp).HydrateNodeFromDB(node);

                            //Add the node to the list.
                            pList.Add((ID8ListItem)node);
                            //PGE.Common.Delivery.Framework.ModelNameFacade.
                        }
                    }
                    else
                    {
                        MessageBox.Show("No result found for given search criteria");
                    }
                }
                catch (Exception ex)
                {
                }

                //Set the boolean to true once the list is built
                _treeLoaded = true;

                //Reset all the values once the list is built
                ResetValues();
            }

            public string Name
            {
                get { return "PGE List Builder"; }
            }

            public void UpdateNode(ref object vData, Miner.Interop.ID8ListItem pListItem)
            {

            }

            private void ResetValues()
            {
                _sessionName = string.Empty;
            }

            private void SetValuesFromForm(SessionFilterForm filterForm)
            {
                
                //Get session name
                if (filterForm.SessionName != null) _sessionName = filterForm.SessionName.ToUpper();
               
                //Once the value is set, let's reset the boolean back to false we we are not rebuilding the tree
                //when the plus sign on the child node is click.
                _treeLoaded = false;
            }

            private string BuildWhereClause()
            {
                ///This method is to build the criteria list, only add to the search criteria if there's 
                ///value from the original form. the query will be a join query from the mm_session and pge_session
                ///table.
                string whereClause, value;
                //string orderBy = " ORDER BY PROCESS.MM_SESSION.SESSION_ID";
                string orderBy = " ORDER BY PROCESS.MM_SESSION.SESSION_NAME ASC";
                List<string> clauses = new List<string>();
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT DISTINCT PROCESS.MM_SESSION.SESSION_ID, PROCESS.MM_SESSION.SESSION_NAME FROM ");
                sqlBuilder.Append("PROCESS.MM_SESSION, PROCESS.MM_PX_VERSIONS WHERE ");
                sqlBuilder.Append("PROCESS.MM_SESSION.SESSION_ID = PROCESS.MM_PX_VERSIONS.NODE_ID AND PROCESS.MM_PX_VERSIONS.STATUS <> 4 AND ");
     

                if (_sessionName != null)
                {
                    value = string.Format("{0} LIKE '%{1}%'", "UPPER(PROCESS.MM_SESSION.SESSION_NAME)", _sessionName);
                    if (_sessionName.Equals("_"))
                    {
                        _sessionName = "'%\\_%' ESCAPE '\\'";
                        value = string.Format("{0} LIKE {1}", "UPPER(PROCESS.MM_SESSION.SESSION_NAME)", _sessionName);
                    }
                    clauses.Add(value);
                }

                //This is to join the list into one string
                string clause = string.Join(" AND ", clauses.Select(w => w.ToString()).ToArray());

                whereClause = sqlBuilder.ToString() + clause + orderBy;

                return whereClause;
            }
        }
    }
}
