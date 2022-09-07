using System;
using System.Collections.Generic;
using Miner.Interop;

namespace PGE.BatchApplication.ArcFmWebConfigLimited.Core
{
    internal class Constants
    {
        public const char Delimiter = ',';

        public const string NoneText = "<None>";
        public const string TrueText = "TRUE";
        public const string FalseText = "FALSE";

        public const string RootDataset = "Root";

        public struct Property
        {
            public Property(int idx, string d)
                : this()
            {
                Index = idx;
                Desc = d;
            }

            public int Index { get; private set; }
            public string Desc { get; private set; }
        }

        internal class SimpleSetting
        {
            //The Headers enum along with the simpleSettings Dictionary directly below make up the indices of the array
            //in which each field's properties are stored. The first three are negative because they are part of the ID,
            //not the values array
            public enum Headers
            {
                ObjectClassRo = -3,
                SubtypeRo = -2,
                FieldRo = -1,
                DatasetRo = 0,
                SubtypeCodeRo = 1,
                FieldAliasRo = 2,
                Index = 3,
                Visible=0,
                DefaultValueRo=8
            }

            public const int NumColsToConcat = 3;

            public static readonly Dictionary<mmFieldSettingType, Property> Settings
                = new Dictionary<mmFieldSettingType, Property>
                {
                    {mmFieldSettingType.mmFSVisible, new Property(4, mmFieldSettingType.mmFSVisible.ToString())},
                     {mmFieldSettingType.mmFSEditable , new Property(5, mmFieldSettingType.mmFSEditable.ToString())},
                    {mmFieldSettingType.mmFSAllowNulls, new Property(6, mmFieldSettingType.mmFSAllowNulls.ToString())},
                     {mmFieldSettingType.mmFSAllowMassAttributeUpdate, new Property(7, mmFieldSettingType.mmFSAllowMassAttributeUpdate.ToString())},
                };

            public static readonly int NumProperties = Enum.GetNames(typeof (Headers)).Length + Settings.Count -
                                                       NumColsToConcat;
        }
    }
}