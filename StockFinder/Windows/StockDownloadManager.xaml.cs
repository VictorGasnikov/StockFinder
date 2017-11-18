using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockFinder.Windows
{
    /// <summary>
    /// Interaction logic for StockDownloadManager.xaml
    /// </summary>
    public partial class StockDownloadManager : Window
    {

        public StockDownloader Downloader { get; private set; }
        public StockDownloadManager()
        {
            InitializeComponent();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
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
            Thread thread = new Thread(DownloadStocks);
            thread.Start();
        }

        private void DownloadStocks()
        {
            Dispatcher.Invoke(() =>
            {
                btnCancel.IsEnabled = false;
                btnDownload.IsEnabled = false;
            });
            Downloader = new StockDownloader();
            Downloader.StatusUpdate += UpdateStatus;
            Downloader.DownloadComplete += Complete;
            Downloader.DownloadStocks();
        }

        private void Complete(StockDownloader downloader)
        {
            Dispatcher.Invoke(() =>
            {
                btnDownload.Click -= btnDownload_Click;
                btnDownload.Click += (sender, args) => DialogResult = true;
                btnDownload.Content = "Finish";
                btnDownload.IsEnabled = true;
            });

        }

        public void UpdateStatus(string line)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBox.AppendText(line + "\n");
                ProgressBox.ScrollToEnd();
            });
        }
    }
}
