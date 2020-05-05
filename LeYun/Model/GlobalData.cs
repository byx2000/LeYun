using LeYun.View;
using LeYun.View.Dlg;
using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LeYun.Model
{
    static class GlobalData
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        // 配置文件键值
        public const string MaxNodeXKey = "MaxNodeX";
        public const string MaxNodeYKey = "MaxNodeY";
        public const string RecordPathKey = "RecordPath";
        public const string ActiveStateKey = "IsActive";
        public const string LineThicknessKey = "LineThickness";
        public const string NodeButtonWidthKey = "NodeButtonWidth";
        public const string DemoDurationKey = "DemoDuration";
        public const string PopupAfterDemoKey = "PopupAfterDemo";

        // 各个子页面
        public static PathProjectPage PathProjectPage = null;
        public static RouteRecordPage RouteRecordPage = null;
        public static SettingPage SettingPage = null;
        public static AboutPage AboutPage = null;

        // 各个子页面的ViewModel
        public static PathProjectPageViewModel PathProjectPageViewModel = null;
        public static RouteRecordPageViewModel RouteRecordPageViewModel = null;
        public static SettingPageViewModel SettingPageViewModel = null;
        public static AboutPageViewModel AboutPageViewModel = null;

        // 当前页面
        private static Page currentPage = null;
        public static Page CurrentPage
        {
            get { return currentPage; }
            set 
            { 
                currentPage = value;
                RaisePropertyChanged("CurrentPage");
            }
        }

        // 页面标签选中情况
        private static bool isPathProjectPageChecked = false;
        public static bool IsPathProjectPageChecked
        {
            get { return isPathProjectPageChecked; }
            set 
            { 
                isPathProjectPageChecked = value;
                RaisePropertyChanged("IsPathProjectPageChecked");
            }
        }

        // 节点X坐标最大值
        private static double maxNodeX = 30;
        public static double MaxNodeX 
        { 
            get { return maxNodeX; }
            set
            {
                maxNodeX = value;
                RaisePropertyChanged("MaxNodeX");
            }
        }

        // 节点Y坐标最大值
        private static double maxNodeY = 20;
        public static double MaxNodeY
        {
            get { return maxNodeY; }
            set 
            { 
                maxNodeY = value;
                RaisePropertyChanged("MaxNodeY");
            }
        }

        // 记录存储路径
        private static string recordPath = "./record/";
        public static string RecordPath
        {
            get { return recordPath; }
            set 
            { 
                recordPath = value;
                RaisePropertyChanged("RecordPath");
            }
        }


        // 激活状态
        private static bool isActive = false;
        public static bool IsActive 
        { 
            get { return isActive; }
            set
            {
                isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        // 路径线条粗细
        private static double lineThickness = 2.5;
        public static double LineThickness
        {
            get { return lineThickness; }
            set 
            {
                lineThickness = value;
                RaisePropertyChanged("LineThickness");
            }
        }

        // 节点按钮直径
        private static double nodeButtonWidth = 15;
        public static double NodeButtonWidth
        {
            get { return nodeButtonWidth; }
            set 
            { 
                nodeButtonWidth = value;
                RaisePropertyChanged("NodeButtonWidth");
            }
        }

        // 演示时长
        private static double demoDuration = 20;
        public static double DemoDuration
        {
            get { return demoDuration; }
            set
            {
                demoDuration = value;
                RaisePropertyChanged("DemoDuration");
            }
        }

        // 演示后是否弹出提示
        private static bool popupAfterDemo = true;
        public static bool PopupAfterDemo
        {
            get { return popupAfterDemo; }
            set 
            { 
                popupAfterDemo = value;
                RaisePropertyChanged("PopupAfterDemo");
            }
        }

        // 静态构造函数
        static GlobalData()
        {
            // 读取配置文件
            try
            {
                MaxNodeX = int.Parse(ReadConfiguration(MaxNodeXKey));
                MaxNodeY = int.Parse(ReadConfiguration(MaxNodeYKey));
                RecordPath = ReadConfiguration(RecordPathKey);
                IsActive = bool.Parse(ReadConfiguration(ActiveStateKey));
                LineThickness = double.Parse(ReadConfiguration(LineThicknessKey));
                NodeButtonWidth = double.Parse(ReadConfiguration(NodeButtonWidthKey));
                DemoDuration = double.Parse(ReadConfiguration(DemoDurationKey));
                PopupAfterDemo = bool.Parse(ReadConfiguration(PopupAfterDemoKey));
            }
            catch (Exception)
            {
                MsgBox.Show("配置文件读取失败！");
                MaxNodeX = 30;
                MaxNodeY = 20;
                RecordPath = "./record";
                IsActive = false;
                LineThickness = 2.5;
                NodeButtonWidth = 15;
                DemoDuration = 20;
                PopupAfterDemo = true;
            }

            // 创建各个子页面
            PathProjectPage = new PathProjectPage();
            RouteRecordPage = new RouteRecordPage();
            SettingPage = new SettingPage();
            AboutPage = new AboutPage();

            // 创建各个子页面的ViewModel
            PathProjectPageViewModel = new PathProjectPageViewModel();
            RouteRecordPageViewModel = new RouteRecordPageViewModel();
            SettingPageViewModel = new SettingPageViewModel();
            AboutPageViewModel = new AboutPageViewModel();

            currentPage = PathProjectPage;
            IsPathProjectPageChecked = true;
        }

        // 通知属性改变
        private static void RaisePropertyChanged(string propertyName)
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(propertyName));
            }
        }

        // 读取配置文件
        public static string ReadConfiguration(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        // 写入配置文件
        public static void WriteConfiguration(string key, string value)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings[key].Value = value;
            cfa.Save();
        }
    }
}
