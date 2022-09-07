using Miner.Interop;

namespace PGE.BatchApplication.ArcFmWebConfigLimited.Core.Enumerator
{
    public struct ObjectFrame
    {
        public bool DuplicateSubtype;
        public ID8ListItem Field;
        public ID8ListItem FieldSetting;
        public PropertyEnumerator.DatasetObject OClass;
        public IMMSubtype Subtype;
    }
}