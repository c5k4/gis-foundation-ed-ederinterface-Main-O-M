namespace PGE.BatchApplication.PageDotConfigured.ConfigValidation.Json
{
    public class MapServiceLayer
    {
        public MapServiceLayer(int id, string name, bool isRelIndic, int relatedTableId):
            this(id, name, isRelIndic)
        {
            RelatedTableId = relatedTableId;
        }
        public MapServiceLayer(int id, string name, bool isRelIndic)
        {
            Id = id;
            Name = name;
            IsLayerUrl = isRelIndic;
        }

        public MapServiceLayer(int id, string name, int classId)
        {
            Id = id;
            Name = name;
            ClassId = classId;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public int ClassId { get; set; }
        public bool IsLayerUrl { get; private set; }
        public int RelatedTableId { get; private set; }
    }
}