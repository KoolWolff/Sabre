using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class WXYFile
    {
        public UInt32 PreviewCount;
        public UInt16 ProjectCount;
        public UInt32 FileCount;
        public List<Preview> Previews = new List<Preview>();
        public List<string> Projects = new List<string>();
        public List<string> FileNames = new List<string>();
        public List<FileEntry> Files = new List<FileEntry>();
        public BinaryReader br;
        public Header h;
        public WXYFile(string fileLocation)
        {
            br = new BinaryReader(File.OpenRead(fileLocation));
            h = new Header(br);
            PreviewCount = br.ReadUInt32();
            for (int i = 0; i < PreviewCount; i++)
            {
                Previews.Add(new Preview(br));
            }
            ProjectCount = br.ReadUInt16();
            for(int i = 0; i < ProjectCount; i++)
            {
                Projects.Add(Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16())));
            }
            FileCount = br.ReadUInt32();
            for(int i = 0; i < FileCount; i++)
            {
                FileNames.Add(Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16())));
            }
            for(int i = 0; i < FileCount; i++)
            {
                Files.Add(new FileEntry(br));
            }
            foreach(FileEntry fe in Files)
            {
                fe.Data = br.ReadBytes((int)fe.CompressedSize);
                fe.Data = DecompressDeflate(fe.Data);
            }
        }
        public class Header
        {
            public string Magic;
            public UInt32 FileVersion;
            public string OINK;
            public string Name, Author, Version, Category, SubCategory;
            public Header(BinaryReader br)
            {
                UInt16 NameL, AuthorL, VersionL, CategoryL, SubCategoryL;
                byte[] NameB, AuthorB, VersionB, CategoryB, SubCategoryB;
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                FileVersion = br.ReadUInt32();
                if (FileVersion < 6 || FileVersion > 7) throw new Exception("WXY Version: " + FileVersion + " not supported");
                OINK = Encoding.ASCII.GetString(br.ReadBytes(4));

                NameL = br.ReadUInt16();
                AuthorL = br.ReadUInt16();
                VersionL = br.ReadUInt16();
                CategoryL = br.ReadUInt16();
                SubCategoryL = br.ReadUInt16();

                NameB = br.ReadBytes(NameL);
                AuthorB = br.ReadBytes(AuthorL);
                VersionB = br.ReadBytes(VersionL);
                CategoryB = br.ReadBytes(CategoryL);
                SubCategoryB = br.ReadBytes(SubCategoryL);

                if (FileVersion > 7)
                {
                    NameB = DecompressMetadataString(NameB);
                    AuthorB = DecompressMetadataString(AuthorB);
                    VersionB = DecompressMetadataString(VersionB);
                    CategoryB = DecompressMetadataString(CategoryB);
                    SubCategoryB = DecompressMetadataString(SubCategoryB);

                    Name = Encoding.UTF8.GetString(DecompressDeflate(Functions.StringToByteArray(Name)));
                    Author = Encoding.UTF8.GetString(DecompressDeflate(Functions.StringToByteArray(Author)));
                    Version = Encoding.UTF8.GetString(DecompressDeflate(Functions.StringToByteArray(Version)));
                    Category = Encoding.UTF8.GetString(DecompressDeflate(Functions.StringToByteArray(Category)));
                    SubCategory = Encoding.UTF8.GetString(DecompressDeflate(Functions.StringToByteArray(SubCategory)));
                }  
                else
                {
                    Name = Encoding.ASCII.GetString(DecompressDeflate(NameB));
                    Author = Encoding.ASCII.GetString(DecompressDeflate(AuthorB));
                    Version = Encoding.ASCII.GetString(DecompressDeflate(VersionB));
                    Category = Encoding.ASCII.GetString(DecompressDeflate(CategoryB));
                    SubCategory = Encoding.ASCII.GetString(DecompressDeflate(SubCategoryB));
                }
            }
        }
        public class FileEntry
        {
            public string ProjectParent;
            public string Name;
            public byte[] SHA256;
            public byte[] MD5;
            public UInt32 Adler32; //ffs why 3 checksums
            public UInt32 UncompressedSize;
            public UInt32 CompressedSize;
            public CompressionType Compression;
            public byte[] Data;
            public FileEntry(BinaryReader br)
            {
                SHA256 = br.ReadBytes(32);
                MD5 = br.ReadBytes(16);
                Adler32 = br.ReadUInt32();
                UncompressedSize = br.ReadUInt32();
                CompressedSize = br.ReadUInt32();
                Compression = (CompressionType)br.ReadByte();
            }
        }
        public class Preview
        {
            public PreviewType Type;
            public UInt32 DataSize;
            public byte[] Data;
            public Preview(BinaryReader br)
            {
                Type = (PreviewType)br.ReadByte();
                DataSize = br.ReadUInt32();
                Data = br.ReadBytes((int)DataSize);
                if (Type == PreviewType.Image) Data = DecompressDeflate(Data);
            }
        }
        public enum PreviewType : byte
        {
            Image,
            Model 
        }
        public enum CompressionType : byte
        {
            Uncompressed, 
            Deflate, 
            Zlib 
        }

        public void Write(string fileLocation)
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileLocation)))
            {
                byte[] NameA = CompressDeflate(Encoding.ASCII.GetBytes(h.Name));
                byte[] AuthorA = CompressDeflate(Encoding.ASCII.GetBytes(h.Author));
                byte[] VersionA = CompressDeflate(Encoding.ASCII.GetBytes(h.Version));
                byte[] CategoryA = CompressDeflate(Encoding.ASCII.GetBytes(h.Category));
                byte[] SubCategoryA = CompressDeflate(Encoding.ASCII.GetBytes(h.SubCategory));
                bw.Write(h.Magic.ToCharArray());
                bw.Write(h.FileVersion);
                bw.Write(h.OINK.ToCharArray());

                bw.Write((UInt16)NameA.Length);
                bw.Write((UInt16)AuthorA.Length);
                bw.Write((UInt16)VersionA.Length);
                bw.Write((UInt16)CategoryA.Length);
                bw.Write((UInt16)SubCategoryA.Length);

                bw.Write(NameA);
                bw.Write(AuthorA);
                bw.Write(VersionA);
                bw.Write(CategoryA);
                bw.Write(SubCategoryA);
            }
        }
        public static byte[] DecompressDeflate(byte[] input)
        {
            return Ionic.Zlib.DeflateStream.UncompressBuffer(input);
        }
        public static byte[] CompressDeflate(byte[] input)
        {
            return Ionic.Zlib.DeflateStream.CompressBuffer(input);
        }
        public static byte[] DecompressMetadataString(byte[] deflateData)
        {
            byte[] newDeflate = deflateData;
            for (int i = 0; i < deflateData.Length; i++)
            {
                newDeflate[i] = (byte)(newDeflate[i] ^ (byte)(i * 42 + (newDeflate.Length - i) * 17));
            }
            return newDeflate;
        }
    }
}
