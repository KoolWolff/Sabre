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
            for(int i = 0; i < h.FileCount; i++)
            {
                Files.Add(new ArchiveFile(br.ReadUInt32()));
            }
            foreach(ArchiveFile f in Files)
            {
                f.NameLength = br.ReadUInt16();
                f.Name = Encoding.ASCII.GetString(br.ReadBytes(f.NameLength));
                f.DataOffset = br.ReadUInt64();
                f.DataSize = br.ReadUInt64();
                br.BaseStream.Seek((long)f.DataOffset, SeekOrigin.Begin);
                f.Data = br.ReadBytes((int)f.DataSize);
                f.Data = Functions.DecompressGZip(f.Data);
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
            public UInt16 SecondaryTypeLength;
            public string SecondaryType;
            public UInt16 NameLength, AuthorLength, VersionLength;
            public string Name, Author, Version;
            public Metadata(BinaryReader br)
            {
                PrimaryType = (SkinType)br.ReadByte();
                SecondaryTypeLength = br.ReadUInt16();
                SecondaryType = Encoding.ASCII.GetString(br.ReadBytes(SecondaryTypeLength));
                NameLength = br.ReadUInt16();
                AuthorLength = br.ReadUInt16();
                VersionLength = br.ReadUInt16();
                Name = Encoding.ASCII.GetString(br.ReadBytes(NameLength));
                Author = Encoding.ASCII.GetString(br.ReadBytes(AuthorLength));
                Version = Encoding.ASCII.GetString(br.ReadBytes(VersionLength));
            }
        }
        public class ArchiveFile
        {
            public UInt16 NameLength;
            public string Name;
            public UInt64 DataOffset, DataSize;
            public byte[] Data;
            public UInt32 mOffset;
            public ArchiveFile(UInt32 metaOffset)
            {
                mOffset = metaOffset;
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
