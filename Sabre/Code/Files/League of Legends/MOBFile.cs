using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;

namespace Sabre
{
    class MOBFile
    {
        public string Magic;
        public uint Version;
        public uint NumberOfObjects;
        public uint Zero;
        public ObservableCollection<MOBObject> MobObjects = new ObservableCollection<MOBObject>();
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
            public string Name {get; set; }
            public char[] MOBObjectNameChar;
            public uint ObjectZero1 = 0;
            public MapObjectType Flag { get; set; }
            public byte ObjectZero2 = 0;
            public float[] Position = new float[3];
            public float[] Rotation = new float[3];
            public float[] Scale = new float[3];
            public float[] HealthBarPosition1 = new float[3];
            public float[] HealthBarPosition2 = new float[3];
            public uint ObjectZero3 = 0;
            #region DataBindedProps
            public float Position__X { get; set; }
            public float Position__Y { get; set; }
            public float Position__Z { get; set; }

            public float Rotation__X { get; set; }
            public float Rotation__Y { get; set; }
            public float Rotation__Z { get; set; }

            public float Scaling__X { get; set; }
            public float Scaling__Y { get; set; }
            public float Scaling__Z { get; set; }

            public float Healthbar__X { get; set; }
            public float Healthbar__Y { get; set; }
            public float Healthbar__Z { get; set; }

            public float Healthbar__Bounding__X { get; set; }
            public float Healthbar__Bounding__Y { get; set; }
            public float Healthbar__Bounding__Z { get; set; }
            #endregion
            public MOBObject(BinaryReader br)
            {
                Name = Encoding.ASCII.GetString(br.ReadBytes(60));
                MOBObjectNameChar = GetCharsFromString(Name, 60);
                Name = GetStringFromChars(MOBObjectNameChar);
                ObjectZero1 = br.ReadUInt16();
                Flag = (MapObjectType)br.ReadByte();
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
                Position__X = Position[0];
                Position__Y = Position[1];
                Position__Z = Position[2];

                Rotation__X = Rotation[0];
                Rotation__Y = Rotation[1];
                Rotation__Z = Rotation[2];

                Scaling__X = Scale[0];
                Scaling__Y = Scale[1];
                Scaling__Z = Scale[2];
            
                Healthbar__X = HealthBarPosition1[0];
                Healthbar__Y = HealthBarPosition1[1];
                Healthbar__Z = HealthBarPosition1[2];

                Healthbar__Bounding__X = HealthBarPosition2[0];
                Healthbar__Bounding__Y = HealthBarPosition2[1];
                Healthbar__Bounding__Z = HealthBarPosition2[2];
            }
            public MOBObject(string name)
            {
                Name = name;
                MOBObjectNameChar = GetCharsFromString(Name, Name.Length);
                Name = GetStringFromChars(MOBObjectNameChar);
                ObjectZero1 = 0;
                Flag = MapObjectType.LevelProp;
                ObjectZero2 = 0;
                for (int i = 0; i < 3; i++)
                {
                    Position[i] = 0;
                }
                for (int i = 0; i < 3; i++)
                {
                    Rotation[i] = 0;
                }
                for (int i = 0; i < 3; i++)
                {
                    Scale[i] = 0;
                }
                for (int i = 0; i < 3; i++)
                {
                    HealthBarPosition1[i] = 0;
                }
                for (int i = 0; i < 3; i++)
                {
                    HealthBarPosition2[i] = 0;
                }
                ObjectZero3 = 0;
                Position__X = Position[0];
                Position__Y = Position[1];
                Position__Z = Position[2];

                Rotation__X = Rotation[0];
                Rotation__Y = Rotation[1];
                Rotation__Z = Rotation[2];

                Scaling__X = Scale[0];
                Scaling__Y = Scale[1];
                Scaling__Z = Scale[2];

                Healthbar__X = HealthBarPosition1[0];
                Healthbar__Y = HealthBarPosition1[1];
                Healthbar__Z = HealthBarPosition1[2];

                Healthbar__Bounding__X = HealthBarPosition2[0];
                Healthbar__Bounding__Y = HealthBarPosition2[1];
                Healthbar__Bounding__Z = HealthBarPosition2[2];
            }
            public void Write(BinaryWriter bw)
            {
                bw.Write(Name.ToCharArray());
                if (Name.Length != 60)
                {
                    for (int i = Name.Length; i < 60; i++)
                    {
                        bw.Write((byte)0);
                    }
                }
                bw.Write((UInt16)ObjectZero1);
                bw.Write((byte)Flag);
                bw.Write((byte)ObjectZero2);

                bw.Write(Position__X);
                bw.Write(Position__Y);
                bw.Write(Position__Z);

                bw.Write(Rotation__X);
                bw.Write(Rotation__Y);
                bw.Write(Rotation__Z);

                bw.Write(Scaling__X);
                bw.Write(Scaling__Y);
                bw.Write(Scaling__Z);

                bw.Write(Healthbar__X);
                bw.Write(Healthbar__Y);
                bw.Write(Healthbar__Z);

                bw.Write(Healthbar__Bounding__X);
                bw.Write(Healthbar__Bounding__Y);
                bw.Write(Healthbar__Bounding__Z);

                bw.Write((UInt32)ObjectZero3);
            }
        }
        public enum MapObjectType : byte {
            BarrackSpawn = 0,
            NexusSpawn = 1,
            LevelSize = 2,
            Barrack = 3,
            Nexus = 4,
            Turret = 5,
            Shop = 6,
            Lake = 7,
            Nav = 8,
            Info = 9,
            LevelProp = 10 };
        public void Write()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "Select the path where you want to save your MOB file";
            sfd.Filter = "MOB File | *.mob";
            sfd.DefaultExt = "mob";

            if (sfd.ShowDialog() == true)
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(sfd.FileName)))
                {
                    bw.Write(Magic.ToCharArray());
                    bw.Write(Version);
                    bw.Write((UInt32)MobObjects.Count);
                    bw.Write(Zero);
                    foreach (MOBObject entry in MobObjects)
                    {
                        entry.Write(bw);
                    }
                }
            }
        }
        public void AddEntry()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { MobObjects.Add(new MOBObject("Entry name here")); }));
        }
        public void RemoveEntry(System.Collections.IList selectedEntries)
        {
            foreach (MOBObject se in selectedEntries)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { MobObjects.Remove(se); }));
            }
        }
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
