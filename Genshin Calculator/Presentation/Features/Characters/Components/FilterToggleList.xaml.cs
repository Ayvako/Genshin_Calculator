using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Genshin_Calculator.Presentation.Features.Characters.Components
{
    public enum FilterSelectionState
    {
        None = 0,

        Selected = 1,

        Excluded = 2,
    }

    public partial class FilterToggleList : UserControl
    {
        public static readonly DependencyProperty IconConverterProperty =
            DependencyProperty.Register(nameof(IconConverter), typeof(IValueConverter), typeof(FilterToggleList));

        public static readonly DependencyProperty ToggleCommandProperty =
            DependencyProperty.Register(nameof(ToggleCommand), typeof(ICommand), typeof(FilterToggleList));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(FilterToggleList), new PropertyMetadata(null, (d, e) => ((FilterToggleList)d).OnListPropertyChanged(ref ((FilterToggleList)d).selectedItemsObservable, e)));

        public static readonly DependencyProperty ExcludedItemsProperty =
            DependencyProperty.Register(nameof(ExcludedItems), typeof(IList), typeof(FilterToggleList), new PropertyMetadata(null, (d, e) => ((FilterToggleList)d).OnListPropertyChanged(ref ((FilterToggleList)d).excludedItemsObservable, e)));

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IEnumerable), typeof(FilterToggleList), new PropertyMetadata(null, (d, e) => ((FilterToggleList)d).OnListPropertyChanged(ref ((FilterToggleList)d).itemsObservable, e)));

        public static readonly DependencyProperty SelectionStateProperty =
            DependencyProperty.RegisterAttached("SelectionState", typeof(FilterSelectionState), typeof(FilterToggleList), new FrameworkPropertyMetadata(FilterSelectionState.None));

        private INotifyCollectionChanged? selectedItemsObservable;

        private INotifyCollectionChanged? excludedItemsObservable;

        private INotifyCollectionChanged? itemsObservable;

        public FilterToggleList()
        {
            this.InitializeComponent();

            this.Loaded += (_, _) => this.ScheduleRefresh();
            this.ItemsHost.ItemContainerGenerator.StatusChanged += (_, _) =>
            {
                if (this.ItemsHost.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                {
                    this.ScheduleRefresh();
                }
            };
        }

        public IEnumerable Items { get => (IEnumerable)this.GetValue(ItemsProperty); set => this.SetValue(ItemsProperty, value); }

        public IList SelectedItems { get => (IList)this.GetValue(SelectedItemsProperty); set => this.SetValue(SelectedItemsProperty, value); }

        public IList ExcludedItems { get => (IList)this.GetValue(ExcludedItemsProperty); set => this.SetValue(ExcludedItemsProperty, value); }

        public IValueConverter IconConverter { get => (IValueConverter)this.GetValue(IconConverterProperty); set => this.SetValue(IconConverterProperty, value); }

        public ICommand ToggleCommand { get => (ICommand)this.GetValue(ToggleCommandProperty); set => this.SetValue(ToggleCommandProperty, value); }

        public static void SetSelectionState(DependencyObject element, FilterSelectionState value) => element.SetValue(SelectionStateProperty, value);

        public static FilterSelectionState GetSelectionState(DependencyObject element) => (FilterSelectionState)element.GetValue(SelectionStateProperty);

        private static void ToggleItemState(object sender, IList? primaryList, IList? secondaryList)
        {
            if (sender is not FrameworkElement { DataContext: not null } fe || primaryList == null || secondaryList == null)
            {
                return;
            }

            var item = fe.DataContext;

            if (primaryList.Contains(item))
            {
                primaryList.Remove(item);
            }
            else
            {
                secondaryList.Remove(item);
                primaryList.Add(item);
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject? parent)
            where T : DependencyObject
        {
            if (parent == null)
            {
                yield break;
            }

            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                {
                    yield return typedChild;
                }

                foreach (var descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private void OnListPropertyChanged(ref INotifyCollectionChanged? backingField, DependencyPropertyChangedEventArgs e)
        {
            if (backingField != null)
            {
                backingField.CollectionChanged -= this.OnCollectionChanged;
            }

            backingField = e.NewValue as INotifyCollectionChanged;

            if (backingField != null)
            {
                backingField.CollectionChanged += this.OnCollectionChanged;
            }

            this.ScheduleRefresh();
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => this.ScheduleRefresh();

        private void ScheduleRefresh()
        {
            if (this.IsLoaded)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(this.RefreshVisualStates));
            }
        }

        private void RefreshVisualStates()
        {
            foreach (var button in FindVisualChildren<Button>(this.ItemsHost))
            {
                if (button.DataContext is null)
                {
                    continue;
                }

                var state = FilterSelectionState.None;

                if (this.ExcludedItems?.Contains(button.DataContext) == true)
                {
                    state = FilterSelectionState.Excluded;
                }
                else if (this.SelectedItems?.Contains(button.DataContext) == true)
                {
                    state = FilterSelectionState.Selected;
                }

                SetSelectionState(button, state);
            }
        }

        private void OnLeftClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                e.Handled = true;
                return;
            }

            ToggleItemState(sender, this.SelectedItems, this.ExcludedItems);
            e.Handled = true;
        }

        private void OnRightClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                e.Handled = true;
                return;
            }

            ToggleItemState(sender, this.ExcludedItems, this.SelectedItems);
            e.Handled = true;
        }
    }
}