using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace Sabre
{
    class WADFile
    {
        public BinaryReader br;
        public Header header;
        public List<Entry> Entries = new List<Entry>();
        public WADFile(string fileLocation)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            header = new Header(br);
            for(int i = 0; i < header.FileCount; i++)
            {
                Entries.Add(new Entry(br));
            }
            foreach(Entry e in Entries)
            {
                if(e.TypeOfCompression == CompressionType.Compressed)
                {
                    br.BaseStream.Seek(e.FileDataOffset, SeekOrigin.Begin);
                    e.Data = br.ReadBytes((int)e.CompressedSize);
                }
                else if(e.TypeOfCompression == CompressionType.String)
                {
                    br.BaseStream.Seek(e.FileDataOffset, SeekOrigin.Begin);
                    e.Name = Encoding.ASCII.GetString(br.ReadBytes((int)br.ReadUInt32()));
                }
            }
        }
        public class Header
        {
            public string Magic;
            public byte Major;
            public byte Minor;
            public byte ECDSALength;
            public byte[] ECDSA;
            public byte[] ZeroPadding;
            public UInt64 Checksum;
            public UInt16 TOCStartOffset;
            public UInt16 TOCFileEntrySize;
            public UInt32 FileCount;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(2));
                Major = br.ReadByte();
                Minor = br.ReadByte();
                ECDSALength = br.ReadByte();
                ECDSA = br.ReadBytes(80);
                ZeroPadding = br.ReadBytes(3);
                Checksum = br.ReadUInt64();
                TOCStartOffset = br.ReadUInt16();
                TOCFileEntrySize = br.ReadUInt16();
                FileCount = br.ReadUInt32();
            }
        }
        public class Entry
        {
            public string FileSize;
            public string Name;
            public byte[] Data;
            public UInt64 XXHashFilePath;
            public UInt32 FileDataOffset;
            public UInt32 CompressedSize;
            public UInt32 UncompressedSize;
            public CompressionType TypeOfCompression;
            public UInt64 SHA256;
            public Entry(BinaryReader br)
            {
                XXHashFilePath = br.ReadUInt64();
                FileDataOffset = br.ReadUInt32();
                CompressedSize = br.ReadUInt32();
                UncompressedSize = br.ReadUInt32();
                TypeOfCompression = (CompressionType)br.ReadUInt32();
                SHA256 = br.ReadUInt64();
                FileSize = Functions.SizeSuffix(UncompressedSize);
            }
        }
        public enum CompressionType : UInt32
        {
            Uncompressed = 0,
            Compressed = 1,
            String = 2
        }
        static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}
