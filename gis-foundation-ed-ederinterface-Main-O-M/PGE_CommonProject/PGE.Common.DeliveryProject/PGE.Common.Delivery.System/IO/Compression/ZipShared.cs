using System;
using System.IO;

namespace PGE.Common.Delivery.Systems.IO.Compression
{
    internal class ZipShared
    {
        protected internal static string StringFromBuffer(byte[] buf, int start, int maxlength)
        {
            int i;
            var c = new char[maxlength];
            for (i = 0; (i < maxlength) && (i < buf.Length) && (buf[i] != 0); i++)
            {
                c[i] = (char) buf[i]; // System.BitConverter.ToChar(buf, start+i*2);
            }
            var s = new String(c, 0, i);
            return s;
        }

        protected internal static int ReadSignature(Stream s)
        {
            int n = 0;
            var sig = new byte[4];
            n = s.Read(sig, 0, sig.Length);
            int signature = (((sig[3]*256 + sig[2])*256) + sig[1])*256 + sig[0];
            return signature;
        }

        protected internal static DateTime PackedToDateTime(Int32 packedDateTime)
        {
            var packedTime = (Int16) (packedDateTime & 0x0000ffff);
            var packedDate = (Int16) ((packedDateTime & 0xffff0000) >> 16);

            int year = 1980 + ((packedDate & 0xFE00) >> 9);
            int month = (packedDate & 0x01E0) >> 5;
            int day = packedDate & 0x001F;


            int hour = (packedTime & 0xF800) >> 11;
            int minute = (packedTime & 0x07E0) >> 5;
            int second = packedTime & 0x001F;

            DateTime d = DateTime.Now;
            try
            {
                d = new DateTime(year, month, day, hour, minute, second, 0);
            }
            catch
            {
            }

            return d;
        }


        protected internal static Int32 DateTimeToPacked(DateTime time)
        {
            var packedDate =
                (UInt16)
                ((time.Day & 0x0000001F) | ((time.Month << 5) & 0x000001E0) | (((time.Year - 1980) << 9) & 0x0000FE00));
            var packedTime =
                (UInt16)
                ((time.Second & 0x0000001F) | ((time.Minute << 5) & 0x000007E0) | ((time.Hour << 11) & 0x0000F800));
            return (Int32) (((UInt32) (packedDate << 16)) | packedTime);
        }
    }
}