using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StockFinder
{
    public static class StockUtils
    {
        public static readonly List<string> AllSymbols = new List<string>();
        public static readonly List<StockInfo> AllStocks = new List<StockInfo>();

        public static IEnumerable<IEnumerable<T>> Split<T>(T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static string[] ParseCsvRow(string r)
        {

            string[] c;
            string t;
            List<string> resp = new List<string>();
            bool cont = false;
            string cs = "";

            c = r.Split(new char[] { ',' }, StringSplitOptions.None);

            foreach (string y in c)
            {
                string x = y;


                if (cont)
                {
                    // End of field
                    if (x.EndsWith("\""))
                    {
                        cs += "," + x.Substring(0, x.Length - 1);
                        resp.Add(cs);
                        cs = "";
                        cont = false;
                        continue;

                    }
                    else
                    {
                        // Field still not ended
                        cs += "," + x;
                        continue;
                    }
                }

                // Fully encapsulated with no comma within
                if (x.StartsWith("\"") && x.EndsWith("\""))
                {
                    if ((x.EndsWith("\"\"") && !x.EndsWith("\"\"\"")) && x != "\"\"")
                    {
                        cont = true;
                        cs = x;
                        continue;
                    }

                    resp.Add(x.Substring(1, x.Length - 2));
                    continue;
                }

                // Start of encapsulation but comma has split it into at least next field
                if (x.StartsWith("\"") && !x.EndsWith("\""))
                {
                    cont = true;
                    cs += x.Substring(1);
                    continue;
                }

                // Non encapsulated complete field
                resp.Add(x);

            }

            return resp.ToArray();

        }

        public static void DownloadTickers()
        {
            AllSymbols.Clear();
            AllSymbols.AddRange(GetTickersFor("nasdaq"));
            AllSymbols.AddRange(GetTickersFor("nyse"));
            AllSymbols.AddRange(GetTickersFor("amex"));
            //StreamReader file = new StreamReader(@"C:\Users\Me\Desktop\nasdaqtraded.txt");
            //string line;
            //file.ReadLine();
            //while ((line = file.ReadLine()) != null)
            //{
            //    string[] seg = line.Split('|');
            //    AllSymbols.Add(seg[1]);
            //}
        }

        private static IEnumerable<string> GetTickersFor(string market)
        {
            string url = $"http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange={market}&render=download";
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            Debug.WriteLine("start");
            WebResponse response = request.GetResponse();
            Debug.WriteLine("end");
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            reader.ReadLine();
            string res = reader.ReadToEnd();
            string[] lines = res.Split('\n');
            return lines.Select(line => ParseCsvRow(line)[0]);
        }
    }
}
