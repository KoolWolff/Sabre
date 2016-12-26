using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;

namespace Sabre
{
    class FlipViewDownloader
    {
        WebClient wc = new WebClient();
        public List<View> Views = new List<View>();
        public FlipViewDownloader(MainWindow mw)
        {
            Directory.CreateDirectory("Images");
            List<string> temp = DownloadViews();
            wc.DownloadFile(temp[1], "Images\\" + "thumb1.png");
            wc.DownloadFile(temp[4], "Images\\" + "thumb2.png");
            wc.DownloadFile(temp[7], "Images\\" + "thumb3.png");
            Views.Add(new View(temp[0], temp[2], new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\" + "thumb1.png", UriKind.Relative)))));
            Views.Add(new View(temp[3], temp[5], new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\" + "thumb2.png", UriKind.Relative)))));
            Views.Add(new View(temp[6], temp[8], new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\" + "thumb3.png", UriKind.Relative)))));
            mw.gridFW1.Background = Views[0].Image;
            mw.gridFW2.Background = Views[1].Image;
            mw.gridFW3.Background = Views[2].Image;
            mw.flipHome.BannerText = Views[0].Name;
        }
        public class View
        {
            public string Name;
            public string skinURL;
            public ImageBrush Image;
            public View(string name, string skinurl, ImageBrush img)
            {
                Name = name;
                skinURL = skinurl;
                Image = img;
            }
        }
        private static List<string> DownloadViews()
        {
            WebClient wc = new WebClient();
            string url = "https://drive.google.com/uc?export=download&id=0Bz9aB-8O_UqfTl9VR1hsRDBudWM";
            wc.DownloadFile(url, "fwitems");
            List<string> ret = new List<string>();
            ret.AddRange(File.ReadAllLines("fwitems"));
            return ret;
        }
    }
}
