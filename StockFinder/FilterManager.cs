using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace StockFinder
{
    public static class FilterManager
    {
        public static IEnumerable<StockInfo> Filter(IEnumerable<Filter> filters)
        {
            return Filter(StockUtils.AllStocks, filters);
        }

        public static IEnumerable<StockInfo> Filter(IEnumerable<StockInfo> stocks, IEnumerable<Filter> filters)
        {
            List<Filter> filtersList = filters.ToList();
            return stocks.Where(s => filtersList.TrueForAll(f => f.Check(s)));
        }
    }
}
