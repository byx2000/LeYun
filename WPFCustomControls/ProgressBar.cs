using ProgressBar.Converter;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class ProgressBar : Control
    {
        // 值
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ProgressBar), 
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 是否启用动画
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(ProgressBar), 
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));



        Border frontBar;

        // 构造函数
        public ProgressBar()
        {
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2cc0ff"));
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e4e4e4"));
            Loaded += OnLoaded;
        }

        // 加载时播放动画
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (EnableAnimation)
            {
                PlayAnimation();
            }
        }

        // 播放动画
        public void PlayAnimation()
        {
            if (frontBar != null)
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0;
                animation.To = frontBar.ActualWidth;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                animation.EasingFunction = new CircleEase();
                animation.FillBehavior = FillBehavior.Stop;
                frontBar.BeginAnimation(Border.WidthProperty, animation);
            }
        }

        // 静态构造函数
        static ProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(typeof(ProgressBar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Border backBar = (Border)GetTemplateChild("PART_Back");
            if (backBar != null)
            {
                // 绑定背景色
                Binding backgroundBinding = new Binding("Background") { Source = this };
                backBar.SetBinding(Border.BackgroundProperty, backgroundBinding);
            }

            frontBar = (Border)GetTemplateChild("PART_Front");
            if (frontBar != null)
            {
                // 绑定前景色
                Binding backgroundBinding = new Binding("Foreground") { Source = this };
                frontBar.SetBinding(Border.BackgroundProperty, backgroundBinding);

                // 绑定进度条长度
                MultiBinding widthBinding = new MultiBinding();
                widthBinding.Converter = new FrontBarWidth();
                widthBinding.Bindings.Add(new Binding("ActualWidth") { Source = backBar });
                widthBinding.Bindings.Add(new Binding("Value") { Source = this });
                frontBar.SetBinding(WidthProperty, widthBinding);
            }
        }
    }
}

namespace ProgressBar.Converter
{
    class FrontBarWidth : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double maxWidth = (double)values[0];
            double val = (double)values[1];
            return maxWidth * val;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
