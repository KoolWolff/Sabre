using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class DBFile
    {
        public BinaryReader br;
        public Header header;
        public List<Entry> Entries = new List<Entry>();
        public List<FileEntry> FileEntries = new List<FileEntry>();
        public DBFile(string fileLocation)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
        }
        public class Header
        {
            public string Magic;
            public byte Major;
            public byte Minor;
            public UInt32 NumberOfSkins;
            public UInt32 NumberOfInstalledSkins;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(2));
                Major = br.ReadByte();
                Minor = br.ReadByte();
                NumberOfSkins = br.ReadUInt32();
                NumberOfInstalledSkins = br.ReadUInt32();
            }
        }
        public class Entry
        {
            public EntryType Type;
            public UInt16 FileNameLength;
            public string FileName;
            public Entry(BinaryReader br)
            {
                Type = (EntryType)br.ReadUInt16();
                FileNameLength = br.ReadUInt16();
                FileName = Encoding.ASCII.GetString(br.ReadBytes(FileNameLength));
            }
        }
        public class FileEntry
        {
            public UInt16 Length;
            public string File;
            public FileEntry(BinaryReader br)
            {
                Length = br.ReadUInt16();
                File = Encoding.ASCII.GetString(br.ReadBytes(Length));
            }
        }
        public enum EntryType : UInt16
        {
            Installed = 1,
            Uninstalled = 2
        }
    }
}
