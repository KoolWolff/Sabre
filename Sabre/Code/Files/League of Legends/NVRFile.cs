using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class NVRFile
    {
        public BinaryReader br;
        public Header header;
        public List<Material> Materials = new List<Material>();
        public List<VertexBuffer> VertexBuffers = new List<VertexBuffer>();
        public List<IndexBuffer> IndexBuffers = new List<IndexBuffer>();
        public List<Mesh> Meshes = new List<Mesh>();
        public List<Node> Nodes = new List<Node>();
        public NVRFile(string fileLocation)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));

            header = new Header(br);
            for(int i = 0; i < header.MaterialCount; i++)
            {
                Materials.Add(new Material(br));
            }
            for (int i = 0; i < header.VertexBufferCount; i++)
            {
                VertexBuffers.Add(new VertexBuffer(br));
            }
            for (int i = 0; i < header.IndexBufferCount; i++)
            {
                IndexBuffers.Add(new IndexBuffer(br));
            }
            for (int i = 0; i < header.MeshCount; i++)
            {
                Meshes.Add(new Mesh(br));
            }
            for (int i = 0; i < header.NodeCount; i++)
            {
                Nodes.Add(new Node(br));
            }
        }
        public class Header
        {
            public string Magic;
            public UInt16 MajorVersion;
            public UInt16 MinorVersion;
            public UInt32 MaterialCount;
            public UInt32 VertexBufferCount;
            public UInt32 IndexBufferCount;
            public UInt32 MeshCount;
            public UInt32 NodeCount;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                MajorVersion = br.ReadUInt16();
                MinorVersion = br.ReadUInt16();
                MaterialCount = br.ReadUInt32();
                VertexBufferCount = br.ReadUInt32();
                IndexBufferCount = br.ReadUInt32();
                MeshCount = br.ReadUInt32();
                NodeCount = br.ReadUInt32();
            }
        }
        public class IndexBuffer
        {
            public List<object> Indices = new List<object>();
            public UInt32 Length;
            public D3DFORMAT Format;
            public IndexBuffer(BinaryReader br)
            {
                Length = br.ReadUInt32();
                Format = (D3DFORMAT)br.ReadUInt32();
                for (int i = 0; i < Length; i++)
                {
                    if(Format == D3DFORMAT.D3DFMT_INDEX16)
                    {
                        Indices.Add(br.ReadUInt16());
                    }
                    else if (Format == D3DFORMAT.D3DFMT_INDEX32)
                    {
                        Indices.Add(br.ReadUInt32());
                    }
                    else
                    {
                        br.BaseStream.Seek(br.BaseStream.Position + Length, SeekOrigin.Begin);
                    }
                }
            }
        }
        public class VertexBuffer
        {
            public List<Vertex> Vertices = new List<Vertex>();
            public VertexFormat Format;
            public UInt32 Length;
            public VertexBuffer(BinaryReader br)
            {
                Length = br.ReadUInt32();
                Format = getVertexFormat(this);
                if(Format == VertexFormat.unknown)
                {
                    br.BaseStream.Seek(br.BaseStream.Position + Length, SeekOrigin.Begin);
                }
                else
                {
                    for(int i = 0; i < Length; i++)
                    {
                        Vertices.Add(new Vertex(br, Format));
                    }
                }
            }
        }
        public class Material
        {
            public string Name;
            public UInt32 Unk;
            public MaterialType MaterialType1;
            public Color DiffuseColor;
            public string DiffuseTexture;
            public Color EmissiveColor;
            public string EmissiveTexture;
            public string _Name;
            public MaterialType MaterialType2;
            public UInt32 Flags;
            public Channel[] Channels = new Channel[8];
            public Material(BinaryReader br)
            {
                Name = Encoding.ASCII.GetString(br.ReadBytes(260));
                MaterialType1 = (MaterialType)br.ReadUInt32();
                DiffuseColor = new Color(br);
                DiffuseTexture = Encoding.ASCII.GetString(br.ReadBytes(260));
                EmissiveColor = new Color(br);
                EmissiveTexture = Encoding.ASCII.GetString(br.ReadBytes(260));
                _Name = Encoding.ASCII.GetString(br.ReadBytes(260));
                MaterialType2 = (MaterialType)br.ReadUInt32();
                Flags = br.ReadUInt32();
                for(int i = 0; i < 8; i++)
                {
                    Channels[i] = new Channel(br);
                }
            }
        }
        public class Mesh
        {
            public Quality QualityLevel;
            public Sphere BoundingSphere;
            public Box BoundingBox;
            public UInt32 Material;
            public IndexedPrimitive[] IndexedPrimitives = new IndexedPrimitive[2];
            public Mesh(BinaryReader br)
            {
                QualityLevel = (Quality)br.ReadUInt32();
                BoundingSphere = new Sphere(br);
                BoundingBox = new Box(br);
                Material = br.ReadUInt32();
                IndexedPrimitives[0] = new IndexedPrimitive(br);
                IndexedPrimitives[1] = new IndexedPrimitive(br);
            }
        }
        public class Channel
        {
            public Color ColorValue;
            public string Texture;
            public D3DMatrix Matrix;
            public Channel(BinaryReader br)
            {
                ColorValue = new Color(br);
                Texture = Encoding.ASCII.GetString(br.ReadBytes(260));
                Matrix = new D3DMatrix(br);
            }
        }
        public class Vertex
        {
            public VertexFormat Format;
            public float[] Position = new float[3];
            public float[] Normal = new float[3];
            public float[] UV = new float[2];
            public byte[] Tangent = new byte[4];
            public byte[] BiTangent = new byte[4];
            public Vertex(BinaryReader br, VertexFormat format)
            {
                Format = format;
                if(format == VertexFormat.Basic)
                {
                    for(int i = 0; i < 3; i++)
                    {
                        Position[i] = br.ReadSingle();
                    }
                }
                if (format == VertexFormat.complexTangent)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Position[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Normal[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        UV[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Tangent[i] = br.ReadByte();
                    }
                }
                if (format == VertexFormat.complexBiTangent)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Position[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Normal[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        UV[i] = br.ReadSingle();
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Tangent[i] = br.ReadByte();
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        BiTangent[i] = br.ReadByte();
                    }
                }
            }
        }
        public class Node
        {
            public Box BoundingBox;
            public UInt32 FirstMesh;
            public UInt32 MeshCount;
            public UInt32 FirstChildNode;
            public UInt32 ChildNodeCount;
            public Node(BinaryReader br)
            {
                BoundingBox = new Box(br);
                FirstMesh = br.ReadUInt32();
                MeshCount = br.ReadUInt32();
                FirstChildNode = br.ReadUInt32();
                ChildNodeCount = br.ReadUInt32();
            }
        }

        public class Vector
        {
            public float x, y, z;
            public Vector(BinaryReader br)
            {
                x = br.ReadSingle();
                y = br.ReadSingle();
                z = br.ReadSingle();
            }
            public Vector(float x, float z, float y)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
        public class Box
        {
            public Vector Min;
            public Vector Max;
            public Box(BinaryReader br)
            {
                Min = new Vector(br);
                Max = new Vector(br);
            }
            public Box(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
            {
                Min.x = minX;
                Min.y = minY;
                Min.z = minZ;
                Max.x = maxX;
                Max.y = maxY;
                Max.z = maxZ;
            }
        }
        public class Sphere
        {
            public Vector Center;
            public float Radius;
            public Sphere(BinaryReader br)
            {
                Center = new Vector(br);
                Radius = br.ReadSingle();
            }
            public Sphere(float x, float y, float z, float radius)
            {
                Center.x = x;
                Center.y = y;
                Center.z = z;
                Radius = radius;
            }
        }
        public class Color
        {
            public float r, g, b, a;
            public Color(BinaryReader br)
            {
                r = br.ReadSingle();
                g = br.ReadSingle();
                b = br.ReadSingle();
                a = br.ReadSingle();
            }
            public Color(float r, float g, float b, float a)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }
        }
        public class D3DMatrix
        {
            public PRSMATRIX Matrix = new PRSMATRIX();
            public D3DMatrix(BinaryReader br)
            {
                Matrix._11 = br.ReadSingle();
                Matrix._12 = br.ReadSingle();
                Matrix._13 = br.ReadSingle();
                Matrix._14 = br.ReadSingle();

                Matrix._21 = br.ReadSingle();
                Matrix._22 = br.ReadSingle();
                Matrix._23 = br.ReadSingle();
                Matrix._24 = br.ReadSingle();

                Matrix._31 = br.ReadSingle();
                Matrix._32 = br.ReadSingle();
                Matrix._33 = br.ReadSingle();
                Matrix._34 = br.ReadSingle();

                Matrix._41 = br.ReadSingle();
                Matrix._42 = br.ReadSingle();
                Matrix._43 = br.ReadSingle();
                Matrix._44 = br.ReadSingle();
            }
        }
        public class IndexedPrimitive
        {
            public UInt32 VertexBuffer;
            public UInt32 FirstVertex;
            public UInt32 VertexCount;
            public UInt32 IndexBuffer;
            public UInt32 FirstIndex;
            public UInt32 IndexCount;
            public IndexedPrimitive(BinaryReader br)
            {
                VertexBuffer = br.ReadUInt32();
                FirstVertex = br.ReadUInt32();
                VertexCount = br.ReadUInt32();
                IndexBuffer = br.ReadUInt32();
                FirstIndex = br.ReadUInt32();
                IndexCount = br.ReadUInt32();
            }
        }
        public struct PRSMATRIX
        {
            public float _11;
            public float _12;
            public float _13;
            public float _14;
            public float _21;
            public float _22;
            public float _23;
            public float _24;
            public float _31;
            public float _32;
            public float _33;
            public float _34;
            public float _41;
            public float _42;
            public float _43;
            public float _44;
        }
        public enum D3DFORMAT : UInt32
        {
            D3DFMT_UNKNOWN = 0x0,
            D3DFMT_R8G8B8 = 0x14,
            D3DFMT_A8R8G8B8 = 0x15,
            D3DFMT_X8R8G8B8 = 0x16,
            D3DFMT_R5G6B5 = 0x17,
            D3DFMT_X1R5G5B5 = 0x18,
            D3DFMT_A1R5G5B5 = 0x19,
            D3DFMT_A4R4G4B4 = 0x1A,
            D3DFMT_R3G3B2 = 0x1B,
            D3DFMT_A8 = 0x1C,
            D3DFMT_A8R3G3B2 = 0x1D,
            D3DFMT_X4R4G4B4 = 0x1E,
            D3DFMT_A2B10G10R10 = 0x1F,
            D3DFMT_A8B8G8R8 = 0x20,
            D3DFMT_X8B8G8R8 = 0x21,
            D3DFMT_G16R16 = 0x22,
            D3DFMT_A2R10G10B10 = 0x23,
            D3DFMT_A16B16G16R16 = 0x24,
            D3DFMT_A8P8 = 0x28,
            D3DFMT_P8 = 0x29,
            D3DFMT_L8 = 0x32,
            D3DFMT_A8L8 = 0x33,
            D3DFMT_A4L4 = 0x34,
            D3DFMT_V8U8 = 0x3C,
            D3DFMT_L6V5U5 = 0x3D,
            D3DFMT_X8L8V8U8 = 0x3E,
            D3DFMT_Q8W8V8U8 = 0x3F,
            D3DFMT_V16U16 = 0x40,
            D3DFMT_A2W10V10U10 = 0x43,
            D3DFMT_UYVY = 0x59565955,
            D3DFMT_R8G8_B8G8 = 0x47424752,
            D3DFMT_YUY2 = 0x32595559,
            D3DFMT_G8R8_G8B8 = 0x42475247,
            D3DFMT_DXT1 = 0x31545844,
            D3DFMT_DXT2 = 0x32545844,
            D3DFMT_DXT3 = 0x33545844,
            D3DFMT_DXT4 = 0x34545844,
            D3DFMT_DXT5 = 0x35545844,
            D3DFMT_D16_LOCKABLE = 0x46,
            D3DFMT_D32 = 0x47,
            D3DFMT_D15S1 = 0x49,
            D3DFMT_D24S8 = 0x4B,
            D3DFMT_D24X8 = 0x4D,
            D3DFMT_D24X4S4 = 0x4F,
            D3DFMT_D16 = 0x50,
            D3DFMT_D32F_LOCKABLE = 0x52,
            D3DFMT_D24FS8 = 0x53,
            D3DFMT_D32_LOCKABLE = 0x54,
            D3DFMT_S8_LOCKABLE = 0x55,
            D3DFMT_L16 = 0x51,
            D3DFMT_VERTEXDATA = 0x64,
            D3DFMT_INDEX16 = 0x65,
            D3DFMT_INDEX32 = 0x66,
            D3DFMT_Q16W16V16U16 = 0x6E,
            D3DFMT_MULTI2_ARGB8 = 0x3154454D,
            D3DFMT_R16F = 0x6F,
            D3DFMT_G16R16F = 0x70,
            D3DFMT_A16B16G16R16F = 0x71,
            D3DFMT_R32F = 0x72,
            D3DFMT_G32R32F = 0x73,
            D3DFMT_A32B32G32R32F = 0x74,
            D3DFMT_CxV8U8 = 0x75,
            D3DFMT_A1 = 0x76,
            D3DFMT_A2B10G10R10_XR_BIAS = 0x77,
            D3DFMT_BINARYBUFFER = 0xC7,
            D3DFMT_FORCE_DWORD = 0x7FFFFFFF,
        }
        public enum MaterialType
        {
            MATERIAL_TYPE_DEFAULT = 0x0,
            MATERIAL_TYPE_DECAL = 0x1,
            MATERIAL_TYPE_WALL_OF_GRASS = 0x2,
            MATERIAL_TYPE_FOUR_BLEND = 0x3,
            MATERIAL_TYPE_COUNT = 0x4,
            MATERIAL_TYPE_FORCE_DWORD = 0x7FFFFFFF
        };
        public enum Quality : UInt32
        {
            QUALITY_VERY_LOW = 0,
            QUALITY_LOW = 1,
            QUALITY_MEDIUM = 2,
            QUALITY_HIGH = 3,
            QUALITY_VERY_HIGH = 4,
            QUALITY_COUNT = 5
        }
        public enum VertexFormat
        {
            Basic,
            complexTangent,
            complexBiTangent,
            unknown
        }
        public static VertexFormat getVertexFormat(VertexBuffer buffer)
        {
            if(buffer.Length % 40 == 0)
            {
                return VertexFormat.complexBiTangent;
            }
            else if (buffer.Length % 36 == 0)
            {
                return VertexFormat.complexTangent;
            }
            else if (buffer.Length % 12 == 0)
            {
                return VertexFormat.Basic;
            }
            return VertexFormat.unknown;
        }
    }
}