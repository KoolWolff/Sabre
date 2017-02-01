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
        public string ChewyIsLove;
        public List<ArchiveFile> Files = new List<ArchiveFile>();
        public Header h;
        public Metadata m;
        public BinaryReader br;
        public SBRFile(string fileLocation)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));

            h = new Header(br);
            m = new Metadata(br);
            for(int i = 0; i < h.FileCount; i++)
            {
                Files.Add(new ArchiveFile(br));
            }
            ChewyIsLove = Encoding.ASCII.GetString(br.ReadBytes((int)br.BaseStream.Length - (int)br.BaseStream.Position));
        }
        public class Header
        {
            public string Magic;
            public byte Major;
            public byte Minor;
            public string OINK;
            public UInt32 FileCount;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(2));
                Major = br.ReadByte();
                Minor = br.ReadByte();
                OINK = Encoding.ASCII.GetString(br.ReadBytes(4));
                FileCount = br.ReadUInt32();
            }
        }
        public class Metadata
        {
            public SkinType PrimaryType;
            public string SecondaryType;
            public string Name, Author, Version;
            public Metadata(BinaryReader br)
            {
                PrimaryType = (SkinType)br.ReadByte();
                SecondaryType = Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16()));
                Name = Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16()));
                Author = Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16()));
                Version = Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16()));
            }
        }
        public class ArchiveFile
        {
            public string Name;
            public UInt64 DataOffset, DataSize;
            public byte[] Data;
            public ArchiveFile(BinaryReader br)
            {
                Name = Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16()));
                DataOffset = br.ReadUInt64();
                DataSize = br.ReadUInt64();
                br.BaseStream.Seek((long)DataOffset, SeekOrigin.Begin);
                Data = br.ReadBytes((int)DataSize);
                Data = Functions.DecompressGZip(Data);
            }
        }
        public enum SkinType : byte
        {
            Champion,
            Map,
            HUD,
            Other
        }
    }
}
