using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockFinder.Controls
{
    /// <summary>
    /// Interaction logic for EditableLabel.xaml
    /// </summary>
    public partial class EditableLabel : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableLabel), new UIPropertyMetadata(string.Empty, TextChangedCallback));
        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditableLabel), new UIPropertyMetadata(false, IsEditingChangedCallback));

        private static void IsEditingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditableLabel el = (EditableLabel)d;
            el.IsEditing = (bool)e.NewValue;
        }

        private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditableLabel el = (EditableLabel)d;
            el.Text = e.NewValue as string;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsEditing
        {
            get
            {
                return (bool)GetValue(IsEditingProperty);
            }
            set
            {
                SetValue(IsEditingProperty, value);
                if (value)
                {
                    label.Visibility = Visibility.Collapsed;
                    textBox.Visibility = Visibility.Visible;
                    UpdateLayout();
                }
                else
                {
                    textBox.Visibility = Visibility.Collapsed;
                    label.Visibility = Visibility.Visible;
                }
            }
        }
        public EditableLabel()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
