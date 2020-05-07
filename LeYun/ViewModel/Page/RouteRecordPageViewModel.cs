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
        public DelegateCommand ImportRecordCommand { get; }
        public DelegateCommand RenameRecordCommand { get; }
        public DelegateCommand DeleteRecordCommand { get; }

        // 所有历史记录
        public ObservableCollection<ProblemRecord> Records { get; set; } = GlobalData.Records;

        // 当前选中记录
        private ProblemRecord selectedRecord;
        public ProblemRecord SelectedRecord
        {
            get { return selectedRecord; }
            set
            {
                selectedRecord = value;
                RaisePropertyChanged("SelectedRecord");
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
        public RouteRecordPageViewModel()
        {
            LoadRecordsCommand = new DelegateCommand(LoadRecords);
            ViewCarDetailCommand = new DelegateCommand(ViewCarDetail);
            ImportRecordCommand = new DelegateCommand(ImportRecord);
            RenameRecordCommand = new DelegateCommand(RenameRecord);
            DeleteRecordCommand = new DelegateCommand(DeleteRecord);
        }

        // 删除记录
        private void DeleteRecord(object obj)
        {
            try
            {
                GlobalData.RemoveRecord(SelectedRecord);
            }
            catch (Exception e)
            {
                MsgBox.Show("删除失败！\n" + e.Message);
            }
        }

        // 重命名记录
        private void RenameRecord(object obj)
        {
            RenameDlg dlg = new RenameDlg();
            RenameDlgViewModel viewModel = new RenameDlgViewModel();
            viewModel.Title = "重命名";
            viewModel.NewName = SelectedRecord.Name;

            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            if (!viewModel.IsCancel)
            {
                try
                {
                    GlobalData.RenameRecord(SelectedRecord, viewModel.NewName);
                }
                catch (Exception e)
                {
                    MsgBox.Show("重命名失败！\n" + e.Message);
                }
            }
        }

        // 导入历史记录
        private void ImportRecord(object obj)
        {
            GlobalData.CurrentPage = GlobalData.PathProjectPage;
            GlobalData.IsPathProjectPageChecked = true;
            GlobalData.PathProjectPageViewModel.Record = (ProblemRecord)SelectedRecord.Clone();
            //GlobalData.PathProjectPageViewModel.Segments = SelectedRecord.Segments;
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
            if (SelectedRecord.Cars[iCar].Path.Count == 0)
            {
                SystemSounds.Beep.Play();
                MsgBox.Show("该车辆未使用！");
                return;
            }

            CarDetailDlg dlg = new CarDetailDlg();
            CarDetailDlgViewModel viewModel = new CarDetailDlgViewModel();

            viewModel.ID = iCar;

            viewModel.WeightLimit = SelectedRecord.Cars[iCar].WeightLimit;
            viewModel.Weight = SelectedRecord.Cars[iCar].Weight;

            viewModel.DisLimit = SelectedRecord.Cars[iCar].DisLimit;
            viewModel.Dis = SelectedRecord.Cars[iCar].Dis;

            viewModel.Time = SelectedRecord.Cars[iCar].Time;
            viewModel.TotalTime = SelectedRecord.TotalTime;

            viewModel.NodeCount = SelectedRecord.Cars[iCar].Path.Count;
            viewModel.TotalNodeCount = SelectedRecord.Nodes.Count - 1;

            viewModel.NodeList = SelectedRecord.Cars[iCar].Path;

            viewModel.Start = SelectedRecord.Nodes[0];

            dlg.DataContext = viewModel;
            dlg.ShowDialog();
        }

        // 加载时
        private void LoadRecords(object obj)
        {
            // 清空搜索词
            SearchText = "";

            // 清空搜索结果
            SearchResult.Clear();

            // 默认选择第一项
            if (Records.Count > 0)
            {
                SelectedRecord = Records[0];
            }
            else
            {
                SelectedRecord = null;
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
            try
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
            catch (Exception)
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
