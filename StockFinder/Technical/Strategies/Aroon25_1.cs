using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockFinder.Technical.Indicators;

namespace StockFinder.Technical.Strategies
{
    public class Aroon25_1 : TradingStrategy
    {
        private bool _bought;
        public Aroon25_1(decimal buyingPower) : base(buyingPower)
        {
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
            if ((previousDown?.Value >= 50 || previousUp?.Value < 100) && currentDown.Value < 50 && currentUp.Value == 100 && !_bought)
            {
                int shares = decimal.ToInt32(Math.Floor(BuyingPower / entry.Close));
                _bought = Trade(entry.Date, TradeType.Buy, entry.Close, shares);
            }
            else if ((previousUp?.Value >= 50 || previousDown?.Value < 100) && currentUp.Value < 50 && currentDown.Value == 100 && _bought)
            {
                _bought = Trade(entry.Date, TradeType.Sell, entry.Close, SharesOwned);
            }
        }

        protected override void Initialize(HistoricalPriceVolume historicalData)
        {
            AroonIndicator aroon = new AroonIndicator("Aroon", 25);
            aroon.Calculate(historicalData);
            AddIndicator(aroon);
        }

        protected override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
