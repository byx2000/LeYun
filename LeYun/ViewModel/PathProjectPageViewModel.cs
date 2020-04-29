using LeYun.Kernel;
using LeYun.Model;
using LeYun.View.Dlg;
using LeYun.ViewModel.Dlg;
using Microsoft.Win32;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFCustomControls;

namespace LeYun.ViewModel
{
    class PathProjectPageViewModel : ViewModelBase
    {
        // 命令
        public DelegateCommand AddCarCommand { get; }
        public DelegateCommand EditCarCommand { get; }
        public DelegateCommand AddNodeCommand { get; }
        public DelegateCommand EditNodeCommand { get; }
        public DelegateCommand ClearCommand { get; }
        public DelegateCommand SolveCommand { get; }
        public DelegateCommand ModeChangeCommand { get; }
        public DelegateCommand SetAlgoParamCommand { get; }
        public DelegateCommand ImportNodesFromFileCommand { get; }
        public DelegateCommand ImportCarsFromFileCommand { get; }
        public DelegateCommand ShowAddCarPopupCommand { get; }
        public DelegateCommand ShowAddNodePopupCommand { get; }
        public DelegateCommand SetRunParamCommand { get; }
        public DelegateCommand MouseAddNodeCommand { get; }
        public DelegateCommand ShowSavePopupCommand { get; }
        public DelegateCommand SaveCarCommand { get; }
        public DelegateCommand SaveNodeCommand { get; }
        public DelegateCommand SaveResultCommand { get; }
        public DelegateCommand RemoveNodeCommand { get; }
        public DelegateCommand RemoveCarCommand { get; }
        public DelegateCommand PlayDemoCommand { get; }

        // 当前问题记录
        private ProblemRecord record = new ProblemRecord();
        public ProblemRecord Record
        {
            get { return record; }
            set
            {
                record = value;
                RaisePropertyChanged("Record");
            }
        }

        // 路径线条
        private ObservableCollection<Segment> segments = new ObservableCollection<Segment>();
        public ObservableCollection<Segment> Segments
        {
            get { return segments; }
            set
            {
                segments = value;
                RaisePropertyChanged("Segments");
            }
        }

        // 算法参数
        private int GenerationCount = 2000;
        private double WTime = 100, WDis = 1, WCar = 1;

        // 车辆添加方式弹窗
        private bool isAddCarPopupVisible = false;
        public bool IsAddCarPopupVisible 
        { 
            get { return isAddCarPopupVisible; }
            private set
            {
                isAddCarPopupVisible = value;
                RaisePropertyChanged("IsAddCarPopupVisible");
            }
        }

        // 节点添加方式弹窗
        private bool isAddNodePopupVisible = false;
        public bool IsAddNodePopupVisible
        {
            get { return isAddNodePopupVisible; }
            private set
            {
                isAddNodePopupVisible = value;
                RaisePropertyChanged("IsAddNodePopupVisible");
            }
        }

        // 保存弹窗
        private bool isSavePopupVisible = false;
        public bool IsSavePopupVisible
        {
            get { return isSavePopupVisible; }
            set
            {
                isSavePopupVisible = value;
                RaisePropertyChanged("IsSavePopupVisible");
            }
        }

        // 是否在演示
        private bool isPlayingDemo = false;
        public bool IsPlayingDemo
        {
            get { return isPlayingDemo; }
            set 
            { 
                isPlayingDemo = value;
                RaisePropertyChanged("IsPlayingDemo");
            }
        }

        // 车辆线条信息（仅用于演示）
        private CarRuntimeInfoCollection carRuntimeInfos = new CarRuntimeInfoCollection();
        public CarRuntimeInfoCollection CarRuntimeInfos 
        { 
            get { return carRuntimeInfos; }
            set
            {
                carRuntimeInfos = value;
                RaisePropertyChanged("CarRuntimeInfos");
            }
        }

        // 当前选中节点下标
        private int currentNodeIndex;
        public int CurrentNodeIndex
        {
            get { return currentNodeIndex; }
            set 
            { 
                currentNodeIndex = value;
                RaisePropertyChanged("CurrentNodeIndex");
            }
        }


