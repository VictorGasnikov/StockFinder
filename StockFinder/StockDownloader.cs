using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Data;
using StockFinder.Technical;
using StockFinder.Technical.Indicators;

namespace StockFinder
{
    public delegate void StatusUpdate(string line);

    public class StockDownloader
    {
        public event StatusUpdate StatusUpdate = line => { };
        public event Action<StockDownloader> DownloadComplete = downloader => { };

        public int StocksLoaded { get; private set; }

        #region declarations

        private const string YahooFormat = "a" +
                                           "b" +
                                           "p" +
                                           "o" +
                                           "y" +
                                           "d" +
                                           "r1" +
                                           "q" +
                                           "c1" +
                                           "p2" +
                                           "d1" +
                                           "t1" +
                                           "c8" +
                                           "c3" +
                                           "g" +
                                           "h" +
                                           "l1" +
                                           "t8" +
                                           "m3" +
                                           "m4" +
                                           "m5" +
                                           "m6" +
                                           "m7" +
                                           "m8" +
                                           "s6" +
                                           "k" +
                                           "j" +
                                           "j5" +
                                           "k4" +
                                           "j6" +
                                           "k5" +
                                           "j1" +
                                           "f6" +
                                           "n" +
                                           "s" +
                                           "x" +
                                           "j2" +
                                           "v" +
                                           "a5" +
                                           "b6" +
                                           "k3" +
                                           "a2" +
                                           "e" +
                                           "e7" +
                                           "e8" +
                                           "e9" +
                                           "b4" +
                                           "j4" +
                                           "p5" +
                                           "p6" +
                                           "r" +
                                           "r5" +
                                           "r6" +
                                           "r7" +
                                           "s7" +
                                           "";

        #endregion

        public void DownloadStocks()
        {
            try
            {
                //                List<string> symbols = new List<string>();
                StatusUpdate("Searching for symbols");

                if (StockUtils.AllSymbols.Count == 0)
                {
                    //                    foreach (string se in new[] { "amex" })
                    foreach (string se in new[] { "nasdaq", "nyse", "amex" })
                    //                    foreach (string se in new[] { "nasdaq" })
                    {
                        List<string> currentTickers = GetTickersFor(se);
                        StatusUpdate($"Received {currentTickers.Count} symbols from {se}");
                        StockUtils.AllSymbols.AddRange(currentTickers);
                    }
                }

                StatusUpdate($"Downloading data for {StockUtils.AllSymbols.Count} stocks");
                StockUtils.AllStocks.Clear();
                StockUtils.AllStocks.AddRange(DownloadStockData(StockUtils.AllSymbols.ToArray()));

                StocksLoaded = StockUtils.AllStocks.Count;

                StatusUpdate($"Built a database for {StocksLoaded} stocks");
                DownloadComplete(this);
            }
            catch (WebException)
            {

            }
        }

        public Dictionary<string, string[]> GetDataFromYahoo(string[] symbols, string format)
        {
            string url = $"http://finance.yahoo.com/d/quotes.csv?s={string.Join(",", symbols)}&f=s{format}";
            WebRequest request = WebRequest.Create(url);
            //            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            string res = new StreamReader(stream).ReadToEnd();
            string[] lines = res.Split('\n');
            return
                lines.Take(lines.Length - 1)
                    .TakeWhile(line => line != null)
                    .Select(StockUtils.ParseCsvRow)
                    .ToDictionary(data => data[0], data => data.Skip(1).ToArray());
        }

        public IEnumerable<Tuple<string, decimal, int, decimal, int>> GetBidAskQuote(string[] symbols)
        {
            return GetDataFromYahoo(symbols, "bb6aa5").Select(pair => new Tuple<string, decimal, int, decimal, int>(pair.Key, NumberUtils.ParseDecimal(pair.Value[0]), NumberUtils.ParseInt(pair.Value[1]), NumberUtils.ParseDecimal(pair.Value[2]), NumberUtils.ParseInt(pair.Value[3])));
        }

        public string DownloadHistoricalData(string symbol, DateTime from, DateTime to)
        {
            string url = $"http://real-chart.finance.yahoo.com/table.csv?s={symbol}&d={to.Month - 1}&e={to.Day}&f={to.Year}&g=d&a={from.Month - 1}&b={from.Day}&c={from.Year}&ignore=.csv";
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            reader.ReadLine();
            return /*string res =*/ reader.ReadToEnd();
            //            string[] lines = res.Split('\n');
            //            HistoricalPriceVolume data = new HistoricalPriceVolume(lines.Take(lines.Length - 1)
            //                .TakeWhile(line => line != null)
            //                .Select(StockUtils.ParseCsvRow).Select(line => new HistoricalPriceVolumeEntry
            //                {
            //                    Date = DateTime.Parse(line[0]),
            //                    Open = NumberUtils.ParseDecimal(line[1]),
            //                    High = NumberUtils.ParseDecimal(line[2]),
            //                    Low = NumberUtils.ParseDecimal(line[3]),
            //                    Close = NumberUtils.ParseDecimal(line[4]),
            //                    Volume = NumberUtils.ParseInt(line[5])
            //
            //                }) /*.Reverse()*/.ToList());
            //
            //
            //            return data;
        }

