using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre
{
    class TroyiniFile
    {
        public BinaryReader br;
        public Header header;
        private UInt16 Type00 = Convert.ToUInt16("0000000000000001", 2);
        private UInt16 Type01 = Convert.ToUInt16("0000000000000010", 2);
        private UInt16 Type02 = Convert.ToUInt16("0000000000000100", 2);
        private UInt16 Type03 = Convert.ToUInt16("0000000000001000", 2);
        private UInt16 Type04 = Convert.ToUInt16("0000000000010000", 2);
        private UInt16 Type05 = Convert.ToUInt16("0000000000100000", 2);
        private UInt16 Type06 = Convert.ToUInt16("0000000001000000", 2);
        private UInt16 Type07 = Convert.ToUInt16("0000000010000000", 2);
        private UInt16 Type08 = Convert.ToUInt16("0000000100000000", 2);
        private UInt16 Type09 = Convert.ToUInt16("0000001000000000", 2);
        private UInt16 Type10 = Convert.ToUInt16("0000010000000000", 2);
        private UInt16 Type11 = Convert.ToUInt16("0000100000000000", 2);
        private UInt16 Type12 = Convert.ToUInt16("0001000000000000", 2);
        private UInt16 Type13 = Convert.ToUInt16("0010000000000000", 2);
        private UInt16 Type14 = Convert.ToUInt16("0100000000000000", 2);
        private UInt16 Type15 = Convert.ToUInt16("1000000000000000", 2);

        public TroyiniFile(string fileLocation)
        {
            br = new BinaryReader(File.OpenRead(fileLocation));

            header = new Header(br);
        }
        public class Header
        {
            public byte Version;
            public UInt16 StringTableLength;
            public UInt16 BitMask;
            public Header(BinaryReader br)
            {
                Version = br.ReadByte();
                StringTableLength = br.ReadUInt16();
                BitMask = br.ReadUInt16();
            }
        }

        private static bool GetBitmaskedType(UInt16 bitmask, UInt16 type)
        {
            if((bitmask & type) == type) return true;
            else return false;
        }
    }
}
