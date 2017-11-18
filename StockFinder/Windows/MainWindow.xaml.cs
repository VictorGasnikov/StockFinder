using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using StockFinder.Controls;
using StockFinder.Windows.BackTest;
using StockFinder.Windows.Scanner;
using Timer = System.Timers.Timer;

namespace StockFinder.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public readonly Dictionary<string, DataGridTextColumn> GridColumns = new Dictionary<string, DataGridTextColumn>();
        private List<string> _shownFilters = new List<string>();

        private readonly OpenFileDialog _openDialog;
        private readonly SaveFileDialog _saveDialog;
        private string _saveLocation;

        public readonly AutoRefresher AutoRefresher;
        public readonly WatchListManager WatchListManager = new WatchListManager();
        public readonly ScannerManager ScannerManager = new ScannerManager();
        public readonly Dictionary<ScannerSettings, ScannerListControl> ScannerList = new Dictionary<ScannerSettings, ScannerListControl>();

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            Height = SystemParameters.WorkArea.Height / 1.3;
            Width = SystemParameters.WorkArea.Width / 1.3;

            EnvironmentSettings settings = new EnvironmentSettings
            {
                AutoRefreshRate = 30000
            };

            Application.Current.Properties["settings"] = settings;

            AutoRefresher = new AutoRefresher(settings.AutoRefreshRate);

            string filter = "Stock Scanner File|*.sss";
            _openDialog = new OpenFileDialog { Filter = filter };
            _saveDialog = new SaveFileDialog { Filter = filter };

            DateTime now = DateTime.Now.Date;
            DateTime minus10Years = new DateTime(now.Year - 10, now.Month, now.Day);
            dtTechAnalysisFrom.SelectedDate = new DateTime(now.Year - 2, now.Month, now.Day).Add(TimeSpan.FromDays(1));
            dtTechAnalysisFrom.DisplayDateStart = minus10Years;
            dtTechAnalysisTo.DisplayDateStart = minus10Years;

            AddGridColumn("symbol");
            AddGridColumn("price");
            //            foreach (KeyValuePair<string, StockProperty> entry in StockInfo.DefaultProperties)
            //            {
            //                var criteria = new FilterCriteria
            //                {
            //                    Label = entry.Value.Label,
            //                    Variable = entry.Key,
            //                    Min = 0,
            //                    Max = 1,
            //                    Visibility = Visibility.Collapsed
            //                };
            //                FilterStates.Children.Add(criteria);
            //            }

            AddFilter("price", true).IsUsed = true;
            AddFilter("pe_ratio", true).IsUsed = true;
            AddFilter("pb_ratio", true).IsUsed = true;
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            FindStocks();
        }

        public void FindStocksThreadSafe()
        {
            Dispatcher.Invoke(FindStocks);
        }

        public void FindStocks()
        {
            List<Filter> filters = GetCriteria().Where(criteria => criteria.IsUsed).Select(c => c.GetFilter()).ToList();
            Thread thread = new Thread(() => FindStocks(filters));
            thread.Start();
        }


        private void FindStocks(List<Filter> filters)
        {
            //string[] symbols = StockUtils.AllSymbols.ToArray();

            //List<StockInfo> stocks = new List<StockInfo>();
            //Dispatcher.Invoke(() => LblLoadProgress.Content = "Downloading Data");
            //foreach (IEnumerable<string> batch in StockUtils.Split(symbols, 500))
            //{
            //    string url = $"http://finance.yahoo.com/d/quotes.csv?s={string.Join(",", batch)}&f={"sl1r"}";
            //    WebRequest request = WebRequest.Create(url);
            //    request.Credentials = CredentialCache.DefaultCredentials;
            //    WebResponse response = request.GetResponse();
            //    Stream stream = response.GetResponseStream();
            //    string res = new StreamReader(stream).ReadToEnd();
            //    string[] lines = res.Split('\n');
            //    stocks.AddRange(
            //        lines.Take(lines.Length - 1)
            //            .TakeWhile(line => line != null)
            //            .Select(StockUtils.ParseCsvRow)
            //            .Select(
            //                vals =>
            //                    new StockInfo()
            //                    {
            //                        Ticker = vals[0],
            //                        LastTradePrice = NumberUtils.parseDouble(vals[1]),
            //                        Pe = NumberUtils.parseDouble(vals[2])
            //                    }));
            //}
            UpdateStatus($"Filtering stocks by {filters.Count} filters");
            IEnumerable<StockInfo> stocks = FilterManager.Filter(filters);
            UpdateStatus("Done");


            Dispatcher.Invoke(() =>
            {
                //                grid.DataContext = stocks;
                //grid.Items.Refresh();
                grid.Items.Clear();
                foreach (StockInfo stock in stocks)
                {
                    grid.Items.Add(stock);
                }
            });
        }

        public void UpdateStatus(string line)
        {
            Dispatcher.Invoke(() =>
            {
                LblLoadProgress.Text = line;
            });
        }

        public IEnumerable<FilterCriteria> GetCriteria()
        {
            return from e in Filters.Children.Cast<UIElement>() where e is FilterCriteria select e as FilterCriteria;
        }

        private void BtnLoadStocks_Click(object sender, RoutedEventArgs e)
        {
            //            OpenFileDialog dialog = new OpenFileDialog();
            //            if (dialog.ShowDialog() == true)
            //            {
            //                StreamReader file = new StreamReader(dialog.FileName);
            //                string line;
            //                StockUtils.AllSymbols.Clear();
            //                file.ReadLine();
            //                while ((line = file.ReadLine()) != null)
            //                {
            //                    string[] seg = line.Split('|');
            //                    StockUtils.AllSymbols.Add(seg[1]);
            //                }
            //                LblStocksLoaded.Content = $"Loaded {StockUtils.AllSymbols.Count} stock symbols";
            //            }
            //            Thread thread = new Thread(DownloadStocks);
            //            thread.Start();
            StockDownloadManager downloadManager = new StockDownloadManager();
            if (downloadManager.ShowDialog() == true)
            {
                AllowSearch = true;
                btnLoadStocks.Content = "Reload stock database";
                LblLoadProgress.Text = $"Loaded {downloadManager.Downloader.StocksLoaded} stocks";
            }
        }

        public bool AllowSearch
        {
            set
            {
                btnFindStocks.IsEnabled = value;
            }
        }

        //        private void DownloadStocks()
        //        {
        //            StockDownloader downloader = new StockDownloader();
        //            downloader.StatusUpdate += UpdateStatus;
        //            downloader.DownloadStocks();
        //        }

        public void ClearGridColumns()
        {
            grid.Columns.Clear();
            GridColumns.Clear();
        }

        public void AddGridColumn(string variable)
        {
            GridColumnWithVariable column = new GridColumnWithVariable { Header = StockInfo.AllPropertiesLabels[variable], Variable = variable, };
            Binding binding = new Binding($"Properties[{variable}]") { Mode = BindingMode.OneWay, StringFormat = "" };
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

        public bool SafeRemoveGridColumn(string variable)
        {
            try
            {
                RemoveGridColumn(variable);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void HideAllFilters()
        {
            Filters.Children.Clear();
            _shownFilters.Clear();
        }

        public void RemoveFilter(string variable)
        {
            //            if (StockInfo.DefaultProperties.ContainsKey(variable))
            //            {
            //                
            //            }
            //            else if (StockInfo.PropertyDefinitions.ContainsKey(variable))
            //            {
            //
            //            }
            foreach (FilterCriteria criteria in GetCriteria())
            {
                if (criteria.Variable == variable)
                {
                    RemoveFilter(criteria);
                    return;
                }
            }
        }

        public void RemoveFilter(FilterCriteria criteria)
        {
            Filters.Children.Remove(criteria);
            _shownFilters.Remove(criteria.Variable);
        }

        public FilterCriteria AddFilter(string variable, bool addToList)
        {
            //            foreach (FilterCriteria criteria in GetCriteria().Where(criteria => criteria.Variable.Equals(variable)))
            //            {
            ////                _shownFilters.Add(variable);
            //                criteria.Visibility = Visibility.Visible;
            //                return criteria;
            //            }
            StockProperty property;
            bool isCustom = false;
            if (StockInfo.DefaultProperties.ContainsKey(variable))
            {
                property = StockInfo.DefaultProperties[variable];
            }
            else if (StockInfo.PropertyDefinitions.ContainsKey(variable))
            {
                property = StockInfo.PropertyDefinitions[variable];
                isCustom = true;
            }
            else
            {
                return null;
            }
            FilterCriteria criteria = new FilterCriteria(variable, property.Label, property.DefaultFilterMinValue, property.DefaultFilterMaxValue, true, isCustom);
            Filters.Children.Add(criteria);
            if (addToList)
            {
                _shownFilters.Add(variable);
            }
            return criteria;
        }

        private void btnEditColumns_Click(object sender, RoutedEventArgs e)
        {
            StockPropertySelector selector = new StockPropertySelector(GridColumns.Keys.ToList());
            if (selector.ShowDialog() == true)
            {
                IEnumerable<string> newList = selector.GetSelectedProperties().ToList();
                IEnumerable<string> oldList = GridColumns.Keys.ToList();
                //                ClearGridColumns();
                IEnumerable<string> add = newList.Except(oldList);
                IEnumerable<string> remove = oldList.Except(newList);

                foreach (string variable in remove)
                {
                    RemoveGridColumn(variable);
                }

                foreach (string columnVariable in add)
                {
                    AddGridColumn(columnVariable);
                }
            }
        }

        private void BtnCustomizeFilters_OnClick(object sender, RoutedEventArgs e)
        {
            StockPropertySelector selector = new StockPropertySelector(_shownFilters, true);
            if (selector.ShowDialog() == true)
            {
                List<string> newList = selector.GetSelectedProperties().ToList();
                IEnumerable<string> oldList = _shownFilters;
                IEnumerable<string> add = newList.Except(oldList);
                List<string> remove = oldList.Except(newList).ToList();

                _shownFilters = newList;

                List<FilterCriteria> criterias = GetCriteria().ToList();

                foreach (FilterCriteria criteria in criterias)
                {
                    if (remove.Contains(criteria.Variable))
                    {
                        Filters.Children.Remove(criteria);
                    }
                }
                foreach (string variable in add)
                {
                    AddFilter(variable, false);
                }
            }
        }

        private void btnAddCustomVariable_Click(object sender, RoutedEventArgs e)
        {
            VariableEditor editor = new VariableEditor();
            if (editor.ShowDialog() == true)
            {
                VariableListitem variable = new VariableListitem(editor.Variable, editor.Formula, editor.Label, editor.AllowFiltering);
                CustomVariables.Children.Add(variable);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnOpenInMorningStar_Click(object sender, RoutedEventArgs e)
        {
            foreach (StockInfo stock in grid.SelectedItems)
            {
                Process.Start($"http://quote.morningstar.ca/quicktakes/Stock/s_ca.aspx?t={stock.Symbol}&region=USA");
            }
        }

        private void btnOpenInNASDAQ_Click(object sender, RoutedEventArgs e)
        {
            foreach (StockInfo stock in grid.SelectedItems)
            {
                Process.Start($"http://www.nasdaq.com/symbol/{stock.Symbol}/real-time");
            }
        }

        private void btnOpenInYahoo_Click(object sender, RoutedEventArgs e)
        {
            foreach (StockInfo stock in grid.SelectedItems)
            {
                Process.Start($"https://ca.finance.yahoo.com/q?s={stock.Symbol}");
            }
        }

        private void btnOpenInTradingView_Click(object sender, RoutedEventArgs e)
        {
            foreach (StockInfo stock in grid.SelectedItems)
            {
                Process.Start($"https://www.tradingview.com/chart/?symbol={stock.Symbol}");
            }
        }

        private void btnAnalyseWithAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            //                HistoricalPriceVolume graph = StockDownloader.DownloadHistoricalData(stock.Symbol, null, null);
            var stocks = grid.SelectedItems.Cast<StockInfo>().ToList();
            //            if (stocks == null)
            //            {
            //                throw new Exception("Null!");
            //            }
            BulkAnalysis dialogAnalysis = new BulkAnalysis(stocks);
            if (dialogAnalysis.ShowDialog() == true)
            {

            }
        }

        private void MenuAction_Open(object sender, RoutedEventArgs e)
        {
            if (_openDialog.ShowDialog() == true)
            {
                StockFinderSettings settings = StockFinderSettings.Load(_saveLocation = _openDialog.FileName);
                if (settings != null)
                {
                    HideAllFilters();
                    ClearGridColumns();
                    foreach (UIElement element in CustomVariables.Children.Cast<UIElement>().ToList())
                    {
                        VariableListitem listitem = element as VariableListitem;
                        listitem?.Remove();
                    }
                    StockInfo.PropertyDefinitions.Clear();


                    foreach (CustomStockProperty property in settings.CustomProperties)
                    {
                        VariableListitem listitem = new VariableListitem(property.Variable, property.Formula, property.Label, property.AllowFiltering);
                        CustomVariables.Children.Add(listitem);
                    }
                    foreach (FilterState filter in settings.Filters)
                    {
                        FilterCriteria criteria = AddFilter(filter.Variable, true);
                        criteria.IsUsed = filter.IsUsed;
                        criteria.Min = filter.Min;
                        criteria.Max = filter.Max;
                    }
                    foreach (GridColumnState column in settings.Columns.OrderBy(col => col.Id))
                    {
                        AddGridColumn(column.Variable);
                    }
                    ClearAllScanners();
                    foreach (ScannerSettings scanner in settings.Scanners)
                    {
                        AddScanner(scanner);
                    }
                }
            }
        }

        private void MenuAction_Save(object sender, RoutedEventArgs e)
        {
            if (_saveLocation != null)
            {
                SaveFile(_saveLocation);
            }
            else
            {
                SaveAs();
            }
        }

        private void SaveFile(string location)
        {
            List<FilterState> filterStates = GetCriteria().Select(criteria => new FilterState
            {
                Variable = criteria.Variable,
                Min = criteria.Min,
                Max = criteria.Max,
                IsUsed = criteria.IsUsed
            }).ToList();

            List<GridColumnState> columnStates = grid.Columns.Cast<GridColumnWithVariable>().Select(col => new GridColumnState { Id = col.DisplayIndex, Variable = col.Variable }).ToList();

            StockFinderSettings settings = new StockFinderSettings
            {
                CustomProperties = StockInfo.PropertyDefinitions.Values.ToList(),
                Filters = filterStates,
                Columns = columnStates,
                Scanners = ScannerManager.Scanners
            };
            settings.Save(_saveLocation = location);
        }

        private void SaveAs()
        {

            if (_saveDialog.ShowDialog() == true)
            {
                SaveFile(_saveDialog.FileName);
            }
        }

        private void MenuAction_SaveAs(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void MenuAction_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnReloadAndFind_Click(object sender, RoutedEventArgs e)
        {
            ReloadAndFind();
        }

        public void ReloadAndFind()
        {
            Thread thread = new Thread(DownloadStocks);
            thread.Start();
        }

        private void DownloadStocks()
        {
            Dispatcher.Invoke(() =>
            {
                AllowSearch = false;
                LblLoadProgress.Text = "Reloading Database...";
            });
            StockDownloader downloader = new StockDownloader();
            downloader.DownloadComplete += Complete;
            downloader.DownloadStocks();
        }

        private void Complete(StockDownloader downloader)
        {
            Dispatcher.Invoke(() =>
            {
                AllowSearch = true;
                FindStocks();
            });
        }

        private void MenuAction_Options(object sender, RoutedEventArgs e)
        {
            Options options = new Options();
            if (options.ShowDialog() == true)
            {
                // TODO implement options
            }
        }

        private void btnToggleAutoRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!AutoRefresher.RefreshMain)
            {
                btnToggleAutoRefresh.Content = "Auto Refresh: On";
                //                _autoRefresher = new AutoRefresher(((EnvironmentSettings)Application.Current.Properties["settings"]).AutoRefreshRate);
                AutoRefresher.RefreshMain = true;
            }
            else
            {
                btnToggleAutoRefresh.Content = "Auto Refresh: Off";
                AutoRefresher.RefreshMain = false;
            }
        }

        private void btnAddScanner_Click(object sender, RoutedEventArgs e)
        {
            ScannerSettingsWindow settingsWindow = new ScannerSettingsWindow();
            if (settingsWindow.ShowDialog() == true)
            {
                AddScanner(settingsWindow.Settings);
            }
        }

        public void AddScanner(ScannerSettings settings)
        {
            ScannerManager.Scanners.Add(settings);
            ScannerListControl control = new ScannerListControl(settings);
            ScannerList.Add(settings, control);
            Scanners.Children.Add(control);
        }

        public void RemoveScanner(ScannerSettings settings)
        {
            RemoveScanner(settings, ScannerList[settings]);
        }

        public void RemoveScanner(ScannerSettings settings, ScannerListControl control)
        {
            ScannerManager.Scanners.Remove(settings);
            Scanners.Children.Remove(control);
        }

        public void ClearAllScanners()
        {
            ScannerManager.ClearAll();
            ScannerList.Clear();
            Scanners.Children.Clear();
        }

        private void btnAddStockToWatchlist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddToWatchList_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (StockInfo stock in grid.SelectedItems)
            {
                WatchListStock watchListStock = WatchListManager.Add(stock.Symbol);
                if (watchListStock != null)
                {
                    watchListStock.Ask = (decimal)stock.Properties["ask"];
                    watchListStock.Bid = (decimal)stock.Properties["bid"];
                }
                else
                {
                    MessageBox.Show($"Stock {stock.Symbol} is already in the watch list!", "Watch list duplicate");
                }
            }
        }

        private void BtnClearStockWatchlist_OnClick(object sender, RoutedEventArgs e)
        {
            WatchListManager.Clear();
        }
    }

    public class AutoRefresher : IDisposable
    {
        private int _interval;
        private bool _refreshMain;
        public readonly Timer Timer;

        private event Action refresh = () => { };
        public event Action Refresh
        {
            add
            {
                refresh += value;
                if (refresh.GetInvocationList().Length > 0)
                {
                    Timer.Enabled = true;
                }
            }
            remove
            {
                refresh -= value;
                if (refresh.GetInvocationList().Length == 0)
                {
                    Timer.Enabled = false;
                }
            }
        }

        public int Interval
        {
            get { return _interval; }
            set
            {

                _interval = value;
                Timer.Interval = _interval;
            }
        }

        public bool RefreshMain
        {
            get
            {
                return _refreshMain;
            }
            set
            {
                _refreshMain = value;
                if (_refreshMain)
                {
                    Refresh += MainWindow.Instance.FindStocksThreadSafe;
                }
                else
                {
                    Refresh -= MainWindow.Instance.FindStocksThreadSafe;
                }
            }
        }
        public AutoRefresher(int interval)
        {
            Timer = new Timer(interval);
            Timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            DownloadStocks();
            refresh();
        }

        private void DownloadStocks()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.Instance.AllowSearch = false;
                MainWindow.Instance.LblLoadProgress.Text = "Reloading Database...";
            });
            StockDownloader downloader = new StockDownloader();
            downloader.DownloadComplete += Complete;
            downloader.DownloadStocks();
        }

        private void Complete(StockDownloader downloader)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.Instance.AllowSearch = true;
                MainWindow.Instance.LblLoadProgress.Text = $"Loaded {StockUtils.AllSymbols.Count} Stocks";
                refresh();
            });
        }

        public void Dispose()
        {
            Timer.Stop();
            Timer.Dispose();
        }
    }

    public class WatchListManager
    {
        private readonly Dictionary<string, WatchListStock> _watchList = new Dictionary<string, WatchListStock>();
        private readonly Timer _timer = new Timer(500);

        public WatchListManager()
        {
            _timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            StockDownloader downloader = new StockDownloader();
            downloader.GetBidAskQuote(_watchList.Keys.ToArray());
            foreach (Tuple<string, decimal, int, decimal, int> tuple in downloader.GetBidAskQuote(_watchList.Keys.ToArray()))
            {
                WatchListStock stock = _watchList[tuple.Item1];
                stock.Bid = tuple.Item2;
                stock.BidSize = tuple.Item3;
                stock.Ask = tuple.Item4;
                stock.AskSize = tuple.Item5;
            }
        }

        public WatchListStock Add(string symbol)
        {
            if (_watchList.ContainsKey(symbol))
            {
                return null;
            }
            WatchListStock watchListStock = new WatchListStock(symbol);
            MainWindow.Instance.Watchlist.Children.Add(watchListStock);
            _watchList.Add(symbol, watchListStock);
            _timer.Enabled = true;
            return watchListStock;
        }

        public void Remove(string symbol)
        {
            MainWindow.Instance.Watchlist.Children.Remove(_watchList[symbol]);
            _watchList.Remove(symbol);
            if (_watchList.Count == 0)
            {
                _timer.Enabled = false;
            }
        }

        public void Clear()
        {
            _timer.Enabled = false;
            MainWindow.Instance.Watchlist.Children.Clear();
            _watchList.Clear();
        }
    }
}
