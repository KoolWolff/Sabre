using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace Sabre.Code.Files
{
    class SBRFile
    {
        public BinaryReader br;
        public SBRFile(string fileLocation)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
        }
        public class ArchiveFile
        {
            string FilePath;
            uint NumOfBytes;
            public DeflateStream ds;

            public ArchiveFile(BinaryReader br)
            {
                FilePath = br.ReadString();
                NumOfBytes = br.ReadUInt32();
            }
            public static byte[] DecompressFile(string fileLocation, long offset, int NumberOfBytes)
            {
                byte[] buffer = new byte[NumberOfBytes];
                DeflateStream ds = new DeflateStream(new FileStream(fileLocation, FileMode.Open), CompressionMode.Decompress);
                ds.Read(buffer, (int)offset, NumberOfBytes);
                return buffer;
            }
        }
    }
}
