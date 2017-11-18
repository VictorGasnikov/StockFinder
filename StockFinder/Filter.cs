using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFinder
{
    public class Filter
    {
        public string Value { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public bool Evaluate { get; set; }

        public Filter(string value, decimal min, decimal max, bool evaluate)
        {
            Value = value;
            Min = min;
            Max = max;
            Evaluate = evaluate;
        }

        public bool Check(StockInfo stock)
        {
            try
            {
                decimal val;
                object o = stock.Properties[Value];
                if (o is decimal)
                {
                    val = (decimal) o;
                }
                else
                {
                    if (!decimal.TryParse(stock.Properties[Value].ToString(), out val))
                    {
                        return false;
                    }
                }
                return val >= Min && val <= Max;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
