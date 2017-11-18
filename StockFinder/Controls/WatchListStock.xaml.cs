using System;
using System.Collections.Generic;
using System.Globalization;
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
using StockFinder.Windows;

namespace StockFinder.Controls
{
    /// <summary>
    /// Interaction logic for WatchListStock.xaml
    /// </summary>
    public partial class WatchListStock : UserControl
    {
        private decimal _bid;
        private decimal _ask;
        private int _bidSize;
        private int _askSize;
        private readonly string _ticker;

        public decimal Bid
        {
            get { return _bid; }
            set
            {
                //                if (value < _bid)
                //                {
                //                    Dispatcher.Invoke(() =>
                //                    {
                //                        lblBid.Background = Brushes.Red;
                //                    });
                //                }
                //                else if (value > _bid)
                //                {
                //                    Dispatcher.Invoke(() =>
                //                    {
                //                        lblBid.Background = Brushes.Green;
                //                    });
                //                }
                //                else
                //                {
                //                    Dispatcher.Invoke(() =>
                //                    {
                //                        lblBid.Background = Brushes.Transparent;
                //                    });
                //                }
                //                Dispatcher.Invoke(() =>
                //                {
                //                    lblBid.Text = value.ToString();
                //                });
                //                _bid = value;
                ChangePrice(lblBid, ref _bid, value);
            }
        }
        public decimal Ask
        {
            get { return _ask; }
            set
            {
                //                if (value < _ask)
                //                {
                //                    Dispatcher.Invoke(() =>
                //                    {
                //                        lblAsk.Background = Brushes.Red;
                //                    });
                //                }
                //                else if (value > _ask)
                //                {
                //                    Dispatcher.Invoke(() =>
                //                    {
                //                        lblAsk.Background = Brushes.Green;
                //                    });
                //                }
                //                else
                //                {
                //                    Dispatcher.Invoke(() =>
                //                    {
                //                        lblAsk.Background = Brushes.Transparent;
                //                    });
                //                }
                //                Dispatcher.Invoke(() =>
                //                {
                //                    lblAsk.Text = value.ToString();
                //                });
                //                _ask = value;
                ChangePrice(lblAsk, ref _ask, value);
            }
        }

        public int BidSize
        {
            get
            {
                return _bidSize;
            }
            set
            {
                ChangeSize(lblBidSize, ref _bidSize, value);
            }
        }
        public int AskSize
        {
            get
            {
                return _askSize;
            }
            set
            {
                ChangeSize(lblAskSize, ref _askSize, value);
            }
        }

        private void ChangePrice(TextBlock text, ref decimal variable, decimal updated)
        {
            decimal current = variable;
            Dispatcher.Invoke(() =>
            {
                if (updated != -1)
                {
                    text.Text = updated.ToString("G");
                    if (updated < current)
                    {
                        text.Background = Brushes.Red;
                    }
                    else if (updated > current)
                    {
                        text.Background = Brushes.Green;
                    }
                    else
                    {
                        text.Background = Brushes.Transparent;
                    }
                }
                else
                {
                    text.Text = "----";
                    text.Background = Brushes.Orange;
                }
            });
            variable = updated;
        }
        private void ChangeSize(TextBlock text, ref int variable, int updated)
        {
            decimal current = variable;
            Dispatcher.Invoke(() =>
            {
                if (updated != -1)
                {
                    text.Text = updated.ToString();
                    if (updated < current)
                    {
                        text.Background = Brushes.Red;
                    }
                    else if (updated > current)
                    {
                        text.Background = Brushes.Green;
                    }
                    else
                    {
                        text.Background = Brushes.Transparent;
                    }
                }
                else
                {
                    text.Text = "----";
                    text.Background = Brushes.Orange;
                }
            });
            variable = updated;
        }

        public WatchListStock(string ticker)
        {
            InitializeComponent();
            lblSymbol.Text = _ticker = ticker;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.WatchListManager.Remove(_ticker);
        }
    }
}