        public HistoricalPriceVolume ParseHistoricalData(string res)
        {
            string[] lines = res.Split('\n');
            HistoricalPriceVolume data = new HistoricalPriceVolume(lines.Take(lines.Length - 1)
                .TakeWhile(line => line != null)
                .Select(StockUtils.ParseCsvRow).Select(line => new HistoricalPriceVolumeEntry
                {
                    Date = DateTime.Parse(line[0]),
                    Open = NumberUtils.ParseDecimal(line[1]),
                    High = NumberUtils.ParseDecimal(line[2]),
                    Low = NumberUtils.ParseDecimal(line[3]),
                    Close = NumberUtils.ParseDecimal(line[4]),
                    Volume = NumberUtils.ParseInt(line[5])

                }) /*.Reverse()*/.ToList());


            return data;
        }

        public List<StockInfo> DownloadStockData(string[] symbols)
        {
            ConcurrentBag<IEnumerable<StockInfo>> stocks = new ConcurrentBag<IEnumerable<StockInfo>>();
            Parallel.ForEach(
                StockUtils.Split(symbols.Where(symbol => !(symbol.Contains("^") || symbol.Contains("."))).ToArray(), 500),
                batch =>
                {
                    string url = $"http://finance.yahoo.com/d/quotes.csv?s={string.Join(",", batch)}&f={YahooFormat}";
                    // Symbol, Name, Price, EPS, P/E, Book Value, P/B, Volume, 
                    WebRequest request = WebRequest.Create(url);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    string res = new StreamReader(stream).ReadToEnd();
                    string[] lines = res.Split('\n');
                    stocks.Add(
                        lines.Take(lines.Length - 1)
                            .TakeWhile(line => line != null)
                            .Select(StockUtils.ParseCsvRow)
                            .Select(vals => new StockInfo(vals)));
                });

            //            IEnumerator<IEnumerable<string>> a =
            //                StockUtils.Split(symbols.Where(symbol => !(symbol.Contains("^") || symbol.Contains("."))).ToArray(), 500).GetEnumerator();
            //            a.MoveNext();
            //            IEnumerable<string> batch = a.Current;
            //
            //            //                string url = $"http://finance.yahoo.com/d/quotes.csv?s={string.Join(",", batch)}&f={"snl1erb4p6v"}"; // Symbol, Name, Price, EPS, P/E, Book Value, P/B, Volume, 
            //            string url = $"http://finance.yahoo.com/d/quotes.csv?s={string.Join(",", batch)}&f={YahooFormat}"; // Symbol, Name, Price, EPS, P/E, Book Value, P/B, Volume, 
            //            WebRequest request = WebRequest.Create(url);
            //            request.Credentials = CredentialCache.DefaultCredentials;
            //            WebResponse response = request.GetResponse();
            //            Stream stream = response.GetResponseStream();
            //            string res = new StreamReader(stream).ReadToEnd();
            //            string[] lines = res.Split('\n');
            //            stocks.Add(
            //                lines.Take(lines.Length - 1)
            //                    .TakeWhile(line => line != null)
            //                    .Select(StockUtils.ParseCsvRow)
            //                    .Select(vals => new StockInfo(vals)));
            //            foreach (IEnumerable<string> batch in StockUtils.Split(symbols.Where(symbol => !(symbol.Contains("^") || symbol.Contains("."))).ToArray(), 500))
            //            {
            //                //                string url = $"http://finance.yahoo.com/d/quotes.csv?s={string.Join(",", batch)}&f={"snl1erb4p6v"}"; // Symbol, Name, Price, EPS, P/E, Book Value, P/B, Volume, 
            //                string url = $"http://finance.yahoo.com/d/quotes.csv?s={string.Join(",", batch)}&f={YahooFormat}"; // Symbol, Name, Price, EPS, P/E, Book Value, P/B, Volume, 
            //                WebRequest request = WebRequest.Create(url);
            //                request.Credentials = CredentialCache.DefaultCredentials;
            //                WebResponse response = request.GetResponse();
            //                Stream stream = response.GetResponseStream();
            //                string res = new StreamReader(stream).ReadToEnd();
            //                string[] lines = res.Split('\n');
            //                stocks.Add(
            //                    lines.Take(lines.Length - 1)
            //                        .TakeWhile(line => line != null)
            //                        .Select(StockUtils.ParseCsvRow)
            //                        .Select(vals => new StockInfo(vals)));
            //            }

            IEnumerable<StockInfo> infos = stocks.SelectMany(s => s).ToList();
            return infos.Where(stock => stock.IsValid()).ToList();
        }

        private List<string> GetTickersFor(string market)
        {
            string url =
                $"http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange={market}&render=download";
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            reader.ReadLine();
            string res = reader.ReadToEnd();
            string[] lines = res.Split('\n');
            return new List<string>(lines.Select(line => StockUtils.ParseCsvRow(line)[0]));
        }
    }
}