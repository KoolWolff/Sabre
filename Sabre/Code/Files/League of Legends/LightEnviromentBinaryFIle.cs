using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class LightEnviromentBinaryFile
    {
        public Header header;
        public List<Color> Colors = new List<Color>();
        public LightEnviromentBinaryFile(string fileLocation)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(fileLocation)))
            {
                header = new Header(br);
                while(br.BaseStream.Position != br.BaseStream.Length)
                {
                    Colors.Add(new Color(br));
                }
            }
        }
        public class Header
        {
            public UInt32 Version;
            public UInt32 ColorsOffset;
            public UInt32 Width; //256
            public UInt32 Heigth; //256
            public Sun Sun;
            public Header(BinaryReader br)
            {
                Version = br.ReadUInt32();
                ColorsOffset = br.ReadUInt32();
                Width = br.ReadUInt32();
                Heigth = br.ReadUInt32();
                Sun = new Sun(br);
            }
            public void Write(BinaryWriter bw)
            {
                bw.Write(Version);
                bw.Write(ColorsOffset);
                bw.Write(Width);
                bw.Write(Heigth);
                Sun.Write(bw);
            }
        }
        public class Color
        {
            public byte R, G, B, A;
            public Color(BinaryReader br)
            {
                R = br.ReadByte();
                G = br.ReadByte();
                B = br.ReadByte();
                A = br.ReadByte();
            }
            public void Write(BinaryWriter bw)
            {
                bw.Write(R);
                bw.Write(G);
                bw.Write(B);
                bw.Write(A);
            }
        }
        public class Sun
        {
            public float PositionX, PositionY;
            public float OpcaityOfLightOnCharacters;
            public float Unk1;
            public float Unk2;
            public float Unk3;
            public float Unk4;
            public float Unk5;
            public float Unk6;
            public float Unk7;
            public float Unk8;
            public float Unk9;
            public float Unk10;
            public float Unk11;
            public float Unk12;
            public Sun(BinaryReader br)
            {
                PositionX = br.ReadSingle();
                PositionY = br.ReadSingle();
                OpcaityOfLightOnCharacters = br.ReadSingle();
                Unk1 = br.ReadSingle();
                Unk2 = br.ReadSingle();
                Unk3 = br.ReadSingle();
                Unk4 = br.ReadSingle();
                Unk5 = br.ReadSingle();
                Unk6 = br.ReadSingle();
                Unk7 = br.ReadSingle();
                Unk8 = br.ReadSingle();
                Unk9 = br.ReadSingle();
                Unk10 = br.ReadSingle();
                Unk12 = br.ReadSingle();
                Unk11 = br.ReadSingle();
            }
            public void Write(BinaryWriter bw)
            {
                bw.Write(PositionX);
                bw.Write(PositionY);
                bw.Write(OpcaityOfLightOnCharacters);
                bw.Write(Unk1);
                bw.Write(Unk2);
                bw.Write(Unk3);
                bw.Write(Unk4);
                bw.Write(Unk5);
                bw.Write(Unk6);
                bw.Write(Unk7);
                bw.Write(Unk8);
                bw.Write(Unk9);
                bw.Write(Unk10);
                bw.Write(Unk11);
                bw.Write(Unk12);
            }
        }
        public void Write(string fileLocation)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(fileLocation, FileMode.OpenOrCreate)))
            {
                header.Write(bw);
                foreach(Color c in Colors)
                {
                    c.Write(bw);
                }
            }
        }
        public void ToImage(string fileLocation)
        {
            byte[] Pixels = new byte[Colors.Count * 4];

            UInt32 Offset = 0;
            for (int i = 0; i < Colors.Count; i++)
            {
                Pixels[Offset] = Colors[i].R;
                Pixels[Offset + 1] = Colors[i].G;
                Pixels[Offset + 2] = Colors[i].B;
                Pixels[Offset + 3] = Colors[i].A;

                Offset += 4;
            }

            Byte[] Header = new Byte[]
            {
                0, // ID length
                0, // no color map
                2, // uncompressed, true color
                0, 0, 0, 0,
                0,
                0, 0, 0, 0, // x and y origin
                (Byte)(header.Width & 0x00FF),
                (Byte)((header.Width & 0xFF00) >> 8),
                (Byte)(header.Heigth & 0x00FF),
                (Byte)((header.Heigth & 0xFF00) >> 8),
                32, // 32 bit bitmap
                0
            };

            using (BinaryWriter bw = new BinaryWriter(File.Open(fileLocation + ".tga", FileMode.Create)))
            {
                bw.Write(Header);
                bw.Write(Pixels);
            }
        }
    }
}
