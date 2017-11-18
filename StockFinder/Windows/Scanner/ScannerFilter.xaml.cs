using System.Collections.Generic;
using System.Linq;
using System.Windows;
using StockFinder.Controls;

namespace StockFinder.Windows.Scanner
{
    /// <summary>
    /// Interaction logic for ScannerFilter.xaml
    /// </summary>
    public partial class ScannerFilter : Window
    {
//        private List<string> _variables;
        private readonly List<string> _shownFilters = new List<string>();
        private IEnumerable<FilterState> _states;

        public ScannerFilter(List<FilterState> filters)
        {
            InitializeComponent();
            foreach (FilterState filter in filters)
            {
                FilterCriteria criteria = AddFilter(filter.Variable, true);
                criteria.Min = filter.Min;
                criteria.Max = filter.Max;
                criteria.IsUsed = filter.IsUsed;
            }
        }

        public ScannerFilter() : this(new List<FilterState>())
        {

        }

        private void btnCustomize_Click(object sender, RoutedEventArgs e)
        {
            StockPropertySelector selector = new StockPropertySelector(_shownFilters);
            if (selector.ShowDialog() == true)
            {
                List<string> newVariables = selector.GetSelectedProperties().ToList();
                IEnumerable<string> remove = _shownFilters.Except(newVariables).ToList();
                IEnumerable<string> add = newVariables.Except(_shownFilters);
                foreach (string variable in remove)
                {
                    RemoveFilter(variable);
                }
                foreach (string variable in add)
                {
                    AddFilter(variable, true);
                }
            }
        }
        public FilterCriteria AddFilter(string variable, bool addToList)
        {
            //            foreach (FilterCriteria criteria in GetCriteria().Where(criteria => criteria.Variable.Equals(variable)))
            //            {
            ////                _shownFilters.Add(variable);
            //                criteria.Visibility = Visibility.Visible;
            //                return criteria;
            //            }
            StockProperty property;
            bool isCustom = false;
            if (StockInfo.DefaultProperties.ContainsKey(variable))
            {
                property = StockInfo.DefaultProperties[variable];
            }
            else if (StockInfo.PropertyDefinitions.ContainsKey(variable))
            {
                property = StockInfo.PropertyDefinitions[variable];
                isCustom = true;
            }
            else
            {
                return null;
            }
            FilterCriteria criteria = new FilterCriteria(variable, property.Label, property.DefaultFilterMinValue, property.DefaultFilterMaxValue, true, isCustom);
            Filters.Children.Add(criteria);
            if (addToList)
            {
                _shownFilters.Add(variable);
            }
            return criteria;
        }
        public void RemoveFilter(string variable)
        {
            //            if (StockInfo.DefaultProperties.ContainsKey(variable))
            //            {
            //                
            //            }
            //            else if (StockInfo.PropertyDefinitions.ContainsKey(variable))
            //            {
            //
            //            }
            foreach (FilterCriteria criteria in GetCriteria().Where(criteria => criteria.Variable == variable))
            {
                RemoveFilter(criteria);
                return;
            }
        }

        public void RemoveFilter(FilterCriteria criteria)
        {
            Filters.Children.Remove(criteria);
            _shownFilters.Remove(criteria.Variable);
        }

        public IEnumerable<FilterCriteria> GetCriteria()
        {
            return Filters.Children.Cast<FilterCriteria>();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _states = GetCriteria().Select(c => new FilterState
            {
                Variable = c.Variable,
                Max = c.Max,
                Min = c.Min,
                IsUsed = c.IsUsed
            });
        }

        public IEnumerable<FilterState> GetFilterStates()
        {
            return _states;
        }
    }
}
