using System;
using System.IO;
using System.Windows.Media.Imaging;

#if SILVERLIGHT
namespace Miner.Server.Client.Toolkit.Imaging
#elif WPF
namespace Miner.Mobile.Client.Toolkit.Imaging
#endif
{
    internal class FastPngEncoder
    {
        /// Original PngEncoder courtesy Nikola
        /// http://blogs.msdn.com/b/nikola/archive/2009/03/04/silverlight-super-fast-dymanic-image-generation-code-revisited.aspx
        /// who based off Joe Stegman.
        /// http://blogs.msdn.com/jstegman
        

        private const int Maxblock = 0xFFFF;
        private static readonly byte[] Header = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private static readonly byte[] Ihdr = { (byte)'I', (byte)'H', (byte)'D', (byte)'R' };
        private static readonly byte[] Gama = { (byte)'g', (byte)'A', (byte)'M', (byte)'A' };
        //private static readonly byte[] Srgb = { (byte)'s', (byte)'R', (byte)'G', (byte)'B' };
        private static readonly byte[] Idat = { (byte)'I', (byte)'D', (byte)'A', (byte)'T' };
        private static readonly byte[] Iend = { (byte)'I', (byte)'E', (byte)'N', (byte)'D' };

        public FastPngEncoder(int width, int height)
        {
            PrepareBuffer(width, height);
        }

        public Stream GetImageStream()
        {
            var ms = new MemoryStream();
            ms.Write(_buffer, 0, _buffer.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public void SetPixelSlow(int col, int row, byte red, byte green, byte blue, byte alpha)
        {
            int start = _rowLength * row + col * 4 + 1;
            int blockNum = start / _blockSize;
            start += ((blockNum + 1) * 5);
            start += _dataStart;

            _buffer[start] = red;
            _buffer[start + 1] = green;
            _buffer[start + 2] = blue;
            _buffer[start + 3] = alpha;
        }

        public void SetPixelAtRowStart(int col, int rowStart, byte red, byte green, byte blue, byte alpha)
        {
            int start = rowStart + (col << 2);

            _buffer[start] = red;
            _buffer[start + 1] = green;
            _buffer[start + 2] = blue;
            _buffer[start + 3] = alpha;
        }

        public int GetRowStart(int row)
        {
            int start = _rowLength * row + 1;
            int blockNum = start / _blockSize;
            start += ((blockNum + 1) * 5);
            start += _dataStart;
            return start;
        }

        byte[] _buffer;
        int _rowLength;
        int _blockSize;
        int _dataStart;

        private void PrepareBuffer(int width, int height)
        {
            uint widthLength = (uint)(width * 4) + 1;
            _rowLength = (int)widthLength;
            uint dcSize = widthLength * (uint)height;

            uint rowsPerBlock = Maxblock / widthLength;
            uint blockSize = rowsPerBlock * widthLength;
            _blockSize = (int)blockSize;
            uint blockCount;
            uint remainder = dcSize;

            if ((dcSize % blockSize) == 0)
            {
                blockCount = dcSize / blockSize;
            }
            else
            {
                blockCount = (dcSize / blockSize) + 1;
            }

            uint totalSize = 51 + (dcSize + 12 + 4 + blockCount * 5) + 12; // header, (data), end

            _buffer = new byte[totalSize];

            int currIndex = 0;

            // ********************************
            // ******* write png header ******* 
            Header.CopyTo(_buffer, currIndex);
            currIndex += Header.Length;

            // ********************************
            // ******* Write IHDR ******* 
            //  Width:              4 bytes
            //  Height:             4 bytes
            //  Bit depth:          1 byte
            //  Color type:         1 byte
            //  Compression method: 1 byte
            //  Filter method:      1 byte
            //  Interlace method:   1 byte

            byte[] size = BitConverter.GetBytes((uint)13);
            _buffer[currIndex] = size[3]; currIndex++;
            _buffer[currIndex] = size[2]; currIndex++;
            _buffer[currIndex] = size[1]; currIndex++;
            _buffer[currIndex] = size[0]; currIndex++;

            // "IHDR"
            Ihdr.CopyTo(_buffer, currIndex);
            currIndex += Ihdr.Length;

            size = BitConverter.GetBytes(width);
            _buffer[currIndex] = size[3]; currIndex++;
            _buffer[currIndex] = size[2]; currIndex++;
            _buffer[currIndex] = size[1]; currIndex++;
            _buffer[currIndex] = size[0]; currIndex++;

            size = BitConverter.GetBytes(height);
            _buffer[currIndex] = size[3]; currIndex++;
            _buffer[currIndex] = size[2]; currIndex++;
            _buffer[currIndex] = size[1]; currIndex++;
            _buffer[currIndex] = size[0]; currIndex++;

            _buffer[currIndex] = 8; currIndex++; // 8 bits
            _buffer[currIndex] = 6; currIndex++; // RGBA format
            currIndex += 3; // skip to end of IHDR

            currIndex += 4; // skip CRC, assume 0

            // ********************************
            // ******* Write gAMA chunk ******* 
            size = BitConverter.GetBytes((uint)4); // sizeof(data(gAMA))
            _buffer[currIndex] = size[3]; currIndex++;
            _buffer[currIndex] = size[2]; currIndex++;
            _buffer[currIndex] = size[1]; currIndex++;
            _buffer[currIndex] = size[0]; currIndex++;

            // "GAMA"
            Gama.CopyTo(_buffer, currIndex);
            currIndex += Gama.Length;

            // Set gamma = 1
            //size = BitConverter.GetBytes(1 * 100000);
            size = BitConverter.GetBytes(45455);
            _buffer[currIndex] = size[3]; currIndex++;
            _buffer[currIndex] = size[2]; currIndex++;
            _buffer[currIndex] = size[1]; currIndex++;
            _buffer[currIndex] = size[0]; currIndex++;

            currIndex += 4; // skip CRC, assume 0

            // ********************************
            // The sRGB chunk contains:

            // Rendering intent 1 byte 

            // The following values are defined for rendering intent:

            // 0 Perceptual for images preferring good adaptation to the output device gamut at the expense of colorimetric accuracy, such as photographs. 
            // 1 Relative colorimetric for images requiring colour appearance matching (relative to the output device white point), such as logos. 
            // 2 Saturation for images preferring preservation of saturation at the expense of hue and lightness, such as charts and graphs. 
            // 3 Absolute colorimetric for images requiring preservation of absolute colorimetry, such as previews of images destined for a different output device (proofs). 

            // ******* Write sRGB chunk ******* 
            //size = BitConverter.GetBytes((uint)4); // sizeof(data(sRGB))
            //_buffer[currIndex] = size[3]; currIndex++;
            //_buffer[currIndex] = size[2]; currIndex++;
            //_buffer[currIndex] = size[1]; currIndex++;
            //_buffer[currIndex] = size[0]; currIndex++;

            //// "sRGB"
            //Srgb.CopyTo(_buffer, currIndex);
            //currIndex += Srgb.Length;

            //// Set sRGB = 1
            //size = new byte[1];
            //_buffer[currIndex] = size[0]; currIndex++;

            //currIndex += 4; // skip CRC, assume 0

            // ***************************************
            // ******* Write IDAT (data) chunk ******* 
            size = BitConverter.GetBytes(dcSize + 2 + 4 + blockCount * 5); // image data size + 2 bytes for compression header + 4 bytes for adler checksum + blocks overhead
            _buffer[currIndex] = size[3]; currIndex++;
            _buffer[currIndex] = size[2]; currIndex++;
            _buffer[currIndex] = size[1]; currIndex++;
            _buffer[currIndex] = size[0]; currIndex++;

            // "IDAT"
            Idat.CopyTo(_buffer, currIndex);
            currIndex += Idat.Length;

            // write compression header
            _buffer[currIndex] = 0x78; currIndex++;
            _buffer[currIndex] = 0xDA; currIndex++;

            _dataStart = currIndex;

            // write image data
            //currIndex += (int)dcSize; // !!!
            for (uint blocks = 0; blocks < blockCount; blocks++)
            {
                // Write LEN
                var length = (ushort)((remainder < blockSize) ? remainder : blockSize);

                if (length == remainder)
                {
                    _buffer[currIndex] = 1;
                }
                else
                {
                    _buffer[currIndex] = 0;

                }
                currIndex++;

                size = BitConverter.GetBytes(length);
                _buffer[currIndex] = size[0]; currIndex++;
                _buffer[currIndex] = size[1]; currIndex++;

                // Write one's compliment of LEN
                size = BitConverter.GetBytes((ushort)~length);
                _buffer[currIndex] = size[0]; currIndex++;
                _buffer[currIndex] = size[1]; currIndex++;

                // Write blocks
                //for (int i = currIndex; i < currIndex + length; i++)
                //{
                //    _buffer[i] = 200;
                //}
                currIndex += length;

                // Next block
                remainder -= blockSize;
            }

            currIndex += 4; // skip adler32 checksum, assume 0
            currIndex += 4; // skip CRC, assume 0

            // ********************************
            // ******* Write IEND chunk ******* 
            currIndex += 4; // sizeof(data(IEND)) is 0

            // "IEND"
            Iend.CopyTo(_buffer, currIndex);
            currIndex += Iend.Length;
            _buffer[currIndex] = 81; currIndex++; // CRC
            _buffer[currIndex] = 189; currIndex++; // CRC
            _buffer[currIndex] = 159; currIndex++; // CRC
            _buffer[currIndex] = 125;
        }

        public static MemoryStream Encode(WriteableBitmap bmp)
        {
            // Creates PNG with CRC, slower and more memory
            //var imageData = new EditableImage(bmp.PixelWidth, bmp.PixelHeight);

            // Creates PNG with no CRC or secondary buffers, faster and less memory
            var imageData = new FastPngEncoder(bmp.PixelWidth, bmp.PixelHeight);
#if WPF
            var pixels = new int[bmp.PixelWidth * bmp.PixelHeight];
            var widthInBytes = 4 * bmp.PixelWidth;
            bmp.CopyPixels(pixels, widthInBytes, 0);
#endif
            for (int y = 0; y < bmp.PixelHeight; ++y)
            {
                for (int x = 0; x < bmp.PixelWidth; ++x)
                {
#if SILVERLIGHT
                    int pixel = bmp.Pixels[bmp.PixelWidth * y + x];
#elif WPF
                    int pixel = pixels[bmp.PixelWidth * y + x];
#endif
                    // Creates PNG with no CRC or secondary buffers, faster and less memory
                    imageData.SetPixelSlow(x, y,

                                (byte)((pixel >> 16) & 0xFF),

                                (byte)((pixel >> 8) & 0xFF),

                                (byte)(pixel & 0xFF),

                                (byte)((pixel >> 24) & 0xFF)

                                );
                }

            }

            // Creates PNG with no CRC or secondary buffers, faster and less memory
            return imageData.GetImageStream() as MemoryStream;
        }
    }

}
