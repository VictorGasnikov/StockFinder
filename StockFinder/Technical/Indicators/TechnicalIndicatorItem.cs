using System;

namespace StockFinder.Technical.Indicators
{
    public class TechnicalIndicatorItem
    {
        public DateTime Date { get; }
        public decimal Value { get; }

//        public decimal this[int id] => Data[id];

        public TechnicalIndicatorItem(DateTime date, decimal value)
        {
            Date = date;
            Value = value;
        }
    }
}
