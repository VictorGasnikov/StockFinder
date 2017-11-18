using System.Windows;
using StockFinder.Technical.Strategies;

namespace StockFinder.Windows.BackTest
{
    /// <summary>
    /// Interaction logic for BacktestTradesLog.xaml
    /// </summary>
    public partial class BacktestTradesLog : Window
    {
        public BacktestTradesLog(BacktestResult result)
        {
            InitializeComponent();
            Title = $"Backtest trade log - {result.Stock.Properties["name"]} ({result.Stock.Symbol})";
            foreach (TradeInfo trade in result.Trades)
            {
                grid.Items.Add(trade);
            }
        }
    }
}
