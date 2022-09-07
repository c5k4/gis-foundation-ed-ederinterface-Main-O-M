using System.Windows;
using System.IO;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Interface that a page template can implement
    /// to export its contents to a PDF file.
    /// </summary>
    public interface IPageTemplateExport
    {
        /// <summary>
        /// A method to export the UI view of the template to a PDF file.
        /// </summary>
        /// <param name="view">The contents of the page template.</param>
        /// <param name="file">The handle to a PDF file where the contents will be written.</param>
        void ExportToPdf(UIElement view, Stream file);
    }
}
