using LeYun.View;
using LeYun.View.Dlg;
using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public const string ShowProgressDuringDemoKey = "ShowProgressDuringDemo";
        public const string ShowCarRuntimeInfoDuringDemoKey = "ShowCarRuntimeInfoDuringDemo";

        // 各个子页面
        public static PathProjectPage PathProjectPage;
        public static RouteRecordPage RouteRecordPage;
        public static SettingPage SettingPage;
        public static AboutPage AboutPage;

        // 各个子页面的ViewModel
        public static PathProjectPageViewModel PathProjectPageViewModel;
        public static RouteRecordPageViewModel RouteRecordPageViewModel;
        public static SettingPageViewModel SettingPageViewModel;
        public static AboutPageViewModel AboutPageViewModel;

        // 当前页面
        private static Page currentPage;
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

        // 历史记录
        private static ObservableCollection<ProblemRecord> records = new ObservableCollection<ProblemRecord>();
        public static ObservableCollection<ProblemRecord> Records
        {
            get { return records; }
            set 
            { 
                records = value;
                RaisePropertyChanged("Records");
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

        // 演示时是否显示进度
        private static bool showProgressDuringDemo = true;
        public static bool ShowProgressDuringDemo
        {
            get { return showProgressDuringDemo; }
            set 
            { 
                showProgressDuringDemo = value;
                RaisePropertyChanged("ShowProgressDuringDemo");
            }
        }

        // 演示时是否显示车辆实时信息
        private static bool showCarRuntimeInfoDuringDemo = true;
        public static bool ShowCarRuntimeInfoDuringDemo
        {
            get { return showCarRuntimeInfoDuringDemo; }
            set 
            { 
                showCarRuntimeInfoDuringDemo = value;
                RaisePropertyChanged("ShowCarRuntimeInfoDuringDemo");
            }
        }

        // 从配置文件读取所有设置
        public static void ReadSettings()
        {
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
                ShowProgressDuringDemo = bool.Parse(ReadConfiguration(ShowProgressDuringDemoKey));
                ShowCarRuntimeInfoDuringDemo = bool.Parse(ReadConfiguration(ShowCarRuntimeInfoDuringDemoKey));
            }
            catch (Exception e)
            {
                MsgBox.Show("配置文件读取失败！\n" + e.Message);
                MaxNodeX = 30;
                MaxNodeY = 20;
                RecordPath = "./record";
                IsActive = false;
                LineThickness = 2.5;
                NodeButtonWidth = 15;
                DemoDuration = 20;
                PopupAfterDemo = true;
                ShowProgressDuringDemo = true;
                ShowCarRuntimeInfoDuringDemo = true;
            }
        }

        // 保存所有设置到配置文件
        public static void SaveSettings()
        {
            try
            {
                WriteConfiguration(MaxNodeXKey, MaxNodeX.ToString());
                WriteConfiguration(MaxNodeYKey, MaxNodeY.ToString());
                WriteConfiguration(RecordPathKey, RecordPath);
                WriteConfiguration(ActiveStateKey, IsActive.ToString());
                WriteConfiguration(LineThicknessKey, LineThickness.ToString());
                WriteConfiguration(NodeButtonWidthKey, NodeButtonWidth.ToString());
                WriteConfiguration(DemoDurationKey, DemoDuration.ToString());
                WriteConfiguration(PopupAfterDemoKey, PopupAfterDemo.ToString());
                WriteConfiguration(ShowProgressDuringDemoKey, ShowProgressDuringDemo.ToString());
                WriteConfiguration(ShowCarRuntimeInfoDuringDemoKey, ShowCarRuntimeInfoDuringDemo.ToString());
            }
            catch (Exception)
            {
                
            }
        }

        // 恢复默认设置
        public static void RestoreSettings()
        {
            MaxNodeX = 30;
            MaxNodeY = 20;
            LineThickness = 2.5;
            NodeButtonWidth = 15;
            DemoDuration = 20;
            PopupAfterDemo = true;
            ShowProgressDuringDemo = true;
            ShowCarRuntimeInfoDuringDemo = true;
        }

        // 初始化所有页面
        public static void InitPages()
        {
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

            // 设置各个页面的视图模型
            PathProjectPage.DataContext = PathProjectPageViewModel;
            RouteRecordPage.DataContext = RouteRecordPageViewModel;
            SettingPage.DataContext = SettingPageViewModel;
            AboutPage.DataContext = AboutPageViewModel;

            // 默认显示线路规划页面
            CurrentPage = PathProjectPage;
            IsPathProjectPageChecked = true;
        }

        // 读取所有历史记录
        public static void ReadRecords()
        {
            new Thread(delegate ()
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    // 读取文件
                    try
                    {
                        // 读取所有历史记录
                        DirectoryInfo dir = new DirectoryInfo(RecordPath);
                        FileInfo[] files = dir.GetFiles();

                        List<ProblemRecord> records = new List<ProblemRecord>();
                        for (int i = 0; i < files.Length; ++i)
                        {
                            if (files[i].Extension == ".rec")
                            {
                                records.Add(new ProblemRecord());
                                records[i].ReadFromFile(files[i].FullName);
                                records[i].Filename = files[i].FullName;
                            }
                        }

                        // 按照时间先后排序
                        records.Sort(delegate (ProblemRecord r1, ProblemRecord r2)
                        {
                            return r2.CreateTime.CompareTo(r1.CreateTime);
                        });

                        Records.Clear();
                        for (int i = 0; i < records.Count; ++i)
                        {
                            Records.Add(records[i]);
                        }
                    }
                    catch (Exception e)
                    {
                        Records.Clear();
                        MsgBox.Show("读取记录出错！\n" + e.Message);
                    }

                    // 计算相关数据
                    for (int iRecord = 0; iRecord < Records.Count; ++iRecord)
                    {
                        Records[iRecord].TotalTime = Records[iRecord].GetTotalTime();
                        Records[iRecord].TotalDis = Records[iRecord].GetTotalDistance();
                        Records[iRecord].UseCarCount = Records[iRecord].GetUseCarCount();
                        Records[iRecord].TotalLoadRate = Records[iRecord].GetTotalLoadRate();

                        for (int iCar = 0; iCar < Records[iRecord].Paths.Count; ++iCar)
                        {
                            Records[iRecord].Cars[iCar].Dis = Records[iRecord].GetCarDistance(iCar);
                            Records[iRecord].Cars[iCar].Weight = Records[iRecord].GetCarWeight(iCar);
                            Records[iRecord].Cars[iCar].Path = Records[iRecord].GetCarPath(iCar);
                            Records[iRecord].Cars[iCar].Time = Records[iRecord].GetCarTime(iCar);
                        }

                        for (int iNode = 1; iNode < Records[iRecord].Nodes.Count; ++iNode)
                        {
                            Records[iRecord].Nodes[iNode].ServedTime = Records[iRecord].GetNodeServedTime(iNode);
                        }
                    }
                }));
            }).Start();
        }

        // 保存所有历史记录
        public static void SaveRecords()
        {
            try
            {
                for (int i = 0; i < Records.Count; ++i)
                {
                    Records[i].SaveToFile(Records[i].Filename);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("保存历史记录失败！\n" + e.Message);
            }
        }

        // 添加历史记录
        public static void AddRecord(ProblemRecord rec)
        {
            ProblemRecord record = (ProblemRecord)rec.Clone();

            record.TotalTime = record.GetTotalTime();
            record.TotalDis = record.GetTotalDistance();
            record.TotalLoadRate = record.GetTotalLoadRate();
            record.UseCarCount = record.GetUseCarCount();
            for (int iCar = 0; iCar < record.Paths.Count; ++iCar)
            {
                record.Cars[iCar].Dis = record.GetCarDistance(iCar);
                record.Cars[iCar].Weight = record.GetCarWeight(iCar);
                record.Cars[iCar].Path = record.GetCarPath(iCar);
                record.Cars[iCar].Time = record.GetCarTime(iCar);
            }
            for (int iNode = 1; iNode < record.Nodes.Count; ++iNode)
            {
                record.Nodes[iNode].ServedTime = record.GetNodeServedTime(iNode);
            }

            record.Filename = RecordPath + "/" + record.CreateTime.ToString("yyyy-MM-dd-HH-mm-ss") + ".rec";
            Records.Insert(0, record);
        }

        // 删除历史记录
        public static void RemoveRecord(ProblemRecord record)
        {
            FileInfo file = new FileInfo(record.Filename);
            file.Delete();
            Records.Remove(record);
        }

        // 重命名历史记录
        public static void RenameRecord(ProblemRecord record, string newName)
        {
            record.Name = newName;
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
