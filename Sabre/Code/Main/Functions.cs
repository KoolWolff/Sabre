using System;
using System.IO;
using System.Windows.Controls;
using zlib;

namespace Sabre
{
    class Functions
    {
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        public static byte[] DecompressZlib(byte[] inData)
        {
            byte[] outData;
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyZlibStream(inMemoryStream, outZStream);
                outZStream.finish();
                outData = outMemoryStream.ToArray();
            }
            return outData;
        }
        public static void CopyZlibStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }
        public static void SwitchGrids(Grid hider, Grid shower)
        {
            hider.Visibility = System.Windows.Visibility.Hidden;
            shower.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
