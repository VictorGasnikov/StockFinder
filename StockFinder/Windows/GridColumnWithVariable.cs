using System.Windows.Controls;

namespace StockFinder.Windows
{
    public class GridColumnWithVariable : DataGridTextColumn
    {
        public string Variable { get; set; }
    }
}