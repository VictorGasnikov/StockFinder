using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StockFinder.Technical.Indicators
{
    public class AroonIndicator : TechnicalIndicator
    {
        //        protected sealed override int BeginCalculationOffset { get; set; }

        private int _period;
//        private Graph _upGraph;
//        private Graph _downGraph;

        private readonly List<TechnicalIndicatorItem> _upItems = new List<TechnicalIndicatorItem>();
        private readonly List<TechnicalIndicatorItem> _downItems = new List<TechnicalIndicatorItem>();


        public Graph AroonUp => Graphs["AroonUp"];
        public Graph AroonDown => Graphs["AroonDown"];

        public AroonIndicator(string name, int period) : base(name, period)
        {
            _period = period;
        }

        protected override void CalculationStart()
        {
        }

        protected override void CalculationEnd()
        {
            _upItems.Reverse();
            _downItems.Reverse();
            Add(new Graph("AroonUp", _upItems));
            Add(new Graph("AroonDown", _downItems));
        }

        protected override void CalculateFor(int current)
        {
            HistoricalPriceVolumeEntry entry = HistoricalData.Items[current];
            //            int current = i + 1;

            DateTime date = entry.Date;
            //            int back = current - Period;
            //            back = back < 0 ? 0 : back;

            //            List<HistoricalPriceVolumeEntry> range = HistoricalData.GetPrevAsList(current, BeginCalculationOffset);
            List<HistoricalPriceVolumeEntry> range = GetPrevRange();

            int dayHigh = IndicatorUtils.GetHighestId(range.Select(e => e.High));
            //            int daysSinceHigh = range.Count - dayHigh;
            decimal aroonUp = 100 * (BeginCalculationOffset - dayHigh) / (decimal)BeginCalculationOffset;

            int dayLow = IndicatorUtils.GetLowestId(range.Select(e => e.Low));
            //            int daysSinceLow = range.Count - dayLow;
            decimal aroonDown = 100 * (BeginCalculationOffset - dayLow) / (decimal)BeginCalculationOffset;

            //            Debug.WriteLine("");
            //            Debug.WriteLine($"Date: {date.ToString("d")}");
            //            Debug.WriteLine($"Up: {aroonUp}");
            //            Debug.WriteLine($"Down: {aroonDown}");

            _upItems.Add(new TechnicalIndicatorItem(date, aroonUp));
            _downItems.Add(new TechnicalIndicatorItem(date, aroonDown));
        }
    }
}
