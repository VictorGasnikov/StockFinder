using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StockFinder.Windows
{
    /// <summary>
    /// Interaction logic for VariableEditor.xaml
    /// </summary>
    public partial class VariableEditor : Window
    {
        public VariableEditor()
        {
            InitializeComponent();
            foreach (KeyValuePair<string, StockProperty> property in StockInfo.DefaultProperties)
            {
                grid.Items.Add(property);
            }
        }

        public VariableEditor(string label, string variable, string formula, bool allowFiltering) : this()
        {
            txtLabel.Text = label;
            txtVariable.Text = variable;
            txtFormula.Text = formula;
            chkAllowFilter.IsChecked = allowFiltering;
            btnSubmit.Content = "Apply";
        }

        public string Label => txtLabel.Text;
        public string Variable => txtVariable.Text;
        public string Formula => txtFormula.Text;
        public bool AllowFiltering => chkAllowFilter.IsChecked == true;

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
    public class VariableListValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string label = (string)values[0];
            string variable = (string)values[1];
            string formula = (string)values[2];
            return label.Length > 0 && variable.Length > 0 && formula.Length > 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
