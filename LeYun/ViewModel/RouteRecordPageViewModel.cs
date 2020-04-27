using LeYun.Model;
using LeYun.View.Dlg;
using LeYun.ViewModel.Dlg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace LeYun.ViewModel
{
    class RouteRecordPageViewModel : ViewModelBase
    {
        // 命令
        public DelegateCommand LoadRecordsCommand { get; }
        public DelegateCommand ViewCarDetailCommand { get; }
        public DelegateCommand ViewNodeDetailCommand { get; }
        public DelegateCommand ImportRecordCommand { get; }
        public DelegateCommand RenameRecordCommand { get; }
        public DelegateCommand DeleteRecordCommand { get; }

        // 主窗口视图模型
        private MainWindowViewModel mainWindowViewModel;

        // 线路规划页面视图模型
        private PathProjectPageViewModel pathProjectPageViewModel;

        // 所有历史记录
        public ObservableCollection<ProblemRecord> Records { get; }

        // 当前选中的记录编号
        private int currentRecordIndex;
        public int CurrentRecordIndex
        {
            get { return currentRecordIndex; }
            set 
            { 
                currentRecordIndex = value;
                RaisePropertyChanged("CurrentRecordIndex");
                RaisePropertyChanged("CurrentRecord");
            }
        }

        // 当前选中记录
        private ProblemRecord currentRecord;
        public ProblemRecord CurrentRecord
        {
            get { return currentRecord; }
            set 
            { 
                currentRecord = value;
                RaisePropertyChanged("CurrentRecord");
            }
        }

        // 当前搜索文本
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;

                // 只有正式版软件才能搜索历史记录
                if (GlobalData.IsActive)
                {
                    // 搜索文本改变时更新搜索结果
                    SearchResult.Clear();
                    for (int i = 0; i < Records.Count; ++i)
                    {
                        if (Records[i].Name.Contains(searchText))
                        {
                            SearchResult.Add(Records[i]);
                        }
                    }
                }                

                RaisePropertyChanged("SearchText");
                RaisePropertyChanged("SearchResult");
            }
        }

        // 搜索结果
        public ObservableCollection<ProblemRecord> SearchResult { get; } = new ObservableCollection<ProblemRecord>();

        // 构造函数
        public RouteRecordPageViewModel(MainWindowViewModel mainWindowViewModel, PathProjectPageViewModel pathProjectPageViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            this.pathProjectPageViewModel = pathProjectPageViewModel;

            Records = new ObservableCollection<ProblemRecord>();
            LoadRecordsCommand = new DelegateCommand(LoadRecords);
            ViewCarDetailCommand = new DelegateCommand(ViewCarDetail);
            ImportRecordCommand = new DelegateCommand(ImportRecord);
            ViewNodeDetailCommand = new DelegateCommand(ViewNodeDetail);
            RenameRecordCommand = new DelegateCommand(RenameRecord);
            DeleteRecordCommand = new DelegateCommand(DeleteRecord);
        }

        // 删除记录
        private void DeleteRecord(object obj)
        {            
            FileInfo file = new FileInfo(CurrentRecord.Filename);
            file.Delete();
            Records.Remove(CurrentRecord);
        }

        // 重命名记录
        private void RenameRecord(object obj)
        {
            RenameDlg dlg = new RenameDlg();
            RenameDlgViewModel viewModel = new RenameDlgViewModel();
            viewModel.Title = "重命名";
            viewModel.NewName = CurrentRecord.Name;

            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            if (!viewModel.IsCancel)
            {
                try
                {
                    CurrentRecord.Name = viewModel.NewName;
                    CurrentRecord.SaveToFile(CurrentRecord.Filename);
                }
                catch (Exception e)
                {
                    MsgBox.Show("更新记录失败！\n" + e.Message);
                }
            }
        }

        // 查看节点详情
        private void ViewNodeDetail(object obj)
        {
            int iNode = (int)obj;

            NodeDetailDlg dlg = new NodeDetailDlg();
            NodeDetailDlgViewModel viewModel = new NodeDetailDlgViewModel();
            List<Node> nodes = new List<Node>();
            int oldID = CurrentRecord.Nodes[iNode].ID;
            nodes.Add(CurrentRecord.Nodes[iNode]);
            nodes[0].ID = 1;
            viewModel.Nodes = nodes;

            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            CurrentRecord.Nodes[iNode].ID = oldID;
        }

        // 导入历史记录
        private void ImportRecord(object obj)
        {
            mainWindowViewModel.CurrentPage = mainWindowViewModel.pathProjectPage;
            pathProjectPageViewModel.Record = CurrentRecord;

            ObservableCollection<Segment> Segments = new ObservableCollection<Segment>();
            for (int i = 0; i < CurrentRecord.Paths.Count; ++i)
            {
                if (CurrentRecord.Paths[i].Count > 0)
                {
                    Brush brush = Util.RandomColorBrush();
                    Point last = new Point(CurrentRecord.Nodes[0].X, CurrentRecord.Nodes[0].Y);
                    for (int j = 0; j < CurrentRecord.Paths[i].Count; ++j)
                    {
                        Segments.Add(new Segment { X1 = last.X, Y1 = last.Y, X2 = CurrentRecord.Nodes[CurrentRecord.Paths[i][j]].X, Y2 = CurrentRecord.Nodes[CurrentRecord.Paths[i][j]].Y, Stroke = brush });
                        last = new Point(CurrentRecord.Nodes[CurrentRecord.Paths[i][j]].X, CurrentRecord.Nodes[CurrentRecord.Paths[i][j]].Y);
                    }
                    Segments.Add(new Segment { X1 = last.X, Y1 = last.Y, X2 = CurrentRecord.Nodes[0].X, Y2 = CurrentRecord.Nodes[0].Y, Stroke = brush });
                }
            }

            pathProjectPageViewModel.Segments = Segments;

            mainWindowViewModel.IsPathProjectPageCheck = true;
        }

        // 查看车辆详情
        private void ViewCarDetail(object obj)
        {
            // 只有正式版软件才能查看车辆详情
            if (!GlobalData.IsActive)
            {
                SystemSounds.Beep.Play();
                MsgBox.Show("只有正式版软件才能查看车辆详情\n请前往“设置”页面激活软件");
                return;
            }

            // 获取车辆索引
            int iCar = (int)obj;

            // 车辆未使用时弹出提示
            if (CurrentRecord.Cars[iCar].Path.Count == 0)
            {
                SystemSounds.Beep.Play();
                MsgBox.Show("该车辆未使用！");
                return;
            }

            CarDetailDlg dlg = new CarDetailDlg();
            CarDetailDlgViewModel viewModel = new CarDetailDlgViewModel();

            viewModel.ID = iCar;

            viewModel.WeightLimit = CurrentRecord.Cars[iCar].WeightLimit;
            viewModel.Weight = CurrentRecord.Cars[iCar].Weight;

            viewModel.DisLimit = CurrentRecord.Cars[iCar].DisLimit;
            viewModel.Dis = CurrentRecord.Cars[iCar].Dis;

            viewModel.Time = CurrentRecord.Cars[iCar].Time;
            viewModel.TotalTime = CurrentRecord.TotalTime;

            viewModel.NodeCount = CurrentRecord.Cars[iCar].Path.Count;
            viewModel.TotalNodeCount = CurrentRecord.Nodes.Count - 1;

            viewModel.NodeList = CurrentRecord.Cars[iCar].Path;

            viewModel.Start = CurrentRecord.Nodes[0];

            dlg.DataContext = viewModel;
            dlg.ShowDialog();
        }

        // 加载时读取所有历史记录
        private void LoadRecords(object obj)
        {
            try
            {
                // 读取所有历史记录
                DirectoryInfo dir = new DirectoryInfo(GlobalData.RecordPath);
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

                // 清空搜索词
                SearchText = "";

                // 清空搜索结果
                SearchResult.Clear();

                // 默认选择第一项
                if (Records.Count > 0)
                {
                    CurrentRecordIndex = 0;
                }
                else
                {
                    CurrentRecordIndex = -1;
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
                Records[iRecord].UseCarCount = 0;
                Records[iRecord].TotalTime = -1;
                Records[iRecord].TotalDis = 0;
                Records[iRecord].TotalLoadRate = 0;
                for (int iCar = 0; iCar < Records[iRecord].Paths.Count; ++iCar)
                {
                    if (Records[iRecord].Paths[iCar].Count > 0)
                    {
                        Records[iRecord].UseCarCount++;
                        Records[iRecord].Cars[iCar].Weight = 0;
                        Records[iRecord].Cars[iCar].Dis = 0;
                        Point last = new Point(Records[iRecord].Nodes[0].X, Records[iRecord].Nodes[0].Y);
                        for (int i = 0; i < Records[iRecord].Paths[iCar].Count; ++i)
                        {
                            int iNode = Records[iRecord].Paths[iCar][i];
                            Records[iRecord].Cars[iCar].Weight += Records[iRecord].Nodes[iNode].Demand;
                            Records[iRecord].Cars[iCar].Dis += Math.Sqrt((Records[iRecord].Nodes[iNode].X - last.X) * (Records[iRecord].Nodes[iNode].X - last.X) + (Records[iRecord].Nodes[iNode].Y - last.Y) * (Records[iRecord].Nodes[iNode].Y - last.Y));
                            last = new Point(Records[iRecord].Nodes[iNode].X, Records[iRecord].Nodes[iNode].Y);
                            Records[iRecord].Cars[iCar].Path.Add(Records[iRecord].Nodes[iNode]);
                        }
                        Records[iRecord].TotalLoadRate += Records[iRecord].Cars[iCar].LoadRate;
                        Records[iRecord].Cars[iCar].Dis += Math.Sqrt((Records[iRecord].Nodes[0].X - last.X) * (Records[iRecord].Nodes[0].X - last.X) + (Records[iRecord].Nodes[0].Y - last.Y) * (Records[iRecord].Nodes[0].Y - last.Y));
                        Records[iRecord].TotalDis += Records[iRecord].Cars[iCar].Dis;
                        Records[iRecord].Cars[iCar].Time = Records[iRecord].Cars[iCar].Dis / Records[iRecord].CarSpeed * 60 + Records[iRecord].NodeStayTime * Records[iRecord].Paths[iCar].Count;
                        Records[iRecord].TotalTime = Math.Max(Records[iRecord].TotalTime, Records[iRecord].Cars[iCar].Time);
                    }
                }


                Records[iRecord].TotalLoadRate /= Records[iRecord].UseCarCount;
            }
        }
    }

    class CreateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime time = (DateTime)value;
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class PathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<Node> path = (List<Node>)value;
            string str = "0->";
            for (int i = 0; i < path.Count; ++i)
            {
                str += path[i].ID.ToString();
                str += "->";
            }
            str += "0";
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SearchResultVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value > 0)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class EmptyPageVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int recordListCount = (int)values[0];
            int searchTextLength = (int)values[1];
            int searchResultCount = (int)values[2];
            if (recordListCount == 0 || (searchTextLength > 0 && searchResultCount == 0))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SearchEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GlobalData.IsActive;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
