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
        public BinaryReader br;
        public Header h;
        public List<Entry> Entries = new List<Entry>();
        public BINFile(string fileLocation)
        {

        }
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
        public class Entry
        {
            public UInt32 Length;
            public EntryType Type;
            public UInt32 PropHash;
            public Property Prop;
            public UInt16 Count;
            public List<Value> Values = new List<Value>();
            public Entry(BinaryReader br)
            {
                Type = (EntryType)br.ReadUInt32();
            }
            public void Read(BinaryReader br, List<String> Hashes)
            {
                Length = br.ReadUInt32();
                Count = br.ReadUInt16();
                for(int i = 0; i < Count; i++)
                {
                    Values.Add(new Value());
                }
            }
        }
        public class Property
        {
            public UInt32 Hash;
            public string Prop;
            public Property(string prop, UInt32 hash)
            {
                Hash = hash;
                Prop = prop;
            }
            public Property(UInt32 hash)
            {
                Hash = hash;
                Prop = hash.ToString();
            }
        }
        public class Value
        {

        }
        public class ValueList
        {

        }
        public enum EntryType : UInt32
        {
            CharacterRecord = 602544405,
            SkinCharacterDataProperties = 2607278582,
            CharacterAnimations = 4126869447,
            CharacterSpells = 1585338886,
            CharacterMeta = 4160558231,
            InteractionData = 1250691283,
            ItemData = 608970470,
            MapAudio = 3010308524
        }
        public enum ValueType : byte
        {
            UShortVector3 = 0,
            BooleanValue = 1,
            ByteValue2 = 3,
            UShortValue = 5,
            UIntValue4 = 6,
            UIntValue3 = 7,
            UIntVector2 = 9,
            FloatValue2 = 10,
            FloatVector2 = 11,
            FloatVector3 = 12,
            FloatVector4 = 13,
            ByteVector4_1 = 15,
            StringValue = 16,
            UIntValue = 17,
            SameTypeValuesList1 = 18,
            ValueList2 = 19,
            ValueList = 20,
            ByteVector4_2 = 21,
            SameTypeValuesList2 = 22,
            DoubleTypesValuesList = 23
        }
    }
}
