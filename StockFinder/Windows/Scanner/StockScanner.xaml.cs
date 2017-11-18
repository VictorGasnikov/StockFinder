using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using StockFinder.Controls;

namespace StockFinder.Windows.Scanner
{
    /// <summary>
    /// Interaction logic for StockScanner.xaml
    /// </summary>
    public partial class StockScanner : Window
    {

        private ScannerSettings _settings;
        public readonly Dictionary<string, DataGridTextColumn> GridColumns = new Dictionary<string, DataGridTextColumn>();
        public StockScanner(ScannerSettings settings)
        {
            InitializeComponent();
            LoadSettings(settings);
            MainWindow.Instance.AutoRefresher.Refresh += AutoRefresherOnRefresh;
        }

        private void AutoRefresherOnRefresh()
        {
            FindStocks(_settings.Filters);
        }

        private void btnAlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {
            Topmost = _settings.AlwaysOnTop = (bool)btnAlwaysOnTop.IsChecked;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            ScannerSettingsWindow settings = new ScannerSettingsWindow(_settings);
            if (settings.ShowDialog() == true)
            {
                LoadSettings(settings.Settings);
            }
        }

        public void LoadSettings(ScannerSettings settings)
        {
            _settings = settings;
            Title = _settings.Name;
            btnAlwaysOnTop.IsChecked = _settings.AlwaysOnTop;
            Topmost = _settings.AlwaysOnTop;
            ClearGridColumns();
            foreach (GridColumnState column in _settings.Columns.OrderBy(c => c.Id))
            {
                AddGridColumn(column.Variable);
            }
        }

        public ScannerSettings SaveSettings()
        {
            _settings.Columns = grid.Columns.Cast<GridColumnWithVariable>().Select(col => new GridColumnState
            {
                Id = col.DisplayIndex,
                Variable = col.Variable
            }).ToList();
            _settings.AlwaysOnTop = btnAlwaysOnTop.IsChecked.Value;
            return _settings;
        }

        public void ClearGridColumns()
        {
            grid.Columns.Clear();
            GridColumns.Clear();
        }

        public void AddGridColumn(string variable)
        {
            GridColumnWithVariable column = new GridColumnWithVariable { Header = StockInfo.AllPropertiesLabels[variable], Variable = variable };
            Binding binding = new Binding($"Properties[{variable}]") { Mode = BindingMode.OneWay };
            column.Binding = binding;
            GridColumns.Add(variable, column);
            grid.Columns.Add(column);
        }

        public void RemoveGridColumn(string variable)
        {
            DataGridTextColumn column = GridColumns[variable];
            GridColumns.Remove(variable);
            grid.Columns.Remove(column);
        }
//        public void FindStocks()
//        {
//            Thread thread = new Thread(() => FindStocks(_settings.Filters));
//            thread.Start();
//        }


        private void FindStocks(List<Filter> filters)
        {
//            UpdateStatus($"Filtering stocks by {filters.Count} filters");
            IEnumerable<StockInfo> stocks = FilterManager.Filter(filters);
//            UpdateStatus("Done");

            Dispatcher.Invoke(() =>
            {
                grid.Items.Clear();
                foreach (StockInfo stock in stocks)
                {
                    grid.Items.Add(stock);
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.Instance.AutoRefresher.Refresh -= AutoRefresherOnRefresh;
        }

        //        public void UpdateStatus(string line)
        //        {
        ////            Dispatcher.Invoke(() =>
        ////            {
        //////                LblLoadProgress.Text = line;
        ////            });
        //        }
    }
}
