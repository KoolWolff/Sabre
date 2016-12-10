using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class WPKFile
    {
        public BinaryReader br;
        public Header header;
        public List<AudioFile> AudioFiles = new List<AudioFile>();
        public WPKFile(string fileLocation, bool extractAudio = false)
        {
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            header = new Header(br);
            for(int i = 0; i < header.AudioCount; i++)
            {
                AudioFiles.Add(new AudioFile(br.ReadUInt32()));
            }
            UInt32 zero = br.ReadUInt32();
            if(zero != 0)
            {
                br.BaseStream.Position -= 4;
            }
            foreach(var a in AudioFiles)
            {
                a.DataOffset = br.ReadUInt32();
                a.DataSize = br.ReadUInt32();
                a.NameLength = br.ReadUInt32();
                a.tempName = br.ReadChars((int)a.NameLength * 2);
                a.Name = GetWPKName(a.tempName);
                br.ReadUInt16();
                UInt16 zero1 = br.ReadUInt16();
                if(zero1 != 0)
                {
                    br.BaseStream.Position -= 2;
                }
            }
            br.Dispose();
            br.Close();
            if(extractAudio == true)
            {
                foreach(var a in AudioFiles)
                {
                    AudioFile.ExtractFile(fileLocation, a.Name, a.DataOffset, a.DataSize);
                }
            }
        }
        public class Header
        {
            public string Magic;
            public UInt32 Version;
            public UInt32 AudioCount;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                Version = br.ReadUInt32();
                AudioCount = br.ReadUInt32();
            }
        }
        public class AudioFile
        {
            public UInt32 DataOffset;
            public UInt32 DataSize;
            public UInt32 NameLength;
            public char[] tempName;
            public string Name;
            public AudioFile(UInt32 metaOffset)
            {

            }
            public static void ExtractFile(string fileLocation, string extractPath, UInt32 DataOffset, UInt32 DataSize)
            {
                using (BinaryReader br = new BinaryReader(new FileStream(fileLocation, FileMode.Open)))
                { 
                    br.BaseStream.Seek(DataOffset, SeekOrigin.Begin);
                    File.WriteAllBytes(extractPath, br.ReadBytes((int)DataSize));
                    br.Dispose();
                    br.Close();
                }
            }
        }
        public static string GetWPKName(char[] tempName)
        {
            string name = "";
            foreach(char c in tempName)
            {
                if(c != '\0')
                {
                    name += c;
                }
            }
            return name;
        }
    }
}
