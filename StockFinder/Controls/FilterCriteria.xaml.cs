using System.Windows.Media;

namespace StockFinder.Controls
{
    /// <summary>
    /// Interaction logic for FilterCriteria.xaml
    /// </summary>
    /// 

    public partial class FilterCriteria
    {
        public string Label
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }

        public string Min
        {
            get { return min.Text; }
            set { min.Text = value; }
        }

        public decimal MinDecimal
        {
            get
            {
                return NumberUtils.ParseDecimal(min.Text);
            }
            set
            {
                min.Text = value.ToString();
            }
        }
        public string Max
        {
            get { return max.Text; }
            set { max.Text = value; }
        }
        public decimal MaxDecimal
        {
            get
            {
                return NumberUtils.ParseDecimal(max.Text);
            }
            set
            {
                max.Text = value.ToString();
            }
        }
        public bool IsUsed
        {
            get
            {
                return (bool)enabled.IsChecked;
            }
            set
            {
                enabled.IsChecked = value;
            }
        }

        private bool _isCustom;
        public bool IsCustom
        {
            set
            {
                grid.Background = value ? Brushes.Yellow : Brushes.Transparent;
                _isCustom = value;
            }
            get { return _isCustom; }
        }

        public string Variable { get; set; }

        public FilterCriteria()
        {
            InitializeComponent();
        }

        public FilterCriteria(string variable, string label, string min, string max, bool isUsed, bool isCustom) : this()
        {
            Variable = variable;
            Label = label;
            Min = min;
            Max = max;
            IsUsed = isUsed;
            IsCustom = isCustom;
        }


        public Filter GetFilter()
        {
            return new Filter(Variable, MinDecimal, MaxDecimal, IsUsed);
        }
    }
}
