using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sabre
{
    class RAFFile
    {
        private BinaryReader br;
        public UInt32 EntryCount;
        public UInt32 PathListSize;
        public UInt32 PathListCount;
        public Header h;
        public List<PathListEntry> Paths = new List<PathListEntry>();
        public List<FileListEntry> Files = new List<FileListEntry>();
        public RAFFile(string fileLocation, Logger log, string fileName, bool extractFiles = true)
        {
            br = new BinaryReader(File.OpenRead(fileLocation));
            h = new Header(br, log);
            EntryCount = br.ReadUInt32();
            for (int i = 0; i < EntryCount; i++)
            {
                Files.Add(new FileListEntry(br, log));
            }
            PathListSize = br.ReadUInt32();
            PathListCount = br.ReadUInt32();
            for (int i = 0; i < PathListCount; i++)
            {
                Paths.Add(new PathListEntry(br, log));
            }
            foreach (PathListEntry ple in Paths)
            {
                br.BaseStream.Seek(ple.PathOffset + h.PathListOffset, SeekOrigin.Begin);
                ple.Path = Encoding.ASCII.GetString(br.ReadBytes((int)ple.PathLength));
                ple.Path = ple.Path.Remove(ple.Path.Length - 1, 1);
            }
            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].FileName = Paths[(int)Files[i].PathListIndex].Path;
            }
            if (extractFiles)
            {
                br = new BinaryReader(File.OpenRead(fileName + ".dat"));
                foreach (FileListEntry fle in Files)
                {
                    br.BaseStream.Seek(fle.DataOffset, SeekOrigin.Begin);
                    fle.Data = br.ReadBytes((int)fle.DataSize);
                    fle.Decompressed = Functions.DecompressZlib(fle.Data);
                    fle.Size = Functions.SizeSuffix(fle.Decompressed.Length);
                }
            }
        }

        public class Header
        {
            public int Magic;
            public UInt32 Version;
            public UInt32 ManagerIndex;
            public UInt32 FileListOffset;
            public UInt32 PathListOffset;
            public Header(BinaryReader br, Logger log)
            {
                Magic = br.ReadInt32();
                if (Magic != 0x18be0ef0)
                    log.Write("RAF::IO | NOT A VALID RAF FILE", Logger.WriterType.WriteError);
                Version = br.ReadUInt32();
                ManagerIndex = br.ReadUInt32();
                FileListOffset = br.ReadUInt32();
                PathListOffset = br.ReadUInt32();
            }
        }
        public class PathListEntry
        {
            public string Path;
            public UInt32 PathOffset;
            public UInt32 PathLength;
            public PathListEntry(BinaryReader br, Logger log)
            {
                try
                {
                    PathOffset = br.ReadUInt32();
                    PathLength = br.ReadUInt32();
                }
                catch (Exception) { log.Write("RAF::IO::PathListEntry | FAILED TO READ PATH ENTRY", Logger.WriterType.WriteError); }
            }
        }
        public class FileListEntry
        {
            public string FileName { get; set; }
            public string Size { get; set; }
            public byte[] Decompressed;
            public byte[] Data;
            public UInt32 PathHash;
            public UInt32 DataOffset;
            public UInt32 DataSize;
            public UInt32 PathListIndex;
            public FileListEntry(BinaryReader br, Logger log)
            {
                try
                {
                    PathHash = br.ReadUInt32();
                    DataOffset = br.ReadUInt32();
                    DataSize = br.ReadUInt32();
                    PathListIndex = br.ReadUInt32();
                }
                catch (Exception) { log.Write("RAF::IO::FileListEntry | FAILED TO READ LIST ENTRY", Logger.WriterType.WriteError); }
            }
        }
    }
}
