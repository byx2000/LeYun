﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LeYun.ViewModel
{
    class MouseBehaviour
    {
        // 鼠标按下事件（包括左键、右键和中键）
        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached("MouseDownCommand", typeof(ICommand), typeof(MouseBehaviour), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseDownCommandChanged)));

        private static void MouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            element.MouseDown += element_MouseDown;
        }

        private static void element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ICommand command = GetMouseDownCommand(element);
            command.Execute(e);
        }

        public static void SetMouseDownCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseDownCommandProperty, value);
        }

        private static ICommand GetMouseDownCommand(FrameworkElement element)
        {
            return (ICommand)element.GetValue(MouseDownCommandProperty);
        }

        // 鼠标左键按下事件
        public static readonly DependencyProperty MouseLeftButtonDownCommandProperty =
            DependencyProperty.RegisterAttached("MouseLeftButtonDownCommand", typeof(ICommand), typeof(MouseBehaviour),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseLeftButtonDownCommandChanged)));

        private static void MouseLeftButtonDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            element.MouseLeftButtonDown += element_MouseLeftButtonDown;
        }

        private static void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ICommand command = GetMouseLeftButtonDownCommand(element);
            command.Execute(e);
        }

        public static void SetMouseLeftButtonDownCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseLeftButtonDownCommandProperty, value);
        }

        public static ICommand GetMouseLeftButtonDownCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MouseLeftButtonDownCommandProperty);
        }
    }
}
