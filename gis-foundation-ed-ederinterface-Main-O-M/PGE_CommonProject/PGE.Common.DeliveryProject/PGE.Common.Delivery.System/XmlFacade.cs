using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using PGE.Common.Delivery.Diagnostics;
using System.Xml.Serialization;

namespace PGE.Common.Delivery.Systems
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlFacade
    {
        /// <summary>
        /// Validates the given XML using the XSD file specified.
        /// </summary>
        /// <param name="xmlString">The XML string to be validated</param>
        /// <param name="xsdFile">Location of the XSD file or Content of the XSD file</param>
        /// <param name="content">Boolean value indicating if the passed in xsdFile string is a location or content. Set this to true if the passed in value is the content of the XSD file.</param>
        /// <returns>Returns true if the XML string validates without error against the XSD</returns>
        public static bool ValidateXML(string xmlString, string xsdFile,bool content)
        {
            Stream strm = null;
            StreamReader schStrmReader = null;
            MemoryStream memStrm = null;
            if (content)
            {
                byte[] bytes  = Encoding.ASCII.GetBytes(xsdFile);
                memStrm = new MemoryStream(bytes);
                strm = (Stream)memStrm;
            }
            else
            {
                schStrmReader = new StreamReader(xsdFile);
                strm = schStrmReader.BaseStream; 
            }
            try
            {
                return ValidateXML(xmlString, strm);
            }
            catch (Exception GenExp)
            {
                throw new Exception(GenExp.Message, GenExp);
            }
            finally
            {
                if (memStrm != null) memStrm.Close();
                if(schStrmReader!=null) schStrmReader.Close();
            }
            //return false;
        }

        /// <summary>
        /// Validates the given XML using the XSD file specified.
        /// </summary>
        /// <param name="xmlString">The XML string to be validated</param>
        /// <param name="schemaStream">An object of type System.IO.Stream that has the XSD file</param>
        /// <returns>Returns true if the XML string validates without error against the XSD</returns>
        public static bool ValidateXML(string xmlString, Stream schemaStream)
        {
            XmlReader reader = null;
            XmlSchemaSet myschema = new XmlSchemaSet();
            ValidationEventHandler eventHandler = new ValidationEventHandler(ShowCompileErrors);
            TextReader xmlStringReader = new StringReader(xmlString);
            try
            {
                //Create the XmlParserContext.
                XmlParserContext context = new XmlParserContext(null, null, "", XmlSpace.None);

                //Implement the reader. 
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                myschema.Add("", XmlReader.Create(schemaStream));
                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.Schemas.Add(myschema);
                readerSettings.ValidationEventHandler += eventHandler;
                reader = XmlReader.Create(xmlStringReader, readerSettings);
                //Set the schema type and add the schema to the reader.
                //reader.Settings.ValidationType= ValidationType.Schema;
                reader.Settings.Schemas.Add(myschema);

                while (reader.Read())
                {
                }

                return true;
            }
            catch (XmlException XmlExp)
            {
                throw new Exception(XmlExp.Message, XmlExp);
            }
            catch (XmlSchemaException XmlSchExp)
            {
                string message = XmlSchExp.Message;
                return false;
            }
            catch (Exception GenExp)
            {
                throw new Exception(GenExp.Message, GenExp);
            }
            finally
            {
                reader.Close();
                xmlStringReader.Close();
            }
        }
        /// <summary>
        /// Event handling for XSD validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected static void ShowCompileErrors(object sender, ValidationEventArgs args)
        {
            throw new XmlSchemaException(args.Message);
        }

        /// <summary>
        /// Transforms the given XML file using the XSL file specified and writes the output to the file specified by the outputfile path.
        /// </summary>
        /// <param name="xmlFilePath">File path to the XML File</param>
        /// <param name="xslFilePath">File path to the XSL File</param>
        /// <param name="outputFilePath">File path where the output will be written</param>
        /// <param name="xsltArgsList">Any arguments that should be to the XSL during transformation. This can be null</param>
        /// <returns>Returns true if the Transformation was successful</returns>
        public static bool Transform(string xmlFilePath,string xslFilePath,string outputFilePath,XsltArgumentList xsltArgsList)
        {
            // Don't continue if we don't have a destination b/c it will error.
            if (string.IsNullOrEmpty(outputFilePath))
            {
                EventLogger.Warn("Output file path is empty"); 
                return false;
            }
            if (xsltArgsList == null) xsltArgsList = new XsltArgumentList();
            //
            // The System.Xml.XPath.XPathDocument is the fastest of Xml classes, because it's read only, 
            // and is the preferred class when the speed of XSLT transformations is the highest priority.
            //		
            if (File.Exists(xmlFilePath) && File.Exists(xslFilePath))
            {
                // Create xpath document using the xml file.
                XPathDocument xpd = new XPathDocument(xmlFilePath);

                // Initialize for XSLT Transformations.
                XslCompiledTransform xslt = new XslCompiledTransform();

                // Initialize text writer.
                XmlTextWriter xtw = null;

                try
                {
                    // Create text writer to change the xml to use the xslt formatting.
                    xtw = new XmlTextWriter(outputFilePath, Encoding.UTF8);

                    xslt.Load(xslFilePath);

                    // Transform the xml file, which will overwrite existing or create the html file.
                    xslt.Transform(xpd, xsltArgsList, xtw);

                    // If the xml succeeded then flag it.
                    return true;
                }
                catch (XsltCompileException xce)
                {
                    // XslTransform Load exception, mostlikey invalid xsl syntax.
                    EventLogger.Warn(xce.Message + "\n" + xce.StackTrace);
                    return false;
                }
                catch (Exception ex)
                {
                    EventLogger.Warn(ex.Message + "\n" + ex.StackTrace);
                    // Unknow exception, but most likely invalid html path.
                    return false;
                }
                finally
                {
                    // Close the stream.						 
                    if (xtw != null)
                        xtw.Close();
                }
            }
            else
            {
                EventLogger.Warn("Unable to find the files" + xmlFilePath + "and/or" + xslFilePath);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlToDeSerialize"></param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(XmlDocument xmlToDeSerialize)
        {
            T deserializedObj;

            MemoryStream memStream = new MemoryStream();
            xmlToDeSerialize.Save(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            XmlSerializer xmlSeri = new XmlSerializer(typeof(T));

            deserializedObj = (T)xmlSeri.Deserialize(memStream);
            return deserializedObj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static XmlDocument Serialize<T>(T objectToSerialize)
        {
            XmlDocument serializedObject = new XmlDocument();

            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(xmlTextWriter, objectToSerialize);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

            memoryStream.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memoryStream);
            string xml = sr.ReadToEnd();
            serializedObject.LoadXml(xml);
            memoryStream.Close();
            xmlTextWriter.Close();
            return serializedObject;
        }
    }
}
