using System;
using System.Windows;
using System.Windows.Controls;
using StockFinder.Windows;

namespace StockFinder.Controls
{
    /// <summary>
    /// Interaction logic for VariableListitem.xaml
    /// </summary>
    public partial class VariableListitem : UserControl
    {
        public static readonly DependencyProperty VariableProperty = DependencyProperty.Register("Variable", typeof(string), typeof(VariableListitem), new UIPropertyMetadata(String.Empty, VariableChangedCallback));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(VariableListitem), new UIPropertyMetadata(String.Empty, LabelChangedCallback));
        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(VariableListitem), new UIPropertyMetadata(false, IsEditingChangedCallback));
        public static readonly DependencyProperty AllowFilteringProperty = DependencyProperty.Register("AllowFiltering", typeof(bool), typeof(VariableListitem), new UIPropertyMetadata(false, AllowFilteringChangedCallback));
        //        public static readonly DependencyProperty ShowInGridProperty = DependencyProperty.Register("ShowInGrid", typeof(bool), typeof(VariableListitem), new UIPropertyMetadata(false, ShowInGridChangedCallback));
        public static readonly DependencyProperty FormulaProperty = DependencyProperty.Register("Formula", typeof(string), typeof(VariableListitem), new UIPropertyMetadata(String.Empty, FormulaChangedCallback));

        #region Dependecy Property declaration
        private static void FormulaChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VariableListitem vli = (VariableListitem)d;
            vli.Formula = e.NewValue as string;
        }

        private static void AllowFilteringChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VariableListitem vli = (VariableListitem)d;
            vli.AllowFiltering = (bool)e.NewValue;
        }
        //
        //        private static void ShowFilterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //        {
        //            VariableListitem vli = (VariableListitem)d;
        //            vli.ShowFilter = (bool)e.NewValue;
        //        }


        private static void IsEditingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VariableListitem vli = (VariableListitem)d;
            vli.IsEditing = (bool)e.NewValue;
        }

        private static void VariableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VariableListitem vli = (VariableListitem)d;
            vli.Variable = e.NewValue as string;
        }
        private static void LabelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VariableListitem vli = (VariableListitem)d;
            vli.Label = e.NewValue as string;
        }
        #endregion

        private FilterCriteria _criteria;
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public void ChangeLabel(string newLabel)
        {
            Label = newLabel;
            StockInfo.PropertyDefinitions[Variable].Label = newLabel;
        }
        public string Variable
        {
            get
            {
                return (string)GetValue(VariableProperty);
            }
            set
            {
                SetValue(VariableProperty, value);
            }
        }

        public void ChangeVariable(string newVariable)
        {
            CustomStockProperty prop = StockInfo.PropertyDefinitions[Variable];
            StockInfo.PropertyDefinitions.Remove(Variable);
            Variable = newVariable;
            StockInfo.PropertyDefinitions.Add(newVariable, prop);
        }

        public string Formula
        {
            get
            {
                return (string)GetValue(FormulaProperty);
            }
            set
            {
                SetValue(FormulaProperty, value);
            }
        }

        public void ChangeFormula(string newFormula)
        {
            Formula = newFormula;
            StockInfo.PropertyDefinitions[Variable].Formula = newFormula;
        }

        public bool AllowFiltering
        {
            get
            {
                return (bool)GetValue(AllowFilteringProperty);
            }
            set
            {
                SetValue(AllowFilteringProperty, value);
                //                criteria.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void ChangeAllowFiltering(bool allowFiltering)
        {
            AllowFiltering = allowFiltering;
            StockInfo.PropertyDefinitions[Variable].AllowFiltering = allowFiltering;
        }
        //        public bool ShowInGrid
        //        {
        //            get
        //            {
        //                return (bool)GetValue(ShowInGridProperty);
        //            }
        //            set
        //            {
        //                SetValue(ShowInGridProperty, value);
        //            }
        //        }
        public bool IsEditing
        {
            get
            {
                return (bool)GetValue(IsEditingProperty);
            }
            set
            {
                SetValue(IsEditingProperty, value);
            }
        }

        //        public VariableListitem()
        //        {
        //        }

        public VariableListitem(string variable, string formula, string label, bool allowFiltering)// : this()
        {
            MainWindow.Instance.AllowSearch = false;
            MainWindow.Instance.LblLoadProgress.Text = "Reload the database";
            DataContext = this;
            InitializeComponent();
            Variable = variable;
            Formula = formula;
            Label = label;
            AllowFiltering = allowFiltering;
            //            Dispatcher.BeginInvoke(new Action(() =>
            //            {
            try
            {
                StockInfo.PropertyDefinitions.Add(Variable, new CustomStockProperty(Variable, Formula, Label) { AllowFiltering = AllowFiltering });
                //                    _criteria = new FilterCriteria { Label = Label, IsCustom = true, Variable = Variable, Min = 0, Max = 1 };
                //                    MainWindow.Instance.FilterStates.Children.Add(_criteria);
                //                    _criteria.Visibility = Visibility.Collapsed;
                //                    MainWindow.Instance.AddFilter(Variable, true);
            }
            catch(Exception e)
            {
                // ignored
            }
            //            }), DispatcherPriority.ContextIdle, null);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //            // lblLabel.IsEditing = !lblLabel.IsEditing;
            //            IsEditing = !IsEditing;
            //            lblLabel.IsEditing = IsEditing;
            //            lblVariable.IsEditing = IsEditing;
            //            lblFormula.IsEditing = IsEditing;
            //            if (IsEditing)
            //            {
            //                btnEdit.Content = "Done";
            //            }
            //            else
            //            {
            //                btnEdit.Content = "Edit";
            //            }
            VariableEditor editor = new VariableEditor(Label, Variable, Formula, AllowFiltering);
            if (editor.ShowDialog() == true)
            {
                ChangeLabel(editor.Label);
                ChangeVariable(editor.Variable);
                ChangeFormula(editor.Formula);
                ChangeAllowFiltering(editor.AllowFiltering);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Remove();
        }

        public void Remove()
        {
            MainWindow.Instance.RemoveFilter(Variable);
            MainWindow.Instance.SafeRemoveGridColumn(Variable);
            MainWindow.Instance.CustomVariables.Children.Remove(this);
            StockInfo.PropertyDefinitions.Remove(Variable);
        }
    }
}
