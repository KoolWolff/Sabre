using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class Config
    {
        public byte Version;
        public string Magic;
        public UInt32 EntryCount;
        public UInt32 Checksum;
        public BinaryReader br;
        public List<Entry> Entries = new List<Entry>();
        public Config(string fileLocation, Logger log)
        {
            br = new BinaryReader(File.OpenRead(fileLocation));

            Version = br.ReadByte();
            Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
            if(Magic != "SCFG")
                log.Write("CFG::IO | INCORRECT CONFIG FILE", Logger.WriterType.WriteError);
            EntryCount = br.ReadUInt32();
            Checksum = br.ReadUInt32();
            for(int i = 0; i < EntryCount; i++)
            {
                Entries.Add(new Entry(br));
            }
            if(Checksum != Hash.ConfigChecksum(EntryCount, Entries))
                log.Write("CFG::IO | INCORRECT CONFIG CHECKSUM", Logger.WriterType.WriteError);
        }
        public class Entry
        {
            public SettingType Type;
            public EntryType entryType;
            public UInt16 StringLength;
            public string StringValue;
            public UInt32 UIntValue;
            public bool Enum;
            public Entry(BinaryReader br)
            {
                Type = (SettingType)br.ReadByte();
                entryType = (EntryType)br.ReadByte();
                switch(entryType)
                {
                    case EntryType.String:
                        StringLength = br.ReadUInt16();
                        StringValue = Encoding.ASCII.GetString(br.ReadBytes(StringLength));
                        break;
                    case EntryType.Bool:
                        Enum = Convert.ToBoolean(br.ReadByte());
                        break;
                    case EntryType.UInt:
                        UIntValue = br.ReadUInt32();
                        break;
                }
            }
        }
        public enum SettingType : byte
        {
            AccentColor = 0,
            Theme = 1,
            DetectLoLLocationOnStartup = 3,
            LoLLocation = 4,
        }
        public enum EntryType : byte
        {
            String = 0,
            Bool = 1,
            UInt = 2
        }
    }
}
