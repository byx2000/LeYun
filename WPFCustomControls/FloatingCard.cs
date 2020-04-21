using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFCustomControls
{

    [ContentProperty("Content")]
    public class FloatingCard : Control
    {
        // 内容
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(FloatingCard), 
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 圆角
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(FloatingCard), 
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));



        // 静态构造函数
        static FloatingCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingCard), new FrameworkPropertyMetadata(typeof(FloatingCard)));
        }

        // 构造函数
        public FloatingCard()
        {
            Background = Brushes.White;
            BorderBrush = Brushes.Black;
            BorderThickness = new Thickness(0);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Border shadow = (Border)GetTemplateChild("PART_Shadow");
            Border content = (Border)GetTemplateChild("PART_Content");
            if (shadow != null && content != null)
            {
                Binding cornerRadiusBinding = new Binding() { Path = new PropertyPath("CornerRadius"), Source = this };
                shadow.SetBinding(Border.CornerRadiusProperty, cornerRadiusBinding);
                content.SetBinding(Border.CornerRadiusProperty, cornerRadiusBinding);
            }
            
        }
    }
}
