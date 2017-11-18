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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace StockFinder.Windows.Scanner
{
    /// <summary>
    /// Interaction logic for ScannerSettingsWindow.xaml
    /// </summary>
    public partial class ScannerSettingsWindow : Window
    {

        public readonly ScannerSettings Settings;

        public ScannerSettingsWindow(ScannerSettings settings)
        {
            Settings = settings;
            InitializeComponent();
            Topmost = settings.AlwaysOnTop;
            txtName.Text = settings.Name;
            chkAlwaysOnTop.IsChecked = settings.AlwaysOnTop;

        }

        public ScannerSettingsWindow() : this(new ScannerSettings())
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //            ScannerSettings settings = new ScannerSettings();
            Settings.Name = txtName.Text;
            Settings.AlwaysOnTop = chkAlwaysOnTop.IsChecked.Value;
            Settings.UpdateSettings();
            DialogResult = true;
        }

        private void btnEditColumns_Click(object sender, RoutedEventArgs e)
        {
            List<string> variables = Settings.Columns.Select(c => c.Variable).ToList();
            StockPropertySelector selector = new StockPropertySelector(variables) { Topmost = Settings.AlwaysOnTop };
            if (selector.ShowDialog() == true)
            {
                List<string> newVariables = selector.GetSelectedProperties().ToList();
                List<string> remove = variables.Except(newVariables).ToList();
                IEnumerable<string> add = newVariables.Except(variables);

                //                foreach (string variable in remove)
                //                {
                //                    variables.Remove(variable);
                //                }
                variables = variables.Except(remove).ToList();
                variables.AddRange(add);
                Settings.Columns.Clear();
                int columnId = 0;
                Settings.Columns.AddRange(variables.Select(v => new GridColumnState
                {
                    Id = columnId++,
                    Variable = v
                }));
            }
        }

        private void btnEditFilters_Click(object sender, RoutedEventArgs e)
        {
            ScannerFilter filter = new ScannerFilter(Settings.FilterStates) { Topmost = Settings.AlwaysOnTop };
            if (filter.ShowDialog() == true)
            {
                Settings.FilterStates.Clear();
                Settings.FilterStates.AddRange(filter.GetFilterStates());
            }
        }
    }

    public class ScannerSettings
    {
        public List<GridColumnState> Columns;

        [XmlIgnore]
        public List<Filter> Filters;
        public List<FilterState> FilterStates;
        public string Name { get; set; }
        public bool AlwaysOnTop { get; set; }

        public event Action SettingsUpdated = () => { };

        public void UpdateSettings()
        {
            SettingsUpdated();
        }

        public ScannerSettings()
        {
            Columns = new List<GridColumnState>();
            FilterStates = new List<FilterState>();
            Filters = new List<Filter>();
        }
    }
}
