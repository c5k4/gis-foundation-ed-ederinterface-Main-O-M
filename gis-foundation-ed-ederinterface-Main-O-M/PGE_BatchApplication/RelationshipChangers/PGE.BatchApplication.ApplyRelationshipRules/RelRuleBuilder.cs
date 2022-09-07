using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Runtime.InteropServices;
using System.IO;

namespace PGE.BatchApplication.ApplyRelationshipRules
{
    public class RelRuleBuilder : IDisposable
    {
        IWorkspace _ws = null;
        IRelationshipClass _relClass = null;
        IRelationshipRule _relRule = null;
        string _relClassName = null;

        public RelRuleBuilder(string relClassName, string ConnectionFileLocation, 
            int destSubtype, int[] destCardinality, int originSubtype, int[] originCardinality)
        {
            _relClassName = relClassName;

            FileInfo fi = new FileInfo(ConnectionFileLocation);
            if (fi.Exists && fi.Extension.ToUpper() == ".SDE")
            {
                SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
                _ws = wsFactory.OpenFromFile(ConnectionFileLocation, 0);
            }
            else
            {
                AccessWorkspaceFactory accessWSFactory = new AccessWorkspaceFactoryClass();
                _ws = accessWSFactory.OpenFromFile(ConnectionFileLocation, 0);
            }

            IFeatureWorkspace featWorkspace = _ws as IFeatureWorkspace;

            try
            {
                _relClass = featWorkspace.OpenRelationshipClass(relClassName);
            }
            catch
            {
                //Table could not be found
                Console.WriteLine("\nERROR: The relationship class \"" + relClassName + "\" could not be found\n");
                Console.WriteLine("Press a key to continue.");
                Console.ReadKey(true);
            }

            if (_relClass != null)
            {
                _relRule = new RelationshipRuleClass();
                _relRule.DestinationClassID = _relClass.DestinationClass.ObjectClassID;
                _relRule.DestinationMinimumCardinality = destCardinality[0];
                _relRule.DestinationMaximumCardinality = destCardinality[1];
                _relRule.DestinationSubtypeCode = destSubtype;

                _relRule.OriginClassID = _relClass.OriginClass.ObjectClassID;
                _relRule.OriginMinimumCardinality = originCardinality[0];
                _relRule.OriginMaximumCardinality = originCardinality[1];
                _relRule.OriginSubtypeCode = originSubtype;
            }
        }

        public void Attach()
        {
            _relClass.AddRelationshipRule(_relRule);

            Console.WriteLine("Successfully attached relationship rule to " + _relClassName);
            Console.WriteLine("   Destination Subtype " + _relRule.DestinationSubtypeCode + "; Cardinality " + _relRule.DestinationMinimumCardinality + "-" + _relRule.DestinationMaximumCardinality);
            Console.WriteLine("   Origin Subtype " + _relRule.OriginSubtypeCode + "; Cardinality " + _relRule.OriginMinimumCardinality + "-" + _relRule.OriginMaximumCardinality);
        }

        public void Dispose()
        {
            Marshal.FinalReleaseComObject(_relRule);
        }
    }
}
