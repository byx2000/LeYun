using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LeYun.ViewModel
{
    class DataGridRowObserver
    {
        public static bool GetObserved(DependencyObject obj)
        {
            return (bool)obj.GetValue(ObservedProperty);
        }
        public static void SetObserved(DependencyObject obj, bool value)
        {
            obj.SetValue(ObservedProperty, value);
        }
        public static readonly DependencyProperty ObservedProperty =
            DependencyProperty.RegisterAttached("Observed", typeof(bool), typeof(DataGridRowObserver), new PropertyMetadata(new PropertyChangedCallback(OnObservedChanged)));

        private static void OnObservedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if ((bool)e.NewValue)
            {
                dataGrid.LoadingRow += DataGrid_LoadingRow;
            }
            else
            {
                dataGrid.LoadingRow -= DataGrid_LoadingRow;
            }
        }

        private static void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseDoubleClick += Row_MouseDoubleClick;
            e.Row.MouseEnter += Row_MouseEnter;
            e.Row.MouseLeave += Row_MouseLeave;
        }

        private static void Row_MouseLeave(object sender, MouseEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            DependencyObject obj = row;
            while (!(obj is DataGrid))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            ICommand command = GetMouseLeaveCommand(obj);
            if (command != null)
            {
                command.Execute(row);
            }
        }

        private static void Row_MouseEnter(object sender, MouseEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            DependencyObject obj = row;
            while (!(obj is DataGrid))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            ICommand command = GetMouseEnterCommand(obj);
            if (command != null)
            {
                command.Execute(row);
            }
        }

        private static void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            DependencyObject obj = row;
            while (!(obj is DataGrid))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            ICommand command = GetMouseDoubleClickCommand(obj);
            if (command != null)
            {
                command.Execute(row);
            }
        }

        // 双击
        public static ICommand GetMouseDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseDoubleClickCommandProperty);
        }

        public static void SetMouseDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseDoubleClickCommandProperty, value);
        }
        public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("MouseDoubleClickCommand", typeof(ICommand), typeof(DataGridRowObserver));

        // 鼠标进入
        public static ICommand GetMouseEnterCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseEnterCommandProperty);
        }
        public static void SetMouseEnterCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseEnterCommandProperty, value);
        }
        public static readonly DependencyProperty MouseEnterCommandProperty =
            DependencyProperty.RegisterAttached("MouseEnterCommand", typeof(ICommand), typeof(DataGridRowObserver));

        // 鼠标离开
        public static ICommand GetMouseLeaveCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseLeaveCommandProperty);
        }
        public static void SetMouseLeaveCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseLeaveCommandProperty, value);
        }
        public static readonly DependencyProperty MouseLeaveCommandProperty =
            DependencyProperty.RegisterAttached("MouseLeaveCommand", typeof(ICommand), typeof(DataGridRowObserver));
    }
}
