using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre.Code.Files
{
    class SBTFile
    {
        public List<Image> Images = new List<Image>();
        public Header header;

        public SBTFile(string fileLocation)
        {
            BinaryReader br = new BinaryReader(File.Open(fileLocation, FileMode.Open));

            header = new Header(br);
            for(int i = 0; i < header.NumOfImages; i++)
            {
                Images.Add(new Image(br));
            }
        }
        public class Header
        {
            public string Magic;
            public UInt32 Version;
            public UInt32 NumOfImages;
            public UInt16 ThemeNameLength;
            public UInt16 ThemeAuthorLength;
            public UInt16 ThemeVersionLength;
            public string ThemeName;
            public string ThemeAuthor;
            public string ThemeVersion;
            public Header(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                if(Magic != "SBRTHEME")
                {
                    throw new Exception();
                }
                Version = br.ReadUInt32();
                NumOfImages = br.ReadUInt32();
                ThemeNameLength = br.ReadUInt16();
                ThemeAuthorLength = br.ReadUInt16();
                ThemeVersionLength = br.ReadUInt16();
                ThemeName = Encoding.ASCII.GetString(br.ReadBytes(ThemeNameLength));
                ThemeAuthor = Encoding.ASCII.GetString(br.ReadBytes(ThemeAuthorLength));
                ThemeVersion = Encoding.ASCII.GetString(br.ReadBytes(ThemeVersionLength));
            }
        }
        public class Image
        {
            public ImageType Type;
            public UInt32 ImageHash;
            public UInt32 ImageLength;
            public byte[] ImageBytes;
            public Image(BinaryReader br)
            {
                Type = (ImageType)br.ReadByte();
                ImageHash = br.ReadUInt32();
                ImageLength = br.ReadUInt32();
                ImageBytes = br.ReadBytes((int)ImageLength);
            }
        }
        public class Colors
        {
            public byte[] BrownRGBA = new byte[4];
            public byte[] BlueRGBA = new byte[4];
            public byte[] DarkBlueRGBA = new byte[4];
            public Colors(BinaryReader br)
            {
                for(int i = 0; i < 4; i++)
                {
                    BrownRGBA[i] = br.ReadByte();
                }
                for(int i = 0; i < 4; i++)
                {
                    BlueRGBA[i] = br.ReadByte();
                }
                for(int i = 0; i < 4; i++)
                {
                    DarkBlueRGBA[i] = br.ReadByte();
                }
            }
        }
        public enum ImageType : byte
        {
            background1 = 1,
            background2 = 2,
            sabreLogo = 3,
            skinCollectionIcon = 4,
            skinCreationIcon = 5,
            settingsIcon = 6,
            xIcon = 7,
            minIcon = 8,
            screenshotIcon = 9,
            backArrowIcon = 10,
            hoverIcon = 11
        }
    }
}
