using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre.Code.Files
{
    class NVRFile
    {
        public BinaryReader br;
        public NVRFile(string fileLocation)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
        }
        public class Header
        {
            public string Magic;
            public UInt16 MajorVersion;
            public UInt16 MinorVersion;
            public UInt32 Num1;
            public UInt32 Num2;
            public UInt32 Num3;
            public UInt32 NumberOfObjects;
            public UInt32 NumberOfNodes;
            public Header(BinaryReader br)
            {

            }
        }
        public class Shader
        {
            public string MaterialName; //264
            public UInt32 ID; //?
            public float[] Unk = new float[4];
            public string TextureName; //336
            public float Unk1;
            public Shader(BinaryReader br)
            {

            }
        }
        public class Vertex
        {
            public Vertex(BinaryReader br)
            {

            }
        }
        public class Node
        {
            public Node(BinaryReader br)
            {

            }
        }
    }
}
