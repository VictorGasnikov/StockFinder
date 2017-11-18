using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using StockFinder.Technical.Strategies;

namespace StockFinder.Windows.BackTest
{
    /// <summary>
    /// Interaction logic for BacktestResults.xaml
    /// </summary>
    public partial class BacktestResults : Window
    {
        public BacktestResults(List<BacktestResult> results)
        {
            InitializeComponent();
            foreach (BacktestResult result in results)
            {
                grid.Items.Add(result);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BacktestResult result = ((FrameworkElement)sender).DataContext as BacktestResult;
            Debug.WriteLine(result.Stock.Symbol);
            BacktestTradesLog log = new BacktestTradesLog(result);
            log.Show();
        }
    }
}
