extern alias silverPDFdll;

using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

using silverPDFdll.PdfSharp.Drawing;
using silverPDFdll.PdfSharp.Pdf;

#if SILVERLIGHT
using Miner.Server.Client.Toolkit.Imaging;
#elif WPF
using System.Windows.Media;
using Miner.Mobile.Client.Toolkit.Imaging;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Toolkit
#elif WPF
namespace Miner.Mobile.Client.Toolkit
#endif
{
    /// <summary>
    /// A utility class to create PDF files.
    /// </summary>
    public static class PdfExporter
    {
        #region public methods

        /// <summary>
        /// Exports the UIElement to the given stream in PDF format.
        /// </summary>
        /// <param name="element">The UIElement to export.</param>
        /// <param name="file">The file stream to export to.</param>
        public static void ExportToStream(UIElement element, Stream file)
        {
            if (element == null) return;
            if (file == null) return;

            // Save the document...
#if SILVERLIGHT
            var wb = new WriteableBitmap(element, null);

            using (Stream imgStream = BuildStream(wb))
            {
                // Create a new PDF document
                var document = new PdfDocument();

                //Create an image
                XImage img = XImage.FromStream(imgStream);
#elif WPF
            var bitmap = new RenderTargetBitmap((int)element.RenderSize.Width, (int)element.RenderSize.Width, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(element);

            // Create a new PDF document
            var document = new PdfDocument();

            //Create an image
            XImage img = XImage.FromBitmapSource(bitmap);
#endif
                // Create an empty page
                PdfPage page = document.AddPage();

                page.Width = img.PointWidth;
                page.Height = img.PointHeight;

                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                gfx.SmoothingMode = XSmoothingMode.AntiAlias;

                gfx.DrawImage(img, 0, 0, page.Width, page.Height);

                gfx.Save();
                page.Close();
                document.Close();
                document.Save(file);
#if SILVERLIGHT
            }
#endif
        }

        #endregion public methods

        #region Private Methods

        private static MemoryStream BuildStream(WriteableBitmap bitmap)
        {
            //Encode the Image as a PNG
            MemoryStream stream  = FastPngEncoder.Encode(bitmap);

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        #endregion private methods
    }
}
