using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre.Code.Files
{
    class SCOFile
    {
        public string FaceInput;
        public string[] FaceInputArr;
        public UInt32 FaceCount;
        public StreamReader sr;
        public Header header;
        public List<Vertex> Vertices = new List<Vertex>();
        public List<Face> Faces = new List<Face>();
        public SCOFile(string fileLocation)
        {
            sr = new StreamReader(File.Open(fileLocation, FileMode.Open));
            header = new Header(sr);
            for(int i = 0; i < Convert.ToInt32(header.VertCount[0]); i++)
            {
                Vertices.Add(new Vertex(sr));
            }
            FaceInput = sr.ReadLine();
            FaceInputArr = FaceInput.Split(' ');
            FaceCount = Convert.ToUInt32(FaceInputArr[1]);
            for(int i = 0; i < FaceCount; i++)
            {
                Faces.Add(new Face(sr));
            }
        }
        public class Header
        {
            public string ObjectBegin;
            public string Material;
            public string CentralPoint;
            public string PivotPoint;
            public string Verts;
            public string[] VertCount;
            public Header(StreamReader sr)
            {
                ObjectBegin = sr.ReadLine();
                Material = sr.ReadLine();
                CentralPoint = sr.ReadLine();
                PivotPoint = sr.ReadLine();
                Verts = sr.ReadLine();
                VertCount = Verts.Split(' ');
                VertCount = VertCount.Where(val => val != "Verts=").ToArray();
            }
        }
        public class Vertex
        {
            public string[] Input;
            public string s;
            public float[] Coords = new float[3];
            public Vertex(StreamReader sr)
            {
                s = sr.ReadLine();
                Input = s.Split(' ');
                Coords[0] = Convert.ToSingle(Input[0]);
                Coords[1] = Convert.ToSingle(Input[1]);
                Coords[2] = Convert.ToSingle(Input[2]);
            }
        }
        public class Face
        {
            public string[] Input;
            public string s;
            public UInt16 IndCount;
            public UInt32[] Indices = new UInt32[3];
            public string Material;
            public float[] U = new float[3];
            public float[] V = new float[3];
            public Face(StreamReader sr)
            {
                s = sr.ReadLine();
                Input = s.Split(' ');
                IndCount = Convert.ToUInt16(Input[0]);
                Indices[0] = Convert.ToUInt32(Input[1]);
                Indices[1] = Convert.ToUInt32(Input[2]);
                Indices[2] = Convert.ToUInt32(Input[3]);
                Material = Input[4];
                U[0] = Convert.ToSingle(Input[5]);
                U[1] = Convert.ToSingle(Input[6]);
                U[2] = Convert.ToSingle(Input[7]);
                V[0] = Convert.ToSingle(Input[8]);
                V[1] = Convert.ToSingle(Input[9]);
                V[2] = Convert.ToSingle(Input[10]);
            }
        }
    }
}
