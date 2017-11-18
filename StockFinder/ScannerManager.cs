using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockFinder.Windows.Scanner;

namespace StockFinder
{
    public class ScannerManager
    {
        public List<ScannerSettings> Scanners = new List<ScannerSettings>();

        public void ClearAll()
        {
            Scanners.Clear();
        }
    }
}
