using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StockFinder.Technical.Indicators
{
    public class Aroon
    {
        public int Period { get; set; }
        public Dictionary<DateTime, decimal[]> AroonGraph = new Dictionary<DateTime, decimal[]>();
        public Aroon(int period)
        {
            Period = period;
        }

        public void Calculate(HistoricalPriceVolume data)
        {
            List<HistoricalPriceVolumeEntry> graph = data.Items;
            for (int i = Period; i < graph.Count; i++)
            {
                HistoricalPriceVolumeEntry entry = graph[i];
                int current = i + 1;

                DateTime date = entry.Date;
                int back = current - Period;
                back = back < 0 ? 0 : back;

                List<HistoricalPriceVolumeEntry> range = graph.GetRange(i - Period, Period + 1);

                int dayHigh = IndicatorUtils.GetHighestId(range.Select(e => e.High));
                int daysSinceHigh = range.Count - dayHigh;
                decimal aroonUp = 100 * (Period - daysSinceHigh) / (decimal)Period;

                int dayLow = IndicatorUtils.GetLowestId(range.Select(e => e.Low));
                int daysSinceLow = range.Count - dayLow;
                decimal aroonDown = 100 * (Period - daysSinceLow) / (decimal)Period;

                Debug.WriteLine("");
                Debug.WriteLine($"Date: {date.ToString("d")}");
                Debug.WriteLine($"Up: {aroonUp}");
                Debug.WriteLine($"Down: {aroonDown}");

                AroonGraph.Add(date, new[] { aroonUp, aroonDown });
            }
        }
    }
}
