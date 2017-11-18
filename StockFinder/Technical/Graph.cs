using System;
using System.Collections.Generic;
using System.Linq;
using StockFinder.Technical.Indicators;

namespace StockFinder.Technical
{
    public class Graph
    {
        public string Name { get; set; }
        public readonly Dictionary<DateTime, TechnicalIndicatorItem> Data = new Dictionary<DateTime, TechnicalIndicatorItem>();
        protected readonly List<TechnicalIndicatorItem> Items = new List<TechnicalIndicatorItem>();
        public TechnicalIndicatorItem this[DateTime date] => Data[date];

//        public TechnicalIndicatorItem this[DateTime date]
//        {
//            get { return Graphs[date]; }
//            set { Graphs[date] = value; }
//        }

        public Graph(string name)
        {
            Name = name;
        }

        public Graph(string name, List<TechnicalIndicatorItem> items) : this(name)
        {
            Items = items.ToList();
            foreach (TechnicalIndicatorItem item in items)
            {
                Data.Add(item.Date, item);
            }
        }

        public void Add(TechnicalIndicatorItem item)
        {
            Data.Add(item.Date, item);
            Items.Add(item);
        }

        public Graph GetPrev(int current, int back)
        {
            return new Graph(Name, Items.GetRange(current, back + 1));
        }
    }
}
