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
            Window window = (Window)d;
            window.Loaded += Window_Loaded;
        }

        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = (Window)sender;
            ICommand command = GetLoadedCommand(window);
            command.Execute(e);
        }
    }
}
