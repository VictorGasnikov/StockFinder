using System;
using System.Collections.Generic;
using System.Linq;
using StockFinder.Technical.Indicators;

namespace StockFinder.Technical.Strategies
{
    public abstract class TradingStrategy
    {
        public decimal Capital { get; }
        public readonly Dictionary<string, TechnicalIndicator> Indicators = new Dictionary<string, TechnicalIndicator>();
        protected HistoricalPriceVolume HistoricalData;
        public StockInfo Stock { get; set; }
        public decimal BuyingPower => SharesOwned < 0 ? Account + SharesOwned * CurrentSharePrice : Account;
        public decimal Account { get; set; }
        public decimal CurrentSharePrice { get; set; }
        protected int SharesOwned { get; set; }
        public decimal Commissions { get; set; }

        protected List<TradeInfo> Trades;

        public TradingStrategy(decimal buyingPower)
        {
            Capital = buyingPower;
        }

        public BacktestResult BackTest(StockInfo stock, HistoricalPriceVolume historicalData)
        {
            Stock = stock;
            HistoricalData = historicalData;
            Account = Capital;
            SharesOwned = 0;
            Indicators.Clear();
            Trades = new List<TradeInfo>();
            Reset();

            Initialize(HistoricalData);

            int offset = Indicators.Values.Max(ind => ind.BeginCalculationOffset);
            for (int i = HistoricalData.Items.Count - offset - 1; i >= 0; i--)
            {
                CurrentSharePrice = HistoricalData.Items[i].Open; // Wrong
                Signal(HistoricalData.Items[i]);
            }
            return new BacktestResult(Capital, Account + SharesOwned * CurrentSharePrice, Stock, Trades, HistoricalData.Items[0]);
        }

        protected abstract void Signal(HistoricalPriceVolumeEntry entry);
        protected abstract void Initialize(HistoricalPriceVolume historicalData);
        protected abstract void Reset();

        public void AddIndicator(TechnicalIndicator indicator)
        {
            Indicators.Add(indicator.Name, indicator);
        }

        public bool Trade(DateTime date, TradeType type, decimal price, int amount)
        {
            return Trade(new TradeInfo { Date = date, Type = type, Amount = amount, Price = price });
        }

        public bool Trade(TradeInfo trade)
        {
            decimal totalPrice = trade.Price * trade.Amount;
            if ((trade.Type == TradeType.Buy || trade.Type == TradeType.Short || trade.Type == TradeType.Cover) && totalPrice + Commissions > BuyingPower)
            {
                return false;
            }
            switch (trade.Type)
            {
                case TradeType.Buy:
                    SharesOwned += trade.Amount;
                    //                    BuyingPower -= totalPrice;
                    Account -= totalPrice;
                    break;
                case TradeType.Sell:
                    SharesOwned -= trade.Amount;
                    //                    BuyingPower += totalPrice;
                    Account += totalPrice;
                    break;
                case TradeType.Short:
                    SharesOwned -= trade.Amount;
                    Account += totalPrice;
                    break;
                case TradeType.Cover:
                    SharesOwned += trade.Amount;
                    Account -= totalPrice;
                    break;
            }
            //            if (trade.Type == TradeType.Buy || trade.Type == TradeType.Cover)
            //            {
            //                SharesOwned += trade.Amount;
            //                BuyingPower -= totalPrice;
            //            }
            //            else if (trade.Type =)
            //            else
            //            {
            //                SharesOwned -= trade.Amount;
            //                BuyingPower += totalPrice;
            //            }
            Trades.Add(trade);
            return true;
        }

        public int GetMaxBuy()
        {
            return Decimal.ToInt32(BuyingPower / CurrentSharePrice);
        }
    }

    public class BacktestResult
    {
        //        public int Buys { get; set; }
        //        public int Sells { get; set; }
        //        public int Shorts { get; set; }
        //        public int Covers { get; set; }
        public StockInfo Stock { get; }
        public List<TradeInfo> Trades { get; }
        public DateTime From { get; }
        public DateTime To { get; }
        public decimal Capital { get; }
        //        public List<TransactionPair> Pairs = new List<TransactionPair>();
        public decimal Result { get; }
        public decimal Profit { get; }
        public decimal Gain { get; }

        public BacktestResult(decimal capital, decimal result, StockInfo stock, List<TradeInfo> trades, HistoricalPriceVolumeEntry last)
        {
            Stock = stock;
            Trades = trades;
            Capital = capital;
            Result = result;
            Profit = result - capital;
            Gain = Profit / Capital * 100;

            decimal profit = 0;
            int shares = 0;
            foreach (TradeInfo trade in trades)
            {
                if (trade.Type == TradeType.Buy || trade.Type == TradeType.Cover)
                {
                    profit -= trade.Price * trade.Amount;
                    shares += trade.Amount;
                }
                else
                {
                    profit += trade.Price * trade.Amount;
                    shares -= trade.Amount;
                }
            }

            profit += shares * last.Close;

            Profit = profit;



            //            for (int i = 0; i < trades.Count; i++)
            //            {
            //                if (i % 2 == 0 && i + 1 < trades.Count)
            //                {
            //                    if (tr)
            //                }
            //            }
            //            foreach (TradeInfo trade in trades)
            //            {
            //                switch (trade.Type)
            //                {
            //                    case TradeType.Buy:
            //                        Buys++;
            //                        break;
            //                    case TradeType.Sell:
            //                        Sells++;
            //                        break;
            //                    case TradeType.Short:
            //                        Shorts++;
            //                        break;
            //                    case TradeType.Cover:
            //                        Covers++;
            //                        break;
            //                }
            //            }

        }
    }

    //    public class TransactionPair
    //    {
    //        public TransactionPairType Type { get; set; }
    //        public TradeInfo First { get; set; }
    //        public TradeInfo Second { get; set; }
    //
    //        public decimal Profit => (Second.Price - First.Price) * (decimal)Type;
    //
    //        public TransactionPair(TransactionPairType type, TradeInfo first, TradeInfo second)
    //        {
    //            Type = type;
    //            First = first;
    //            Second = second;
    //        }
    //    }
    //
    //    public enum TransactionPairType
    //    {
    //        Long = 1,
    //        Short = -1
    //    }

    public class TradeInfo
    {
        public DateTime Date { get; set; }
        public TradeType Type { get; set; }

        /// <summary>
        /// Price per share
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Amount of shares
        /// </summary>
        public int Amount { get; set; }
    }

    public enum TradeType
    {
        Empty = 0,
        Buy = 1,
        Sell = 2,
        Short = 3,
        Cover = 4
    }
}
