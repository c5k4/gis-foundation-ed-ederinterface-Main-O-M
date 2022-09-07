using System;
using System.Collections.Generic;
using System.Text;
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
    /// The concrete implementation of SearchItem representing an address search.
    /// </summary>
    public class AddressSearch : SearchItem
    {
        public AddressSearch()
            : base(new AddressLocateTask())
        {
            IsCustom = true;
        }

        public AddressSearch(ILocateTask task) :base(task)
        {
            IsCustom = true;
        }

        /// <summary>
        /// Gets or sets the url of the geocode service.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the fields of the geocode service.
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// Gets or sets the default portion of the address to append to
        /// // the user query
        /// </summary>
        public string AddressDefault { get; set; }

        /// <summary>
        /// Gets or sets the tool tip displayed as gray text in the query text box
        /// when the query text box is empty
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Gets or sets the address score.
        /// </summary>
        public int AddressScore { get; set; }

        /// <summary>
        /// Perform the address locate for this search item
        /// </summary>
        /// <param name="query">the query string for this search</param>
        public override void LocateAsync(string query)
        {
            query = query.Replace("\"", "");
            Results.Clear();

            LocateParameters parameter = GetParameters(query);
            ExecuteTaskAsync(parameter, Url);
        }

        private AddressLocateParameters GetParameters(string query)
        {
            var parameter = new AddressLocateParameters();
            parameter.AddressScore = AddressScore;
            parameter.MaxRecords = MaxRecords;
            parameter.Fields = Fields;
            parameter.UserQuery = query;
            parameter.SpatialReference = SpatialReference;
            parameter.AddressDefault = AddressDefault;

            return parameter;
        }
    }
}
