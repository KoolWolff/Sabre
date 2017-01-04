using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class ReleaseManifestFile
    {
        private BinaryReader br;
        public Header h;
        public List<RMFile> Files = new List<RMFile>();
        public List<RMDirectory> Directories = new List<RMDirectory>();
        public List<string> Names = new List<string>();
        public UInt32 FileCount;
        public UInt32 StringCount;
        public UInt32 StringSize;
        public ReleaseManifestFile(string fileLocation)
        {
            br = new BinaryReader(File.OpenRead(fileLocation)); 

            h = new Header(br);
            for(int i = 0; i < h.DirectoryCount; i++)
            {
                Directories.Add(new RMDirectory(br));
            }
            FileCount = br.ReadUInt32();
            for (int i = 0; i < FileCount; i++)
            {
                Files.Add(new RMFile(br));
            }
            StringCount = br.ReadUInt32();
            StringSize = br.ReadUInt32();
            for (int i = 0; i < StringCount; i++)
            {
                Names.Add(Functions.ReadNullTerminatedString(br));
            }
            foreach (RMFile f in Files)
            {
                f.Name = Names[(int)f.NameIndex];
            }
            foreach (RMDirectory d in Directories)
            {
                d.Name = Names[(int)d.NameIndex];
                for (int i = 0; i < d.FileCount; i++)
                {
                    d.FileList.Add(Files[i + (int)d.FileListStartIndex]);
                }
                for (int i = 0; i < d.SubDirectoryCount; i++)
                {
                    d.SubDirectoryList.Add(Directories[i + (int)d.SubDirectoryStartIndex]);
                }
                GetDirParent(Directories, d);
                d.DirectoryFullPath = d.GetFullPath();
                foreach (RMFile f in d.FileList)
                {
                    f.Directory = d;
                    f.FullName = d.DirectoryFullPath + "\\" + f.Name;
                }
            }
        }

        public class Header
        {
            public string Magic;
            public UInt32 Type;
            public UInt32 EntryCount;
            public UInt32 Version;
            public UInt32 DirectoryCount;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                Type = br.ReadUInt32();
                EntryCount = br.ReadUInt32();
                Version = br.ReadUInt32();
                DirectoryCount = br.ReadUInt32();
            }
        }

        public class RMFile
        {
            public string FullName { get; set; }
            public string Name {get; set; }
            public UInt32 NameIndex;
            public UInt32 Version;
            public byte[] MD5; //16 bytes
            public string MD5s;
            public UInt32 DeployType;
            public UInt32 SizeUncompressed;
            public UInt32 SizeCompressed;
            public UInt32 Unk;
            public UInt16 FileType;
            public byte Constant1;
            public byte Constant2;
            public RMDirectory Directory;
            public RMFile(BinaryReader br)
            {
                NameIndex = br.ReadUInt32();
                Version = br.ReadUInt32();
                MD5 = br.ReadBytes(16);
                MD5s = Functions.ByteArrayToString(MD5, Functions.StringCaseType.Lowercase);
                DeployType = br.ReadUInt32();
                SizeUncompressed = br.ReadUInt32();
                SizeCompressed = br.ReadUInt32();
                Unk = br.ReadUInt32();
                FileType = br.ReadUInt16();
                Constant1 = br.ReadByte();
                Constant2 = br.ReadByte();
            }
        }
        public class RMDirectory
        {
            public string DirectoryFullPath { get; set; }
            public string Name {get; set; }
            public List<RMDirectory> SubDirectoryList = new List<RMDirectory>();
            public RMDirectory Parent;
            public List<RMFile> FileList = new List<RMFile>();
            public UInt32 NameIndex;
            public UInt32 SubDirectoryStartIndex;
            public UInt32 SubDirectoryCount;
            public UInt32 FileListStartIndex;
            public UInt32 FileCount;
            public RMDirectory(BinaryReader br)
            {
                NameIndex = br.ReadUInt32();
                SubDirectoryStartIndex = br.ReadUInt32();
                SubDirectoryCount = br.ReadUInt32();
                FileListStartIndex = br.ReadUInt32();
                FileCount = br.ReadUInt32();
            }
            public string GetFullPath()
            {
                if(Parent != null)
                {
                   return Parent.GetFullPath() + "\\" + Name;    
                }
                return Name;
            }
        }
        public static void GetDirParent(List<RMDirectory> dirs, RMDirectory dir)
        {
            if(dir.Name == "") { }
            else
            {
                foreach(RMDirectory rmd in dirs)
                {
                    if(rmd.SubDirectoryList.Contains(dir)) dir.Parent = rmd;
                }
            }
        }
    }
}
