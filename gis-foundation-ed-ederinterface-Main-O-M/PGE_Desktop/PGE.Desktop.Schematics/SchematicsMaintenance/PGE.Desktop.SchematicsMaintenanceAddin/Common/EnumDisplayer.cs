using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace PGE.Desktop.SchematicsMaintenance.Common
{
    /// <summary>
    /// Provides the ability to bind display enum attributed values.
    /// </summary>
    [ContentProperty("OverriddenDisplayEntries")]
    public class EnumDisplayer : IValueConverter
    {
        private IDictionary displayValues;
        private List<EnumDisplayEntry> overriddenDisplayEntries;
        private IDictionary reverseValues;
        private Type type;

        public EnumDisplayer()
        {
        }

        public EnumDisplayer(Type type)
        {
            this.Type = type;
        }

        public Type Type
        {
            get
            {
                return this.type;
            }
            set
            {
                if (!value.IsEnum)
                {
                    throw new ArgumentException(
                        "parameter is not an Enumermated type", "xValue");
                }
                this.type = value;
            }
        }

        public ReadOnlyCollection<string> DisplayNames
        {
            get
            {
                Type displayValuesType =
                    typeof(Dictionary<,>).GetGenericTypeDefinition().
                        MakeGenericType(this.type, typeof(string));
                this.displayValues =
                    (IDictionary)Activator.CreateInstance(displayValuesType);

                this.reverseValues =
                    (IDictionary)
                    Activator.CreateInstance(
                        typeof(Dictionary<,>).GetGenericTypeDefinition().
                            MakeGenericType(typeof(string), this.type));

                var fields =
                    this.type.GetFields(
                        BindingFlags.Public | BindingFlags.Static);
                foreach (var field in fields)
                {
                    DisplayStringAttribute[] a =
                        (DisplayStringAttribute[])
                        field.GetCustomAttributes(
                            typeof(DisplayStringAttribute), false);

                    string displayString = this.GetDisplayStringValue(a);
                    object enumValue = field.GetValue(null);

                    if (displayString == null)
                    {
                        displayString =
                            this.GetBackupDisplayStringValue(enumValue);
                    }
                    if (displayString != null)
                    {
                        this.displayValues.Add(enumValue, displayString);
                        this.reverseValues.Add(displayString, enumValue);
                    }
                }
                return
                    new List<string>(
                        (IEnumerable<string>)this.displayValues.Values).
                        AsReadOnly();
            }
        }

        public List<EnumDisplayEntry> OverriddenDisplayEntries
        {
            get
            {
                if (this.overriddenDisplayEntries == null)
                {
                    this.overriddenDisplayEntries =
                        new List<EnumDisplayEntry>();
                }
                return this.overriddenDisplayEntries;
            }
        }

        #region IValueConverter Members

        object IValueConverter.Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (this.displayValues == null)
            {
                ReadOnlyCollection<string> displayNames = this.DisplayNames;
            }
            return this.displayValues[value];
        }

        object IValueConverter.ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return this.reverseValues[value];
        }

        #endregion

        private string GetDisplayStringValue(DisplayStringAttribute[] a)
        {
            if (a == null || a.Length == 0)
            {
                return null;
            }
            DisplayStringAttribute dsa = a[0];
            if (!string.IsNullOrEmpty(dsa.ResourceKey))
            {
                ResourceManager rm = new ResourceManager(this.type);
                return rm.GetString(dsa.ResourceKey);
            }
            return dsa.Value;
        }

        private string GetBackupDisplayStringValue(object enumValue)
        {
            if (this.overriddenDisplayEntries != null &&
                this.overriddenDisplayEntries.Count > 0)
            {
                EnumDisplayEntry foundEntry =
                    this.overriddenDisplayEntries.Find(
                        delegate(EnumDisplayEntry entry)
                        {
                            object e = Enum.Parse(this.type, entry.EnumValue);
                            return enumValue.Equals(e);
                        });
                if (foundEntry != null)
                {
                    if (foundEntry.ExcludeFromDisplay)
                    {
                        return null;
                    }
                    return foundEntry.DisplayString;
                }
            }
            return Enum.GetName(this.type, enumValue);
        }
    }

    public class EnumDisplayEntry
    {
        public string EnumValue { get; set; }
        public string DisplayString { get; set; }
        public bool ExcludeFromDisplay { get; set; }
    }
}
