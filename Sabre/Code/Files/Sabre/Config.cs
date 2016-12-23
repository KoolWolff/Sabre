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
        public BinaryReader br;
        public List<Setting> Settings = new List<Setting>();
        public Config(string fileLocation, Logger log)
        {
            using (br = new BinaryReader(File.OpenRead(fileLocation)))
            {
                do
                {
                    Settings.Add(new Setting(br));
                } while(br.BaseStream.Position != br.BaseStream.Length);
            }
        }
        public static void Write(List<Setting> settings)
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite("config")))
            {
                foreach(Setting s in settings)
                {
                    if(s.TypeEntry == EntryType.Bool)
                    {
                        bw.Write((byte)EntryType.Bool);
                        bw.Write((byte)s.Type);
                        bw.Write(Convert.ToByte(s.BoolEntry));
                    }
                    else if(s.TypeEntry == EntryType.Float)
                    {
                        bw.Write((byte)EntryType.Float);
                        bw.Write((byte)s.Type);
                        bw.Write(s.FloatEntry);
                    }
                    else if(s.TypeEntry == EntryType.Int)
                    {
                        bw.Write((byte)EntryType.Int);
                        bw.Write((byte)s.Type);
                        bw.Write(s.IntEntry);
                    }
                    else
                    {
                        bw.Write((byte)EntryType.String);
                        bw.Write((byte)s.Type);
                        bw.Write((UInt16)s.StringEntry.Length);
                        bw.Write(s.StringEntry.ToCharArray());
                    }
                }
            }
        }
        public class Setting
        {
            public EntryType TypeEntry;
            public SettingType Type;
            public bool BoolEntry;
            public UInt16 StringLength;
            public string StringEntry;
            public int IntEntry;
            public float FloatEntry;
            public Setting(BinaryReader br)
            {
                TypeEntry = (EntryType)br.ReadByte();
                Type = (SettingType)br.ReadByte();
                if(TypeEntry == EntryType.Bool) BoolEntry = Convert.ToBoolean(br.ReadByte());
                else if(TypeEntry == EntryType.Int) IntEntry = br.ReadInt32();
                else if(TypeEntry == EntryType.Float) FloatEntry = br.ReadSingle();
                else StringLength = br.ReadUInt16(); StringEntry = Encoding.ASCII.GetString(br.ReadBytes(StringLength));
            }
            public Setting(float floatEntry, SettingType type)
            {
                TypeEntry = EntryType.Float;
                Type = type;
                FloatEntry = floatEntry;
            }
            public Setting(string stringEntry, SettingType type)
            {
                TypeEntry = EntryType.String;
                Type = type;
                StringLength = (UInt16)stringEntry.Length;
                StringEntry = stringEntry;
            }
            public Setting(int intEntry, SettingType type)
            {
                TypeEntry = EntryType.Int;
                Type = type;
                IntEntry = intEntry;
            }
            public Setting(bool boolEntry, SettingType type)
            {
                TypeEntry = EntryType.Bool;
                Type = type;
                BoolEntry = boolEntry;
            }
            public static void Write(float floatEntry, SettingType type)
            {
                var addB = File.ReadAllBytes("config");
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite("config")))
                {
                    bw.Write(addB);
                    bw.Write(File.ReadAllBytes("config"));
                    bw.Write((byte)EntryType.Float);
                    bw.Write((byte)type);
                    bw.Write(floatEntry);
                }
            }
            public static void Write(string stringEntry, SettingType type)
            {
                var addB = File.ReadAllBytes("config");
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite("config")))
                {
                    bw.Write(addB);
                    bw.Write((byte)EntryType.String);
                    bw.Write((byte)type);
                    bw.Write((UInt16)stringEntry.Length);
                    bw.Write(stringEntry.ToCharArray());
                }
            }
            public static void Write(int intEntry, SettingType type)
            {
                var addB = File.ReadAllBytes("config");
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite("config")))
                {
                    bw.Write(addB);
                    bw.Write(File.ReadAllBytes("config"));
                    bw.Write((byte)EntryType.Int);
                    bw.Write((byte)type);
                    bw.Write(intEntry);
                }
            }
            public static void Write(bool boolEntry, SettingType type)
            {
                var addB = File.ReadAllBytes("config");
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite("config")))
                {
                    bw.Write(addB);
                    bw.Write(File.ReadAllBytes("config"));
                    bw.Write((byte)EntryType.Bool);
                    bw.Write((byte)type);
                    bw.Write(Convert.ToByte(boolEntry));
                }
            }
        }
        public enum SettingType : byte
        {
            LoLPath = 0,
            Theme = 1,
            Accent = 2,
            WADPath = 3,
            WADExtractionPath = 4,
            MOBPath = 5,
            MOBExtractionPath = 6,
            MOBImportationPath = 7,
            WPKPath = 8,
            WPKExtractionPath = 9,
            WPKImportationPath = 10
        }
        public enum EntryType : byte
        {
            Int,
            Float,
            String,
            Bool
        }
    }
}
