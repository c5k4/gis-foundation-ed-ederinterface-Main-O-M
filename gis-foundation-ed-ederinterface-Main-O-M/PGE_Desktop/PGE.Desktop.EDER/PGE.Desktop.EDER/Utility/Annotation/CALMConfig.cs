using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.Desktop.EDER.Utility.Annotation
{
    /// <summary>
    /// Manages the reading, writing, and storage of CALM toolbar configuration.
    /// Users can configure XML files to be read and populated in the CALM dropdown, allowing
    /// for configurable annotation settings.
    /// </summary>
    public class CALMConfig
    {
        #region Private Members
        private DataTable _userSettings = null;
        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the file path where the configuration file should be located.
        /// Creates the directory if it doesn't exist.
        /// </summary>
        public string FilePath
        {
            get
            {
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                string directory;

                if (!Directory.Exists(sysRegistry.ConfigPath))
                {
                    directory = Path.GetDirectoryName(Assembly.GetAssembly(this.GetType()).Location);
                }
                else
                {
                    directory = sysRegistry.ConfigPath;
                }

                directory = Path.Combine(directory, "CALMConfig");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                return Path.Combine(directory, "CALMSettings.xml");
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CALMConfig()
        {
            //Initialize DataTable and its columns here.
            _userSettings = new DataTable("CALMFavorite");
            _userSettings.Columns.Add("FavoriteName", typeof(string));
            _userSettings.Columns.Add("OffsetX", typeof(double));
            _userSettings.Columns.Add("OffsetY", typeof(double));
            _userSettings.Columns.Add("Rotation", typeof(double));
            _userSettings.Columns.Add("Alignment", typeof(int));
            _userSettings.Columns.Add("DeleteJobNumber", typeof(bool));
            _userSettings.Columns.Add("InlineGroupAnno", typeof(bool));
            _userSettings.Columns.Add("FontSize", typeof (double));
            _userSettings.Columns.Add("Bold", typeof (int));

            //Check to see if settings file exists.
            if (!File.Exists(FilePath))
            {
                //Load up a sample favorite.
                Add("Example Favorite", 5, 5, 45, 1, true, true, 3, 2);

                //Create the XML file.
                StreamWriter sw = File.CreateText(FilePath);
                Serialize(sw);
                sw.Close();
            }
            else
            {
                //Read settings.
                try
                {
                    _userSettings.ReadXml(FilePath);
                }
                catch
                {
                    MessageBox.Show("Error reading the CALM Toolbar settings. Check to ensure that the values are accurate.", "CALM Toolbar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a configuration row to the in-memory configuration table. This method does not write the added record to the
        /// XML file itself.
        /// </summary>
        /// <param name="favoriteName">The user-specified name of this setting group ("favorite")</param>
        /// <param name="offsetX">The X offset of annotation when using these settings.</param>
        /// <param name="offsetY">The Y offset of annotation when using these settings.</param>
        /// <param name="rotation">The rotation, in degrees, of annotation when using these setings.</param>
        /// <param name="alignment">The alignment of annotation when using these settings (0 = Left, 1 = Center, 2 = Right)</param>
        /// <param name="deleteJobNumber">Whether or not to delete job number annotation when using these settings.</param>
        /// <param name="inlineGroupAnno">Whether or not to inline eligible grouped annotation when using these settings.</param>
        public void Add(string favoriteName, double offsetX, double offsetY, double rotation, int alignment, bool deleteJobNumber, bool inlineGroupAnno,
            double fontSize, int bold)
        {
            DataRow dr = _userSettings.NewRow();
            dr["FavoriteName"] = favoriteName;
            dr["OffsetX"] = offsetX;
            dr["OffsetY"] = offsetY;
            dr["Rotation"] = rotation;
            dr["Alignment"] = alignment;
            dr["DeleteJobNumber"] = deleteJobNumber;
            dr["InlineGroupAnno"] = inlineGroupAnno;
            dr["FontSize"] = fontSize;
            dr["Bold"] = bold;

            _userSettings.Rows.Add(dr);
        }

        /// <summary>
        /// For a given favorite name, populates the remaining parameters with the settings for that favorite, if it exists.
        /// </summary>
        /// <param name="favoriteName">The user-specified name of the setting group ("favorite") to find.</param>
        /// <param name="offsetX">Will be populated with a value determining the X offset of annotation when using these settings. Cast to double if necessary.</param>
        /// <param name="offsetY">Will be populated with a value determining the Y offset of annotation when using these settings. Cast to double if necessary.</param>
        /// <param name="rotation">Will be populated with a value determining the rotation, in degrees, of annotation when using these setings. Cast to double if necessary.</param>
        /// <param name="alignment">Will be populated with a value determining the alignment of annotation when using these settings (0 = Left, 1 = Center, 2 = Right). Cast to int if necessary.</param>
        /// <param name="deleteJobNumber">Will be populated with a value determining whether or not to delete job number annotation when using these settings. Cast to boolean if necessary.</param>
        /// <param name="inlineGroupAnno">Will be populated with a value determining whether or not to inline eligible grouped annotation when using these settings. Cast to boolean if necessary.</param>
        /// <returns>Whether or not settings were found for the given favorite name.</returns>
        public bool GetFavoriteSettings(string favoriteName, out string offsetX, out string offsetY, out string rotation, out string alignment, out string deleteJobNumber, out string inlineGroupAnno,
            out string fontSize, out string bold)
        {
            //Reset return strings regardless.
            offsetX = null;
            offsetY = null;
            rotation = null;
            alignment = null;
            deleteJobNumber = null;
            inlineGroupAnno = null;
            fontSize = null;
            bold = null;

            foreach (DataRow dr in _userSettings.Rows)
            {
                if (dr["FavoriteName"].ToString() == favoriteName)
                {
                    offsetX = dr["OffsetX"].ToString();
                    offsetY = dr["OffsetY"].ToString();
                    rotation = dr["Rotation"].ToString();
                    alignment = dr["Alignment"].ToString();
                    deleteJobNumber = dr["DeleteJobNumber"].ToString();
                    inlineGroupAnno = dr["InlineGroupAnno"].ToString();
                    fontSize = dr["FontSize"].ToString();
                    bold = dr["Bold"].ToString();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves all current favorite names that are located in the in-memory configuration table.
        /// </summary>
        /// <returns>A list containing string values of each favorite name.</returns>
        public List<string> GetFavoriteNames()
        {
            List<string> names = new List<string>();
            foreach (DataRow dr in _userSettings.Rows)
            {
                names.Add(dr["FavoriteName"].ToString());
            }

            return names;
        }

        /// <summary>
        /// Writes the current in-memory configuration table to a streamwriter (likely one corresponding to the XML file itself).
        /// </summary>
        /// <param name="sw">The streamwriter to which the serialized data will be written.</param>
        public void Serialize(StreamWriter sw)
        {
            _userSettings.WriteXml(sw);
        }

        #endregion
    }
}
