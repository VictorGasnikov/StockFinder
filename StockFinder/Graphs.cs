using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFinder
{
    public class IntTimeGraph
    {
        public Dictionary<DateTime, int> GraphDictionary = new Dictionary<DateTime, int>();
    }

    public class DecimalTimeGraph
    {
        public Dictionary<DateTime, decimal> GraphDictionary = new Dictionary<DateTime, decimal>();
    }

//    public class HistoricalPriceVolumeEntry
//    {
//        public decimal Open { get; set; }
//        public decimal Close { get; set; }
//        public decimal High { get; set; }
//        public decimal Low { get; set; }
//        public int Volume { get; set; }
//        public DateTime Date { get; set; }
//    }
//
//    public class HistoricalPriceVolume
//    {
//        public Dictionary<DateTime, HistoricalPriceVolumeEntry> GraphDictionary = new Dictionary<DateTime, HistoricalPriceVolumeEntry>();
//
//        public void Add(HistoricalPriceVolumeEntry entry)
//        {
//            GraphDictionary.Add(entry.Date, entry);
//        }
//    }
}
