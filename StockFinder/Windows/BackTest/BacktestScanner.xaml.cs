using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using StockFinder.Technical;
using StockFinder.Technical.Strategies;

namespace StockFinder.Windows.BackTest
{
    /// <summary>
    /// Interaction logic for BulkAnalysis.xaml
    /// </summary>
    public partial class BulkAnalysis
    {

        //        public BulkAnalysis()
        //        {
        //        }
        public List<StockInfo> Stocks;

        public List<HistoricalPriceVolume> Graph = new List<HistoricalPriceVolume>();

        public BulkAnalysis(List<StockInfo> stocks)
        {
            InitializeComponent();
            Stocks = stocks;
            dtFrom.SelectedDate = DateTime.Now.AddYears(-2);
            dtTo.SelectedDate = DateTime.Now;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            DateTime from = dtFrom.SelectedDate.Value;
            DateTime to = dtTo.SelectedDate.Value;
            Bar.Maximum = Stocks.Count;
            Thread thread = new Thread(() =>
            {
                Scan(from, to);
            });
            thread.Start();
            //            DateTime from = dtFrom.SelectedDate.Value;
            //            DateTime to = dtTo.SelectedDate.Value;
            //            Bar.Maximum = Stocks.Count;
            //            int downloaded = 0;
            //            foreach (StockInfo stock in Stocks)
            //            {
            //                StockDownloader downloader = new StockDownloader();
            //                HistoricalPriceVolume graph = downloader.DownloadHistoricalData(stock.Symbol, from, to);
            //                AroonIndicator aroon = new AroonIndicator(5);
            //                aroon.Calculate(graph);
            //                //                Aroon aroon = new Aroon(5);
            //                //                aroon.Calculate(graph);
            //                Bar.Value = ++downloaded;
            //            }
        }

        private void Scan(DateTime from, DateTime to)
        {
            //            DateTime from = dtFrom.SelectedDate.Value;
            //            DateTime to = dtTo.SelectedDate.Value;
            List<BacktestResult> results = new List<BacktestResult>();
            StockDownloader downloader = new StockDownloader();
            AroonCrossStrategy strategy = new AroonCrossStrategy(14, 10000.00m);

//            foreach (StockInfo stock in Stocks)
//            {
//                try
//                {
//                    HistoricalPriceVolume graph = downloader.DownloadHistoricalData(stock.Symbol, from, to);
//
//                    BacktestResult result = strategy.BackTest(stock, graph);
//                    results.Add(result);
//                    //                AroonIndicator aroon = new AroonIndicator("aroon_5", 5);
//                    //                aroon.Calculate(graph);
//
//                    //                Aroon aroon = new Aroon(5);
//                    //                aroon.Calculate(graph);
//                    Dispatcher.Invoke(delegate
//                    {
//                        Bar.Value = ++downloaded;
//                        ProgressBox.AppendText($"Finished analysing data for {stock.Symbol}\n");
//                        ProgressBox.ScrollToEnd();
//                    });
//                }
//                catch (Exception)
//                {
//                    // ignored
//                }
//            }



            results.Clear();

            ConcurrentBag<string> graphs = new ConcurrentBag<string>();

            Parallel.ForEach(Stocks, stock =>
            {
                try
                {
//                    HistoricalPriceVolume graph = downloader.DownloadHistoricalData(stock.Symbol, from, to);
                    graphs.Add(downloader.DownloadHistoricalData(stock.Symbol, from, to));
                    //                    BacktestResult result = strategy.BackTest(stock, graph);
                    //                    results.Add(result);
                }
                catch (Exception)
                {
                    // ignored
                }
            });
//            Debug.WriteLine("par: " + (DateTime.Now - old));

            List<HistoricalPriceVolume> graphs2 = new List<HistoricalPriceVolume>();
//            foreach (StockInfo stock in Stocks)
//            {
//                try
//                {
//                    HistoricalPriceVolume graph = downloader.DownloadHistoricalData(stock.Symbol, from, to);
//                    graphs2.Add(graph);
//                    //                    BacktestResult result = strategy.BackTest(stock, graph);
//                    //                    results.Add(result);
//                }
//                catch (Exception)
//                {
//                    // ignored
//                }
//            }

            Dispatcher.Invoke(delegate
            {
                BacktestResults window = new BacktestResults(results);
                window.Show();
            });
        }
    }

    class StockGraph
    {
        public string Name;
        public HistoricalPriceVolume Graph;
    }
}