        // 构造函数
        public PathProjectPageViewModel()
        {           
            AddCarCommand = new DelegateCommand(AddCar, CantExecuteDuringDemo);
            EditCarCommand = new DelegateCommand(EditCar, CantExecuteDuringDemo);
            AddNodeCommand = new DelegateCommand(AddNode, CantExecuteDuringDemo);
            EditNodeCommand = new DelegateCommand(EditNode, CantExecuteDuringDemo);
            ClearCommand = new DelegateCommand(Clear, CanClear);
            SolveCommand = new DelegateCommand(Solve, CanSolve);
            ModeChangeCommand = new DelegateCommand(ModeChange, CantExecuteDuringDemo);
            SetAlgoParamCommand = new DelegateCommand(SetAlgoParam, CanSetAlgoParam);
            ImportNodesFromFileCommand = new DelegateCommand(ImportNodesFromFile);
            ImportCarsFromFileCommand = new DelegateCommand(ImportCarsFromFile);
            ShowAddCarPopupCommand = new DelegateCommand(ShowAddCarPopup, CantExecuteDuringDemo);
            ShowAddNodePopupCommand = new DelegateCommand(ShowAddNodePopup, CantExecuteDuringDemo);
            SetRunParamCommand = new DelegateCommand(SetRunParam, CantExecuteDuringDemo);
            MouseAddNodeCommand = new DelegateCommand(MouseAddNode, CantExecuteDuringDemo);
            ShowSavePopupCommand = new DelegateCommand(ShowSavePopup, CantExecuteDuringDemo);
            SaveCarCommand = new DelegateCommand(SaveCar, CanSaveCar);
            SaveNodeCommand = new DelegateCommand(SaveNode, CanSaveNode);
            SaveResultCommand = new DelegateCommand(SaveResult, CanSaveResult);
            RemoveNodeCommand = new DelegateCommand(RemoveNode, CantExecuteDuringDemo);
            RemoveCarCommand = new DelegateCommand(RemoveCar, CantExecuteDuringDemo);
            PlayDemoCommand = new DelegateCommand(PlayDemo, CanPlayDemo);
        }

        private bool CantExecuteDuringDemo(object arg)
        {
            return !IsPlayingDemo;
        }

        // 判断是否能播放演示
        private bool CanPlayDemo(object arg)
        {
            return Record.Nodes.Count > 0 && Record.Cars.Count > 0 && Record.Paths.Count > 0 && Segments.Count > 0 && !IsPlayingDemo;
        }

