using LeYun.Model;
using LeYun.View.Dlg;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        
        // 构造函数
        public MainWindow()
        {
            InitializeComponent();
        }

        // 最小化
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // 关闭
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (!ChooseBox.Show("确认退出？"))
            {
                return;
            }

            Close();
        }

        // 鼠标拖动
        private void mainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y <= TitleBar.ActualHeight)
            {
                DragMove();
            }
        }

        // 页面切换
        private void PathProjectPage_Checked(object sender, RoutedEventArgs e)
        {
            GlobalData.CurrentPage = GlobalData.PathProjectPage;
        }
        private void RouteRecordPage_Checked(object sender, RoutedEventArgs e)
        {
            GlobalData.CurrentPage = GlobalData.RouteRecordPage;
        }
        private void SettingPage_Checked(object sender, RoutedEventArgs e)
        {
            GlobalData.CurrentPage = GlobalData.SettingPage;
        }
        private void AboutPage_Checked(object sender, RoutedEventArgs e)
        {
            GlobalData.CurrentPage = GlobalData.AboutPage;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!GlobalData.IsActive)
            {
                ActivateDlg dlg = new ActivateDlg();
                dlg.ShowDialog();
            }
        }
    }
}
