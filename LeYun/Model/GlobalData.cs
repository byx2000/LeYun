using LeYun.View.Dlg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeYun.Model
{
    static class GlobalData
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public const string MaxNodeXKey = "MaxNodeX";
        public const string MaxNodeYKey = "MaxNodeY";
        public const string RecordPathKey = "RecordPath";
        public const string ActiveStateKey = "IsActive";
        public const string ActivateKeyKey = "ActivateKey";

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
        private static double maxNodeY;
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
        public static string RecordPath = "./record/";

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

        // 激活密钥
        public static string ActivateKey = "XXX-XXX-XXX";

        // 静态构造函数
        static GlobalData()
        {
            try
            {
                MaxNodeX = int.Parse(ReadConfiguration(MaxNodeXKey));
                MaxNodeY = int.Parse(ReadConfiguration(MaxNodeYKey));
                RecordPath = ReadConfiguration(RecordPathKey);
                IsActive = bool.Parse(ReadConfiguration(ActiveStateKey));
                ActivateKey = ReadConfiguration(ActivateKeyKey);
            }
            catch (Exception)
            {
                MaxNodeX = 30;
                MaxNodeY = 20;
                RecordPath = "./record";
                IsActive = false;
                ActivateKey = "XXX-XXX-XXX";
            }
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
