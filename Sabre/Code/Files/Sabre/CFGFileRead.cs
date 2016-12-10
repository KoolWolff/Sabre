using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class CFGFileRead
    {
        public uint Version = 1;

        public byte IsLoLPathPresent;
        public byte IsExtractPathPresent;
        public byte IsWADOpenPathPresent;
        public byte IsWADExtractPathPresent;
        public byte IsTroyiniPathPresent;
        public byte IsModelPathPresent;
        public byte IsWGEOPathPresent;
        public byte IsWGEOTexturesPathPresent;
        public byte IsWGEOParticlesPathPresent;

        public string LoLPath;
        public string ExtractPath;
        public string WADOpenPath;
        public string WADExtractPath;
        public string TroyiniPath;
        public string ModelPath;
        public string WGEOPath;
        public string WGEOTexturesPath;
        public string WGEOParticlesPath;

        public CFGFileRead(string fileLocation)
        {
            using (BinaryReader br = new BinaryReader(File.Open(fileLocation, FileMode.Open)))
            {
                this.Read(br);
            }
        }

        public void Read(BinaryReader br)
        {  
            string Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
            if(Magic != "SCFG")
            {
                throw new Exception("Not a valid Config file");
            }
            this.Version = br.ReadUInt16();
            if(Version >= 0)
            {
                IsLoLPathPresent = br.ReadByte();
                IsExtractPathPresent = br.ReadByte();
                IsWADOpenPathPresent = br.ReadByte();
                IsWADExtractPathPresent = br.ReadByte();
                IsTroyiniPathPresent = br.ReadByte();
                IsModelPathPresent = br.ReadByte();
                IsWGEOPathPresent = br.ReadByte();
                IsWGEOTexturesPathPresent = br.ReadByte();
                IsWGEOParticlesPathPresent = br.ReadByte();
                if(IsLoLPathPresent == 1)
                {
                    LoLPath = br.ReadString();
                }
                if(IsExtractPathPresent == 1)
                {
                    ExtractPath = br.ReadString();
                }
                if(IsWADOpenPathPresent == 1)
                {
                    WADOpenPath = br.ReadString();
                }
                if(IsWADExtractPathPresent == 1)
                {
                    WADExtractPath = br.ReadString();
                }
                if(IsTroyiniPathPresent == 1)
                {
                    TroyiniPath = br.ReadString();
                }
                if(IsModelPathPresent == 1)
                {
                    ModelPath = br.ReadString();
                }
                if(IsWGEOPathPresent == 1)
                {
                    WGEOPath = br.ReadString();
                }
                if(IsWGEOTexturesPathPresent == 1)
                {
                    WGEOTexturesPath = br.ReadString();
                }
                if(IsWGEOParticlesPathPresent == 1)
                {
                    WGEOParticlesPath = br.ReadString();
                }
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
    }
}
