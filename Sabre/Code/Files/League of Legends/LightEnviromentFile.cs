using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows;

namespace Sabre
{
    class LightEnvironmentFile
    {
        private UInt32 Version;
        public ObservableCollection<Light> Lights = new ObservableCollection<Light>();
        public LightEnvironmentFile(string fileLocation)
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
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public Vector3 Unknown;
            public LightFlag Flag;
            public float Radius { get; set; }
            public LightType Type { get; set; }
            public float Strength { get; set; }
            public Light(StreamReader sr)
            {
                string[] Line = sr.ReadLine().Split(' ');
                X = float.Parse(Line[0]);
                Y = float.Parse(Line[1]);
                Z = float.Parse(Line[2]);

                R = byte.Parse(Line[3]);
                G = byte.Parse(Line[4]);
                B = byte.Parse(Line[5]);
                Unknown = new Vector3(float.Parse(Line[6]), float.Parse(Line[7]), float.Parse(Line[8]));
                Flag = (LightFlag)UInt32.Parse(Line[9]);
                Radius = float.Parse(Line[10]);
                Type = (LightType)UInt32.Parse(Line[11]);
                Strength = float.Parse(Line[12], CultureInfo.InvariantCulture.NumberFormat);
            }
            public Light()
            {
                X = 0;
                Y = 0;
                Z = 0;

                R = 0;
                G = 0;
                B = 0;
                Unknown = new Vector3(0, 0, 0);
                Flag = (LightFlag)20;
                Radius = 0;
                Type = LightType.R3D_OMNI_LIGHT;
                Strength = 0;
            }
            public void Write(StreamWriter sw)
            {
                sw.Write(X + " ");
                sw.Write(Y + " ");
                sw.Write(Z + " ");
                sw.Write(R + " ");
                sw.Write(G + " ");
                sw.Write(B + " ");
                Unknown.Write(sw);
                sw.Write((UInt32)Flag + " ");
                sw.Write(Radius + " ");
                sw.Write((UInt32)Type + " ");
                sw.Write(Strength + " " + Environment.NewLine);
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
        public void Write()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "Select the path where you want to save your Lights_Env.dat file";
            sfd.Filter = "DAT File | *.dat";
            sfd.DefaultExt = "dat";
            if(sfd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(File.Open(sfd.FileName, FileMode.OpenOrCreate)))
                {
                    sw.Write(3 + Environment.NewLine);
                    foreach (Light light in Lights)
                    {
                        light.Write(sw);
                    }
                }
            }
        }
        public void AddLight()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { Lights.Add(new Light()); }));
        }
        public void RemoveLight(System.Collections.IList selectedEntries)
        {
            foreach (Light light in selectedEntries)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { Lights.Remove(light); }));
            }
        }
    }
}
