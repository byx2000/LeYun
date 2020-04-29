using LeYun.Model;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeYun.View
{
    /// <summary>
    /// PathProjectPage.xaml 的交互逻辑
    /// </summary>
    public partial class PathProjectPage : Page
    {
        public PathProjectPage()
        {
            InitializeComponent();
            Loaded += PathProjectPage_Loaded;
        }

        private void PathProjectPage_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalData.CanvasWidth = canvas.ActualWidth;
            GlobalData.CanvasHeight = canvas.ActualHeight;
        }
    }
}
