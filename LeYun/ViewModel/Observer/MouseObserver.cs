using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeYun.ViewModel
{
    static class MouseObserver
    {
        // 是否监视鼠标位置
        public static bool GetObserve(DependencyObject obj)
        {
            return (bool)obj.GetValue(ObserveProperty);
        }
        public static void SetObserve(DependencyObject obj, bool value)
        {
            obj.SetValue(ObserveProperty, value);
        }
        public static readonly DependencyProperty ObserveProperty =
            DependencyProperty.RegisterAttached("Observe", typeof(bool), typeof(MouseObserver), new FrameworkPropertyMetadata(OnObserveChanged));

        private static void OnObserveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            if ((bool)e.NewValue)
            {
                element.MouseMove += Element_MouseMove;
            }
            else
            {
                element.MouseMove -= Element_MouseMove;
            }
        }

        private static void Element_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Point mousePos = e.GetPosition(element);
            element.SetCurrentValue(MouseXProperty, mousePos.X);
            element.SetCurrentValue(MouseYProperty, mousePos.Y);
        }

        // 鼠标X坐标
        public static double GetMouseX(DependencyObject obj)
        {
            return (double)obj.GetValue(MouseXProperty);
        }
        public static void SetMouseX(DependencyObject obj, double value)
        {
            obj.SetValue(MouseXProperty, value);
        }
        public static readonly DependencyProperty MouseXProperty =
            DependencyProperty.RegisterAttached("MouseX", typeof(double), typeof(MouseObserver));

        // 鼠标Y坐标
        public static double GetMouseY(DependencyObject obj)
        {
            return (double)obj.GetValue(MouseYProperty);
        }
        public static void SetMouseY(DependencyObject obj, double value)
        {
            obj.SetValue(MouseYProperty, value);
        }
        public static readonly DependencyProperty MouseYProperty =
            DependencyProperty.RegisterAttached("MouseY", typeof(double), typeof(MouseObserver));
    }
}
