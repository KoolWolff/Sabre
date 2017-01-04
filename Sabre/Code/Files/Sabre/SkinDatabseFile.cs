using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class SkinDatabaseFile
    {
        public string Version;
        public BinaryReader br;
        public Header h;
        public List<Skin> Skins = new List<Skin>();
        public SkinDatabaseFile(string fileLocation)
        {
            br = new BinaryReader(File.OpenRead(fileLocation));
            h = new Header(br, Version);
            for(int i = 0; i < h.SkinCount; i++)
            {
                Skins.Add(new Skin(br, Version));
            }
        }
        public class Header
        {
            public string Magic;
            public byte Major;
            public byte Minor;
            public UInt32 SkinCount;
            public Header(BinaryReader br, string version)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(2));
                Major = br.ReadByte();
                Minor = br.ReadByte();
                SkinCount = br.ReadByte();
                version = Major + "." + Minor;
            }
        }
        public class Skin
        {
            public string Name;
            public string Author;
            public string Version;
            public bool IsInstalled;
            public UInt32 FileCount;
            public List<string> Files = new List<string>();
            public Skin(BinaryReader br, string version)
            {
                if(version == "1.0")
                {
                    Name = Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16()));
                    Author = Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16()));
                    Version = Encoding.ASCII.GetString(br.ReadBytes(br.ReadUInt16()));
                    IsInstalled = br.ReadBoolean();
                    FileCount = br.ReadUInt32();
                    for(int i = 0; i < FileCount; i++)
                    {
                        Files.Add(Functions.ReadNullTerminatedString(br));
                    }
                }
            }
        }
    }
}
