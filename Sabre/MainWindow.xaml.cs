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

namespace Sabre
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Logger log;
        public MainWindow()
        {
            log = new Logger(DateTime.Now.ToString("HH-mm-ss"));
            log.Write("LOGGER INITIALIZED", Logger.WriterType.WriteMessage);
            InitializeComponent();
            log.Write("SABRE INITIALIZED", Logger.WriterType.WriteMessage);
            Functions.LoadSettings(this);
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
                    log.Write("APPEARANCE CHANGED TO " + comboAccents.SelectedItem.ToString() + " " + comboThemes.SelectedItem.ToString(), Logger.WriterType.WriteMessage);
                }
                catch (Exception) { log.Write("ERROR APPEARANCE TO " + comboAccents.SelectedItem.ToString() + " " + comboThemes.SelectedItem.ToString(), Logger.WriterType.WriteError); }
            }
        }
    }
}
