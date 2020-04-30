﻿using LeYun.Model;
using LeYun.View;
using LeYun.View.Dlg;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeYun.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        // 命令
        public DelegateCommand CloseCommand { get; }
        public DelegateCommand MinimizeCommand { get; }
        public DelegateCommand FullScreenCommand { get; }
        public DelegateCommand SwitchPageCommand { get; }
        public DelegateCommand SaveConfigurationCommand { get; }

        // 各个子页面
        public PathProjectPage pathProjectPage = new PathProjectPage();
        public RouteRecordPage routeRecordPage = new RouteRecordPage();
        public SettingPage configurePage = new SettingPage();
        public AboutPage otherInfoPage = new AboutPage();

        // 子页面的ViewModel
        private PathProjectPageViewModel pathProjectPageViewModel;
        private RouteRecordPageViewModel routeRecordPageViewModel;

        // 特殊处理
        private bool isPathProjectPageCheck;
        public bool IsPathProjectPageCheck 
        { 
            get { return isPathProjectPageCheck; }
            set
            {
                isPathProjectPageCheck = value;
                RaisePropertyChanged("IsPathProjectPageCheck");
            }
        }

        // 当前页面
        private Page currentPage;
        public Page CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;
                RaisePropertyChanged("CurrentPage");
            }
        }


        public MainWindowViewModel()
        {
            pathProjectPageViewModel = new PathProjectPageViewModel();
            routeRecordPageViewModel = new RouteRecordPageViewModel(this, pathProjectPageViewModel);

            pathProjectPage.DataContext = pathProjectPageViewModel;
            routeRecordPage.DataContext = routeRecordPageViewModel;

            CloseCommand = new DelegateCommand(Close);
            MinimizeCommand = new DelegateCommand(Minimize);
            FullScreenCommand = new DelegateCommand(FullScreen);
            SwitchPageCommand = new DelegateCommand(SwitchPage);
            SaveConfigurationCommand = new DelegateCommand(SaveConfiguration);

            IsPathProjectPageCheck = true;
            CurrentPage = pathProjectPage;
        }

        // 保存设置
        private void SaveConfiguration(object obj)
        {           
            GlobalData.WriteConfiguration(GlobalData.LineThicknessKey, GlobalData.LineThickness.ToString());
            GlobalData.WriteConfiguration(GlobalData.NodeButtonWidthKey, GlobalData.NodeButtonWidth.ToString());
            GlobalData.WriteConfiguration(GlobalData.MaxNodeXKey, GlobalData.MaxNodeX.ToString());
            GlobalData.WriteConfiguration(GlobalData.MaxNodeYKey, GlobalData.MaxNodeY.ToString());
            GlobalData.WriteConfiguration(GlobalData.DemoDurationKey, GlobalData.DemoDuration.ToString());
            GlobalData.WriteConfiguration(GlobalData.ActiveStateKey, GlobalData.IsActive.ToString());
            GlobalData.WriteConfiguration(GlobalData.PopupAfterDemoKey, GlobalData.PopupAfterDemo.ToString());
        }

        private void Close(object parameter)
        {
            ((Window)parameter).Close();
        }

        private void Minimize(object parameter)
        {
            ((Window)parameter).WindowState = WindowState.Minimized;
        }

        private void FullScreen(object parameter)
        {
            Window window = (Window)(((RoutedEventArgs)parameter).Source);
            window.Left = 0.0;
            window.Top = 0.0;
            window.Width = SystemParameters.PrimaryScreenWidth;
            window.Height = SystemParameters.PrimaryScreenHeight;
        }

        private void SwitchPage(object parameter)
        {
            switch ((string)parameter)
            {
                case "0":
                    CurrentPage = pathProjectPage;
                    break;
                case "1":
                    CurrentPage = routeRecordPage;
                    break;
                case "2":
                    CurrentPage = configurePage;
                    break;
                case "3":
                    CurrentPage = otherInfoPage;
                    break;
            }
        }
    }
}
