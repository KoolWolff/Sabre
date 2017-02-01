using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace Sabre
{
    class LightFile
    {
        public ObservableCollection<Light> Lights = new ObservableCollection<Light>();
        public LightFile(string fileLocation)
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(fileLocation)))
            {
                while(sr.BaseStream.Position != sr.BaseStream.Length)
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
            public float Radius { get; set; }
            public Light(StreamReader sr)
            {
                string[] Line = sr.ReadLine().Split(' ');
                X = float.Parse(Line[0]);
                Y = float.Parse(Line[1]);
                Z = float.Parse(Line[2]);
                R = byte.Parse(Line[3]);
                G = byte.Parse(Line[4]);
                B = byte.Parse(Line[5]);
                Radius = float.Parse(Line[6]);
            }
            public Light()
            {
                X = 0;
                Y = 0;
                Z = 0;
                R = 0;
                G = 0;
                B = 0;
                Radius = 0;
            }
            public void Write(StreamWriter sw)
            {
                sw.Write(X + " ");
                sw.Write(Y + " ");
                sw.Write(Z + " ");
                sw.Write(R + " ");
                sw.Write(G + " ");
                sw.Write(B + " ");
                sw.Write(Radius + Environment.NewLine);
            }
        }
        public void Write()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "Select the path where you want to save your Lights.dat file";
            sfd.Filter = "DAT File | *.dat";
            sfd.DefaultExt = "dar";
            if(sfd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(File.Open(sfd.FileName, FileMode.OpenOrCreate)))
                {
                    foreach (Light light in Lights)
                    {
                        light.Write(sw);
                    }
                }
            }
        }
    }
}
