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
        private Logger log;
        private Config cfg;
        private string ecdsa;
        private WADFile wad;
        private MOBFile mob;
        public List<string> WADHashes = new List<string>();
        public MainWindow()
        {
            log = new Logger(DateTime.Now.ToString("HH-mm-ss"));
            log.Write("LOGGER INITIALIZED", Logger.WriterType.WriteMessage);
            InitializeComponent();
            log.Write("SABRE INITIALIZED", Logger.WriterType.WriteMessage);
            cfg = new Config("config.ini", log);
            Functions.LoadSettings(cfg, this);
            WADHashes = HASH.GetWADHashes(REST.GetCharacters(true), Environment.CurrentDirectory, 15);
            string tmp = Hash.XXHash("DATA/Characters/Aatrox/Skins/Skin0.bin"); // X2 - F2061FA001024CF7 X - F261FA0124CF7
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
        private void buttonHome(object sender, RoutedEventArgs e)
        {
            foreach(Grid g in sabre.Children)
            {
                g.Visibility = Visibility.Hidden;
            }
            main.Visibility = Visibility.Visible;
        }
         
        private void changeAppearance(object sender, SelectionChangedEventArgs e)
        {
            if(comboAccents.SelectedItem != null && comboThemes.SelectedItem != null)
            {
                try
                {
                    Functions.ChangeAppearance(comboAccents.SelectedItem.ToString(), comboThemes.SelectedItem.ToString());
                    Config.Write(comboThemes.SelectedItem.ToString(), comboAccents.SelectedItem.ToString(), textLoLPath.Text);
                    log.Write("APPEARANCE CHANGED TO " + comboAccents.SelectedItem.ToString() + " " + comboThemes.SelectedItem.ToString(), Logger.WriterType.WriteMessage);
                }
                catch (Exception) { log.Write("ERROR APPEARANCE TO " + comboAccents.SelectedItem.ToString() + " " + comboThemes.SelectedItem.ToString(), Logger.WriterType.WriteError); }
            }
        }

        private void changeLoLPath(object sender, RoutedEventArgs e)
        {
            textLoLPath.Text = Functions.SelectFolder("Select your League of Legends folder");
            Config.Write(comboThemes.SelectedItem.ToString(), comboAccents.SelectedItem.ToString(), textLoLPath.Text);
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
    }
}
