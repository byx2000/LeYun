using BarChart.Converter;
using HorizontalBarChart.Converter;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFCustomControls
{
    [ContentProperty("BarItems")]
    public class HorizontalBarChart : Control
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
        static HorizontalBarChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalBarChart), new FrameworkPropertyMetadata(typeof(HorizontalBarChart)));
        }

        // 构造函数
        public HorizontalBarChart()
        {
            BarItems = new BarItemCollection();
            FontSize = 0.1;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PlayAnimation();
        }

        public void PlayAnimation()
        {
            for (int i = 0; i < bars.Count; ++i)
            {
                DoubleAnimation anim = new DoubleAnimation();
                anim.From = 0;
                anim.To = bars[i].ActualWidth;
                anim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                anim.EasingFunction = new CircleEase();
                anim.FillBehavior = FillBehavior.Stop;
                bars[i].BeginAnimation(Border.WidthProperty, anim);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //绘制纵轴标签
            Grid vLabel = (Grid)GetTemplateChild("PART_VerticalLabel");
            if (vLabel != null)
            {
                for (int i = 0; i < BarItems.Count; ++i)
                {
                    // 创建纵轴标签
                    TextBlock label = new TextBlock()
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949494")),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(10, 0, 10, 0),
                    };

                    // 绑定纵轴标签文字
                    Binding vLabelTextBinding = new Binding() { Path = new PropertyPath("Label"), Source = BarItems[i] };
                    label.SetBinding(TextBlock.TextProperty, vLabelTextBinding);
                    //label.Text = "车辆";

                    // 设置纵轴标签字体大小
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

                    // 将纵轴标签加入网格
                    vLabel.RowDefinitions.Add(new RowDefinition());
                    label.SetValue(Grid.RowProperty, i);
                    vLabel.Children.Add(label);
                }
            }

            // 绘制横轴标签和网格线
            Grid hLabel = (Grid)GetTemplateChild("PART_HorizontalLabel");
            Grid backLine = (Grid)GetTemplateChild("PART_BackLine");
            if (hLabel != null && backLine != null)
            {
                for (int i = 0; i < Segment + 1; ++i)
                {
                    // 创建横轴标签
                    TextBlock label = new TextBlock()
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949494")),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 10, 0, 10),
                    };

                    // 绑定横轴标签数值
                    MultiBinding vLabelValBinding = new MultiBinding();
                    vLabelValBinding.Converter = new VerticalLabelValueConverter();
                    vLabelValBinding.Bindings.Add(new Binding() { Path = new PropertyPath("MinVal"), Source = this });
                    vLabelValBinding.Bindings.Add(new Binding() { Path = new PropertyPath("MaxVal"), Source = this });
                    vLabelValBinding.Bindings.Add(new Binding() { Path = new PropertyPath("Segment"), Source = this });
                    vLabelValBinding.ConverterParameter = Segment - i;
                    vLabelValBinding.StringFormat = string.Format("{0:F2}", 0);
                    label.SetBinding(TextBlock.TextProperty, vLabelValBinding);

                    // 设置横轴标签字体大小
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

                    // 将横轴标签加入网格
                    hLabel.ColumnDefinitions.Add(new ColumnDefinition());
                    label.SetValue(Grid.ColumnProperty, i);
                    hLabel.Children.Add(label);

                    // 创建网格线
                    Border line = new Border()
                    {
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e3e3e3")),
                        BorderThickness = new Thickness(1, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };

                    // 将网格线加入网格
                    backLine.ColumnDefinitions.Add(new ColumnDefinition());
                    line.SetValue(Grid.ColumnProperty, i);
                    backLine.Children.Add(line);
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
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };

                    // 绑定柱状图填充颜色
                    Binding barFillBinding = new Binding() { Path = new PropertyPath("Fill"), Source = BarItems[i], Converter = new BarFillConverter() };
                    bar.SetBinding(Border.BackgroundProperty, barFillBinding);
                    //bar.Background = new SolidColorBrush(BarItems[i].Fill);

                    // 绑定柱状图边距
                    Binding barMarginBinding = new Binding() { Path = new PropertyPath("ActualHeight"), Source = bar, Converter = new global::HorizontalBarChart.Converter.BarMarginConverter() };
                    bar.SetBinding(Border.MarginProperty, barMarginBinding);

                    //  绑定柱状图宽度
                    MultiBinding barWidthBinding = new MultiBinding();
                    barWidthBinding.Converter = new global::HorizontalBarChart.Converter.BarHeightConverter();
                    barWidthBinding.Bindings.Add(new Binding() { Path = new PropertyPath("MinVal"), Source = this });
                    barWidthBinding.Bindings.Add(new Binding() { Path = new PropertyPath("MaxVal"), Source = this });
                    barWidthBinding.Bindings.Add(new Binding() { Path = new PropertyPath("Segment"), Source = this });
                    barWidthBinding.Bindings.Add(new Binding() { Path = new PropertyPath("Value"), Source = BarItems[i] });
                    barWidthBinding.Bindings.Add(new Binding() { Path = new PropertyPath("ActualWidth"), Source = content });
                    bar.SetBinding(Border.WidthProperty, barWidthBinding);

                    // 添加鼠标事件
                    bar.MouseEnter += OnBarMouseEnter;
                    bar.MouseLeave += OnBarMouseLeave;

                    // 将柱状图加入网格
                    content.RowDefinitions.Add(new RowDefinition());
                    bar.SetValue(Grid.RowProperty, i);
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
                popupTextBlock.FontSize = (bar.ActualHeight) / 3;
            }
            else
            {
                popupTextBlock.FontSize = FontSize;
            }

            popup.Width = popupTextBlock.FontSize * str.Length;
            popup.Height = bar.ActualHeight;
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

namespace HorizontalBarChart.Converter
{
    class BarMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double barHeight = (double)value;
            return new Thickness(1, barHeight / 5, 0, barHeight / 5);
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
            double maxWidth = (double)values[4];

            maxWidth *= ((double)segment / (segment + 1));

            if (val <= minVal)
            {
                return 0;
            }
            else if (val >= maxVal)
            {
                return maxWidth;
            }

            return (val - minVal) / (maxVal - minVal) * maxWidth;
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
            double d = height / 4;

            switch (parameter)
            {
                case "0":
                    return new Point(d, 0);
                case "1":
                    return new Point(width, 0);
                case "2":
                    return new Point(width, height);
                case "3":
                    return new Point(d, height);
                case "4":
                    return new Point(d, (height + d) / 2);
                case "5":
                    return new Point(0, height / 2);
                case "6":
                    return new Point(d, (height - d) / 2);
                default:
                    return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class PopupTextMargin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = (double)value;
            return new Thickness(height / 4, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
