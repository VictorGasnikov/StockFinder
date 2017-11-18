using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using StockFinder.Windows.Scanner;

namespace StockFinder
{
    public class StockFinderSettings
    {
        private static XmlSerializer _serializer;

        public List<CustomStockProperty> CustomProperties;
        public List<FilterState> Filters;
        public List<GridColumnState> Columns;
        public List<ScannerSettings> Scanners;

        public StockFinderSettings()
        {

        }

        public static StockFinderSettings Load(string file)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                _serializer = new XmlSerializer(typeof(StockFinderSettings));
                return (StockFinderSettings)_serializer.Deserialize(reader);
            }
        }

        public void Save(string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                _serializer = new XmlSerializer(GetType());
                _serializer.Serialize(writer, this);
            }
        }
    }

    public class FilterState
    {
        public string Variable { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public bool IsUsed { get; set; }
    }

    public class GridColumnState
    {
        public string Variable { get; set; }
        public int Id { get; set; }
    }

}
