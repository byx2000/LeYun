using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LeYun.ViewModel
{
    class WindowBehavior
    {
        // 窗口加载事件
        public static ICommand GetLoadedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadedCommandProperty);
        }
        public static void SetLoadedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadedCommandProperty, value);
        }
        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.RegisterAttached("LoadedCommand", typeof(ICommand), typeof(WindowBehavior), new PropertyMetadata(new PropertyChangedCallback(OnLoadedCommandChanged)));

        private static void OnLoadedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                FrameworkElement window = (FrameworkElement)d;
                window.Loaded += Window_Loaded;
            }
            catch (Exception)
            {

            }
        }

        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement window = (FrameworkElement)sender;
                ICommand command = GetLoadedCommand(window);
                command.Execute(e);
            }
            catch (Exception)
            {

            }
        }

        // 窗口关闭事件
        public static ICommand GetClosingCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClosingCommandProperty);
        }
        public static void SetClosingCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClosingCommandProperty, value);
        }
        public static readonly DependencyProperty ClosingCommandProperty =
            DependencyProperty.RegisterAttached("ClosingCommand", typeof(ICommand), typeof(WindowBehavior), new PropertyMetadata(new PropertyChangedCallback(ClosingCommandChanged)));

        private static void ClosingCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                Window window = (Window)d;
                window.Closing += Window_Closing;
            }
            catch (Exception)
            {

            }
        }

        private static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                FrameworkElement window = (FrameworkElement)sender;
                ICommand command = GetClosingCommand(window);
                command.Execute(e);
            }
            catch (Exception)
            {

            }
        }
    }
}
