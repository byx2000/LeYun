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
        public static double MaxNodeX = 30;
        public static double MaxNodeY = 20;
        public static string RecordPath = "./record/";

        static GlobalData()
        {
            try
            {
                // 读取节点坐标最大值
                MaxNodeX = int.Parse(ConfigurationManager.AppSettings["MaxNodeX"]);
                MaxNodeY = int.Parse(ConfigurationManager.AppSettings["MaxNodeY"]);

                // 读取记录文件保存路径
                RecordPath = ConfigurationManager.AppSettings["RecordPath"];
            }
            catch (Exception)
            {
                MaxNodeX = 30;
                MaxNodeY = 20;
                RecordPath = "./record";
            }
        }
    }
}
