using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Query
#elif WPF
namespace Miner.Mobile.Client.Query
#endif
{
    /// <summary>
    /// The concrete implementation of SearchItem representing a generic search.
    /// </summary>
    public class Search : SearchItem
    {
        public Search()
            : base(new LocateTask())
        {
        }

        /// <summary>
        /// Perform the locate for this search item
        /// </summary>
        /// <param name="query">the query string for this search</param>
        public override void LocateAsync(string query)
        {
            Results.Clear();

            if ((SearchLayers == null) || (SearchLayers.Count == 0))
            {
                OnLocateComplete(new ResultEventArgs(Results));
            }
            else
            {
                if (AnnotationLayerID != -1)
                {
                    SearchLayers[0].ID = AnnotationLayerID;
                }

                foreach (SearchLayer layer in SearchLayers)
                {
                    LocateParameters parameter = new LocateParameters();
                    
                     parameter = GetParameters(layer, query);
                    ExecuteTaskAsync(parameter, layer);
                }
            }
        } 
        public string RemoveSpecialChars(string str)
            {
                // Create  a string array and add the special characters you want to remove
   
                string[] chars = new string[] { " ",",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";","_", "(", ")", ":", "|", "[", "]" }; 
                //Iterate the number of times based on the String array length.
                for(int i=0; i<chars.Length;i++)
             {
                 if(str.Contains(chars[i]))
                    {
                 str=str.Replace(chars[i],"");
                 }
            }

            return str;
            }

        private LocateParameters GetParameters(SearchLayer layer, string query)
        {
            const string quote = "\"";
          
            LocateParameters parameter = new LocateParameters();
            bool exactMatch = query.StartsWith(quote) && query.EndsWith(quote);
            if (exactMatch)
            {
                // Remove the last quote, then the first quote
                query = query.Remove(query.Length - 1).Remove(0, 1);
            }
            parameter.SearchTitle = Title;
            //INC000004150619
            if (parameter.SearchTitle == "Transformer Search" || (parameter.SearchTitle == "Primary Generation Search" && exactMatch== false))// ENOS2EDGIS
            
            {               

                    query = "%"+ query + "%";
                    parameter.ExactMatch = exactMatch;
                    parameter.SearchLayer = layer;
                    parameter.ReturnAttributes = true;
                    parameter.MaxRecords = MaxRecords;
                   // parameter.UserQuery = RemoveSpecialChars(query);- if view create then uncomment
                    parameter.UserQuery = query;
                    parameter.SpatialReference = SpatialReference;

                // parameter.SearchTitle = "Transformer";
            }
           
            else
            {
                parameter.ExactMatch = exactMatch;
                parameter.SearchLayer = layer;
                parameter.ReturnAttributes = true;
                parameter.MaxRecords = MaxRecords;
                parameter.UserQuery = query;
                parameter.SpatialReference = SpatialReference;

               // parameter.SearchTitle = Title; 
            }
            return parameter;
        }
    }
}
