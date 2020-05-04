using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LeYun.ViewModel
{
    class ThumbBehavior
    {
        // 拖动事件
        public static readonly DependencyProperty DragDeltaCommandProperty =
            DependencyProperty.RegisterAttached("DragDeltaCommand", typeof(ICommand), typeof(ThumbBehavior),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(DragDeltaCommandChanged)));

        private static void DragDeltaCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thumb thumb = (Thumb)d;
            thumb.DragDelta += Thumb_DragDelta;
        }

        private static void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ICommand command = GetDragDeltaCommand(element);
            command.Execute(e);
        }

        public static void SetDragDeltaCommand(UIElement element, ICommand value)
        {
            element.SetValue(DragDeltaCommandProperty, value);
        }

        private static ICommand GetDragDeltaCommand(FrameworkElement element)
        {
            return (ICommand)element.GetValue(DragDeltaCommandProperty);
        }
    }
}
