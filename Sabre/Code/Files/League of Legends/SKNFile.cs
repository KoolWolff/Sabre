using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class SKNFile
    {
        public UInt32 Zero;
        public BinaryReader br;
        public Header header;
        public Metadata boundingbox;
        public List<Material> Materials = new List<Material>();
        public List<UInt16> Indices = new List<UInt16>();
        public List<Vertex> Vertices = new List<Vertex>();
        public UInt32 IndCount;
        public UInt32 VertCount;
        public SKNFile(string fileLocation)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            header = new Header(br);
            for(int i = 0; i < header.NumOfMaterials; i++)
            {
                Materials.Add(new Material(br));
            }
            if(header.Version == 4)
            {
                Zero = br.ReadUInt32();
            }
            IndCount = br.ReadUInt32();
            VertCount = br.ReadUInt32();
            if(header.Version == 4)
            {
                boundingbox = new Metadata(br);
                for (int i = 0; i < IndCount; i++)
                {
                    Indices.Add(br.ReadUInt16());
                }
                for (int i = 0; i < VertCount; i++)
                {
                    Vertices.Add(new Vertex(br, (int)boundingbox.AdditionalsCount));
                }
            }
            else if (header.Version == 2)
            {
                for (int i = 0; i < IndCount; i++)
                {
                    Indices.Add(br.ReadUInt16());
                }
                for (int i = 0; i < VertCount; i++)
                {
                    Vertices.Add(new Vertex(br));
                }
            }
        }
        public class Header
        {
            public byte[] Magic;
            public UInt16 Version;
            public UInt16 NumOfObjects;
            public UInt32 NumOfMaterials;
            public Header(BinaryReader br)
            {
                Magic = br.ReadBytes(4);
                Version = br.ReadUInt16();
                NumOfObjects = br.ReadUInt16();
                NumOfMaterials = br.ReadUInt32();
            }
        }
        public class Material
        {
            public string Name;
            public UInt32 StartVertex;
            public UInt32 NumOfVertices;
            public UInt32 StartIndex;
            public UInt32 NumOfIndices;
            public Material(BinaryReader br)
            {
                Name = Encoding.ASCII.GetString(br.ReadBytes(64));
                StartVertex = br.ReadUInt32();
                NumOfVertices = br.ReadUInt32();
                StartIndex = br.ReadUInt32();
                NumOfIndices = br.ReadUInt32();
            }
        }
        public class Metadata
        {
            public UInt32 VertexSize;
            public UInt32 AdditionalsCount;
            public float MinX;
            public float MinY;
            public float MinZ;
            public float MaxX;
            public float MaxY;
            public float MaxZ;
            public float Radius;
            public Metadata(BinaryReader br)
            {
                VertexSize = br.ReadUInt32();
                AdditionalsCount = br.ReadUInt32();
                MinX = br.ReadSingle();
                MinY = br.ReadSingle();
                MinZ = br.ReadSingle();
                MaxX = br.ReadSingle();
                MaxY = br.ReadSingle();
                MaxZ = br.ReadSingle();
                Radius = br.ReadSingle();
            }
        }
        public class Vertex
        {
            public float[] Position = new float[3];
			public byte[] BoneIDs;
			public float[] Weights = new float[4];
			public float[] Normal = new float[3];
			public float[] UV = new float[2];
            public List<Int32> Additionals = new List<Int32>();
            public Vertex(BinaryReader br, int AdditionalCount)
            {
                for(int i = 0; i < 3; i++)
                {
                    Position[i] = br.ReadSingle();
                }
                BoneIDs = br.ReadBytes(4);
                for(int i = 0; i < 2; i++) 
                {
                    Weights[i] = br.ReadSingle();
                }
                for(int i = 0; i < 3; i++)
                {
                    Normal[i] = br.ReadSingle();
                }
                for(int i = 0; i < 2; i++)
                {
                    UV[i] = br.ReadSingle();
                }
                for(int i = 0; i < AdditionalCount; i++)
                {
                    Additionals.Add(br.ReadInt32());
                }
            }
            public Vertex(BinaryReader br)
            {
                for (int i = 0; i < 3; i++)
                {
                    Position[i] = br.ReadSingle();
                }
                BoneIDs = br.ReadBytes(4);
                for (int i = 0; i < 2; i++)
                {
                    Weights[i] = br.ReadSingle();
                }
                for (int i = 0; i < 3; i++)
                {
                    Normal[i] = br.ReadSingle();
                }
                for (int i = 0; i < 2; i++)
                {
                    UV[i] = br.ReadSingle();
                }
            }
        }
    }
}
