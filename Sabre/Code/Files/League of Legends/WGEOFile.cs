using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class WGEOFile
    {
        public BinaryReader br;
        public Header header;
        public ParticleGeometry partGeo;
        public List<Model> Models = new List<Model>();
        public WGEOFile(string fileLocation, bool readParticleGeometry = false) 
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            header = new Header(br);
            for(int i = 0; i < header.ModelCount; i++)
            {
                Models.Add(new Model(br));
            }
            if(readParticleGeometry)
            {
                partGeo = new ParticleGeometry(br);
            }
        }
        public class Header
        {
            public string Magic;
            public UInt32 Version;
            public UInt32 ModelCount;
            public UInt32 FaceCount;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                Version = br.ReadUInt32();
                ModelCount = br.ReadUInt32();
                FaceCount = br.ReadUInt32();
            }
        }
        public class Model
        {
            public string TextureName;
            public string MaterialName;
            public float[] Sphere = new float[4]; //Vec3 - Pos | Sphere Radius 
            public float[] Min = new float[3];
            public float[] Max = new float[3];
            public UInt32 VertCount;
            public UInt32 IndCount;
            public List<Vertex> Vertices = new List<Vertex>();
            public List<UInt16> Indices = new List<UInt16>();
            public Model(BinaryReader br)
            {
                TextureName = Encoding.ASCII.GetString(br.ReadBytes(260));
                MaterialName = Encoding.ASCII.GetString(br.ReadBytes(64));
                for(int i = 0; i < 4; i++)
                {
                    Sphere[i] = br.ReadSingle();
                }
                for(int i = 0; i < 3; i++)
                {
                    Min[i] = br.ReadSingle();
                }
                for(int i = 0; i < 3; i++)
                {
                    Max[i] = br.ReadSingle();
                }
                VertCount = br.ReadUInt32();
                IndCount = br.ReadUInt32();
                for(int i = 0; i < VertCount; i++)
                {
                    Vertices.Add(new Vertex(br));
                }
                for(int i = 0; i < IndCount; i++)
                {
                    Indices.Add(br.ReadUInt16());
                }
            }
        }
        public class Vertex
        {
            public float[] Position = new float[3];
            public float[] UV = new float[2];
            public Vertex(BinaryReader br)
            {
                for(int i = 0; i < 3; i++)
                {
                    Position[i] = br.ReadSingle();
                }
                for(int i = 0; i < 2; i++)
                {
                    UV[i] = br.ReadSingle();
                }
            }
        }
        public class ParticleGeometry
        {
            public float MinX;
            public float MinZ;
            public float MaxX;
            public float MaxZ;
            public float CenterX;
            public float CenterZ;
            public float MinY;
            public float MaxY;
            public UInt32 BucketsPerSideCount;
            public UInt32 VectorCount;
            public UInt32 IndiceCount;
            public List<float[]> Vectors = new List<float[]>();
            public List<ParticleVector> ParticleVectors = new List<ParticleVector>();
            public List<UInt16> Indices = new List<UInt16>();
            public ParticleGeometry(BinaryReader br)
            {
                MinX = br.ReadSingle();
                MinZ = br.ReadSingle();
                MaxX = br.ReadSingle();
                MaxZ = br.ReadSingle();
                CenterX = br.ReadSingle();
                CenterZ = br.ReadSingle();
                MinY = br.ReadSingle();
                MaxY = br.ReadSingle();
                BucketsPerSideCount = br.ReadUInt32();
                VectorCount = br.ReadUInt32();
                IndiceCount = br.ReadUInt32();
                for(int i = 0; i < VectorCount; i++)
                {
                    Vectors.Add(new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle() });
                }
                for(int i = 0; i < IndiceCount; i++)
                {
                    Indices.Add(br.ReadUInt16());
                }
                while(br.BaseStream.Position < br.BaseStream.Length)
                {
                    ParticleVectors.Add(new ParticleVector(br));
                }
            }
        }
        public class ParticleVector
        {
            public float maxOutStickX;
            public float maxOutStickZ;
            public UInt32 startIndex;
            public UInt32 baseVertex;
            public UInt16 insideTrinagleCount;
            public UInt16 stickingOutTrinagleCount;
            public ParticleVector(BinaryReader br)
            {
                maxOutStickX = br.ReadSingle();
                maxOutStickZ = br.ReadSingle();
                startIndex = br.ReadUInt32();
                baseVertex = br.ReadUInt32();
                insideTrinagleCount = br.ReadUInt16();
                stickingOutTrinagleCount = br.ReadUInt16();
            }
        }
    }
}
