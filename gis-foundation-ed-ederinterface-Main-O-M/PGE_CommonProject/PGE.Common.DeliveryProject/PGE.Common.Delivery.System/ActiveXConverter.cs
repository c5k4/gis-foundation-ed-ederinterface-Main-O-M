using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using stdole;
using System.IO;

namespace PGE.Common.Delivery.Systems
{
    /// <summary>
    /// Converts ActiveX controls to Windows.Forms controls.
    /// </summary>
    public class ActiveXConverter : AxHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveXConverter"/> class.
        /// </summary>
        public ActiveXConverter()
            : base(null)
        {

        }

        /// <summary>
        /// Converts the <see cref="Stream"/> to a <see cref="IPictureDisp"/>.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>Returns a <see cref="IPictureDisp"/>; otherwise null.</returns>
        public static IPictureDisp ToPicture(Stream stream)
        {
            Bitmap bitmap = ToBitmap(stream);
            return ToPicture(bitmap);
        }

        /// <summary>
        /// Converts the <see cref="Image"/> to a <see cref="IPictureDisp"/>.
        /// </summary>
        /// <param name="image">The image </param>
        /// <returns>Returns a <see cref="IPictureDisp"/>; otherwise null.</returns>
        public static IPictureDisp ToPicture(Image image)
        {
            return (IPictureDisp)GetIPictureDispFromPicture(image);
        }

        /// <summary>
        /// Converts the <see cref="Stream"/> to a <see cref="IPictureDisp"/>.
        /// </summary>
        /// <param name="stream">The stream </param>
        /// <returns>Returns a <see cref="IPictureDisp"/>; otherwise null.</returns>
        public static Bitmap ToBitmap(Stream stream)
        {
            Bitmap bitmap = new Bitmap(stream);
            bitmap.MakeTransparent(bitmap.GetPixel(0, 0));
            return bitmap;
        }
    }
}
