using System;
using System.Json;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Stores the information about a particular field.
    /// </summary>
    public class Field
    {

        #region public/internal properties
        // Summary:
        //     Gets or sets the alias used for this field.
        public string Alias { get; set; }
        //
        // Summary:
        //     The domain limiting values to a range or a set of codes for the field.
        public Domain Domain { get; private set; }
        //
        // Summary:
        //     Specifies whether this field is read-only or editable. Feature Service layers
        //     only.
        public bool Editable { get; private set; }
        //
        // Summary:
        //     Specifies the maximum length of the field
        public int Length { get; private set; }
        //
        // Summary:
        //     The name of the field.
        public string Name { get; private set; }
        //
        // Summary:
        //     Specifies whether this field is required or can be null.
        public bool Nullable { get; private set; }
        //
        // Summary:
        //     The type of values in the field.
        public ESRI.ArcGIS.Client.Field.FieldType Type { get; private set; }

        #endregion public/internal properties

        internal static Field FromJsonObject(JsonObject json)
        {
            if (json == null) return null;
            if (json.Count == 0) return null;

            Field field = new Field { Editable = false, Nullable = true };
            if (json.ContainsKey(Constants.NameKey))
            {
                if (json[Constants.NameKey] == null)
                {
                    throw new NullReferenceException("name is null.");
                }
                field.Name = (string)json[Constants.NameKey];
            }
            else
            {
                throw new ArgumentException("field name not provided in json");

            }
            if (json.ContainsKey(Constants.AliasKey))
            {
                field.Alias = (string)json[Constants.AliasKey];
            }
            if (json.ContainsKey(Constants.LengthKey))
            {
                field.Length = (int)json[Constants.LengthKey];
            }

            if (json.ContainsKey(Constants.FieldTypeKey))
            {
                string str = (string)json["type"];
                if (string.IsNullOrEmpty(str) == false)
                {
                    try
                    {
                        if (str.StartsWith("esriFieldType"))
                        {
                            str = str.Substring("esriFieldType".Length);
                            field.Type = (ESRI.ArcGIS.Client.Field.FieldType)Enum.Parse(typeof(ESRI.ArcGIS.Client.Field.FieldType), str, true);
                        }
                        else
                        {
                            field.Type = TypeToFieldType(str);
                        }
                    }
                    catch { }
                }
            }

            if (json.ContainsKey(Constants.FieldNullableKey))
            {
                field.Nullable = (bool)json[Constants.FieldNullableKey];
            }
            if (json.ContainsKey(Constants.FieldEditableKey))
            {
                field.Editable = (bool)json[Constants.FieldEditableKey];
            }
            if (json.ContainsKey(Constants.DomainKey))
            {
                JsonObject domainAsJson = json[Constants.DomainKey] as JsonObject;
                if (domainAsJson != null)
                {
                    if (domainAsJson.ContainsKey(Constants.DomainTypeKey))
                    {
                        field.Domain = Domain.FromJsonObject(domainAsJson);
                    }
                }
            }
            return field;
        }

        private static ESRI.ArcGIS.Client.Field.FieldType TypeToFieldType(string type)
        {
            switch (type)
            {
                case "System.Byte[]":
                    return ESRI.ArcGIS.Client.Field.FieldType.Blob;

                case "System.DateTime":
                    return ESRI.ArcGIS.Client.Field.FieldType.Date;

                case "System.Double":
                    return ESRI.ArcGIS.Client.Field.FieldType.Double;

                case "System.Int32":
                    return ESRI.ArcGIS.Client.Field.FieldType.Integer;

                case "System.Single":
                    return ESRI.ArcGIS.Client.Field.FieldType.Single;

                case "System.Int16":
                    return ESRI.ArcGIS.Client.Field.FieldType.SmallInteger;

                case "System.String":
                    return ESRI.ArcGIS.Client.Field.FieldType.String;

                case "Microsoft.SqlServer.Types.SqlGeometry":
                case "Microsoft.SqlServer.Types.SqlGeography":
                    return ESRI.ArcGIS.Client.Field.FieldType.Geometry;
            }
            return ESRI.ArcGIS.Client.Field.FieldType.Unknown;
        }
    }
}
