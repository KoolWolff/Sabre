using MahApps.Metro;
using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using Ionic.Zlib;
using System.Collections.Generic;
using WPFFolderBrowser;
using System.Collections.ObjectModel;
using SabreAPI;
using System.Text;

namespace Sabre
{

    class Functions
    {
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value)
        {
            double len = value;
            int order = 0;
            while (len >= 1024 && ++order < SizeSuffixes.Length)
            {
                len = len / 1024;
            }

            return String.Format("{0:0.##} {1}", len, SizeSuffixes[order]);
        }
        public static string GetLoLPath()
        {
            string LoLLocation = "";
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Riot Games\\League of Legends"))
            {
                if (key != null)
                {
                    Object o = key.GetValue("Path");
                    LoLLocation = o.ToString();
                }
            }
            return LoLLocation;
        }

        public static byte[] DecompressDeflate(byte[] data)
        {
            return Ionic.Zlib.DeflateStream.UncompressBuffer(data);
        }
        public static byte[] DecompressZlib(byte[] inData)
        {
            return ZlibStream.UncompressBuffer(inData);
        }
        public static byte[] DecompressGZip(byte[] gzip)
        {
            return Ionic.Zlib.GZipStream.UncompressBuffer(gzip);
        }
        public static Quaternion Decompress48BitQuaternion(int AxisFlg, short s2f, short s1f, short s0f)
        {
            float s0 = (float)1.41421 * (s0f - 0x3FFF) / 0x7FFF;
            float s1 = (float)1.41421 * (s1f - 0x3FFF) / 0x7FFF;
            float s2 = (float)1.41421 * (s2f - 0x3FFF) / 0x7FFF;
            float s3 = 1.0f - (float)(Math.Pow(s0, 2.0f) + Math.Pow(s1, 2.0f) + Math.Pow(s2, 2.0f));

            switch (AxisFlg)
            {
                case 0:
                    return new Quaternion(s3, s2, s1, s0);
                case 1:
                    return new Quaternion(s2, s3, s1, s0);
                case 2:
                    return new Quaternion(s2, s1, s3, s0);
                case 3:
                    return new Quaternion(s2, s1, s0, s3);
                default:
                    return new Quaternion(0, 0, 0, 0);
            }
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
        public static void LoadSettings(Config cfg, MainWindow mw, out List<string> WADhashes)
        {
            WADhashes = new List<string>();
            if(File.Exists("wadchargen") == false)
            {
                WADhashes = HASH.GetWADHashes(REST.GetCharacters(true), Environment.CurrentDirectory, 15);
            }
            else WADhashes.AddRange(File.ReadAllLines("wadchargen"));
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
            mw.comboAccents.SelectedItem = cfg.Settings.Find(x => x.Type == Config.SettingType.Accent).StringEntry;
            mw.comboThemes.SelectedItem = cfg.Settings.Find(x => x.Type == Config.SettingType.Theme).StringEntry;
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
        public static string SelectFolder(string title, string startupDir)
        {
            WPFFolderBrowserDialog fbd = new WPFFolderBrowserDialog();
            fbd.InitialDirectory = startupDir;
            fbd.Title = title;
            if (fbd.ShowDialog() == true)
            {
                return fbd.FileName;
            }
            else
            {
                return "";
            }
        }

        public static byte[] StringToByteArray(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string ByteArrayToString(byte[] arr, StringCaseType caseType)
        {
            string s = "";
            foreach (byte b in arr)
            {
                if (caseType == StringCaseType.Lowercase) s += b.ToString("x2");
                else s += b.ToString("X2");
            }
            return s;
        }
        public static string ByteArrayToString(byte[] arr)
        {
            string s = "";
            foreach (byte b in arr)
            {
                s += b;
            }
            return s;
        }
        public static string ReadNullTerminatedString(BinaryReader br) //Not sure if this will work efficently
        {
            string s = "";
            do
            {
                s += br.ReadChar();
            } while (s.IndexOf('\u0000') == -1);
            s = s.Replace("\0", string.Empty);
            return s;
        }
        public static string ReadNullTerminatedString(BinaryReader br, long offset)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            string s = "";
            do
            {
                s += br.ReadChar();
            } while (s.IndexOf('\u0000') == -1);
            s = s.Replace("\0", string.Empty);
            return s;
        }

        public static void SaveMOB(MOBFile mob, System.Collections.IList entries)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "Select the path where you want to save your MOB file";
            sfd.Filter = "MOB File | *.mob";
            sfd.DefaultExt = "mob";

            if (sfd.ShowDialog() == true)
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(sfd.FileName)))
                {
                    bw.Write(mob.Magic.ToCharArray());
                    bw.Write(mob.Version);
                    bw.Write((UInt32)entries.Count);
                    bw.Write(mob.Zero);
                    foreach(MOBFile.MOBObject o in entries)
                    {
                        bw.Write(o.Name.ToCharArray());
                        if(o.Name.Length != 60)
                        {
                            for(int i = o.Name.Length; i < 60; i++)
                            {
                                bw.Write((byte)0);
                            }
                        }
                        bw.Write((UInt16)o.ObjectZero1);
                        bw.Write((byte)o.Flag);
                        bw.Write((byte)o.ObjectZero2);

                        bw.Write(o.Position__X);
                        bw.Write(o.Position__Y);
                        bw.Write(o.Position__Z);

                        bw.Write(o.Rotation__X);
                        bw.Write(o.Rotation__Y);
                        bw.Write(o.Rotation__Z);

                        bw.Write(o.Scaling__X);
                        bw.Write(o.Scaling__Y);
                        bw.Write(o.Scaling__Z);

                        bw.Write(o.Healthbar__X);
                        bw.Write(o.Healthbar__Y);
                        bw.Write(o.Healthbar__Z);

                        bw.Write(o.Healthbar__Bounding__X);
                        bw.Write(o.Healthbar__Bounding__Y);
                        bw.Write(o.Healthbar__Bounding__Z);

                        bw.Write((UInt32)o.ObjectZero3);
                    }
                }
            }
        }
        public static void AddMOBEntry(ObservableCollection<MOBFile.MOBObject> entries) 
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { entries.Add(new MOBFile.MOBObject("Entry name here")); }));
        }
        public static void RemoveMOBEntry(ObservableCollection<MOBFile.MOBObject> entries, System.Collections.IList selectedEntries)
        {
            foreach(MOBFile.MOBObject se in selectedEntries)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { entries.Remove(se); }));
            }
        }
        public static void ExtractMOBEntries(System.Collections.IList selectedEntries)
        {
            string path = SelectFolder("Select the path where you want to export your MOB Entries");
            foreach (MOBFile.MOBObject o in selectedEntries)
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(path + "\\" + o.Name + ".mobentry")))
                {
                    bw.Write(o.Name.ToCharArray());
                    if (o.Name.Length != 60)
                    {
                        for (int i = o.Name.Length; i < 60; i++)
                        {
                            bw.Write((byte)0);
                        }
                    }
                    bw.Write((UInt16)o.ObjectZero1);
                    bw.Write((byte)o.Flag);
                    bw.Write((byte)o.ObjectZero2);

                    bw.Write(o.Position__X);
                    bw.Write(o.Position__Y);
                    bw.Write(o.Position__Z);

                    bw.Write(o.Rotation__X);
                    bw.Write(o.Rotation__Y);
                    bw.Write(o.Rotation__Z);

                    bw.Write(o.Scaling__X);
                    bw.Write(o.Scaling__Y);
                    bw.Write(o.Scaling__Z);

                    bw.Write(o.Healthbar__X);
                    bw.Write(o.Healthbar__Y);
                    bw.Write(o.Healthbar__Z);

                    bw.Write(o.Healthbar__Bounding__X);
                    bw.Write(o.Healthbar__Bounding__Y);
                    bw.Write(o.Healthbar__Bounding__Z);

                    bw.Write((UInt32)o.ObjectZero3);
                }
            }
        }
        public static void ExtractWPK(string defaultPath, System.Collections.IList selectedEntries)
        {

        }
        public static void ExtractWAD(System.Collections.IList entries, List<string> Hashes)
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
                        Directory.CreateDirectory("WAD Extract//" + Path.GetDirectoryName(e.Name));
                        if(e.IsBINGenerated)
                        {
                            var f = File.Create("WAD Extract//" + Path.GetDirectoryName(e.Name) + "\\" + Path.GetFileNameWithoutExtension(e.Name) + "_" + e.XXHash + Path.GetExtension(e.Name));
                            f.Dispose();
                            f.Close();
                            File.WriteAllBytes("WAD Extract//" + Path.GetDirectoryName(e.Name) + "\\" + Path.GetFileNameWithoutExtension(e.Name) + "_" + e.XXHash + Path.GetExtension(e.Name), e.Data);
                        }
                        else
                        {
                            var f = File.Create("WAD Extract//" + Path.GetDirectoryName(e.Name) + "\\" + Path.GetFileName(e.Name));
                            f.Dispose();
                            f.Close();
                            File.WriteAllBytes("WAD Extract//" + Path.GetDirectoryName(e.Name) + "\\" + Path.GetFileName(e.Name), e.Data);
                        }
                    }
                }
            }
            GC.Collect();
        }
        public static void ExtractWAD(List<WADFile.Entry> entriesAll, List<string> Hashes)
        {
            foreach (var e in entriesAll)
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
        public static void PopulateFileExtractor(TreeView tree)
        {

        }

        public static string ModifyMOBName(string name)
        {
            for(int i = name.Length; i < 60; i++)
            {
                name += "\0";
            }
            return name;
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

        public enum StringCaseType
        {
            Uppercase,
            Lowercase
        }
    }
    class Hash
    {
        public static byte[] WADPublicKey = {
            0x30, 0x59, 0x30, 0x13, 0x06, 0x07, 0x2A, 0x86, 0x48, 0xCE,
            0x3D, 0x02, 0x01, 0x06, 0x08, 0x2A, 0x86, 0x48, 0xCE, 0x3D,
            0x03, 0x01, 0x07, 0x03, 0x42, 0x00, 0x04, 0x01, 0xE7, 0x1B,
            0xDD, 0x1D, 0x2F, 0xF5, 0x9C, 0x70, 0x8C, 0xEC, 0xAA, 0xE2,
            0x5D, 0xB4, 0xDB, 0x85, 0x50, 0x6D, 0x6B, 0x06, 0xED, 0x3B,
            0xE6, 0x21, 0xF8, 0x1A, 0xD4, 0x85, 0xFD, 0x68, 0x18, 0x8E,
            0xC5, 0x6B, 0xE1, 0x4E, 0x69, 0x00, 0x8C, 0x69, 0xDE, 0x66,
            0xF8, 0x16, 0x9F, 0xF3, 0xB2, 0xF5, 0x38, 0x6B, 0x67, 0xB1,
            0xF1, 0xBE, 0x96, 0x92, 0x04, 0x88, 0x89, 0xEF, 0x3E, 0xE0,
            0x2B
        };
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
        public static string xxHash(string toHash)
        {
            byte[] input = Encoding.ASCII.GetBytes(toHash.ToLower());
            return XXHash.XXH64(input).ToString("X2");
        }
        public static string xxHashDigest(string toHash)
        {
            XXHash.State64 state64 = new XXHash.State64();
            XXHash.ResetState64(state64, 0);
            XXHash.UpdateState64(state64, Functions.StringToByteArray(toHash.ToLower()));
            string outp = XXHash.DigestState64(state64).ToString("X2");
            return outp;
        }
        public static UInt32 Adler32(byte[] toHash)
        {
            const int MOD_ADLER = 65521;
            UInt32 a = 1, b = 0;
            for (int i = 0; i < toHash.Length; ++i)
            {
                a = (a + toHash[i]) % MOD_ADLER;
                b = (b + a) % MOD_ADLER;
            }
            return (b << 16) | a;
        }
    }
}
