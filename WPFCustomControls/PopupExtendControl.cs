using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFCustomControls
{
    public class PopupExtendControl : Control
    {
        // 宿主内容
        public object HostContent
        {
            get { return (object)GetValue(HostContentProperty); }
            set { SetValue(HostContentProperty, value); }
        }
        public static readonly DependencyProperty HostContentProperty =
            DependencyProperty.Register("HostContent", typeof(object), typeof(PopupExtendControl), new PropertyMetadata(null));

        // 弹出窗口内容
        public UIElement PopupContent
        {
            get { return (UIElement)GetValue(PopupContentProperty); }
            set { SetValue(PopupContentProperty, value); }
        }
        public static readonly DependencyProperty PopupContentProperty =
            DependencyProperty.Register("PopupContent", typeof(UIElement), typeof(PopupExtendControl), new PropertyMetadata(null));

        // 弹出动画
        public PopupAnimation PopupAnimation
        {
            get { return (PopupAnimation)GetValue(PopupAnimationProperty); }
            set { SetValue(PopupAnimationProperty, value); }
        }
        public static readonly DependencyProperty PopupAnimationProperty =
            DependencyProperty.Register("PopupAnimation", typeof(PopupAnimation), typeof(PopupExtendControl), new PropertyMetadata(PopupAnimation.Fade));

        // 停靠方式
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(PopupExtendControl), new PropertyMetadata(PlacementMode.Bottom));


        // 静态构造函数
        static PopupExtendControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupExtendControl), new FrameworkPropertyMetadata(typeof(PopupExtendControl)));
        }

        ContentControl host;
        Popup popup;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            host = (ContentControl)GetTemplateChild("PART_Host");
            popup = (Popup)GetTemplateChild("PART_Popup");
            if (host != null && popup != null)
            {
                host.MouseEnter += Host_MouseEnter;
                host.MouseLeave += Host_MouseLeave;
                popup.MouseLeave += Popup_MouseLeave;
            }
        }

        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!host.IsMouseOver)
            {
                popup.IsOpen = false;
            }
        }

        private void Host_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!popup.IsMouseOver)
            {
                popup.IsOpen = false;
            }
        }

        private void Host_MouseEnter(object sender, MouseEventArgs e)
        {         
            popup.IsOpen = true;
        }
    }
}
