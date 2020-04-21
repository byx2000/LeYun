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
using WPFCustomControls;

namespace WPFCustomControls
{
    public class ProgressPie : Control
    {
        static ProgressPie()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressPie), new FrameworkPropertyMetadata(typeof(ProgressPie)));
        }

        public ProgressPie()
        {
            Loaded += OnLoaded;
        }

        // 百分比
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ProgressPie),
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), new ValidateValueCallback(IsValueValid));

        // 验证Value合法性
        private static bool IsValueValid(object value)
        {
            double val = (double)value;
            return val >= 0 && val <= 1;
        }

        // 提示文字
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ProgressPie), 
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 加载事件
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 播放加载动画
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = Value;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animation.EasingFunction = new CircleEase();
            animation.FillBehavior = FillBehavior.Stop;
            BeginAnimation(ValueProperty, animation);
        }
    }

    
}

// 转换器
namespace ProgressPie.Converter
{
    class OuterRadius : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            return Math.Min(actualWidth, actualHeight);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class InnerRadius : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            return 0.7 * Math.Min(actualWidth, actualHeight);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FillOuterRadius : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            double r = 0.95 * Math.Min(actualWidth, actualHeight);
            return new Size(r / 2, r / 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FillInnerRadius : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            double r = 0.75 * Math.Min(actualWidth, actualHeight);
            return new Size(r / 2, r / 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FillP0 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            double len = Math.Min(actualWidth, actualHeight);
            return new Point(actualWidth / 2, (actualHeight - len * 0.95) / 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FillP1 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            double len = Math.Min(actualWidth, actualHeight);
            return new Point(actualWidth / 2, (actualHeight - len * 0.75) / 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FillP2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            double val = (double)values[2];
            double len = Math.Min(actualWidth, actualHeight);
            double angle = val * 360 - 90 - 0.001;

            Point p = CoordTransform.PolarToCartesian(angle, 0.75 * len / 2);
            p.X += actualWidth / 2;
            p.Y += actualHeight / 2;

            return p;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FillP3 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            double val = (double)values[2];
            double len = Math.Min(actualWidth, actualHeight);
            double angle = val * 360 - 90 - 0.001;

            Point p = CoordTransform.PolarToCartesian(angle, 0.95 * len / 2);
            p.X += actualWidth / 2;
            p.Y += actualHeight / 2;

            return p;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class IsLargeArc : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;
            return val > 0.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CurrentValueText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CurrentValueTextFontSize : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            return Math.Min(actualWidth, actualHeight) / 7.5;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TextFontSize : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)values[0];
            double actualHeight = (double)values[1];
            return Math.Min(actualWidth, actualHeight) / 20;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
