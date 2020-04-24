using LeYun.View.Dlg;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeYun.Model
{
    static class GlobalData
    {
        public const string MaxNodeXKey = "MaxNodeX";
        public const string MaxNodeYKey = "MaxNodeY";
        public const string RecordPathKey = "RecordPath";
        public const string ActiveStateKey = "IsActive";
        public const string ActivateKeyKey = "ActivateKey";

        public static double MaxNodeX = 30;
        public static double MaxNodeY = 20;
        public static string RecordPath = "./record/";
        public static bool IsActive = false;
        public static string ActivateKey = "XXX-XXX-XXX";

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

        public static string ReadConfiguration(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void WriteConfiguration(string key, string value)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings[key].Value = value;
            cfa.Save();
        }
    }
}
