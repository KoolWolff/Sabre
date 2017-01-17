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
        public List<Identifier> Identifiers = new List<Identifier>();
        public WADFile(string fileLocation, Logger log, List<string> Hashes)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            header = new Header(br);
            string[] wadhash = File.ReadAllLines("wad.txt");
            for (int i = 0; i < header.FileCount; i++)
            {
                Entries.Add(new Entry(br, header.Version));
                try { Identifiers.Add(new Identifier { xxHash = wadhash[i].Split(' ')[0], Name = wadhash[i].Split(' ')[1] }); }
                catch (Exception) { }
            }
            foreach(Entry e in Entries)
            {
                if(e.Compression == CompressionType.Compressed)
                {
                    ReadData(br, e, true, Identifiers);
                }
                else if(e.Compression == CompressionType.String)
                {
                    ReadString(br, e);
                }
                else if(e.Compression == CompressionType.Uncompressed)
                {
                    ReadData(br, e, false, Identifiers);
                }
                else
                {
                    e.Compression = CompressionType.Unknown;
                }
            }
        }
        public class Header
        {
            public string Version;
            public string Magic;
            public byte Major;
            public byte Minor;
            public byte ECDSALength;
            public byte[] ECDSA {get; set;}
            public byte[] ZeroPadding;
            public UInt64 WADFileID; //According to SS, previously "Checksum"
            public string ChecksumS;
            public UInt16 TOCStartOffset;
            public UInt16 TOCFileEntrySize;
            public UInt32 FileCount;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(2));
                Major = br.ReadByte();
                Minor = br.ReadByte();
                Version = Major.ToString() + "." + Minor.ToString();
                if (Version == "2.0")
                {
                    ECDSALength = br.ReadByte();
                    ECDSA = br.ReadBytes(80);
                    ZeroPadding = br.ReadBytes(3);
                }
                WADFileID = br.ReadUInt64();
                TOCStartOffset = br.ReadUInt16();
                TOCFileEntrySize = br.ReadUInt16();
                FileCount = br.ReadUInt32();   
            }
        }
        public class Entry
        {
            public bool IsBINGenerated = false;
            public string Size { get; set; }
            public string Name { get; set; }
            public byte[] Data;
            public string XXHash { get; set; }
            public UInt32 FileDataOffset;
            public UInt32 CompressedSize;
            public UInt32 UncompressedSize;
            public CompressionType Compression { get; set; }
            public UInt64 SHA256;
            public Entry(BinaryReader br, string Version)
            {
                XXHash = br.ReadUInt64().ToString("X2");
                FileDataOffset = br.ReadUInt32();
                CompressedSize = br.ReadUInt32();
                UncompressedSize = br.ReadUInt32();
                Compression = (CompressionType)br.ReadUInt32();
                if(Version == "2.0")
                    SHA256 = br.ReadUInt64();
                Size = Functions.SizeSuffix(UncompressedSize);
            }
        }
        public static void ReadString(BinaryReader br, Entry entry)
        {
            br.BaseStream.Seek(entry.FileDataOffset, SeekOrigin.Begin);
            entry.Name = Encoding.ASCII.GetString(br.ReadBytes((int)br.ReadUInt32()));
        }
        public static void ReadData(BinaryReader br, Entry entry, bool isCompressed, List<Identifier> identifiers)
        {
            br.BaseStream.Seek(entry.FileDataOffset, SeekOrigin.Begin);
            if(isCompressed)
            {
                entry.Data = br.ReadBytes((int)entry.CompressedSize);
                entry.Data = Functions.DecompressGZip(entry.Data);
            }
            else
            {
                entry.Data = br.ReadBytes((int)entry.UncompressedSize);
            }
            entry.Name = identifiers.Find(x => x.xxHash == entry.XXHash).Name;
        }
        public struct Identifier
        {
            public string xxHash;
            public string Name;
        }
        public enum CompressionType : UInt32
        {
            Uncompressed = 0,
            Compressed = 1,
            String = 2,
            Unknown
        }
    }
}
