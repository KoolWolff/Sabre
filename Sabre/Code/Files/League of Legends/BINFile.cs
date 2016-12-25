using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabre
{
    class BINFile
    {
        public class Header
        {
            public string Magic;
            public UInt32 Version;
            public UInt32 ChildCount;
            public List<Associated> AssociatedBIN = new List<Associated>();
            public UInt32 EntryCount;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                Version = br.ReadUInt32();
                if(Version == 2)
                {
                    ChildCount = br.ReadUInt32();
                    for (int i = 0; i < ChildCount; i++)
                    {
                        AssociatedBIN.Add(new Associated(br));
                    }
                }
                EntryCount = br.ReadUInt32();
            }
            public Header(MemoryStream ms)
            {
                BinaryReader br = new BinaryReader(ms);
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                Version = br.ReadUInt32();
                if (Version == 2)
                {
                    ChildCount = br.ReadUInt32();
                    for (int i = 0; i < ChildCount; i++)
                    {
                        AssociatedBIN.Add(new Associated(br));
                    }
                }
                EntryCount = br.ReadUInt32();
            }
        }
        public class Associated
        {
            public UInt16 Length;
            public string Name;
            public Associated(BinaryReader br)
            {
                Length = br.ReadUInt16();
                Name = Encoding.ASCII.GetString(br.ReadBytes(Length));
            }
        }
    }
}
