using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFCustomControls
{
    public class Dialog : Window
    {
        static Dialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Dialog), new FrameworkPropertyMetadata(typeof(Dialog)));
        }

        // 缩放变换
        ScaleTransform scaleTransform;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Border titleBar = (Border)GetTemplateChild("PART_TitleBar");
            if (titleBar != null)
            {
                titleBar.MouseMove += OnTitleBarMouseMove;
            }

            Button closeButton = (Button)GetTemplateChild("PART_CloseButton");
            if (closeButton != null)
            {
                closeButton.Click += OnCloseButtonClick;
            }

            scaleTransform = (ScaleTransform)GetTemplateChild("PART_ScaleTransform");
        }

        // 点击关闭按钮时关闭窗口
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // 覆盖基类Close函数，添加关闭时动画效果
        public new void Close()
        {
            if (scaleTransform != null)
            {
                DoubleAnimation anim = new DoubleAnimation();
                anim.From = 1;
                anim.To = 0;
                anim.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                anim.EasingFunction = new CircleEase();
                anim.Completed += delegate (object sender, EventArgs e) 
                {
                    base.Close();
                };
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
            }
            else
            {
                base.Close();
            }
        }

        // 拖拽标题栏时移动窗口
        private void OnTitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
