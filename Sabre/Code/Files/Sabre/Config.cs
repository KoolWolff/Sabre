using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class Config
    {
        public uint SettingCount;
        public StreamReader sw;
        public List<string> Entries = new List<string>();
        public Config(string fileLocation, Logger log)
        {
            SettingCount = (uint)File.ReadLines(fileLocation).Count();
            using (sw = new StreamReader(File.OpenRead(fileLocation)))
            {
                for(int i = 0; i < SettingCount; i++)
                {
                    Entries.Add(sw.ReadLine());
                }
            }
        }
        public static void Write(string theme, string accent, string LoLPath)
        {
            using (StreamWriter sw = new StreamWriter(File.OpenWrite("config.ini")))
            {
                sw.Write("Theme= " + theme + Environment.NewLine);
                sw.Write("Accent= " + accent + Environment.NewLine);
                sw.Write("LoLPath= " + LoLPath + Environment.NewLine);
            }
        }
    }
}
