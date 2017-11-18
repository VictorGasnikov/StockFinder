using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using StockFinder.Annotations;
using StockFinder.Windows;
using StockFinder.Windows.Scanner;

namespace StockFinder.Controls
{
    /// <summary>
    /// Interaction logic for ScannerListControl.xaml
    /// </summary>
    public partial class ScannerListControl : UserControl
    {
        public ScannerSettings Settings { get; set; }

        public ScannerListControl(ScannerSettings settings)
        {
            Settings = settings;
            settings.SettingsUpdated += OnSettingsUpdated;
            InitializeComponent();
            lblName.Text = Settings.Name;
        }

        private void OnSettingsUpdated()
        {
            lblName.Text = Settings.Name;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ScannerSettingsWindow settingsWindow = new ScannerSettingsWindow(Settings);
            if (settingsWindow.ShowDialog() == true)
            {
//                Settings = settingsWindow.Settings;

            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            new StockScanner(Settings).Show();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
             MainWindow.Instance.RemoveScanner(Settings, this);
        }
    }
}
