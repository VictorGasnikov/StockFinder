using System;
using System.Collections.Generic;

namespace StockFinder.Technical.Indicators
{
    public abstract class TechnicalIndicator
    {
        public string Name { get; }
        public readonly Dictionary<string, Graph> Graphs = new Dictionary<string, Graph>();
        protected HistoricalPriceVolume HistoricalData;
        public int BeginCalculationOffset { get; }

        private int _current;

        public TechnicalIndicator(string name, int offset)
        {
            Name = name;
            BeginCalculationOffset = offset;
        }

        public Graph GetGraph(string name) => Graphs[name];

        public void Calculate(HistoricalPriceVolume graph)
        {

            HistoricalData = graph;
            CalculationStart();
            for (int i = HistoricalData.GraphData.Count - BeginCalculationOffset - 1; i >= 0; i--)
            {
                _current = i;
                CalculateFor(i);
            }
            CalculationEnd();
        }

        public List<HistoricalPriceVolumeEntry> GetPrevRange()
        {
            return HistoricalData.GetPrevAsList(_current, BeginCalculationOffset);
        }

        public void Add(Graph graph)
        {
            Graphs.Add(graph.Name, graph);
        }

        protected virtual void CalculationStart() { }
        protected virtual void CalculationEnd() { }
        protected abstract void CalculateFor(int current);
    }
}
