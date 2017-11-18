using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockFinder.Technical.Indicators;

namespace StockFinder.Technical.Strategies
{
    public class AroonCrossStrategy : TradingStrategy
    {
        private bool _bought;
        private readonly int _period;
        public AroonCrossStrategy(int period, decimal buyingPower) : base(buyingPower)
        {
            _period = period;
        }

        protected override void Signal(HistoricalPriceVolumeEntry entry)
        {
            AroonIndicator aroon = (AroonIndicator)Indicators["Aroon"];
            DateTime yesterday = entry.Date.AddDays(-1);
            TechnicalIndicatorItem currentUp = aroon.AroonUp.Data[entry.Date];
            TechnicalIndicatorItem currentDown = aroon.AroonDown.Data[entry.Date];

            TechnicalIndicatorItem previousUp = null;
            TechnicalIndicatorItem previousDown = null;
            if (aroon.AroonUp.Data.ContainsKey(yesterday))
            {
                previousUp = aroon.AroonUp.Data[yesterday];
                previousDown = aroon.AroonDown.Data[yesterday];
            }
            if (currentUp.Value > currentDown.Value && !_bought)
            {
                int shares = decimal.ToInt32(Math.Floor(BuyingPower / entry.Close));
                Trade(entry.Date, TradeType.Buy, entry.Close, shares);
                _bought = true;
            }
            else if (currentDown.Value > currentUp.Value && _bought)
            {
//                int max = GetMaxBuy();
//                int owned = Math.Abs(SharesOwned);
//                Trade(entry.Date, TradeType.Cover, entry.Close, max > owned ? owned : max);
                Trade(entry.Date, TradeType.Sell, entry.Close, Math.Abs(SharesOwned));
                _bought = false;
            }
        }

        protected override void Initialize(HistoricalPriceVolume historicalData)
        {
            AroonIndicator aroon = new AroonIndicator("Aroon", _period);
            aroon.Calculate(historicalData);
            AddIndicator(aroon);
        }

        protected override void Reset()
        {
            _bought = false;
        }
    }
}
