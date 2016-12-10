using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sabre
{
    class CFGFileSave
    {
        public uint Version = 1;

        public uint IsLoLPathPresent = 0;
        public uint IsExtractPathPresent = 0;
        public uint IsWADOpenPathPresent = 0;
        public uint IsWADExtractPathPresent = 0;
        public uint IsTroyiniPathPresent = 0;
        public uint IsModelPathPresent = 0;
        public uint IsWGEOPathPresent = 0;
        public uint IsWGEOTexturesPathPresent = 0;
        public uint IsWGEOParticlesPathPresent = 0;

        public string LoLPath = "LoLPath";
        public string ExtractPath = "ExtractPath";
        public string WADOpenPath = "WadOpenPath";
        public string WADExtractPath = "WADExtractPath";
        public string TroyiniPath = "TroyiniPath";
        public string ModelPath = "ModelPath";
        public string WGEOPath = "WGEOPath";
        public string WGEOTexturesPath = "WGEOTexturesPath";
        public string WGEOParticlesPath = "WGEOParticlesPath";

        string Magic = "SCFG";

        public CFGFileSave(string fileLocation)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(fileLocation, FileMode.Create)))
            {
                this.Write(bw);
            }
        }

        public void Write(BinaryWriter bw)
        {

            bw.Write(GetCharsFromString(Magic, 4));
            bw.Write((UInt16)Version);

            if(LoLPath != "")
            {
                IsLoLPathPresent = 1;
            }

            if(ExtractPath != "")
            {
                IsExtractPathPresent = 1;
            }

            if(WADOpenPath != "")
            {
                IsWADOpenPathPresent = 1;
            }

            if(WADExtractPath != "")
            {
                IsWADExtractPathPresent = 1;
            }

            if(TroyiniPath != "")
            {
                IsTroyiniPathPresent = 1;
            }

            if(ModelPath != "")
            {
                IsModelPathPresent = 1;
            }

            if(WGEOPath != "")
            {
                IsWGEOPathPresent = 1;
            }

            if(WGEOTexturesPath != "")
            {
                IsWGEOTexturesPathPresent = 1;
            }

            if(WGEOParticlesPath != "")
            {
                IsWGEOParticlesPathPresent = 1;
            }

            bw.Write((byte)IsLoLPathPresent);
            bw.Write((byte)IsExtractPathPresent);
            bw.Write((byte)IsWADOpenPathPresent);
            bw.Write((byte)IsWADExtractPathPresent);
            bw.Write((byte)IsTroyiniPathPresent);
            bw.Write((byte)IsModelPathPresent);
            bw.Write((byte)IsWGEOPathPresent);
            bw.Write((byte)IsWGEOTexturesPathPresent);
            bw.Write((byte)IsWGEOParticlesPathPresent);

            if(IsLoLPathPresent == 1)
            {
                bw.Write((string)LoLPath);
            }
            if(IsExtractPathPresent == 1)
            {
                bw.Write((string)ExtractPath);
            }
            if(IsWADOpenPathPresent == 1)
            {
                bw.Write((string)WADOpenPath);
            }
            if(IsWADExtractPathPresent == 1)
            {
                bw.Write((string)WADExtractPath);
            }
            if(IsTroyiniPathPresent == 1)
            {
                bw.Write((string)TroyiniPath);
            }
            if(IsModelPathPresent == 1)
            {
                bw.Write((string)ModelPath);
            }
            if(IsWGEOPathPresent == 1)
            {
                bw.Write((string)WGEOPath);
            }
            if(IsWGEOTexturesPathPresent == 1)
            {
                bw.Write((string)WGEOTexturesPath);
            }
            if(IsWGEOParticlesPathPresent == 1)
            {
                bw.Write((string)WGEOParticlesPath);
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
