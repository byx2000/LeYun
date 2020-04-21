using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BarChart.Converter;

namespace WPFCustomControls
{
    public class BarItem : DependencyObject
    {
        // 标签
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(BarItem), 
                new FrameworkPropertyMetadata("label",
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 数值
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(BarItem), 
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 颜色
        public Color Fill
        {
            get { return (Color)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Color), typeof(BarItem), 
                new FrameworkPropertyMetadata((Color)ColorConverter.ConvertFromString("#4da7fc"),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));


    }

    public class BarItemCollection : ObservableCollection<BarItem>
    {

    }

    [ContentProperty("BarItems")]
    public class BarChart : Control
    {
        // 最小值
        public double MinVal
        {
            get { return (double)GetValue(MinValProperty); }
            set { SetValue(MinValProperty, value); }
        }
        public static readonly DependencyProperty MinValProperty =
            DependencyProperty.Register("MinVal", typeof(double), typeof(BarChart), 
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 最大值
        public double MaxVal
        {
            get { return (double)GetValue(MaxValProperty); }
            set { SetValue(MaxValProperty, value); }
        }
        public static readonly DependencyProperty MaxValProperty =
            DependencyProperty.Register("MaxVal", typeof(double), typeof(BarChart), 
                new FrameworkPropertyMetadata(100d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 间隔段数
        public int Segment
        {
            get { return (int)GetValue(SegmentProperty); }
            set { SetValue(SegmentProperty, value); }
        }
        public static readonly DependencyProperty SegmentProperty =
            DependencyProperty.Register("Segment", typeof(int), typeof(BarChart), 
                new FrameworkPropertyMetadata(10,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 数据集
        public BarItemCollection BarItems
        {
            get { return (BarItemCollection)GetValue(BarItemsProperty); }
            set { SetValue(BarItemsProperty, value); }
        }
        public static readonly DependencyProperty BarItemsProperty =
            DependencyProperty.Register("BarItems", typeof(BarItemCollection), typeof(BarChart), 
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        // 保存柱状图border
        List<Border> bars = new List<Border>();

        // popup弹出窗口
        Popup popup;

        // 弹出窗口的文本
        TextBlock popupTextBlock;

        // 静态构造函数
        static BarChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BarChart), new FrameworkPropertyMetadata(typeof(BarChart)));
        }

        // 构造函数
        public BarChart()
        {
            BarItems = new BarItemCollection();
            FontSize = 0.1;
            Loaded += OnLoaded;
        }

        // 加载时播放动画
        private void OnLoaded(object sender, RoutedEventArgs e)
        {            
            for (int i = 0; i < bars.Count; ++i)
            {
                DoubleAnimation anim = new DoubleAnimation();
                anim.From = 0;
                anim.To = bars[i].ActualHeight;
                anim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                anim.EasingFunction = new CircleEase();
                anim.FillBehavior = FillBehavior.Stop;
                bars[i].BeginAnimation(Border.HeightProperty, anim);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 绘制纵轴标签和网格线
            Grid vLabel = (Grid)GetTemplateChild("PART_VerticalLabel");
            Grid backLine = (Grid)GetTemplateChild("PART_BackLine");
            if (vLabel != null && backLine != null)
            {
                for (int i = 0; i < Segment + 1; ++i)
                {
                    // 创建纵轴标签
                    TextBlock label = new TextBlock()
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949494")),
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(10, 0, 10, 0),
                    };

                    // 绑定纵轴标签数值
                    MultiBinding vLabelValBinding = new MultiBinding();
                    vLabelValBinding.Converter = new VerticalLabelValueConverter();
                    vLabelValBinding.Bindings.Add(new Binding() { Path = new PropertyPath("MinVal"), Source = this });
                    vLabelValBinding.Bindings.Add(new Binding() { Path = new PropertyPath("MaxVal"), Source = this });
                    vLabelValBinding.Bindings.Add(new Binding() { Path = new PropertyPath("Segment"), Source = this });
                    vLabelValBinding.ConverterParameter = i;
                    vLabelValBinding.StringFormat = string.Format("{0:F2}", 0);
                    label.SetBinding(TextBlock.TextProperty, vLabelValBinding);

                    // 设置纵轴标签字体大小
                    if (FontSize < 1)
                    {
                        MultiBinding vLabelFontSizeBinding = new MultiBinding();
                        vLabelFontSizeBinding.Converter = new LabelFontSizeConverter();
                        vLabelFontSizeBinding.Bindings.Add(new Binding() { Path = new PropertyPath("Segment"), Source = this });
                        vLabelFontSizeBinding.Bindings.Add(new Binding() { Path = new PropertyPath("ActualHeight"), Source = this });
                        label.SetBinding(TextBlock.FontSizeProperty, vLabelFontSizeBinding);
                    }
                    else
                    {
                        label.FontSize = FontSize;
                    }

                    // 将纵轴标签加入网格
                    vLabel.RowDefinitions.Add(new RowDefinition());
                    label.SetValue(Grid.RowProperty, i);
                    vLabel.Children.Add(label);

                    // 创建网格线
                    Border line = new Border()
                    {
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e3e3e3")),
                        BorderThickness = new Thickness(0, 0, 0, 1),
                        VerticalAlignment = VerticalAlignment.Bottom,
                    };

                    // 将网格线加入网格
                    backLine.RowDefinitions.Add(new RowDefinition());
                    line.SetValue(Grid.RowProperty, i);
                    backLine.Children.Add(line);
                }
            }

            // 绘制横轴标签
            Grid hLabel = (Grid)GetTemplateChild("PART_HorizontalLabel");
            if (hLabel != null)
            {
                for (int i = 0; i < BarItems.Count; ++i)
                {
                    // 创建横轴标签
                    TextBlock label = new TextBlock()
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949494")),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 10, 0, 10),
                        TextWrapping = TextWrapping.Wrap,
                    };

                    // 绑定横轴标签文字
                    Binding vLabelTextBinding = new Binding() { Path = new PropertyPath("Label"), Source = BarItems[i] };
                    label.SetBinding(TextBlock.TextProperty, vLabelTextBinding);

                    // 设置横轴标签字体大小
                    if (FontSize < 1)
                    {
                        MultiBinding hLabelFontSizeBinding = new MultiBinding();
                        hLabelFontSizeBinding.Converter = new LabelFontSizeConverter();
                        hLabelFontSizeBinding.Bindings.Add(new Binding() { Path = new PropertyPath("Segment"), Source = this });
                        hLabelFontSizeBinding.Bindings.Add(new Binding() { Path = new PropertyPath("ActualHeight"), Source = this });
                        label.SetBinding(TextBlock.FontSizeProperty, hLabelFontSizeBinding);
                    }
                    else
                    {
                        label.FontSize = FontSize;
                    }

                    // 将横轴标签加入网格
                    hLabel.ColumnDefinitions.Add(new ColumnDefinition());
                    label.SetValue(Grid.ColumnProperty, i);
                    hLabel.Children.Add(label);
                }
            }

            // 绘制柱状图
            Grid content = (Grid)GetTemplateChild("PART_Content");
            if (content != null)
            {
                for (int i = 0; i < BarItems.Count; ++i)
                {
                    // 创建柱状图
                    Border bar = new Border()
                    {
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Style = (Style)FindResource("barStyle"),
                    };

                    // 绑定柱状图边距
                    Binding barMarginBinding = new Binding() { Path = new PropertyPath("ActualWidth"), Source = bar, Converter = new BarMarginConverter() };
                    bar.SetBinding(Border.MarginProperty, barMarginBinding);

                    // 绑定柱状图填充颜色
                    Binding barFillBinding = new Binding() { Path = new PropertyPath("Fill"), Source = BarItems[i], Converter = new BarFillConverter() };
                    bar.SetBinding(Border.BackgroundProperty, barFillBinding);

                    // 绑定柱状图高度
                    MultiBinding barHeightBinding = new MultiBinding();
                    barHeightBinding.Converter = new BarHeightConverter();
                    barHeightBinding.Bindings.Add(new Binding() { Path = new PropertyPath("MinVal"), Source = this });
                    barHeightBinding.Bindings.Add(new Binding() { Path = new PropertyPath("MaxVal"), Source = this });
                    barHeightBinding.Bindings.Add(new Binding() { Path = new PropertyPath("Segment"), Source = this });
                    barHeightBinding.Bindings.Add(new Binding() { Path = new PropertyPath("Value"), Source = BarItems[i] });
                    barHeightBinding.Bindings.Add(new Binding() { Path = new PropertyPath("ActualHeight"), Source = content });
                    bar.SetBinding(Border.HeightProperty, barHeightBinding);

                    // 添加鼠标事件
                    bar.MouseEnter += OnBarMouseEnter;
                    bar.MouseLeave += OnBarMouseLeave;

                    // 将柱状图加入网格
                    content.ColumnDefinitions.Add(new ColumnDefinition());
                    bar.SetValue(Grid.ColumnProperty, i);
                    content.Children.Add(bar);
                    bar.Tag = bars.Count;
                    bars.Add(bar);
                }
            }

            popup = (Popup)GetTemplateChild("PART_popup");
            popupTextBlock = (TextBlock)GetTemplateChild("PART_Value");
        }

        // 鼠标进入时显示popup窗口
        private void OnBarMouseEnter(object sender, MouseEventArgs e)
        {
            Border bar = (Border)sender;

            string str = string.Format("{0:F2}", BarItems[(int)bar.Tag].Value);
            popupTextBlock.Text = str;
            if (FontSize < 1)
            {
                popupTextBlock.FontSize = (bar.ActualWidth) / str.Length;
            }
            else
            {
                popupTextBlock.FontSize = FontSize;
            }                

            popup.Width = bar.ActualWidth;
            popup.Height = 50;
            popup.PlacementTarget = bar;
            popup.IsOpen = true;
        }

        // 鼠标离开时关闭popup窗口
        private void OnBarMouseLeave(object sender, MouseEventArgs e)
        {
            popup.IsOpen = false;
        }
    }
}

namespace BarChart.Converter
{
    class VerticalLabelValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double minVal = (double)values[0];
            double maxVal = (double)values[1];
            int segment = (int)values[2];
            double index = (int)parameter;

