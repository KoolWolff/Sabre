using MahApps.Metro;
using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using zlib;
using System.Collections.Generic;

namespace Sabre
{
    class Functions
    {
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
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
        public static void LoadSettings(MainWindow mw)
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
            mw.comboAccents.SelectedItem = Functions.GetAccent();
            mw.comboThemes.SelectedItem = Functions.GetTheme();
        }
    }
}
