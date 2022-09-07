using System.Collections.Generic;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Export
#elif WPF
namespace Miner.Mobile.Client.Export
#endif
{
    class Worksheet
    {
        public Worksheet()
        {
            Relationships = new List<RelationshipResults>();
        }
        public string Name { get; set; }
        public IResultSet ResultSet { get; set; }
        public IList<RelationshipResults> Relationships { get; set; }
    }

    class RelationshipResults
    {
        public RelationshipResults()
        {
            Results = new List<RelationshipResult>();
        }
        public string Name { get; set; }
        public IList<RelationshipResult> Results { get; set; }
    }

    class RelationshipResult
    {
        public int ObjectID { get; set; }
        public IResultSet ResultSet { get; set; }
    }


}
