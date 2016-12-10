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
        public List<Face> Faces = new List<Face>();
        public UInt32[] Nulls = new UInt32[3];
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
                if (header.IsUnknownPresent == 1)
                {
                    br.ReadBytes((int)header.NumberOfVertices * 4);
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
            public UInt32 Two; //?
            public float[] Min = new float[3];
            public float[] Max = new float[3];
            public UInt32 IsUnknownPresent;
            public UInt32 IsColored; //v2
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
                    Two = br.ReadUInt32();
                    for (int i = 0; i < 3; i++)
                    {
                        Min[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Max[i] = br.ReadSingle();
                    }
                    IsUnknownPresent = br.ReadUInt32();
                }
                else if(Major == 2 && Minor == 2)
                {
                    NumberOfVertices = br.ReadUInt32();
                    NumberOfFaces = br.ReadUInt32();
                    IsColored = br.ReadUInt32();
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
                for(int i = 0; i < 3; i++) //12
                {
                    Indices[i] = br.ReadUInt32();
                }
                Name = Encoding.ASCII.GetString(br.ReadBytes(64));
                for (int i = 0; i < 3; i++) //12
                {
                    U[i] = br.ReadSingle();
                }
                for (int i = 0; i < 3; i++) //12
                {
                    V[i] = br.ReadSingle();
                }
            }
        }
    }
}
