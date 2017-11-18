using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StockFinder.Windows
{
    /// <summary>
    /// Interaction logic for StockPropertySelector.xaml
    /// </summary>
    public partial class StockPropertySelector : Window
    {
        private readonly Dictionary<string, CheckBox> _checkBoxes = new Dictionary<string, CheckBox>();
        public StockPropertySelector(List<string> keys, bool filterOnly = false)
        {
            InitializeComponent();

            foreach (PropertyGroup propertyGroup in StockInfo.Groups)
            {
                TreeViewItem item = new TreeViewItem
                {
                    IsExpanded = true,
                    Header = propertyGroup.Name
                };
                foreach (string variable in propertyGroup.Variables)
                {
                    StockProperty property = StockInfo.DefaultProperties[variable];
                    if (filterOnly && !property.AllowFiltering)
                    {
                        continue;
                    }
                    CheckBox checkbox = new CheckBox
                    {
                        Content = property.Label,
                        IsChecked = keys.Contains(variable)
                    };
                    _checkBoxes.Add(variable, checkbox);
                    TreeViewItem subItem = new TreeViewItem { Header = checkbox };
                    item.Items.Add(subItem);

                }
                DefaultProperties.Items.Add(item);
            }
            //            foreach (KeyValuePair<string, StockProperty> entry in filterOnly ? StockInfo.DefaultProperties.Where(prop => prop.Value.AllowFiltering) : StockInfo.DefaultProperties)
            //            {
            //                CheckBox checkbox = new CheckBox
            //                {
            //                    Content = entry.Value.Label,
            //                    IsChecked = keys.Contains(entry.Key)
            //                };
            //                CheckBoxes.Add(entry.Key, checkbox);
            //                DefaultProperties.Items.Add(checkbox);
            //            }
            foreach (KeyValuePair<string, CustomStockProperty> entry in filterOnly ? StockInfo.PropertyDefinitions.Where(prop => prop.Value.AllowFiltering) : StockInfo.PropertyDefinitions)
            {
                CheckBox checkbox = new CheckBox
                {
                    Content = entry.Value.Label,
                    IsChecked = keys.Contains(entry.Key)
                };
                _checkBoxes.Add(entry.Key, checkbox);
                CustomProperties.Children.Add(checkbox);
            }
        }

        //        public StockPropertySelector(Dictionary<string, DataGridTextColumn>.KeyCollection keys):this()
        //        {
        //
        //        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public IEnumerable<string> GetSelectedProperties()
        {
            return _checkBoxes.Where(entry => entry.Value.IsChecked == true).Select(entry => entry.Key);
        }
    }
}
