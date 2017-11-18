using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using Jace;
using NCalc;

namespace ConsoleApplication1
{
    static class Mainclass
    {
        static void Main(string[] args)
        {
            decimal[] decimals = new[] { 1000m, 1m, 2m, 2.5m, 123m, 123m, 1.3m, 25m };
            double[] numbers = new[] { 1000, 1, 2, 2.5, 123, 123, 1.3, 25 };

            int h1 = highest1(numbers);
            int h = highest(numbers);
            int d1 = highest2(decimals);
            int d2 = highest3(decimals);
            Debug.WriteLine(h1 + " " + h);
            Debug.WriteLine(d1 + " " + d2);
            // one
//            List<decimal> inverted = numbers.ToList();
//
//            decimal max = inverted.Max();
//            for (int i = 0; i < inverted.Count; i++)
//            {
//                decimal d = inverted[i];
//                if (d == max)
//                {
//                    Debug.WriteLine(i);
//                    break;
//                }
//            }

            //two

            //            List<decimal> inverted2 = numbers.ToList();


        }

        public static int highest1(double[] numbers)
        {
            List<double> inverted = numbers.ToList();

            double max = inverted.Max();
            for (int i = 0; i < inverted.Count; i++)
            {
                double d = inverted[i];
                if (d.Equals(max))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int highest(double[] numbers)
        {
            double max = numbers[0];
            int maxId = 0;

            //            decimal max = inverted.Max();
            for (int i = 1; i < numbers.Length; i++)
            {
                double d = numbers[i];
                if (d > max)
                {
                    max = d;
                    maxId = i;
                }
            }

            return maxId;
        }public static int highest2(decimal[] numbers)
        {
            List<decimal> inverted = numbers.ToList();

            decimal max = inverted.Max();
            for (int i = 0; i < inverted.Count; i++)
            {
                decimal d = inverted[i];
                if (d.Equals(max))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int highest3(decimal[] numbers)
        {
            decimal max = numbers[0];
            int maxId = 0;

            //            decimal max = inverted.Max();
            for (int i = 1; i < numbers.Length; i++)
            {
                decimal d = numbers[i];
                if (d > max)
                {
                    max = d;
                    maxId = i;
                }
            }

            return maxId;
        }
    }
}
