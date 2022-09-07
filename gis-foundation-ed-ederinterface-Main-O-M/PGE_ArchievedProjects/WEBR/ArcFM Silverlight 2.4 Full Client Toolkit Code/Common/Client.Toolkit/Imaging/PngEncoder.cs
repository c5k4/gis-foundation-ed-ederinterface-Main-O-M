using System;
using System.IO;
using System.Windows.Media.Imaging;

#if SILVERLIGHT
namespace Miner.Server.Client.Toolkit.Imaging
#elif WPF
namespace Miner.Mobile.Client.Toolkit.Imaging
#endif
{
    /// <summary>
    /// Utility class for encoding PNG image data.
    /// </summary>
    internal class PngEncoder
    {
        /// Original EditableImage and PngEncoder classes courtesy Joe Stegman.
        /// http://blogs.msdn.com/jstegman

        private const int Adler32Base = 65521;
        private const int Maxblock = 0xFFFF;
        private static readonly byte[] Header = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private static readonly byte[] Ihdr = { (byte)'I', (byte)'H', (byte)'D', (byte)'R' };
        private static readonly byte[] Gama = { (byte)'g', (byte)'A', (byte)'M', (byte)'A' };
        private static readonly byte[] Idat = { (byte)'I', (byte)'D', (byte)'A', (byte)'T' };
        private static readonly byte[] Iend = { (byte)'I', (byte)'E', (byte)'N', (byte)'D' };
        private static readonly byte[] Fourbytedata = { 0, 0, 0, 0 };
        private static readonly byte[] Argb = { 0, 0, 0, 0, 0, 0, 0, 0, 8, 6, 0, 0, 0 };


        /// <summary>
        /// Encodes the provided buffer into PNG format and returns a stream.
        /// </summary>
        /// <param name="data">The raw data to encode.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>Returns a PNG encoded stream.</returns>
        public static Stream Encode(byte[] data, int width, int height)
        {
            var ms = new MemoryStream();

            // Write PNG header
            ms.Write(Header, 0, Header.Length);

            // Write IHDR
            //  Width:              4 bytes
            //  Height:             4 bytes
            //  Bit depth:          1 byte
            //  Color type:         1 byte
            //  Compression method: 1 byte
            //  Filter method:      1 byte
            //  Interlace method:   1 byte

            byte[] size = BitConverter.GetBytes(width);
            Argb[0] = size[3]; Argb[1] = size[2]; Argb[2] = size[1]; Argb[3] = size[0];

            size = BitConverter.GetBytes(height);
            Argb[4] = size[3]; Argb[5] = size[2]; Argb[6] = size[1]; Argb[7] = size[0];

            // Write IHDR chunk
            WriteChunk(ms, Ihdr, Argb);

            // Set gamma = 1
            size = BitConverter.GetBytes(1 * 100000);
            Fourbytedata[0] = size[3]; Fourbytedata[1] = size[2]; Fourbytedata[2] = size[1]; Fourbytedata[3] = size[0];

            // Write gAMA chunk
            WriteChunk(ms, Gama, Fourbytedata);

            // Write IDAT chunk
            uint widthLength = (uint)(width * 4) + 1;
            uint dcSize = widthLength * (uint)height;

            // First part of ZLIB header is 78 1101 1010 (DA) 0000 00001 (01)
            // ZLIB info
            //
            //  CMF Byte: 78
            //  CINFO = 7 (32K window size)
            //  CM = 8 = (deflate compression)
            // FLG Byte: DA
            //  FLEVEL = 3 (bits 6 and 7 - ignored but signifies max compression)
            //  FDICT = 0 (bit 5, 0 - no preset dictionary)
            //  FCHCK = 26 (bits 0-4 - ensure CMF*256+FLG / 31 has no remainder)
            // Compressed data
            //  FLAGS: 0 or 1
            //    00000 00 (no compression) X (X=1 for last block, 0=not the last block)
            //    LEN = length in bytes (equal to ((width*4)+1)*height
            //    NLEN = one's compliment of LEN
            //    Example: 1111 1011 1111 1111 (FB), 0000 0100 0000 0000 (40)
            //    Data for each line: 0 [RGBA] [RGBA] [RGBA] ...
            //    ADLER32

            uint adler = ComputeAdler32(data);
            var comp = new MemoryStream();

            // Calculate number of 64K blocks
            uint rowsPerBlock = Maxblock / widthLength;
            uint blockSize = rowsPerBlock * widthLength;
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

            // Write headers
            comp.WriteByte(0x78);
            comp.WriteByte(0xDA);

            for (uint blocks = 0; blocks < blockCount; blocks++)
            {
                // Write LEN
                var length = (ushort)((remainder < blockSize) ? remainder : blockSize);

                if (length == remainder)
                {
                    comp.WriteByte(0x01);
                }
                else
                {
                    comp.WriteByte(0x00);
                }

                comp.Write(BitConverter.GetBytes(length), 0, 2);

                // Write one's compliment of LEN
                comp.Write(BitConverter.GetBytes((ushort)~length), 0, 2);

                // Write blocks
                comp.Write(data, (int)(blocks * blockSize), length);

                // Next block
                remainder -= blockSize;
            }

            WriteReversedBuffer(comp, BitConverter.GetBytes(adler));
            comp.Seek(0, SeekOrigin.Begin);

            var dat = new byte[comp.Length];
            comp.Read(dat, 0, (int)comp.Length);

            WriteChunk(ms, Idat, dat);

            // Write IEND chunk
            WriteChunk(ms, Iend, new byte[0]);

            // Reset stream
            ms.Seek(0, SeekOrigin.Begin);

            return ms;

            // See http://www.libpng.org/pub/png//spec/1.2/PNG-Chunks.html
            // See http://www.libpng.org/pub/png/book/chapter08.html#png.ch08.div.4
            // See http://www.gzip.org/zlib/rfc-zlib.html (ZLIB format)
            // See ftp://ftp.uu.net/pub/archiving/zip/doc/rfc1951.txt (ZLIB compression format)
        }

        public static Stream Encode(WriteableBitmap wb)
        {
            int w = wb.PixelWidth;
            int h = wb.PixelHeight;
#if WPF
            var pixels = new int[w * h];
            var widthInBytes = 4 * w;
            wb.CopyPixels(pixels, widthInBytes, 0);
#endif

            int rowLength = w * 4 + 1;
            var buffer = new byte[rowLength * h];
            for (int y = 0; y < h; y++)
            {
                buffer[y * rowLength] = 0; // filter byte

                for (int x = 0; x < w; x++)
                {
                    int adr = y * rowLength + x * 4 + 3;
#if SILVERLIGHT
                    int pixel = wb.Pixels[x + y * w];
#elif WPF
                    int pixel = pixels[x + y * w];
#endif

                    buffer[adr--] = (byte)(pixel & 0xff); pixel >>= 8;
                    buffer[adr--] = (byte)(pixel & 0xff); pixel >>= 8;
                    buffer[adr--] = (byte)(pixel & 0xff); pixel >>= 8;
                    buffer[adr] = (byte)(pixel & 0xff);
                }
            }

            return Encode(buffer, w, h);
        } 

        private static void WriteReversedBuffer(Stream stream, byte[] data)
        {
            int size = data.Length;
            var reorder = new byte[size];

            for (int idx = 0; idx < size; idx++)
            {
                reorder[idx] = data[size - idx - 1];
            }
            stream.Write(reorder, 0, size);
        }

        private static void WriteChunk(Stream stream, byte[] type, byte[] data)
        {
            int idx;
            int size = type.Length;
            var buffer = new byte[type.Length + data.Length];

            // Initialize buffer
            for (idx = 0; idx < type.Length; idx++)
            {
                buffer[idx] = type[idx];
            }

            for (idx = 0; idx < data.Length; idx++)
            {
                buffer[idx + size] = data[idx];
            }

            // Write length
            WriteReversedBuffer(stream, BitConverter.GetBytes(data.Length));

            // Write type and data
            stream.Write(buffer, 0, buffer.Length);   // Should always be 4 bytes

            // Compute and write the CRC
            WriteReversedBuffer(stream, BitConverter.GetBytes(GetCRC(buffer)));
        }

        private static readonly uint[] CrcTable = new uint[256];
        private static bool _crcTableComputed;

        private static void MakeCRCTable()
        {
            for (int n = 0; n < 256; n++)
            {
                var c = (uint)n;
                for (int k = 0; k < 8; k++)
                {
                    if ((c & (0x00000001)) > 0)
                        c = 0xEDB88320 ^ (c >> 1);
                    else
                        c = c >> 1;
                }
                CrcTable[n] = c;
            }

            _crcTableComputed = true;
        }

        private static uint UpdateCRC(uint crc, byte[] buf, int len)
        {
            uint c = crc;

            if (!_crcTableComputed)
            {
                MakeCRCTable();
            }

            for (int n = 0; n < len; n++)
            {
                c = CrcTable[(c ^ buf[n]) & 0xFF] ^ (c >> 8);
            }

            return c;
        }

        /* Return the CRC of the bytes buf[0..len-1]. */
        private static uint GetCRC(byte[] buf)
        {
            return UpdateCRC(0xFFFFFFFF, buf, buf.Length) ^ 0xFFFFFFFF;
        }

        private static uint ComputeAdler32(byte[] buf)
        {
            uint s1 = 1;
            uint s2 = 0;
            int length = buf.Length;

            for (int idx = 0; idx < length; idx++)
            {
                s1 = (s1 + buf[idx]) % Adler32Base;
                s2 = (s2 + s1) % Adler32Base;
            }

            return (s2 << 16) + s1;
        }
    }
}
