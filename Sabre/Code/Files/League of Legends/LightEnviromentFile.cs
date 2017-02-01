using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Sabre
{
    class LightEnviromentFile
    {
        private UInt32 Version;
        public List<Light> Lights = new List<Light>();
        public LightEnviromentFile(string fileLocation)
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(fileLocation)))
            {
                Version = UInt32.Parse(sr.ReadLine());
                while (sr.BaseStream.Position != sr.BaseStream.Length)
                {
                    Lights.Add(new Light(sr));
                }
            }
        }
        public class Light
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Unknown;
            public LightFlag Flag;
            public float Radius;
            public LightType Type;
            public float Opacity;
            public Light(StreamReader sr)
            {
                string[] Line = sr.ReadLine().Split(' ');
                Position = new Vector3(float.Parse(Line[0]), float.Parse(Line[1]), float.Parse(Line[2]));
                Color = new Color(byte.Parse(Line[3]), byte.Parse(Line[4]), byte.Parse(Line[5]));
                Unknown = new Vector3(float.Parse(Line[6]), float.Parse(Line[7]), float.Parse(Line[8]));
                Flag = (LightFlag)UInt32.Parse(Line[9]);
                Radius = float.Parse(Line[10]);
                Type = (LightType)UInt32.Parse(Line[11]);
                Opacity = float.Parse(Line[12], CultureInfo.InvariantCulture.NumberFormat);
            }
            public void Write(StreamWriter sw)
            {
                Position.Write(sw);
                Color.Write(sw);
                Unknown.Write(sw);
                sw.Write((UInt32)Flag + " ");
                sw.Write(Radius + " ");
                sw.Write((UInt32)Type + " ");
                sw.Write(Opacity + " " + Environment.NewLine);
            }
        }
        public class Vector3
        {
            public float X, Y, Z;
            public Vector3(float X, float Y, float Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }
            public void Write(StreamWriter sw)
            {
                sw.Write(X + " ");
                sw.Write(Y + " ");
                sw.Write(Z + " ");
            }
        }
        public class Color
        {
            public byte R, G, B;
            public Color(byte R, byte G, byte B)
            {
                this.R = R;
                this.G = G;
                this.B = B;
            }
            public void Write(StreamWriter sw)
            {
                sw.Write(R + " ");
                sw.Write(G + " ");
                sw.Write(B + " ");
            }
        }
        public enum LightFlag : UInt32
        {
            R3D_LIGHT_ON = 2,
            R3D_LIGHT_STATIC = 4,
            R3D_LIGHT_DYNAMIC = 8,
            R3D_LIGHT_HEAP = 16,
            R3D_LIGHT_AUTOREMOVE = 32,
            R3D_LIGHT_ALWAYSVISIBLE = 64
        }
        public enum LightType : UInt32
        {
            R3D_OMNI_LIGHT = 0,
            R3D_DIRECT_LIGHT = 1,
            R3D_SPOT_LIGHT = 2,
            R3D_PROJECTOR_LIGHT = 3,
            R3D_CUBE_LIGHT = 4
        }
        public void Write(string fileLocation)
        {
            using (StreamWriter sw = new StreamWriter(File.Open(fileLocation, FileMode.OpenOrCreate)))
            {
                sw.Write(3 + Environment.NewLine);
                foreach(Light light in Lights)
                {
                    light.Write(sw);
                }
            }
        }
    }
}
