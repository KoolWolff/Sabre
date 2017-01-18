using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class SCBFile 
    {
        public BinaryReader br;
        public Header header;
        public List<Vertex> Vertices = new List<Vertex>();
        public List<byte[]> Tangents = new List<byte[]>();
        public List<Face> Faces = new List<Face>();
        public List<Color> Colors = new List<Color>();
        public SCBFile(string fileLocation)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            header = new Header(br);
            if(header.Major == 3 && header.Minor == 2)
            {
                for (int i = 0; i < header.NumberOfVertices; i++)
                {
                    Vertices.Add(new Vertex(br));
                }
                if (header.HasTangent == 1)
                {
                    for(int i = 0; i < header.NumberOfVertices; i++)
                    {
                        Tangents.Add(br.ReadBytes(4));
                    }
                    br.ReadBytes(12);
                }
                else
                {
                    br.ReadBytes(12);
                }
                for (int i = 0; i < header.NumberOfFaces; i++)
                {
                    Faces.Add(new Face(br));
                }
                if (header.HasVCP == 1)
                {
                    for (int i = 0; i < header.NumberOfFaces; i++)
                    {
                        Colors.Add(new Color(br, false));
                    }
                }
            }   
            else if(header.Major == 2 && header.Minor == 2)
            {
                for (int i = 0; i < header.NumberOfVertices + 1; i++)
                {
                    Vertices.Add(new Vertex(br));
                }
                for (int i = 0; i < header.NumberOfFaces; i++)
                {
                    Faces.Add(new Face(br));
                }
                if(header.HasVCP == 1)
                {
                    for (int i = 0; i < header.NumberOfFaces; i++)
                    {
                        Colors.Add(new Color(br, false));
                    }
                }
            }
        }
        public class Header
        {
            public string Magic;
            public UInt16 Major;
            public UInt16 Minor;
            public string Name;
            public UInt32 NumberOfVertices;
            public UInt32 NumberOfFaces;
            public UInt32 HasVCP; //Virtual Color Projection ????
            public float[] Min = new float[3];
            public float[] Max = new float[3];
            public UInt32 HasTangent;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(8));
                Major = br.ReadUInt16();
                Minor = br.ReadUInt16();
                Name = Encoding.ASCII.GetString(br.ReadBytes(128));
                if(Major == 3 && Minor == 2)
                {
                    NumberOfVertices = br.ReadUInt32();
                    NumberOfFaces = br.ReadUInt32();
                    HasVCP = br.ReadUInt32();
                    for (int i = 0; i < 3; i++)
                    {
                        Min[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Max[i] = br.ReadSingle();
                    }
                    HasTangent = br.ReadUInt32();
                }
                else if(Major == 2 && Minor == 2)
                {
                    NumberOfVertices = br.ReadUInt32();
                    NumberOfFaces = br.ReadUInt32();
                    HasVCP = br.ReadUInt32();
                    for (int i = 0; i < 3; i++)
                    {
                        Min[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Max[i] = br.ReadSingle();
                    }
                }
            }
        }
        public class Vertex
        {
            public float[] Position = new float[3];
            public Vertex(BinaryReader br)
            {
                for(int i = 0; i < 3; i++)
                {
                    Position[i] = br.ReadSingle();
                }
            }
        }
        public class Face
        {
            public UInt32[] Indices = new UInt32[3];
            public string Name;
            public float[] U = new float[3];
            public float[] V = new float[3];
            public Face(BinaryReader br)
            {
                for(int i = 0; i < 3; i++) 
                {
                    Indices[i] = br.ReadUInt32();
                }
                Name = Encoding.ASCII.GetString(br.ReadBytes(64));
                for (int i = 0; i < 3; i++) 
                {
                    U[i] = br.ReadSingle();
                    V[i] = br.ReadSingle();
                }
            }
        }
        public class Color
        {
            public byte R, G, B, A;
            public Color(BinaryReader br, bool readA)
            {
                R = br.ReadByte();
                G = br.ReadByte();
                B = br.ReadByte();
                if (readA) A = br.ReadByte();
            }
        }
    }
}
