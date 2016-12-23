using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro;
using System.IO;
using SabreAPI;

namespace Sabre
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Config cfg;
        private Logger log;
        private string ecdsa;
        private WADFile wad;
        private MOBFile mob;
        private WPKFile wpk;
        public List<string> WADHashes = new List<string>();
        public MainWindow()
        {
            log = new Logger(DateTime.Now.ToString("HH-mm-ss"));
            log.Write("LOGGER INITIALIZED", Logger.WriterType.WriteMessage);
            InitializeComponent();
            log.Write("SABRE INITIALIZED", Logger.WriterType.WriteMessage);
            if (File.Exists("config") == false)
            {
                var c = File.Create("config");
                c.Close();
                Config.Setting.Write(Functions.GetLoLPath(), Config.SettingType.LoLPath);
                Config.Setting.Write("BaseDark", Config.SettingType.Theme);
                Config.Setting.Write("Teal", Config.SettingType.Accent);
                Config.Setting.Write("", Config.SettingType.WADPath);
                Config.Setting.Write("", Config.SettingType.WADExtractionPath);
                Config.Setting.Write("", Config.SettingType.MOBPath);
                Config.Setting.Write("", Config.SettingType.MOBExtractionPath);
                Config.Setting.Write("", Config.SettingType.MOBImportationPath);
                Config.Setting.Write("", Config.SettingType.WPKPath);
                Config.Setting.Write("", Config.SettingType.WPKExtractionPath);
                Config.Setting.Write("", Config.SettingType.WPKImportationPath);
            }
            cfg = new Config("config", log);
            Functions.LoadSettings(cfg, this, out WADHashes);
        }
        
        private void buttonGit(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/LoL-Sabre/Sabre");
            }
            catch (Exception) { }
        }

        private void deleteLogs(object sender, RoutedEventArgs e)
        {
            try
            {
                log.DeleteLogs();
            } catch(Exception) { }
        
        }

        private void openSettings(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(main, gridSettings);
        }

        private void openSkinCollection(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(main, gridSkinCollection);
        }

        private void openSkinCreation(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(main, gridSkinCreation);
        }  
         
        private void changeAppearance(object sender, SelectionChangedEventArgs e)
        {
            if(comboAccents.SelectedItem != null && comboThemes.SelectedItem != null)
            {
                try
                {
                    Functions.ChangeAppearance(comboAccents.SelectedItem.ToString(), comboThemes.SelectedItem.ToString());
                    log.Write("APPEARANCE CHANGED TO " + comboAccents.SelectedItem.ToString() + " " + comboThemes.SelectedItem.ToString(), Logger.WriterType.WriteMessage);
                    cfg.Settings.Find(x => x.Type == Config.SettingType.Theme).StringEntry = comboThemes.SelectedItem.ToString();
                    cfg.Settings.Find(x => x.Type == Config.SettingType.Theme).StringLength = (UInt16)comboThemes.SelectedItem.ToString().Length;
                    cfg.Settings.Find(x => x.Type == Config.SettingType.Accent).StringEntry = comboAccents.SelectedItem.ToString();
                    cfg.Settings.Find(x => x.Type == Config.SettingType.Accent).StringLength = (UInt16)comboAccents.SelectedItem.ToString().Length;
                }
                catch (Exception) { log.Write("ERROR APPEARANCE TO " + comboAccents.SelectedItem.ToString() + " " + comboThemes.SelectedItem.ToString(), Logger.WriterType.WriteError); }
            }
        }

        private void changeLoLPath(object sender, RoutedEventArgs e)
        {
            textLoLPath.Text = Functions.SelectFolder("Select your League of Legends folder");
        }

        private void tileWADExtractor_Click(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(gridSkinCreation, gridWADExtractor);
        }

        private void btnWADExtractorPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                wad = new WADFile(ofd.FileName, log, WADHashes);
                ecdsa = "";
                foreach(byte b in wad.header.ECDSA)
                {
                    ecdsa += b.ToString("X2");
                }
                dataWADExtractor.ItemsSource = wad.Entries;
                textWADExtractorPath.Text = ofd.FileName;
            }
        }

        private void btnWADExtractAll_Click(object sender, RoutedEventArgs e)
        {
            Functions.ExtractWAD(wad.Entries, WADHashes);
        }

        private void btnWADExtractSelected_Click(object sender, RoutedEventArgs e)
        {
            Functions.ExtractWAD(dataWADExtractor.SelectedItems, WADHashes);
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(gridWADExtractor.Visibility == Visibility.Visible && e.Key == Key.E)
            {
                MessageBox.Show(ecdsa);
            }
        }

        private void tileMOBEditor_Click(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(gridSkinCreation, gridMOBEditor);
        }

        private void btnMOBEditorPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "MOB File (*.mob)|*.mob";
            if (ofd.ShowDialog() == true)
            {
                mob = new MOBFile(ofd.FileName);
                dataMOBEditor.ItemsSource = mob.MobObjects;
                textMOBEditorPath.Text = ofd.FileName;
            }
        }

        private void btnMOBEditorSave_Click(object sender, RoutedEventArgs e)
        {
            Functions.SaveMOB(mob, dataMOBEditor.Items);
        }

        private void btnMOBEditorAddEntry_Click(object sender, RoutedEventArgs e)
        {
            Functions.AddMOBEntry(mob.MobObjects);
        }

        private void btnMOBEditorRemoveEntry_Click(object sender, RoutedEventArgs e)
        {
            Functions.RemoveMOBEntry(mob.MobObjects, dataMOBEditor.SelectedItems);
        }

        private void btnMOBEditorExtractEntries_Click(object sender, RoutedEventArgs e)
        {
            Functions.ExtractMOBEntries(dataMOBEditor.SelectedItems);
        }

        private void tileWPKEditor_Click(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(gridSkinCreation, gridWPKEditor);
        }

        private void btnWPKEditorPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "WPK File (*.wpk)|*.wpk";
            if (ofd.ShowDialog() == true)
            {
                wpk = new WPKFile(ofd.FileName);
                dataWPKEditor.ItemsSource = wpk.AudioFiles;
                textWPKEditorPath.Text = ofd.FileName;
            }
        }

        private void buttonWPKEditorExtractSelected_Click(object sender, RoutedEventArgs e)
        {
            string dir = Functions.SelectFolder("Select the folder where you want to export your files", cfg.Settings.Find(x => x.Type == Config.SettingType.WPKExtractionPath).StringEntry);
            foreach(WPKFile.AudioFile a in dataWPKEditor.SelectedItems)
            {
                Directory.CreateDirectory(dir + "\\" + System.IO.Path.GetFileNameWithoutExtension(wpk.fileLoc) + "\\");
                File.WriteAllBytes(dir + "\\" + System.IO.Path.GetFileNameWithoutExtension(wpk.fileLoc) + "\\" + a.Name, a.Data);
            }
        }

        private void btnLoLPath_Click(object sender, RoutedEventArgs e)
        {
            textLoLPath.Text = Functions.SelectFolder(btnLoLPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.LoLPath).StringEntry = textLoLPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.LoLPath).StringLength = (UInt16)textLoLPath.Text.Length;
        }

        private void btnWADPath_Click(object sender, RoutedEventArgs e)
        {
            textWADPath.Text = Functions.SelectFolder(btnWADPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.WADPath).StringEntry = textWADPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.WADPath).StringLength = (UInt16)textWADPath.Text.Length;
        }

        private void btnWADExtractionPath_Click(object sender, RoutedEventArgs e)
        {
            textWADExtractionPath.Text = Functions.SelectFolder(btnWADExtractionPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.WADExtractionPath).StringEntry = textWADExtractionPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.WADExtractionPath).StringLength = (UInt16)textWADExtractionPath.Text.Length;
        }

        private void btnMOBPath_Click(object sender, RoutedEventArgs e)
        {
            textMOBPath.Text = Functions.SelectFolder(btnMOBPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.MOBPath).StringEntry = textMOBPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.MOBPath).StringLength = (UInt16)textMOBPath.Text.Length;
        }

        private void btnMOBExtractionPath_Click(object sender, RoutedEventArgs e)
        {
            textMOBExtractionPath.Text = Functions.SelectFolder(btnMOBExtractionPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.MOBExtractionPath).StringEntry = textMOBExtractionPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.MOBExtractionPath).StringLength = (UInt16)textMOBExtractionPath.Text.Length;
        }

        private void btnMOBImportationPath_Click(object sender, RoutedEventArgs e)
        {
            textMOBImportationPath.Text = Functions.SelectFolder(btnMOBImportationPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.MOBImportationPath).StringEntry = textMOBImportationPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.MOBImportationPath).StringLength = (UInt16)textMOBImportationPath.Text.Length;
        }

        private void btnWPKPath_Click(object sender, RoutedEventArgs e)
        {
            textWPKPath.Text = Functions.SelectFolder(btnWPKPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.WPKPath).StringEntry = textWPKPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.WPKPath).StringLength = (UInt16)textWPKPath.Text.Length;
        }

        private void btnWPKExtractionPath_Click(object sender, RoutedEventArgs e)
        {
            textWPKExtractionPath.Text = Functions.SelectFolder(btnWPKExtractionPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.WPKExtractionPath).StringEntry = textWPKExtractionPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.WPKExtractionPath).StringLength = (UInt16)textWPKExtractionPath.Text.Length;
        }

        private void btnWPKImportationPath_Click(object sender, RoutedEventArgs e)
        {
            textWPKImportationPath.Text = Functions.SelectFolder(btnWPKImportationPath.ToolTip.ToString());
            cfg.Settings.Find(x => x.Type == Config.SettingType.WPKImportationPath).StringEntry = textWPKImportationPath.Text;
            cfg.Settings.Find(x => x.Type == Config.SettingType.WPKImportationPath).StringLength = (UInt16)textWPKImportationPath.Text.Length;
        }

        private void buttonHome(object sender, RoutedEventArgs e)
        {
            foreach (Grid g in sabre.Children)
            {
                g.Visibility = Visibility.Hidden;
            }
            main.Visibility = Visibility.Visible;
            Config.Write(cfg.Settings);
        }

        private void buttonSettings(object sender, RoutedEventArgs e)
        {
            foreach (Grid g in sabre.Children)
            {
                g.Visibility = Visibility.Hidden;
            }
            gridSettings.Visibility = Visibility.Visible;
            Config.Write(cfg.Settings);
        }

        private void buttonSkinCreation(object sender, RoutedEventArgs e)
        {
            foreach (Grid g in sabre.Children)
            {
                g.Visibility = Visibility.Hidden;
            }
            gridSkinCreation.Visibility = Visibility.Visible;
            Config.Write(cfg.Settings);
        }

        private void buttonSkinCollection(object sender, RoutedEventArgs e)
        {
            foreach (Grid g in sabre.Children)
            {
                g.Visibility = Visibility.Hidden;
            }
            gridSkinCollection.Visibility = Visibility.Visible;
            Config.Write(cfg.Settings);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Config.Write(cfg.Settings);
        }
    }
}
