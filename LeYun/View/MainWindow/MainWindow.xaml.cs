﻿using LeYun.Model;
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

            // 设置各个子页面的视图模型
            GlobalData.PathProjectPage.DataContext = GlobalData.PathProjectPageViewModel;
            GlobalData.RouteRecordPage.DataContext = GlobalData.RouteRecordPageViewModel;
            GlobalData.SettingPage.DataContext = GlobalData.SettingPageViewModel;
            GlobalData.AboutPage.DataContext = GlobalData.AboutPageViewModel;
        }

        // 最小化
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // 关闭
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // 关闭前保存设置
        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalData.WriteConfiguration(GlobalData.LineThicknessKey, GlobalData.LineThickness.ToString());
            GlobalData.WriteConfiguration(GlobalData.NodeButtonWidthKey, GlobalData.NodeButtonWidth.ToString());
            GlobalData.WriteConfiguration(GlobalData.MaxNodeXKey, GlobalData.MaxNodeX.ToString());
            GlobalData.WriteConfiguration(GlobalData.MaxNodeYKey, GlobalData.MaxNodeY.ToString());
            GlobalData.WriteConfiguration(GlobalData.DemoDurationKey, GlobalData.DemoDuration.ToString());
            GlobalData.WriteConfiguration(GlobalData.ActiveStateKey, GlobalData.IsActive.ToString());
            GlobalData.WriteConfiguration(GlobalData.PopupAfterDemoKey, GlobalData.PopupAfterDemo.ToString());
            GlobalData.WriteConfiguration(GlobalData.RecordPathKey, GlobalData.RecordPath);
        }

        // 鼠标拖动
        private void mainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y <= TitleBar.ActualHeight)
            {
                DragMove();
            }
        }

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
    }
}
