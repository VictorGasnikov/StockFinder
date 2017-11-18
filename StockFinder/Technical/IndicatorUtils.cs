using System.Collections.Generic;
using System.Linq;

namespace StockFinder.Technical
{
    public static class IndicatorUtils
    {
        public static int GetHighestId(IEnumerable<decimal> numbers)
        {
            List<decimal> inverted = numbers.ToList();

            decimal max = inverted.Max();
            for (int i = 0; i < inverted.Count; i++)
            {
                decimal d = inverted[i];
                if (d == max)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int GetLowestId(IEnumerable<decimal> numbers)
        {
            List<decimal> inverted = numbers.ToList();

            decimal min = inverted.Min();
            for (int i = 0; i < inverted.Count; i++)
            {
                decimal d = inverted[i];
                if (d == min)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
