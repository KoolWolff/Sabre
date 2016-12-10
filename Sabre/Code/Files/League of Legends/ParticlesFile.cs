using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class ParticlesFile
    {
        public StreamReader sr;
        public List<Particle> Particles = new List<Particle>();
        public int numLine;
        public ParticlesFile(string fileLocation)
        {
            numLine = File.ReadAllLines(fileLocation).Length;
            sr = new StreamReader(File.Open(fileLocation, FileMode.Open));
            for(int i = 0; i < numLine; i++)
            {
                Particles.Add(new Particle(sr));
            }
        }
        public class Particle
        {
            public string input;
            public string[] particle;
            public string Name;
            public float[] Position = new float[3];
            public int MaxVal;
            public float[] Rotation = new float[3];
            public List<string> Flags = new List<string>();
            public Particle(StreamReader sr)
            {
                input = sr.ReadLine();
                particle = input.Split(' ');
                Name = particle[0];
                Position[0] = Convert.ToSingle(particle[1]);
                Position[1] = Convert.ToSingle(particle[2]);
                Position[2] = Convert.ToSingle(particle[3]);
                MaxVal = Convert.ToInt32(particle[4]);
                Rotation[0] = Convert.ToSingle(particle[5]);
                Rotation[1] = Convert.ToSingle(particle[6]);
                Rotation[2] = Convert.ToSingle(particle[7]);
                for(int i = particle.Length, j = 0; i < (particle.Length - 8); i++, j++)
                {
                    Flags[j] = particle[i];
                }
            }
        }
    }
}
