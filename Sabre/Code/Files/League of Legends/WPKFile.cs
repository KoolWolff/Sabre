using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace Sabre
{
    class WPKFile
    {
        public string fileLoc;
        public BinaryReader br;
        public Header header;
        public ObservableCollection<AudioFile> AudioFiles = new ObservableCollection<AudioFile>();
        public uint LastOffset = 0;
        public WPKFile(string fileLocation)
        {
            fileLoc = fileLocation;
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            header = new Header(br);
            for (int i = 0; i < header.AudioCount; i++)
            {
                AudioFiles.Add(new AudioFile(br.ReadUInt32()));
            }
            LastOffset = AudioFiles[0].DataOffset;
            foreach (var a in AudioFiles)
            {
                br.BaseStream.Seek(a.mtOff, SeekOrigin.Begin);
                a.DataOffset = br.ReadUInt32();
                a.DataSize = br.ReadUInt32();
                a.NameLength = br.ReadUInt32();
                a.tempName = br.ReadChars((int)a.NameLength * 2);
                a.Name = GetWPKName(a.tempName);
            }
            foreach (var a in AudioFiles)
            {
                br.BaseStream.Seek(a.DataOffset, SeekOrigin.Begin);
                a.Data = br.ReadBytes((int)a.DataSize);
                br.BaseStream.Seek(a.DataOffset + 24, SeekOrigin.Begin);
                double frequency = br.ReadUInt32();
                double blockByteCount = br.ReadUInt32();
                a.Duration = (a.DataSize - 44) / (blockByteCount * frequency);
            }
            br.Dispose();
            br.Close();
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
            public UInt32 mtOff;
            public UInt32 DataOffset;
            public UInt32 DataSize;
            public UInt32 NameLength;
            public char[] tempName;
            public string Name {get; set; }
            public double Duration { get; set; }
            public byte[] Data;
            public AudioFile(UInt32 metaOffset)
            {
                mtOff = metaOffset;
            }
        }
        public static string GetWPKName(char[] tempName)
        {
            string name = "";
            foreach (char c in tempName)
            {
                if (c != '\0')
                {
                    name += c;
                }
            }
            return name;
        }
    }
}
