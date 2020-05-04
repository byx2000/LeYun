using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeYun.ViewModel
{
    class AutoSelectBehavior
    {
        public static bool GetAutoSelectOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSelectOnFocusProperty);
        }
        public static void SetAutoSelectOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSelectOnFocusProperty, value);
        }
        public static readonly DependencyProperty AutoSelectOnFocusProperty =
            DependencyProperty.RegisterAttached("AutoSelectOnFocus", typeof(bool), typeof(AutoSelectBehavior), new PropertyMetadata(new PropertyChangedCallback(OnAutoSelectOnFocusChanged)));

        private static void OnAutoSelectOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if (textBox == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
            }
            else
            {
                textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.LostFocus -= TextBox_LostFocus;
            }
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
        }

        private static void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            textBox.Focus();
            e.Handled = true;
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            textBox.SelectAll();
            textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
        }
    }
}
