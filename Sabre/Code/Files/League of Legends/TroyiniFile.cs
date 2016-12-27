using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class TroyiniFile
    {
        public List<TroyiniProp> Props = new List<TroyiniProp>();
        public List<TroyiniData> Data = new List<TroyiniData>();
        public BinaryReader br;
        public Header header;
        private UInt16 Type00 = Convert.ToUInt16("0000000000000001", 2);
        private UInt16 Type01 = Convert.ToUInt16("0000000000000010", 2);
        private UInt16 Type02 = Convert.ToUInt16("0000000000000100", 2);
        private UInt16 Type03 = Convert.ToUInt16("0000000000001000", 2);
        private UInt16 Type04 = Convert.ToUInt16("0000000000010000", 2);
        private UInt16 Type05 = Convert.ToUInt16("0000000000100000", 2);
        private UInt16 Type06 = Convert.ToUInt16("0000000001000000", 2);
        private UInt16 Type07 = Convert.ToUInt16("0000000010000000", 2);
        private UInt16 Type08 = Convert.ToUInt16("0000000100000000", 2);
        private UInt16 Type09 = Convert.ToUInt16("0000001000000000", 2);
        private UInt16 Type10 = Convert.ToUInt16("0000010000000000", 2);
        private UInt16 Type11 = Convert.ToUInt16("0000100000000000", 2);
        private UInt16 Type12 = Convert.ToUInt16("0001000000000000", 2);
        private UInt16 Type13 = Convert.ToUInt16("0010000000000000", 2);
        private UInt16 Type14 = Convert.ToUInt16("0100000000000000", 2);
        private UInt16 Type15 = Convert.ToUInt16("1000000000000000", 2);

        public TroyiniFile(string fileLocation)
        {
            br = new BinaryReader(File.OpenRead(fileLocation));

            header = new Header(br);

            if (HasValueFromType(header.BitMask, Type00))
                ReadData(br, ValueType.UIntValue);
            if (HasValueFromType(header.BitMask, Type01))
                ReadData(br, ValueType.FloatValue);
            if (HasValueFromType(header.BitMask, Type02))
                ReadData(br, ValueType.ByteFloatValue);
            if (HasValueFromType(header.BitMask, Type03))
                ReadData(br, ValueType.UShortValue);
            if (HasValueFromType(header.BitMask, Type04))
                ReadData(br, ValueType.ByteValue);
            if (HasValueFromType(header.BitMask, Type05))
                ReadData(br, ValueType.BooleanValue);
            if (HasValueFromType(header.BitMask, Type06))
                ReadData(br, ValueType.Vector3Bytes);
            if (HasValueFromType(header.BitMask, Type07))
                ReadData(br, ValueType.Vector3Floats);
            if (HasValueFromType(header.BitMask, Type08))
                ReadData(br, ValueType.Vector2Bytes);
            if (HasValueFromType(header.BitMask, Type09))
                ReadData(br, ValueType.Vector2Floats);
            if (HasValueFromType(header.BitMask, Type10))
                ReadData(br, ValueType.Vector4Bytes);
            if (HasValueFromType(header.BitMask, Type11))
                ReadData(br, ValueType.Vector4Floats);
            if (HasValueFromType(header.BitMask, Type12))
                ReadData(br, ValueType.StringValue);
        }
        public class Header
        {
            public byte Version;
            public UInt16 StringTableLength;
            public UInt16 BitMask;
            public Header(BinaryReader br)
            {
                Version = br.ReadByte();
                StringTableLength = br.ReadUInt16();
                BitMask = br.ReadUInt16();
            }
        }
        public class TroyiniData
        {
            public ValueType Type;
            public List<TroyiniValue> Values = new List<TroyiniValue>();
            public TroyiniData(ValueType type)
            {
                Type = type;
            }
        }
        public class TroyiniProp
        {
            public string PrimaryKey;
            public string SecondaryKey;
            public UInt32 Hash;
            public TroyiniProp(UInt32 hash)
            {
                Hash = hash;
            }
        }
        public class TroyiniValue
        {
            public TroyiniData Parent;
            public object Value;
            public TroyiniProp Prop;
            public TroyiniValue(UInt32 key, TroyiniData parent)
            {
                Prop = new TroyiniProp(key);
                Parent = parent;
            }
            public TroyiniValue(UInt32 key, TroyiniData parent, object value)
            {
                Prop = new TroyiniProp(key);
                Parent = parent;
                Value = value;
            }
        }
        private static bool HasValueFromType(UInt16 bitmask, UInt16 type)
        {
            if((bitmask & type) == type) return true;
            else return false;
        }
        private void ReadData(BinaryReader br, ValueType type)
        {
            TroyiniData newData = new TroyiniData(type);
            Data.Add(newData);
            ReadTroyiniData(br, newData);
        }
        private void ReadTroyiniData(BinaryReader br, TroyiniData data)
        {
            UInt16 count = br.ReadUInt16();
            for(int i = 0; i < count; i++)
            {
                TroyiniValue newValue = new TroyiniValue(br.ReadUInt32(), data);
                Props.Add(newValue.Prop);
                data.Values.Add(newValue);
            }
            ReadValues(br, data);
        }
        private void ReadValues(BinaryReader br, TroyiniData data)
        {
            if(data.Type == ValueType.UIntValue)
            {
                for(int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = br.ReadUInt32();
                }
            }
            else if (data.Type == ValueType.FloatValue)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = br.ReadSingle();
                }
            }
            else if (data.Type == ValueType.ByteFloatValue)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = Convert.ToSingle(br.ReadByte() / 10);
                }
            }
            else if (data.Type == ValueType.UShortValue)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = br.ReadUInt16();
                }
            }
            else if (data.Type == ValueType.ByteValue)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = br.ReadByte();
                }
            }
            else if (data.Type == ValueType.BooleanValue)
            {
                ReadBooleans(br, data);
            }
            else if (data.Type == ValueType.Vector3Bytes)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = new byte[] { br.ReadByte(), br.ReadByte(), br.ReadByte(), };
                }
            }
            else if (data.Type == ValueType.StringValue)
            {
                ReadStrings(br, data);
            }
            else if (data.Type == ValueType.Vector3Floats)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), };
                }
            }
            else if (data.Type == ValueType.Vector2Bytes)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = new byte[] { br.ReadByte(), br.ReadByte() };
                }
            }
            else if (data.Type == ValueType.Vector2Floats)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = new float[] { br.ReadSingle(), br.ReadSingle() };
                }
            }
            else if (data.Type == ValueType.Vector4Bytes)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = new byte[] { br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() };
                }
            }
            else if (data.Type == ValueType.Vector4Floats)
            {
                for (int i = 0; i < data.Values.Count; i++)
                {
                    data.Values[i].Value = new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() };
                }
            }
        }
        private void ReadBooleans(BinaryReader br, TroyiniData data)
        {
            int bytesToRead = 0;
            int fullCount = data.Values.Count;
            while (fullCount > 0)
            {
                bytesToRead += 1;
                fullCount -= 8;
            }
            int valueCount = 0;
            for (int i = 0; i < bytesToRead; i++)
            {
                byte readByte = br.ReadByte();
                int byteCount = 0;
                while (byteCount < 8 & valueCount < data.Values.Count)
                {
                    data.Values[valueCount].Value = Convert.ToBoolean(readByte % 2);
                    readByte /= 2;
                    valueCount += 1;
                    byteCount += 1;
                }
            }
        }
        private void ReadStrings(BinaryReader br, TroyiniData data)
        {
            long initialPosition = br.BaseStream.Position;
            for (int i = 0; i < data.Values.Count; i++)
            {
                br.BaseStream.Seek(initialPosition + 2 * i, SeekOrigin.Begin);
                ushort stringOffset = br.ReadUInt16();
                br.BaseStream.Seek(initialPosition + 2 * data.Values.Count + stringOffset, SeekOrigin.Begin);
                string newString = "";
                if (br.BaseStream.Position > br.BaseStream.Length)
                {
                    data.Values[i].Value = "NULL";
                }
                else
                {
                    char readChar = br.ReadChar();
                    while (readChar != '\0')
                    {
                        newString = newString + readChar;
                        readChar = br.ReadChar();
                    }
                    data.Values[i].Value = newString;
                }
            }
        }
        public UInt32 GetHash(string s1, string s2)
        {
            return GetHashS2(GetHashS1(s1), s2);
        }
        private UInt32 GetHashS1(string s1)
        {
            s1 = s1.ToLower();
            UInt32 hash = 0;

            for (int i = 0; i < s1.Length; i++)
            {
                hash = (s1[i] + 65599 * hash) & UInt32.MaxValue;
            }

            hash = (65599 * hash + 42) & UInt32.MaxValue;
            return hash;
        }
        private UInt32 GetHashS2(UInt32 s1Hash, string s2)
        {
            s2 = s2.ToLower();
            UInt32 hash = s1Hash;
            for (int i = 0; i < s2.Length; i++)
            {
                hash = (s2[i] + 65599 * hash) & UInt32.MaxValue;
            }
            return hash;
        }
        public enum ValueType : byte
        {
            UIntValue = 0,
            FloatValue = 1,
            ByteFloatValue = 2,
            UShortValue = 3,
            ByteValue = 4,
            BooleanValue = 5,
            Vector3Bytes = 6,
            StringValue = 12,
            Vector3Floats = 7,
            Vector2Bytes = 8,
            Vector2Floats = 9,
            Vector4Bytes = 10,
            Vector4Floats = 11
        }
    }
}
