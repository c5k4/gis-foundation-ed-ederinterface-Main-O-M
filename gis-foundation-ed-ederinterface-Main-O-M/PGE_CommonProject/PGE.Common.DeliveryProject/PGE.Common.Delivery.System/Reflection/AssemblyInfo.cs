using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PGE.Common.Delivery.Systems.Reflection
{
    /// <summary>
    /// Provides application information from assembly attributes.
    /// </summary>
    public class AssemblyInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfo"/> class.
        /// </summary>
        public AssemblyInfo(Type type)
            : this(type.Assembly)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfo"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AssemblyInfo(Assembly assembly)
        {
            this.Assembly = assembly;
            this.ResourceNames = new List<string>(assembly.GetManifestResourceNames());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the copyright notice as defined by the AssemblyCopyright
        /// attribute in the AssemblyInfo.cs file of the assembly.
        /// </summary>
        public string Copyright
        {
            get
            {
                AssemblyCopyrightAttribute[] appCopyrights = (AssemblyCopyrightAttribute[])this.Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return (appCopyrights != null && appCopyrights.Length > 0) ? appCopyrights[0].Copyright : String.Empty;
            }
        }

        /// <summary>
        /// Gets the assembly directory.
        /// </summary>
        /// <value>The assembly directory.</value>
        public string Directory
        {
            get
            {
                string codeBase = this.Assembly.CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
                return path;
            }
        }

        /// <summary>
        /// Returns a description of this product as defined by
        /// the AssemblyDescription attribute in the
        /// AssemblyInfo.cs file of the assembly.
        /// </summary>
        public string ProductDescription
        {
            get
            {
                AssemblyDescriptionAttribute[] appDescs = (AssemblyDescriptionAttribute[])this.Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return (appDescs != null && appDescs.Length > 0) ? appDescs[0].Description : String.Empty;
            }
        }

        /// <summary>
        /// Returns the name of this product as defined by 
        /// the AssemblyProduct attribute in the
        /// AssemblyInfo.cs file of the assembly.
        /// </summary>
        public string ProductName
        {
            get
            {
                AssemblyProductAttribute[] appProducts = (AssemblyProductAttribute[])this.Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return (appProducts != null && appProducts.Length > 0) ? appProducts[0].Product : String.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the resource names.
        /// </summary>
        /// <value>The resource names.</value>
        public List<string> ResourceNames
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public Version Version
        {
            get
            {
                Version version = this.Assembly.GetName().Version;
                return version;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the assembly.
        /// </summary>
        /// <value>The assembly.</value>
        protected Assembly Assembly
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the resource associated with the specified name.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The stream containing the resource.</returns>
        public Stream LoadResource(string resourceName)
        {
            if (this.ResourceNames.Contains(resourceName))
            {
                Stream stream = this.Assembly.GetManifestResourceStream(resourceName);
                return stream;
            }

            return null;
        }

        /// <summary>
        /// Saves the resource to the specified file.
        /// Should never be used as changing the resource file content changes the signature and would cause lot of confusion.
        /// </summary>
        /// <param name="resouceName">Name of the resouce.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns><c>true</c> if the resource was saved; otherwise <c>false</c>.</returns>
        internal bool SaveResource(string resouceName, string fileName)
        {
            Stream stream = this.LoadResource(resouceName);
            if (stream == null) return false;

            // Create a FileStream object to write a stream to a file
            using (FileStream fs = File.Create(fileName, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)buffer.Length);

                // Use FileStream object to write to the specified file
                fs.Write(buffer, 0, buffer.Length);
            }

            return true;
        }

        #endregion
    }
}
