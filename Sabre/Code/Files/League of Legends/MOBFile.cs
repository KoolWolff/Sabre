using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class MOBFile
    {
        public string Magic;
        public uint Version;
        public uint NumberOfObjects;
        public uint Zero;
        public List<MOBObject> MobObjects = new List<MOBObject>();
        public MOBFile(string fileLocation)
        {
            BinaryReader br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
            if(Magic != "OPAM")
            {
                throw new Exception("Invalid MOB File");
            }
            Version = br.ReadUInt32();
            NumberOfObjects = br.ReadUInt32();
            Zero = br.ReadUInt32();

            for(int i = 0; i < NumberOfObjects; i++)
            {
                MobObjects.Add(new MOBObject(br));
            }
        }
        public class MOBObject
        {
            public string MOBObjectName;
            public char[] MOBObjectNameChar;
            public uint ObjectZero1 = 0;
            public MapObjectType ObjectFlag;
            public byte ObjectZero2 = 0;
            public float[] Position = new float[3];
            public float[] Rotation = new float[3];
            public float[] Scale = new float[3];
            public float[] HealthBarPosition1 = new float[3];
            public float[] HealthBarPosition2 = new float[3];
            public uint ObjectZero3 = 0;
            public MOBObject(BinaryReader br)
            {
                MOBObjectName = Encoding.ASCII.GetString(br.ReadBytes(60));
                MOBObjectNameChar = GetCharsFromString(MOBObjectName, 60);
                MOBObjectName = GetStringFromChars(MOBObjectNameChar);
                ObjectZero1 = br.ReadUInt16();
                ObjectFlag = (MapObjectType)br.ReadByte();
                ObjectZero2 = br.ReadByte();
                for (int i = 0; i < 3; i++)
                {
                    Position[i] = br.ReadSingle();
                }
                for (int i = 0; i < 3; i++)
                {
                    Rotation[i] = br.ReadSingle();
                }
                for (int i = 0; i < 3; i++)
                {
                    Scale[i] = br.ReadSingle();
                }
                for (int i = 0; i < 3; i++)
                {
                    HealthBarPosition1[i] = br.ReadSingle();
                }
                for (int i = 0; i < 3; i++)
                {
                    HealthBarPosition2[i] = br.ReadSingle();
                }
                ObjectZero3 = br.ReadUInt32();
            }
        }
        public enum MapObjectType : byte {
            BarrackSpawn = 0,
            NexusSpawn = 1,
            LevelSize = 2,
            Barrack = 3,
            Nexus = 4,
            Turret = 5,
            Nav = 8,
            Info = 9,
            LevelProp = 10 };
        private static char[] GetCharsFromString(string str, int size)
        {
            char[] final = new char[size];
            int i = 0;
            while (i < size && i < str.Length)
            {
                final[i] = str[i];
                i++;
            }
            return final;
        }
        private static string GetStringFromChars(char[] chars)
        {
            string final = "";
            int i = 0;
            while (i < chars.Length && chars[i] != 0)
            {
                final += chars[i];
                i++;
            }
            return final;
        }
    }

}
