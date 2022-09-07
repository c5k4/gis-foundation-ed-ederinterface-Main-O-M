using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Printing;
using System.Windows.Shapes;

namespace ArcFMSilverlight.Controls.DataGridContextMenu
{
    public class PrintRowDetails
    {
        private static readonly Size StandardLandscapePageSize = new Size(8.5, 11);
        private static readonly Size StandardLandscapePrintableAreaSize = new Size(7.5, 10);
        private const double DotsPerInch = 96;
        private const string PRINT_NAME = "WEBR GIS Attributes";
        private int pages;
        private WriteableBitmap printImage;
       
        public PrintRowDetails()
        {
            
        }

        public void PrintPages(StackPanel control)
        {
            PrintDocument pd = new PrintDocument();

            pd.BeginPrint += (s, args) =>
            {
                pages = 0;
                printImage = new WriteableBitmap(control, null);
            };

            pd.PrintPage += (s, args) =>
            {
                WriteableBitmap pictPart = printImage.Crop(0, (int)args.PrintableArea.Height * pages, (int)args.PrintableArea.Width, (int)args.PrintableArea.Height);

                Canvas cnv = new Canvas()
                {
                    Width = pictPart.PixelWidth,
                    Height = pictPart.PixelHeight,
                    Background = new ImageBrush() { ImageSource = pictPart, Stretch = Stretch.Fill }
                };

                args.PageVisual = cnv;

                pages++;
                if (control.ActualHeight > args.PrintableArea.Height * pages)
                    args.HasMorePages = true;
                else
                    args.HasMorePages = false;
            };

            pd.EndPrint += (s, args) =>
            {
                printImage = null;
            };

            pd.Print(PRINT_NAME);
        }

        //public void PrintScaled(StackPanel control)
        //{
        //    try
        //    {
        //        double scaleX = 1;
        //        double scaleY = 1;
        //        double pixelWidth = DotsPerInch * StandardLandscapePrintableAreaSize.Width;
        //        double pixelHeight = DotsPerInch * StandardLandscapePrintableAreaSize.Height;
        //        // determine scale ratio to fit on a page
        //        if (control.ActualWidth > pixelWidth)
        //            scaleX = pixelWidth / control.ActualWidth;

        //        if (control.ActualHeight > pixelHeight)
        //            scaleY = pixelHeight / control.ActualHeight;

        //        // create scale transform to use the scale above to automatically resize the
        //        // control to be printed.
        //        var transform = new ScaleTransform
        //        {
        //            CenterX = 0,
        //            CenterY = 0,
        //            ScaleX = scaleX,
        //            ScaleY = scaleY
        //        };
        //        //create bit map for control, scaled appropriately
        //        var writableBitMap = new WriteableBitmap(control, transform);
        //        // put bit map on canvas
        //        var canvas = new Canvas
        //        {
        //            Width = pixelWidth,
        //            Height = pixelHeight,
        //            Background = new ImageBrush { ImageSource = writableBitMap, Stretch = Stretch.Fill }
        //        };
        //        // create outer canvas to setup printable area margins
        //        var outerCanvas = new Canvas
        //        {
        //            Width = StandardLandscapePageSize.Width * DotsPerInch,
        //            Height = StandardLandscapePageSize.Height * DotsPerInch
        //        };
        //        outerCanvas.Children.Add(canvas);
        //        //setup margins
        //        canvas.SetValue(Canvas.LeftProperty, DotsPerInch * (StandardLandscapePageSize.Width - StandardLandscapePrintableAreaSize.Width) / 2);
        //        canvas.SetValue(Canvas.TopProperty, DotsPerInch * (StandardLandscapePageSize.Height - StandardLandscapePrintableAreaSize.Height) / 2);
        //        //fore refresh just in case
        //        canvas.InvalidateMeasure();
        //        canvas.UpdateLayout();
        //        // create printable document
        //        var printDocument = new PrintDocument();
        //        printDocument.PrintPage += (s, args) =>
        //        {
        //            args.PageVisual = outerCanvas;
        //            args.HasMorePages = false;
        //        };
        //        // launch print with the tile of Print Screen
        //        printDocument.Print(PRINT_NAME);
        //    }
        //    catch (Exception exception)
        //    {
        //        // replace with real error handling
        //        MessageBox.Show("Error occurred while printing. Error message is " + exception.Message);
        //    }
        //}
    }

    public static class WriteableBitMapExtension
    {
        private const int SizeOfArgb = 4;
        public static WriteableBitmap Crop(this WriteableBitmap bmp, int x, int y, int width, int height)
        {
            var srcWidth = bmp.PixelWidth;
            var srcHeight = bmp.PixelHeight;

            // If the rectangle is completly out of the bitmap
            if (x > srcWidth || y > srcHeight)
            {
                return new WriteableBitmap(0, 0);
            }

            // Clamp to boundaries
            if (x < 0) x = 0;
            if (x + width > srcWidth) width = srcWidth - x;
            if (y < 0) y = 0;
            if (y + height > srcHeight) height = srcHeight - y;

            // Copy the pixels line by line using fast BlockCopy
            var result = new WriteableBitmap(width, height);
            for (var line = 0; line < height; line++)
            {
                var srcOff = ((y + line) * srcWidth + x) * SizeOfArgb;
                var dstOff = line * width * SizeOfArgb;
                Buffer.BlockCopy(bmp.Pixels, srcOff, result.Pixels, dstOff, width * SizeOfArgb);
            }
            return result;
        }
    }

}