        // 播放演示
        private void PlayDemo(object obj)
        {
            if (!GlobalData.IsActive)
            {
                SystemSounds.Beep.Play();
                MsgBox.Show("只有正式版软件才能使用此功能\n请前往“设置”页面激活软件");
                return;
            }

            IsPlayingDemo = true;
            CurrentNodeIndex = -1;

            // 清空所有线条
            Segments.Clear();

            // 清空车辆运行时信息
            CarRuntimeInfos.Clear();

            // 演示时长
            double duration = GlobalData.DemoDuration;

            // 获取配送总时间
            double totalTime = Record.GetTotalTime();

            // 计算时间缩放比例
            double rate = duration / 60 / totalTime;

            // 获取配送时间最长的车辆编号
            int index = Record.GetSlowestCarIndex();

            // 存储动画开始的线条
            List<int> startIndex = new List<int>();

            // 设置动画
            for (int iCar = 0; iCar < Record.Paths.Count; ++iCar)
            {
                int iCarTemp = iCar;

                // 跳过未参与配送车辆
                if (Record.Paths[iCar].Count == 0)
                {
                    continue;
                }

                // 保存起始线条
                int iStart = Segments.Count;
                startIndex.Add(iStart);

                // 提前添加所有线条
                Brush brush = Util.RandomColorBrush();
                for (int i = 0; i < Record.Paths[iCar].Count + 1; ++i)
                {
                    Segments.Add(new Segment { Stroke = brush });
                }

                // 初始化车辆运行时信息
                CarRuntimeInfos.Add(new CarRuntimeInfo { ID = iCar, LineBrush = brush, IsFinished = false, CompletedPercent = 0 });

                // 遍历当前车辆的所有配送点
                Node last = Record.Nodes[0];
                for (int i = 0; i < Record.Paths[iCar].Count; ++i)
                {
                    int iNode = Record.Paths[iCar][i];

                    // 设置动画
                    Segments[iStart + i].SetAnimation(last.X, last.Y, Record.Nodes[iNode].X, Record.Nodes[iNode].Y, last.Distance(Record.Nodes[iNode]) / Record.CarSpeed * rate * 3600);
                    int t = i;
                    Segments[iStart + i].OnAnimationFinish = delegate
                    {
                        if (Record.NodeStayTime > 0)
                        {
                            Thread.Sleep((int)(Record.NodeStayTime * rate * 60 * 1000));
                        }
                        Segments[iStart + t + 1].BeginAnimation();

                        // 更新车辆完成百分比
                        CarRuntimeInfos.AddCompletedPercent(iCarTemp, 1.0 / Record.Paths[iCarTemp].Count);
                    };

                    last = Record.Nodes[iNode];
                }

                // 设置最后一段线条的动画
                Segments[Segments.Count - 1].SetAnimation(last.X, last.Y, Record.Nodes[0].X, Record.Nodes[0].Y, last.Distance(Record.Nodes[0]) / Record.CarSpeed * rate * 3600);
                
                Segments[Segments.Count - 1].OnAnimationFinish = delegate
                {
                    // 更新车辆运行时信息
                    CarRuntimeInfos.SetFinishedState(iCarTemp, true);

                    // 如果是最后一辆车，则演示结束
                    if (iCarTemp == index)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            MsgBox.Show("演示结束！");
                            IsPlayingDemo = false;
                            CommandManager.InvalidateRequerySuggested();
                        }));
                    }
                };
            }

            for (int i = 0; i < startIndex.Count; ++i)
            {
                Segments[startIndex[i]].BeginAnimation();
            }
        }

        // 删除车辆
        private void RemoveCar(object obj)
        {
            int iCar = (int)obj;
            if (iCar < 0)
            {
                return;
            }

            Record.Cars.Remove(iCar);
            Segments.Clear();
        }

        // 删除节点
        private void RemoveNode(object obj)
        {
            int iNode = (int)obj;
            if (iNode < 0)
            {
                return;
            }

            Record.Nodes.Remove(iNode);
            Segments.Clear();
        }

        // 判断是否能保存节点数据
        private bool CanSaveNode(object arg)
        {
            return Record.Nodes.Count > 0 && !IsPlayingDemo;
        }

        // 保存节点数据
        private void SaveNode(object obj)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "保存节点数据到文件";
            dlg.Filter = "节点数据文件 (.node)|*.node|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    Record.Nodes.SaveToFile(dlg.FileName);
                }
                catch (Exception e)
                {
                    MsgBox.Show("保存节点数据文件失败！\n" + e.Message);
                }
            }
        }

        // 判断是否能保存车辆数据
        private bool CanSaveCar(object arg)
        {
            return Record.Cars.Count > 0 && !IsPlayingDemo;
        }

        // 保存车辆数据
        private void SaveCar(object obj)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "保存车辆数据到文件";
            dlg.Filter = "车辆数据文件 (.car)|*.car|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    Record.Cars.SaveToFile(dlg.FileName);
                }
                catch (Exception e)
                {
                    MsgBox.Show("保存车辆数据文件失败！\n" + e.Message);
                }
            }
        }

        // 显示保存弹窗
        private void ShowSavePopup(object obj)
        {
            IsSavePopupVisible = false;
            IsSavePopupVisible = true;
        }

        // 鼠标添加节点
        private void MouseAddNode(object obj)
        {
            Segments.Clear();

            MouseButtonEventArgs args = (MouseButtonEventArgs)obj;
            Canvas canvas = (Canvas)args.Source;
            double canvasWidth = canvas.ActualWidth;
            double canvasHeight = canvas.ActualHeight;
            double xPos = args.GetPosition(canvas).X;
            double yPos = args.GetPosition(canvas).Y;
            double xNode = xPos / canvasWidth * GlobalData.MaxNodeX;
            double yNode = yPos / canvasHeight * GlobalData.MaxNodeY;

            Record.Nodes.Add(new Node { X = xNode, Y = yNode, Demand = 0 });
        }

        // 设置运行参数
        private void SetRunParam(object obj)
        {
            RunParamSetDlg dlg = new RunParamSetDlg();
            RunParamSetDlgViewModel viewModel = new RunParamSetDlgViewModel();
            viewModel.CarSpeed = Record.CarSpeed;
            viewModel.NodeStayTime = Record.NodeStayTime;
            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            if (!viewModel.IsCancel)
            {
                Segments.Clear();
                Record.CarSpeed = viewModel.CarSpeed;
                Record.NodeStayTime = viewModel.NodeStayTime;
            }
        }

        // 显示节点添加方式弹窗
        private void ShowAddNodePopup(object obj)
        {
            IsAddNodePopupVisible = false;
            IsAddNodePopupVisible = true;
        }

        // 显示车辆添加方式弹窗
        private void ShowAddCarPopup(object obj)
        {
            IsAddCarPopupVisible = false;
            IsAddCarPopupVisible = true;
        }

        // 从文件导入车辆信息
        private void ImportCarsFromFile(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "车辆数据文件 (.car)|*.car|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                Segments.Clear();
                try
                {
                    Record.Cars.ReadFromFile(dlg.FileName);
                }
                catch (Exception e)
                {
                    MsgBox.Show("读取车辆数据文件失败！\n" + e.Message);
                }
            }
        }

        // 从文件导入节点信息
        private void ImportNodesFromFile(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "节点数据文件 (.node)|*.node|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                Segments.Clear();
                try
                {
                    Record.Nodes.ReadFromFile(dlg.FileName);
                }
                catch (Exception e)
                {
                    MsgBox.Show("读取节点数据文件失败！\n" + e.Message);
                }
            }
        }

        // 判断是否能执行求解操作
        private bool CanSolve(object arg)
        {
            return Record.Nodes.Count > 0 && Record.Cars.Count > 0 && !IsPlayingDemo;
        }

        // 判断是否能执行清空操作
        private bool CanClear(object arg)
        {
            return (Record.Nodes.Count > 0 || Record.Cars.Count > 0) && !IsPlayingDemo;
        }

        // 判断是否能保存结果
        private bool CanSaveResult(object arg)
        {
            return Record.Cars.Count > 0 && Record.Nodes.Count > 0 && Segments.Count > 0 && !IsPlayingDemo;
        }

        // 保存结果
        private void SaveResult(object parameter)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "保存求解结果到文件";
            dlg.Filter = "求解记录文件 (.rec)|*.rec|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    Record.CreateTime = DateTime.Now;
                    Record.Name = dlg.SafeFileName;
                    Record.SaveToFile(dlg.FileName);
                }
                catch (Exception e)
                {
                    MsgBox.Show("保存求解结果到文件失败！\n" + e.Message);
                }
            }
        }

        // 添加车辆
        private void AddCar(object parameter)
        {
            CarInfoDlg dlg = new CarInfoDlg();
            CarInfoDlgViewModel viewModel = new CarInfoDlgViewModel() { Title = "添加车辆" + Record.Cars.Count.ToString() };
            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            if (!viewModel.IsCancel)
            {
                Record.Cars.Add(new Car { WeightLimit = viewModel.WeightLimit, DisLimit = viewModel.DisLimit });
            }
        }

        // 编辑车辆信息
        private void EditCar(object obj)
        {
            int index = (int)obj;
            if (index < 0)
            {
                return;
            }

            CarInfoDlg dlg = new CarInfoDlg();
            CarInfoDlgViewModel viewModel = new CarInfoDlgViewModel() { Title = "编辑车辆" + index.ToString(), WeightLimit = Record.Cars[index].WeightLimit, DisLimit = Record.Cars[index].DisLimit };
            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            if (!viewModel.IsCancel)
            {
                Segments.Clear();
                Record.Cars[index].WeightLimit = viewModel.WeightLimit;
                Record.Cars[index].DisLimit = viewModel.DisLimit;
            }
        }

        // 添加节点
        private void AddNode(object parameter)
        {
            NodeInfoDlg dlg = new NodeInfoDlg();
            NodeInfoDlgViewModel viewModel = new NodeInfoDlgViewModel() { Title = "添加节点" + Record.Nodes.Count.ToString() };

            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            if (!viewModel.IsCancel)
            {
                Record.Nodes.Add(new Node { X = viewModel.X, Y = viewModel.Y, Demand = viewModel.Demand });
            }
        }

        // 编辑节点信息
        private void EditNode(object parameter)
        {
            int index = (int)parameter;
            if (index < 0)
            {
                return;
            }

            if (CurrentNodeIndex != index)
            {
                CurrentNodeIndex = index;
            }

            NodeInfoDlg dlg = new NodeInfoDlg();
            NodeInfoDlgViewModel viewModel = new NodeInfoDlgViewModel() { Title = "编辑节点" + index.ToString(), X = Record.Nodes[index].X, Y = Record.Nodes[index].Y, Demand = Record.Nodes[index].Demand };
            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            if (!viewModel.IsCancel)
            {
                Segments.Clear();
                Record.Nodes[index].X = viewModel.X;
                Record.Nodes[index].Y = viewModel.Y;
                Record.Nodes[index].Demand = viewModel.Demand;
            }
        }

        // 清空
        private void Clear(object parameter)
        {
            Record.Nodes.Clear();
            Record.Cars.Clear();
            Segments.Clear();
        }

        // 求解
        private void Solve(object parameter)
        {
            CurrentNodeIndex = -1;

            double[] x = new double[Record.Nodes.Count];
            double[] y = new double[Record.Nodes.Count];
            double[] d = new double[Record.Nodes.Count];
            for (int i = 0; i < Record.Nodes.Count; ++i)
            {
                x[i] = Record.Nodes[i].X;
                y[i] = Record.Nodes[i].Y;
                d[i] = Record.Nodes[i].Demand;
            }

            double[] c = new double[Record.Cars.Count];
            double[] m = new double[Record.Cars.Count];
            for (int i = 0; i < Record.Cars.Count; ++i)
            {
                c[i] = Record.Cars[i].WeightLimit;
                m[i] = Record.Cars[i].DisLimit;
            }

            // 弹出loading消息框
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                LoadingDialog.Begin("正在求解...");
            }));

            // 开启计算线程
            new Thread(delegate ()
            {
                VRPSolver.Solve(x, y, d, c, m, WTime, WDis, WCar, GenerationCount, OnSolveFinish, OnSolveError);
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    LoadingDialog.End();
                }));
            }).Start(); 
        }

        // 求解失败回调函数
        private void OnSolveError(int errCode)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                MsgBox.Show("求解失败！");
            }));
        }

        // 求解成功回调函数
        private void OnSolveFinish(int[][] paths, double[] load, double[] mileage)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                Record.Paths.Clear();

                // 绘制路径
                Segments.Clear();
                for (int i = 0; i < paths.Length; ++i)
                {
                    Record.Paths.Add(new ObservableCollection<int>());
                    if (paths[i].Length > 0)
                    {
                        Brush brush = Util.RandomColorBrush();
                        Point last = new Point(Record.Nodes[0].X, Record.Nodes[0].Y);
                        for (int j = 0; j < paths[i].Length; ++j)
                        {
                            Record.Paths[i].Add(paths[i][j]);
                            Segments.Add(new Segment { X1 = last.X, Y1 = last.Y, X2 = Record.Nodes[paths[i][j]].X, Y2 = Record.Nodes[paths[i][j]].Y, Stroke = brush });
                            last = new Point(Record.Nodes[paths[i][j]].X, Record.Nodes[paths[i][j]].Y);
                        }
                        Segments.Add(new Segment { X1 = last.X, Y1 = last.Y, X2 = Record.Nodes[0].X, Y2 = Record.Nodes[0].Y, Stroke = brush });
                    }
                }

                // 保存结果
                try
                {
                    Record.CreateTime = DateTime.Now;
                    Record.Name = record.CreateTime.ToString("yyyy-MM-dd-HH-mm-ss");
                    Record.SaveToFile(GlobalData.RecordPath + "/" + record.Name + ".rec");
                }
                catch (Exception e)
                {
                    MsgBox.Show("保存求解记录失败！\n" + e.Message);
                }
            }));
        }

        // 计算模式改变
        private void ModeChange(object parameter)
        {
            switch ((int)parameter)
            {
                case 0:
                    GenerationCount = 2000;
                    WTime = 100;
                    WDis = 1;
                    WCar = 1;
                    break;
                case 1:
                    GenerationCount = 2000;
                    WTime = 1;
                    WDis = 1;
                    WCar = 100;
                    break;
                case 2:
                    AlgoParamSetDlg dlg = new AlgoParamSetDlg();
                    AlgoParamSetDlgViewMode viewMode = new AlgoParamSetDlgViewMode() 
                    { 
                        GenerationCount = this.GenerationCount, 
                        WTime = this.WTime, 
                        WDis = this.WDis, 
                        WCar = this.WCar
                    };
                    dlg.DataContext = viewMode;
                    dlg.ShowDialog();

                    if (!viewMode.IsCancel)
                    {
                        Segments.Clear();
                        GenerationCount = viewMode.GenerationCount;
                        WTime = viewMode.WTime;
                        WDis = viewMode.WDis;
                        WCar = viewMode.WCar;
                    }

                    break;
            }
        }

        // 设置算法参数
        private void SetAlgoParam(object obj)
        {
            AlgoParamSetDlg dlg = new AlgoParamSetDlg();
            AlgoParamSetDlgViewMode viewMode = new AlgoParamSetDlgViewMode()
            {
                GenerationCount = this.GenerationCount,
                WTime = this.WTime,
                WDis = this.WDis,
                WCar = this.WCar
            };
            dlg.DataContext = viewMode;
            dlg.ShowDialog();

            if (!viewMode.IsCancel)
            {
                Segments.Clear();
                GenerationCount = viewMode.GenerationCount;
                WTime = viewMode.WTime;
                WDis = viewMode.WDis;
                WCar = viewMode.WCar;
            }
        }

        // 判断是否能设置算法参数
        private bool CanSetAlgoParam(object obj)
        {
            if (obj == null)
            {
                return true;
            }
            return (int)obj == 2 && !IsPlayingDemo;
        }
    }

    class NodeButtonMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)values[0];
            double height = (double)values[1];

            return new Thickness(-width / 2, -height / 2, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class NodeButtonXConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double canvasWidth = (double)values[0];
            double xInput = (double)values[1];
            return xInput / GlobalData.MaxNodeX * canvasWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class NodeButtonYConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double canvasHeight = (double)values[0];
            double yInput = (double)values[1];
            return yInput / GlobalData.MaxNodeY * canvasHeight;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class LineColorConverter : IValueConverter
    {
        private Color RandomColor()
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            int R = ran.Next(255);
            int G = ran.Next(255);
            int B = ran.Next(255);
            B = (R + G > 400) ? R + G - 400 : B;//0 : 380 - R - G;
            B = (B > 255) ? 255 : B;
            return Color.FromRgb((byte)R, (byte)G, (byte)B);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(RandomColor());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //class NodeButtonColorConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        int index = (int)value;
    //        if (index == 0)
    //        {
    //            return Brushes.Red;
    //        }
    //        else
    //        {
    //            return new SolidColorBrush(Color.FromRgb(0x38, 0x7c, 0xdf));
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    class NodeButtonColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int curIndex = (int)values[0];
            int selectedIndex = (int)values[1];
            if (curIndex == 0)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dc4e41"));
            }
            else if (curIndex == selectedIndex)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffbd39"));
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#387cdf"));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class NotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class IsVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class RulerWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double canvasWidth = (double)values[0];
                double actualWidth = (double)values[1];
                return canvasWidth / actualWidth;
            }
            catch (Exception)
            {
                return 50;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}