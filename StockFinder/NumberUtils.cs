using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFinder
{
    public static class NumberUtils
    {
        //        public static double ParseDouble(string number)
        //        {
        //            try
        //            {
        //                return double.Parse(number);
        //            }
        //            catch (Exception)
        //            {
        //                return -1;
        //            }
        //        }
        public static decimal ParseDecimal(string number)
        {
            if (number.Equals("max", StringComparison.OrdinalIgnoreCase))
            {
                return decimal.MaxValue;
            }

            if (number.Equals("min", StringComparison.OrdinalIgnoreCase))
            {
                return decimal.MinValue;
            }
            int multiplier = 1;

            if (number.EndsWith("M") || number.EndsWith("m"))
            {
                number = number.Remove(number.Length - 1);
                multiplier = 1000000;
            }
            else if (number.EndsWith("B") || number.EndsWith("b"))
            {
                number = number.Remove(number.Length - 1);
                multiplier = 1000000000;
            }


            decimal output;
            if (decimal.TryParse(number, out output))
            {
                return output * multiplier;
            }
            return decimal.MinusOne;
        }

        public static long ParseLong(string number)
        {
            //            try
            //            {
            //                return long.Parse(number);
            //            }
            //            catch (Exception)
            //            {
            //                return -1;
            //            }


            if (number.Equals("max", StringComparison.OrdinalIgnoreCase))
            {
                return long.MaxValue;
            }

            if (number.Equals("min", StringComparison.OrdinalIgnoreCase))
            {
                return long.MinValue;
            }

            int multiplier = 1;

            if (number.EndsWith("M") || number.EndsWith("m"))
            {
                number = number.Remove(number.Length - 1);
                multiplier = 1000000;
            }
            else if (number.EndsWith("B") || number.EndsWith("b"))
            {
                number = number.Remove(number.Length - 1);
                multiplier = 1000000000;
            }


            long output;
            if (long.TryParse(number, out output))
            {
                return output * multiplier;
            }
            return -1;
        }
        public static int ParseInt(string number)
        {
            //            try
            //            {
            //                return int.Parse(number);
            //            }
            //            catch (Exception)
            //            {
            //                return -1;
            //            }
            if (number.Equals("max", StringComparison.OrdinalIgnoreCase))
            {
                return int.MaxValue;
            }

            if (number.Equals("min", StringComparison.OrdinalIgnoreCase))
            {
                return int.MinValue;
            }
            int multiplier = 1;

            if (number.EndsWith("M") || number.EndsWith("m"))
            {
                number = number.Remove(number.Length - 1);
                multiplier = 1000000;
            }
            else if (number.EndsWith("B") || number.EndsWith("b"))
            {
                number = number.Remove(number.Length - 1);
                multiplier = 1000000000;
            }


            int output;
            if (int.TryParse(number, out output))
            {
                return output * multiplier;
            }
            return -1;
        }
    }
}
