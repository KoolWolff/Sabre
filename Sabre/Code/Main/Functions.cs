using MahApps.Metro;
using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using zlib;
using System.Collections.Generic;
using WPFFolderBrowser;
using System.IO.Compression;
using System.Data.HashFunction;

namespace Sabre
{
    class Functions
    {
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        public static byte[] DecompressZlib(byte[] inData)
        {
            byte[] outData;
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyZlibStream(inMemoryStream, outZStream);
                outZStream.finish();
                outData = outMemoryStream.ToArray();
            }
            return outData;
        }
        public static byte[] DecompressGZip(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
        public static void CopyZlibStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }
        public static void SwitchGrids(Grid hider, Grid shower)
        {
            hider.Visibility = System.Windows.Visibility.Hidden;
            shower.Visibility = System.Windows.Visibility.Visible;
        }
        public static string GetTheme()
        {
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            return appStyle.Item1.Name;
        }
        public static string GetAccent()
        {
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            return appStyle.Item2.Name;
        }
        public static void ChangeAppearance(string accent, string theme)
        {
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent(accent),
                                        ThemeManager.GetAppTheme(theme));
        }
        public static void LoadSettings(Config cfg, MainWindow mw)
        {
            IEnumerable<Accent> accents = ThemeManager.Accents;
            foreach (Accent a in accents)
            {
                mw.comboAccents.Items.Add(a.Name);
            }
            IEnumerable<AppTheme> themes = ThemeManager.AppThemes;
            foreach (AppTheme t in themes)
            {
                mw.comboThemes.Items.Add(t.Name);
            }
            mw.comboAccents.SelectedItem = cfg.Entries.Find(x => x.StartsWith("Accent")).Split(' ')[1];
            mw.comboThemes.SelectedItem = cfg.Entries.Find(x => x.StartsWith("Theme")).Split(' ')[1];
        }
        public static string SelectFolder(string title)
        {
            WPFFolderBrowserDialog fbd = new WPFFolderBrowserDialog();
            fbd.Title = title;
            if(fbd.ShowDialog() == true)
            {
                return fbd.FileName;
            }
            else
            {
                return "";
            }
        }
        public static void ExtractWAD(System.Collections.IList entries, List<WADFile.Entry> entriesAll, ExtractionType typeOfExtraction, List<string> Hashes)
        {
            if(typeOfExtraction == ExtractionType.All)
            {
                foreach(var e in entriesAll)
                {
                    if (e.Compression == WADFile.CompressionType.String)
                    {
                        continue;
                    }
                    else
                    {
                        if (e.Name == null)
                        {
                            Directory.CreateDirectory("WAD Extract//Unknown");
                            if (e.Data[0] == 0x50 && e.Data[1] == 0x52 && e.Data[2] == 0x4F && e.Data[3] == 0x50)
                            {
                                var f = File.Create("WAD Extract//Unknown" + "//" + e.XXHash + ".bin");
                                f.Dispose();
                                f.Close();
                                File.WriteAllBytes("WAD Extract//Unknown" + "//" + e.XXHash + ".bin", e.Data);
                            }
                            else
                            {
                                var f = File.Create("WAD Extract//Unknown" + "//" + e.XXHash);
                                f.Dispose();
                                f.Close();
                                File.WriteAllBytes("WAD Extract//Unknown" + "//" + e.XXHash, e.Data);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(e.Name));
                            var f = File.Create(e.Name);
                            f.Dispose();
                            f.Close();
                        }
                    }
                }
                GC.Collect();
            }
            else
            {
                foreach (WADFile.Entry e in entries)
                {
                    if (e.Compression == WADFile.CompressionType.String)
                    {
                        continue;
                    }
                    else
                    {
                        if (e.Name == null)
                        {
                            Directory.CreateDirectory("WAD Extract//Unknown");
                            if (e.Data[0] == 0x50 && e.Data[1] == 0x52 && e.Data[2] == 0x4F && e.Data[3] == 0x50)
                            {
                                var f = File.Create("WAD Extract//Unknown" + "//" + e.XXHash + ".bin");
                                f.Dispose();
                                f.Close();
                                File.WriteAllBytes("WAD Extract//Unknown" + "//" + e.XXHash + ".bin", e.Data);
                            }
                            else
                            {
                                var f = File.Create("WAD Extract//Unknown" + "//" + e.XXHash);
                                f.Dispose();
                                f.Close();
                                File.WriteAllBytes("WAD Extract//Unknown" + "//" + e.XXHash, e.Data);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(e.Name));
                            var f = File.Create(e.Name);
                            f.Dispose();
                            f.Close();
                        }
                    }
                }
                GC.Collect();
            }
        }
        public enum ExtractionType
        {
            Selected,
            All
        }
    }
    class Hash
    {
        public static uint RAF(string s)
        {
            UInt32 hash = 0;
            UInt32 temp = 0;
            for (int i = 0; i < s.Length; i++)
            {
                hash = (hash << 4) + s.ToLower()[i];
                if (0 != (temp = (hash & 0xF0000000)))
                {
                    hash = hash ^ (temp >> 24);
                    hash = hash ^ temp;
                }
            }
            return hash;
        }
        public static UInt32 Inibin(string section, string name)
        {
            UInt32 hash = 0;
            foreach (var c in section.ToLower())
            {
                hash = c + 65599 * hash;
            }
            hash = (65599 * hash + 42);
            foreach (var c in name.ToLower())
            {
                hash = c + 65599 * hash;
            }
            return hash;
        }
        #region BIN
        public static char[] BINAlpha = "abcdefghijklmnopqrstuvwxyz-_0123456789/".ToCharArray();
        public static uint BIN(string s)
        {
            s = s.ToLower();
            uint hash = 2166136261;
            for (int i = 0; i < s.Length; i++)
            {
                hash = hash ^ s[i];
                hash = hash * 16777619;
            }

            return hash;
        }
        public static uint BIN2(char[] s)
        {
            uint hash = 2166136261;
            for (int i = 0; i < s.Length; i++)
            {
                hash = hash ^ s[i];
                hash = hash * 16777619;
            }

            return hash;
        }
        public static string BINBruteforceLength(uint hashToBrute, int length)
        {
            char[] brute = new char[length];
            string bruteString = "";
            #region Length1
            if (length == 1)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    brute[0] = BINAlpha[a];
                    if (BIN2(brute) == hashToBrute)
                    {
                        foreach (char c in brute)
                        {
                            bruteString += c;
                        }
                        break;
                    }
                }
            }
            #endregion
            #region Length2
            else if (length == 2)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        brute[0] = BINAlpha[a];
                        brute[1] = BINAlpha[b];
                        if (BIN2(brute) == hashToBrute)
                        {
                            foreach (char c in brute)
                            {
                                bruteString += c;
                            }
                            break;
                        }
                    }
                }
            }
            #endregion
            #region Length3
            else if (length == 3)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        for (int c = 0; c < BINAlpha.Length; c++)
                        {
                            brute[0] = BINAlpha[a];
                            brute[1] = BINAlpha[b];
                            brute[2] = BINAlpha[c];
                            if (BIN2(brute) == hashToBrute)
                            {
                                foreach (char charr in brute)
                                {
                                    bruteString += charr;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            #region Length4
            else if (length == 4)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        for (int c = 0; c < BINAlpha.Length; c++)
                        {
                            for (int d = 0; d < BINAlpha.Length; d++)
                            {
                                brute[0] = BINAlpha[a];
                                brute[1] = BINAlpha[b];
                                brute[2] = BINAlpha[c];
                                brute[3] = BINAlpha[d];
                                if (BIN2(brute) == hashToBrute)
                                {
                                    foreach (char charr in brute)
                                    {
                                        bruteString += charr;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Length5
            else if (length == 5)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        for (int c = 0; c < BINAlpha.Length; c++)
                        {
                            for (int d = 0; d < BINAlpha.Length; d++)
                            {
                                for (int e = 0; e < BINAlpha.Length; e++)
                                {
                                    brute[0] = BINAlpha[a];
                                    brute[1] = BINAlpha[b];
                                    brute[2] = BINAlpha[c];
                                    brute[3] = BINAlpha[d];
                                    brute[4] = BINAlpha[e];
                                    if (BIN2(brute) == hashToBrute)
                                    {
                                        foreach (char charr in brute)
                                        {
                                            bruteString += charr;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Length6
            else if (length == 6)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        for (int c = 0; c < BINAlpha.Length; c++)
                        {
                            for (int d = 0; d < BINAlpha.Length; d++)
                            {
                                for (int e = 0; e < BINAlpha.Length; e++)
                                {
                                    for (int f = 0; f < BINAlpha.Length; f++)
                                    {
                                        brute[0] = BINAlpha[a];
                                        brute[1] = BINAlpha[b];
                                        brute[2] = BINAlpha[c];
                                        brute[3] = BINAlpha[d];
                                        brute[4] = BINAlpha[e];
                                        brute[5] = BINAlpha[f];
                                        if (BIN2(brute) == hashToBrute)
                                        {
                                            foreach (char charr in brute)
                                            {
                                                bruteString += charr;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Length7
            else if (length == 7)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        for (int c = 0; c < BINAlpha.Length; c++)
                        {
                            for (int d = 0; d < BINAlpha.Length; d++)
                            {
                                for (int e = 0; e < BINAlpha.Length; e++)
                                {
                                    for (int f = 0; f < BINAlpha.Length; f++)
                                    {
                                        for (int g = 0; g < BINAlpha.Length; g++)
                                        {
                                            brute[0] = BINAlpha[a];
                                            brute[1] = BINAlpha[b];
                                            brute[2] = BINAlpha[c];
                                            brute[3] = BINAlpha[d];
                                            brute[4] = BINAlpha[e];
                                            brute[5] = BINAlpha[f];
                                            brute[6] = BINAlpha[g];
                                            if (BIN2(brute) == hashToBrute)
                                            {
                                                foreach (char charr in brute)
                                                {
                                                    bruteString += charr;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Length8
            else if (length == 8)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        for (int c = 0; c < BINAlpha.Length; c++)
                        {
                            for (int d = 0; d < BINAlpha.Length; d++)
                            {
                                for (int e = 0; e < BINAlpha.Length; e++)
                                {
                                    for (int f = 0; f < BINAlpha.Length; f++)
                                    {
                                        for (int g = 0; g < BINAlpha.Length; g++)
                                        {
                                            for (int h = 0; h < BINAlpha.Length; h++)
                                            {
                                                brute[0] = BINAlpha[a];
                                                brute[1] = BINAlpha[b];
                                                brute[2] = BINAlpha[c];
                                                brute[3] = BINAlpha[d];
                                                brute[4] = BINAlpha[e];
                                                brute[5] = BINAlpha[f];
                                                brute[6] = BINAlpha[g];
                                                brute[7] = BINAlpha[h];
                                                if (BIN2(brute) == hashToBrute)
                                                {
                                                    foreach (char charr in brute)
                                                    {
                                                        bruteString += charr;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Length9
            else if (length == 9)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        for (int c = 0; c < BINAlpha.Length; c++)
                        {
                            for (int d = 0; d < BINAlpha.Length; d++)
                            {
                                for (int e = 0; e < BINAlpha.Length; e++)
                                {
                                    for (int f = 0; f < BINAlpha.Length; f++)
                                    {
                                        for (int g = 0; g < BINAlpha.Length; g++)
                                        {
                                            for (int h = 0; h < BINAlpha.Length; h++)
                                            {
                                                for (int i = 0; i < BINAlpha.Length; i++)
                                                {
                                                    brute[0] = BINAlpha[a];
                                                    brute[1] = BINAlpha[b];
                                                    brute[2] = BINAlpha[c];
                                                    brute[3] = BINAlpha[d];
                                                    brute[4] = BINAlpha[e];
                                                    brute[5] = BINAlpha[f];
                                                    brute[6] = BINAlpha[g];
                                                    brute[7] = BINAlpha[h];
                                                    brute[8] = BINAlpha[i];
                                                    if (BIN2(brute) == hashToBrute)
                                                    {
                                                        foreach (char charr in brute)
                                                        {
                                                            bruteString += charr;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Length10
            else if (length == 10)
            {
                for (int a = 0; a < BINAlpha.Length; a++)
                {
                    for (int b = 0; b < BINAlpha.Length; b++)
                    {
                        for (int c = 0; c < BINAlpha.Length; c++)
                        {
                            for (int d = 0; d < BINAlpha.Length; d++)
                            {
                                for (int e = 0; e < BINAlpha.Length; e++)
                                {
                                    for (int f = 0; f < BINAlpha.Length; f++)
                                    {
                                        for (int g = 0; g < BINAlpha.Length; g++)
                                        {
                                            for (int h = 0; h < BINAlpha.Length; h++)
                                            {
                                                for (int i = 0; i < BINAlpha.Length; i++)
                                                {
                                                    for (int j = 0; j < BINAlpha.Length; j++)
                                                    {
                                                        brute[0] = BINAlpha[a];
                                                        brute[1] = BINAlpha[b];
                                                        brute[2] = BINAlpha[c];
                                                        brute[3] = BINAlpha[d];
                                                        brute[4] = BINAlpha[e];
                                                        brute[5] = BINAlpha[f];
                                                        brute[6] = BINAlpha[g];
                                                        brute[7] = BINAlpha[h];
                                                        brute[8] = BINAlpha[i];
                                                        brute[9] = BINAlpha[j];
                                                        if (BIN2(brute) == hashToBrute)
                                                        {
                                                            foreach (char charr in brute)
                                                            {
                                                                bruteString += charr;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            return bruteString;
        }
        public static string BINDictionaryAttack(uint hashToBrute, int numOfWordsToAppend = 0)
        {
            string tempHash = "";
            List<string> Words = new List<string>();
            if (numOfWordsToAppend == 0)
            {
                int length = File.ReadAllLines("words.txt").Length;
                StreamReader sr = new StreamReader("words.txt");
                for (int i = 0; i < length; i++)
                {
                    Words.Add(sr.ReadLine());
                }
                foreach (string s in Words)
                {
                    uint hash = BIN(s);
                    if (hash == hashToBrute)
                    {
                        return s;
                    }
                }
            }
            else if (numOfWordsToAppend == 2)
            {
                int length = File.ReadAllLines("words.txt").Length;
                StreamReader sr = new StreamReader("words.txt");
                for (int i = 0; i < length; i++)
                {
                    Words.Add(sr.ReadLine());
                }
                for (int a = 0; a < length; a++)
                {
                    for (int b = 0; b < length; b++)
                    {
                        uint hash = BIN(Words[a] + Words[b]);
                        if (hash == hashToBrute)
                        {
                            tempHash = Words[a] + Words[b];
                            return tempHash;
                        }
                    }
                }
            }
            else
            {
                return "Hash length can only be 0 or 2";
            }
            return "Hash not found";
        }
        #endregion
        public static uint Bone(string s)
        {
            uint hash = 0;
            uint temp = 0;
            uint mask = 4026531840;
            s = s.ToLower();
            for (int i = 0; i < s.Length; i++)
            {
                hash = (hash << 4) + s[i];
                temp = hash & mask;
                if (temp != 0)
                {
                    hash = hash ^ (temp >> 24);
                    hash = hash ^ temp;
                }
            }
            return hash;
        }
        public static string XXHash(string toHash)
        {
            string hash = "";
            xxHash xx = new xxHash();
            byte[] temp = xx.ComputeHash(toHash);
            foreach (byte b in temp)
            {
                hash += b.ToString("X2");
            }
            return hash;
        }
    }
}
