/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabre
{
	public class TroyiniFile
	{
		public string FileLocation;
		private byte Version;
		private ushort DataSize;
		private ushort ValueTypes;
		public List<IniBinData> Data = new List<IniBinData>();
		public List<InibinProp> UsedProps = new List<InibinProp>();
		//VALUES TYPES
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
		public class InibinProp
		{
			public string PrimaryKey = null;
			public string SecondaryKey = null;
			public UInt32 Hash = 0;
			public InibinProp(UInt32 hash)
			{
				this.Hash = hash;
			}
		}

		private ushort Type00 = Convert.ToUInt16("0000000000000001", 2);
		private ushort Type01 = Convert.ToUInt16("0000000000000010", 2);
		private ushort Type02 = Convert.ToUInt16("0000000000000100", 2);
		private ushort Type03 = Convert.ToUInt16("0000000000001000", 2);
		private ushort Type04 = Convert.ToUInt16("0000000000010000", 2);
		private ushort Type05 = Convert.ToUInt16("0000000000100000", 2);
		private ushort Type06 = Convert.ToUInt16("0000000001000000", 2);
		private ushort Type07 = Convert.ToUInt16("0000000010000000", 2);
		private ushort Type08 = Convert.ToUInt16("0000000100000000", 2);
		private ushort Type09 = Convert.ToUInt16("0000001000000000", 2);
		private ushort Type10 = Convert.ToUInt16("0000010000000000", 2);
		private ushort Type11 = Convert.ToUInt16("0000100000000000", 2);
		private ushort Type12 = Convert.ToUInt16("0001000000000000", 2);
		private ushort Type13 = Convert.ToUInt16("0010000000000000", 2);
		private ushort Type14 = Convert.ToUInt16("0100000000000000", 2);
		private ushort Type15 = Convert.ToUInt16("1000000000000000", 2);
		private void UpdateValueTypes()
		{
			UInt16 valueTypes = 0;
			foreach (dataCategory_loopVariable in this.Data) {
				dataCategory = dataCategory_loopVariable;
				if (dataCategory.Values.Count > 0) {
					valueTypes += Math.Pow(2, (dataCategory.Type));
				}
			}
			this.ValueTypes = valueTypes;
		}
		private IniBinData GetDataCategory(ValueType categoryType)
		{
			foreach (category_loopVariable in this.Data) {
				category = category_loopVariable;
				if (category.Type == categoryType) {
					return category;
				}
			}
			return null;
		}
		public IniBinValue AddValue(ref ValueType valueType, InibinProp inibinProp, ref object value)
		{
			IniBinData gotDataCategory = GetDataCategory(valueType);
			if (gotDataCategory == null) {
				this.Data.Add(new IniBinData(valueType));
				gotDataCategory = this.Data.Last();
			}
			IniBinValue addedValue = new IniBinValue(0, gotDataCategory, value);
			addedValue.Prop = inibinProp;
			gotDataCategory.Values.Insert(GetPosition(ref gotDataCategory, addedValue.Prop.Hash), addedValue);
			return addedValue;
		}
		private int GetPosition(ref IniBinData data, UInt32 hash)
		{
			int position = 0;
			for (int i = 0; i <= data.Values.Count - 1; i++) {
				if (Convert.ToUInt32(data.Values[i].Prop.Hash) < hash)
                {
					position = i + 1;
				} 
			}
			return position;
		}
		public void InibinFile(string FileLocation)
		{
			this.FileLocation = FileLocation;
			using (BinaryReader br = new BinaryReader(File.Open(FileLocation, FileMode.Open))) {
				this.Version = br.ReadByte();
				if (this.Version != 2) {
					throw new Exception("Nope, not workin");
				}
				this.DataSize = br.ReadUInt16();
				this.ValueTypes = br.ReadUInt16();
				if (this.ValueTypes >= Math.Pow(2, 13))
                {
					throw new Exception();
				}
				if (HasValueFromType(Type00))
                {
					ReadData(ref br, ref ValueType.UIntValue);
				}
				if (HasValueFromType(Type01))
                {
					ReadData(ref br, ref ValueType.FloatValue);
				}
				if (HasValueFromType(Type02))
                {
					ReadData(ref br, ref ValueType.ByteFloatValue);
				}
				if (HasValueFromType(Type03))
                {
					ReadData(ref br, ref ValueType.UShortValue);
				}
				if (HasValueFromType(Type04))
                {
					ReadData(ref br, ref ValueType.ByteValue);
				}
				if (HasValueFromType(Type05))
                {
					ReadData(ref br, ref ValueType.BooleanValue);
				}
				if (HasValueFromType(Type06))
                {
					ReadData(ref br, ref ValueType.Vector3Bytes);
				}
				if (HasValueFromType(Type07))
                {
					ReadData(ref br, ref ValueType.Vector3Floats);
				}
				if (HasValueFromType(Type08))
                {
					ReadData(ref br, ref ValueType.Vector2Bytes);
				}
				if (HasValueFromType(Type09))
                {
					ReadData(ref br, ref ValueType.Vector2Floats);
				}
				if (HasValueFromType(Type10))
                {
					ReadData(ref br, ref ValueType.Vector4Bytes);
				}
				if (HasValueFromType(Type11))
                {
					ReadData(ref br, ref ValueType.Vector4Floats);
				}
				if (HasValueFromType(Type12))
                {
					ReadData(ref br, ref ValueType.StringValue);
				}
			}
			bool hasGroupPartValue = true;
			int i = 1;
			while (hasGroupPartValue) {
				hasGroupPartValue = false;
				UInt32 hash = InibinHash.GetHash("System", "GroupPart" + i);
				foreach (Dat_loopVariable in this.Data) {
					Dat = Dat_loopVariable;
					foreach (void Valu_loopVariable in Dat.Values) {
						Valu = Valu_loopVariable;
						if (Valu.Prop.Hash == hash) {
							if (MainWindow.inibinStrings.PrimaryKeys.Contains(Valu.Value) == false) {
								MainWindow.inibinStrings.PrimaryKeys.Add(Valu.Value);
							}
							hasGroupPartValue = true;
						}
					}
				}
				i += 1;
			}
			foreach (primaryKey_loopVariable in MainWindow.inibinStrings.PrimaryKeys) {
				primaryKey = primaryKey_loopVariable;
				UInt32 primKeyHash = GetHashS1(primaryKey);
				foreach (void secondaryKey_loopVariable in MainWindow.inibinStrings.SecondaryKeys) {
					secondaryKey = secondaryKey_loopVariable;
					UInt32 resultHash = GetHashS2(primKeyHash, secondaryKey);
					foreach (void usedProp_loopVariable in this.UsedProps) {
						usedProp = usedProp_loopVariable;
						if (usedProp.Hash == resultHash) {
							usedProp.PrimaryKey = primaryKey;
							usedProp.SecondaryKey = secondaryKey;
						}
					}
				}
			}
			foreach (Dat_loopVariable in this.Data) {
				Dat = Dat_loopVariable;
				foreach (void Valu_loopVariable in Dat.Values) {
					Valu = Valu_loopVariable;
					if (Valu.Prop.PrimaryKey == null) {
						Valu.Prop.PrimaryKey = "Unknown";
						Valu.Prop.SecondaryKey = ((IniBinProperty)Valu.Prop.Hash).ToString();
					}
				}
			}
		}
		private void ReadData(ref BinaryReader br, ref ValueType ValueType)
		{
			IniBinData newIniBinData = new IniBinData(ValueType);
			this.Data.Add(newIniBinData);
			ReadIniBinData(ref br, ref newIniBinData);
		}
		private void ReadIniBinData(ref BinaryReader br, ref IniBinData IniBinData)
		{
			ushort count = br.ReadUInt16();
			for (int i = 0; i <= count - 1; i++) {
				IniBinValue newValue = new IniBinValue(br.ReadUInt32(), ref IniBinData);
				this.UsedProps.Add(newValue.Prop);
				IniBinData.Values.Add(newValue);
			}
			ReadValues(ref br, ref IniBinData);
		}

		private void ReadValues(ref BinaryReader br, ref IniBinData IniBinData)
		{
			TroyiniFile.ValueType type = IniBinData.Type;
			if (type == ValueType.ByteValue) {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = br.ReadByte();
				}
			} else if (type == ValueType.FloatValue) {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = br.ReadSingle();
				}
			} else if (type == ValueType.StringValue) {
				ReadStrings(ref br, ref IniBinData);
			} else if (type == ValueType.UIntValue) {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = br.ReadUInt32();
				}
			} else if (type == ValueType.UShortValue) {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = br.ReadUInt16();
				}
			} else if (type == ValueType.ByteFloatValue) {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = Convert.ToSingle(br.ReadByte()) / 10;
				}
			} else if (type == ValueType.BooleanValue) {
				ReadBooleans(ref br, ref IniBinData);
			}
            else if (type == ValueType.Vector2Bytes)
            {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = new byte[] {
						br.ReadByte(),
						br.ReadByte()
					};
				}
			}
            else if (type == ValueType.Vector2Floats)
            {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = new float[] {
						br.ReadSingle(),
						br.ReadSingle()
					};
				}
			}
            else if (type == ValueType.Vector3Bytes)
            {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = new byte[] {
						br.ReadByte(),
						br.ReadByte(),
						br.ReadByte()
					};
				}
			}
            else if (type == ValueType.Vector3Floats)
            {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = new float[] {
						br.ReadSingle(),
						br.ReadSingle(),
						br.ReadSingle()
					};
				}
			}
            else if (type == ValueType.Vector4Bytes)
            {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = new byte[] {
						br.ReadByte(),
						br.ReadByte(),
						br.ReadByte(),
						br.ReadByte()
					};
				}
			}
            else if (type == ValueType.Vector4Floats)
            {
				for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
					IniBinData.Values[i].Value = new float[] {
						br.ReadSingle(),
						br.ReadSingle(),
						br.ReadSingle(),
						br.ReadSingle()
					};
				}
			}
		}

		private void ReadStrings(ref BinaryReader br, ref IniBinData IniBinData)
		{
			long initialPosition = br.BaseStream.Position;
			for (int i = 0; i <= IniBinData.Values.Count - 1; i++) {
				br.BaseStream.Seek(initialPosition + 2 * i, SeekOrigin.Begin);
				ushort stringOffset = br.ReadUInt16();
				br.BaseStream.Seek(initialPosition + 2 * IniBinData.Values.Count + stringOffset, SeekOrigin.Begin);
				string newString = "";
				if (br.BaseStream.Position > br.BaseStream.Length) {
					IniBinData.Values[i].Value = "NULL";
				} else {
					char readChar = br.ReadChar();
					while (readChar != 0) {
						newString = newString + readChar;
						readChar = br.ReadChar();
					}
					IniBinData.Values[i].Value = newString;
				}
			}
		}

		private void ReadBooleans(ref BinaryReader br, ref IniBinData IniBinData)
		{
			int bytesToRead = 0;
			int fullCount = IniBinData.Values.Count;
			while (fullCount > 0) {
				bytesToRead += 1;
				fullCount -= 8;
			}
			int valueCount = 0;
			for (int i = 0; i <= bytesToRead - 1; i++) {
				byte readByte = br.ReadByte();
				int byteCount = 0;
				while (byteCount < 8 & valueCount < IniBinData.Values.Count) {
					IniBinData.[ValuesvalueCount].Value = Convert.ToBoolean(readByte % 2);
					readByte = Conversion.Int(readByte / 2);
					valueCount += 1;
					byteCount += 1;
				}
			}
		}

		private bool HasValueFromType(ushort Type)
		{
			if ((this.ValueTypes & Type) == Type) {
				return true;
			}
			return false;
		}

	}

	public class IniBinData
	{
		public TroyiniFile.ValueType Type;
		public List<IniBinValue> Values = new List<IniBinValue>();
		public IniBinData(TroyiniFile.ValueType Type)
		{
			this.Type = Type;
		}
	}

	public class IniBinValue
	{
		public IniBinData Parent;
		public object Value;
		public TroyiniFile.InibinProp Prop;
		public IniBinValue(UInt32 Key, ref IniBinData parent)
		{
			this.Parent = parent;
			this.Prop = new TroyiniFile.InibinProp(Key);
		}
		public IniBinValue(UInt32 Key, ref IniBinData parent, ref object value)
		{
			this.Parent = parent;
			this.Prop = new TroyiniFile.InibinProp(Key);
			this.Value = value;
		}
	}
}*/
