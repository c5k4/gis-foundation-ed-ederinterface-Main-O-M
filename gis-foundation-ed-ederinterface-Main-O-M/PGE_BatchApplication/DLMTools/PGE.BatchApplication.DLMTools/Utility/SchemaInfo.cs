namespace PGE.BatchApplication.DLMTools.Utility
{
    public struct SchemaInfo
    {
        public struct General
        {
            public struct ObjectTools
            {
                public const string AlignAnnoFront = "PGE_ALIGNANNOFRONT";
                public const string AlignAnnoBack = "PGE_ALIGNANNOBACK";
            }

            public struct Fields
            {
                public const string NumberOfUnits = "NUMBEROFUNITS";
                public const string PhaseDesignation = "PHASEDESIGNATION";
                public const string RatedKVA = "RATEDKVA";
            }
        }

        public struct Substation
        {
            public struct Fields
            {
//                public const string NumberOfUnits = "NUMBEROFUNITS";
            }
        }
    }
}
