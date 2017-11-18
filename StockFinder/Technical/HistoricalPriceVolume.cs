using System;
using System.Collections.Generic;
using System.Linq;

namespace StockFinder.Technical
{
    public class HistoricalPriceVolume
    {
        public readonly Dictionary<DateTime, HistoricalPriceVolumeEntry> GraphData = new Dictionary<DateTime, HistoricalPriceVolumeEntry>();
        public readonly List<HistoricalPriceVolumeEntry> Items;
        public HistoricalPriceVolumeEntry this[DateTime date]
        {
            get { return GraphData[date]; }
            set { GraphData[date] = value; }
        }

        public HistoricalPriceVolume()
        {
            
        }

        public HistoricalPriceVolume(List<HistoricalPriceVolumeEntry> items)
        {
            Items = items.ToList();
            foreach (HistoricalPriceVolumeEntry item in items)
            {
                GraphData.Add(item.Date, item);
            }
        }

        public HistoricalPriceVolume GetPrev(int current, int back)
        {
            return new HistoricalPriceVolume(GetPrevAsList(current, back));
        }

        public List<HistoricalPriceVolumeEntry> GetPrevAsList(int current, int back)
        {
            return Items.GetRange(current, back + 1);
        }
    }
    public class HistoricalPriceVolumeEntry
    {
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public int Volume { get; set; }
        public DateTime Date { get; set; }
    }
}
