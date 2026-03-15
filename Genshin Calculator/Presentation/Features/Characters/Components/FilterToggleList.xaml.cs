using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Genshin_Calculator.Presentation.Features.Characters.Components
{
    public partial class FilterToggleList : UserControl
    {
        public static readonly DependencyProperty IconConverterProperty =
            DependencyProperty.Register(nameof(IconConverter), typeof(IValueConverter), typeof(FilterToggleList));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable), typeof(FilterToggleList));

        public static readonly DependencyProperty ToggleCommandProperty =
            DependencyProperty.Register(nameof(ToggleCommand), typeof(ICommand), typeof(FilterToggleList));

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IEnumerable), typeof(FilterToggleList));

        public FilterToggleList()
        {
            this.InitializeComponent();
        }

        public IEnumerable Items
        {
            get { return (IEnumerable)this.GetValue(ItemsProperty); }
            set { this.SetValue(ItemsProperty, value); }
        }

        public IEnumerable SelectedItems
        {
            get { return (IEnumerable)this.GetValue(SelectedItemsProperty); }
            set { this.SetValue(SelectedItemsProperty, value); }
        }

        public ICommand ToggleCommand
        {
            get { return (ICommand)this.GetValue(ToggleCommandProperty); }
            set { this.SetValue(ToggleCommandProperty, value); }
        }

        public IValueConverter IconConverter
        {
            get { return (IValueConverter)this.GetValue(IconConverterProperty); }
            set { this.SetValue(IconConverterProperty, value); }
        }
    }
}