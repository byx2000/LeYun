using LeYun.Model;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LeYun.View.Dlg
{
    /// <summary>
    /// SplashScreen.xaml 的交互逻辑
    /// </summary>
    public partial class Splash : Window
    {
        MainWindow mainWindow = new MainWindow();

        public Splash()
        {
            InitializeComponent();
        }

        private void Splash_Loaded(object sender, RoutedEventArgs e)
        {
            // 启动计时器
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1500);
            timer.Tick += OnTimer;
            timer.Start();
        }

        // 1.5秒后关闭窗口
        private void OnTimer(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            Close();
        }
    }

    class ProgressWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double maxWidth = (double)values[0];
            double rate = (double)values[1];
            return maxWidth * rate;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ProgressValueConverter : IValueConverter
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

    class ActiveStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (GlobalData.IsActive)
            {
                return "正式版";
            }
            else
            {
                return "试用版";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