            double span = (maxVal - minVal) / segment;
            double val = maxVal - index * span;

            return val;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class LabelFontSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int segment = (int)values[0];
            double height = (double)values[1];
            return Math.Min(height / (segment + 1) / 2, 14);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BarMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double barWidth = (double)value;
            return new Thickness(barWidth / 5, 0, barWidth / 5, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BarFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color fill = (Color)value;
            return new SolidColorBrush(fill);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BarHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double minVal = (double)values[0];
            double maxVal = (double)values[1];
            int segment = (int)values[2];
            double val = (double)values[3];
            double maxHeight = (double)values[4];

            //double span = (maxVal - minVal) / segment;
            //maxVal = minVal + span * (segment + 1);
            maxHeight *= ((double)segment / (segment + 1));

            if (val <= minVal)
            {
                return 0;
            }
            else if (val >= maxVal)
            {
                return maxHeight;
            }

            return (val - minVal) / (maxVal - minVal) * maxHeight;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class PopupShapeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)values[0];
            double height = (double)values[1];

            switch (parameter)
            {
                case "1":
                    return new Point(width, 0);
                case "2":
                    return new Point(width, height - 10);
                case "3":
                    return new Point(width / 2 + 10, height - 10);
                case "4":
                    return new Point(width / 2, height);
                case "5":
                    return new Point(width / 2 - 10, height - 10);
                case "6":
                    return new Point(0, height - 10);
                default:
                    return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}