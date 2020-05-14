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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public DelegateCommand SetRunParamCommand { get; }
        public DelegateCommand MouseAddNodeCommand { get; }
        public DelegateCommand SaveCarCommand { get; }
        public DelegateCommand SaveNodeCommand { get; }
        public DelegateCommand SaveResultCommand { get; }
        public DelegateCommand RemoveNodeCommand { get; }
        public DelegateCommand RemoveCarCommand { get; }
        public DelegateCommand PlayDemoCommand { get; }
        public DelegateCommand NodeDragCommand { get; }

        // 画布宽度
        public double CanvasWidth { get; set; }

        // 画布高度
        public double CanvasHeight { get; set; }

        // 鼠标X坐标
        private double mouseX;
        public double MouseX
        {
            get { return mouseX; }
            set 
            { 
                mouseX = value;
                RaisePropertyChanged("MouseX");
            }
        }

        // 鼠标Y坐标
        private double mouseY;
        public double MouseY
        {
            get { return mouseY; }
            set 
            { 
                mouseY = value;
                RaisePropertyChanged("MouseY");
            }
        }


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

        // 算法参数
        private int GenerationCount = 2000;
        private double WTime = 1, WDis = 1, WCar = 100;

        // 拥堵系数
        private double CongestionFactor = 0;

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

        // 当前时间（仅用于演示）
        private AnimatableValue currentTime = new AnimatableValue();
        public AnimatableValue CurrentTime
        {
            get { return currentTime; }
            set 
            { 
                currentTime = value;
                RaisePropertyChanged("CurrentTime");
            }
        }

        // 当前演示进度（仅用于演示）
        private AnimatableValue currentDemoPeogress = new AnimatableValue();
        public AnimatableValue CurrentDemoProgress
        {
            get { return currentDemoPeogress; }
            set 
            { 
                currentDemoPeogress = value;
                RaisePropertyChanged("CurrentDemoProgress");
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
            SetRunParamCommand = new DelegateCommand(SetRunParam, CantExecuteDuringDemo);
            MouseAddNodeCommand = new DelegateCommand(MouseAddNode, CantExecuteDuringDemo);
            SaveCarCommand = new DelegateCommand(SaveCar, CanSaveCar);
            SaveNodeCommand = new DelegateCommand(SaveNode, CanSaveNode);
            SaveResultCommand = new DelegateCommand(SaveResult, CanSaveResult);
            RemoveNodeCommand = new DelegateCommand(RemoveNode, CantExecuteDuringDemo);
            RemoveCarCommand = new DelegateCommand(RemoveCar, CantExecuteDuringDemo);
            PlayDemoCommand = new DelegateCommand(PlayDemo, CanPlayDemo);
            NodeDragCommand = new DelegateCommand(NodeDrag);
        }

        // 节点拖动
        private void NodeDrag(object obj)
        {
            Record.Segments.Clear();
            DragDeltaEventArgs args = (DragDeltaEventArgs)obj;
            Node node = (Node)((Thumb)(args.Source)).DataContext;
            if (CurrentNodeIndex != node.ID) // 优化拖动性能
            {
                CurrentNodeIndex = node.ID;
            }
            node.X += args.HorizontalChange / CanvasWidth * GlobalData.MaxNodeX;
            node.Y += args.VerticalChange / CanvasHeight * GlobalData.MaxNodeY;
        }

        private bool CantExecuteDuringDemo(object arg)
        {
            return !IsPlayingDemo;
        }

        // 判断是否能播放演示
        private bool CanPlayDemo(object arg)
        {
            return Record.Nodes.Count > 0 && Record.Cars.Count > 0 && Record.Paths.Count > 0 && Record.Segments.Count > 0 && !IsPlayingDemo;
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
            Record.Segments.Clear();

            // 清空车辆运行时信息
            CarRuntimeInfos.Clear();

            // 演示时长 s
            double totalDemoTime = GlobalData.DemoDuration;

            // 获取配送总时间 s
            double totalTime = Record.GetTotalTime() * 60;

            // 计算时间缩放比例
            double rate = totalDemoTime / totalTime;

            // 计算节点停留时间 s
            double nodeStayTime = Record.NodeStayTime * rate * 60;

            // 获取配送时间最长的车辆编号
            int index = Record.GetSlowestCarIndex();

            //存储动画开始的线条
            List<int> startIndex = new List<int>();

            //设置动画
            for (int iCar = 0; iCar < Record.Paths.Count; ++iCar)
            {
                int iCarTemp = iCar;

                // 跳过未参与配送车辆
                if (Record.Paths[iCar].Count == 0)
                {
                    continue;
                }

                // 保存起始线条
                int iStart = Record.Segments.Count;
                startIndex.Add(iStart);

                // 提前添加所有线条
                Brush brush = Util.RandomColorBrush();
                for (int i = 0; i < Record.Paths[iCar].Count + 1; ++i)
                {
                    Record.Segments.Add(new Segment { Stroke = brush });
                }

                // 初始化车辆运行时信息
                CarRuntimeInfos.Add(new CarRuntimeInfo { ID = iCar, LineBrush = brush, IsFinished = false, CompletedPercent = 0 });

                // 遍历当前车辆的所有配送点
                Node last = Record.Nodes[0];
                for (int i = 0; i < Record.Paths[iCar].Count; ++i)
                {
                    int iNode = Record.Paths[iCar][i];

                    // 设置动画
                    Record.Segments[iStart + i].XAnimationFrom = last.X;
                    Record.Segments[iStart + i].YAnimationFrom = last.Y;
                    Record.Segments[iStart + i].XAnimationTo = Record.Nodes[iNode].X;
                    Record.Segments[iStart + i].YAnimationTo = Record.Nodes[iNode].Y;
                    Record.Segments[iStart + i].Duration = last.Distance(Record.Nodes[iNode]) / Record.CarSpeed * rate * 3600;
                    if (i != 0)
                    {
                        Record.Segments[iStart + i].Delay = nodeStayTime;
                    }

                    int t = i;
                    Record.Segments[iStart + i].AnimationCompleted = delegate
                    {
                        // 启动下一线条动画
                        Record.Segments[iStart + t + 1].BeginAnimation();

                        if (GlobalData.ShowCarRuntimeInfoDuringDemo)
                        {
                            // 更新车辆完成百分比
                            new Thread(delegate ()
                            {
                                CarRuntimeInfos.AddCompletedPercent(iCarTemp, 1.0 / Record.Paths[iCarTemp].Count);
                            }).Start();
                        }
                    };

                    last = Record.Nodes[iNode];
                }

                // 设置最后一段线条的动画
                Record.Segments[Record.Segments.Count - 1].XAnimationFrom = last.X;
                Record.Segments[Record.Segments.Count - 1].YAnimationFrom = last.Y;
                Record.Segments[Record.Segments.Count - 1].XAnimationTo = Record.Nodes[0].X;
                Record.Segments[Record.Segments.Count - 1].YAnimationTo = Record.Nodes[0].Y;
                Record.Segments[Record.Segments.Count - 1].Duration = last.Distance(Record.Nodes[0]) / Record.CarSpeed * rate * 3600;
                Record.Segments[Record.Segments.Count - 1].Delay = nodeStayTime;
                Record.Segments[Record.Segments.Count - 1].AnimationCompleted = delegate
                {       
                    if (GlobalData.ShowCarRuntimeInfoDuringDemo)
                    {
                        // 更新车辆运行时信息
                        new Thread(delegate ()
                        {
                            CarRuntimeInfos.SetFinishedState(iCarTemp, true);
                        }).Start();
                    }

                    // 如果是最后一辆车，则演示结束
                    if (iCarTemp == index)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            if (GlobalData.PopupAfterDemo)
                            {
                                MsgBox.Show("演示结束！");
                            }
                            IsPlayingDemo = false;
                            CommandManager.InvalidateRequerySuggested();
                        }));
                    }
                };
            }

            if (GlobalData.ShowProgressDuringDemo)
            {
                //启动时间动画
                DoubleAnimation timeAnim = new DoubleAnimation();
                timeAnim.From = 0;
                timeAnim.To = totalTime / 60;
                timeAnim.Duration = new Duration(TimeSpan.FromSeconds(totalDemoTime));
                CurrentTime.BeginAnimation(AnimatableValue.ValueProperty, timeAnim);

                // 启动演示进度动画
                DoubleAnimation progressAnim = new DoubleAnimation();
                progressAnim.From = 0;
                progressAnim.To = 1;
                progressAnim.Duration = new Duration(TimeSpan.FromSeconds(totalDemoTime));
                CurrentDemoProgress.BeginAnimation(AnimatableValue.ValueProperty, progressAnim);
            }

            // 启动线条动画
            for (int i = 0; i < startIndex.Count; ++i)
            {
                Record.Segments[startIndex[i]].BeginAnimation();
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
            Record.Segments.Clear();
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
            Record.Segments.Clear();
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

        // 鼠标添加节点
        private void MouseAddNode(object obj)
        {
            Record.Segments.Clear();
            double x = MouseX / CanvasWidth * GlobalData.MaxNodeX;
            double y = MouseY / CanvasHeight * GlobalData.MaxNodeY;
            Record.Nodes.Add(new Node { X = x, Y = y, Demand = 0 });
            CurrentNodeIndex = Record.Nodes.Count - 1;
        }

        // 设置运行参数
        private void SetRunParam(object obj)
        {
            RunParamSetDlg dlg = new RunParamSetDlg();
            RunParamSetDlgViewModel viewModel = new RunParamSetDlgViewModel();
            viewModel.CarSpeed = Record.CarSpeed;
            viewModel.NodeStayTime = Record.NodeStayTime;
            viewModel.CongestionFactor = CongestionFactor;
            dlg.DataContext = viewModel;
            dlg.ShowDialog();

            if (!viewModel.IsCancel)
            {
                Record.Segments.Clear();
                Record.CarSpeed = viewModel.CarSpeed;
                Record.NodeStayTime = viewModel.NodeStayTime;
                Record.CarSpeed *= (1 - viewModel.CongestionFactor);
                CongestionFactor = 0;
            }
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
                Record.Segments.Clear();
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
                Record.Segments.Clear();
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
            return Record.Nodes.Count > 1 && Record.Cars.Count > 0 && !IsPlayingDemo;
        }

        // 判断是否能执行清空操作
        private bool CanClear(object arg)
        {
            return (Record.Nodes.Count > 0 || Record.Cars.Count > 0) && !IsPlayingDemo;
        }

        // 判断是否能保存结果
        private bool CanSaveResult(object arg)
        {
            return Record.Cars.Count > 0 && Record.Nodes.Count > 0 && Record.Segments.Count > 0 && !IsPlayingDemo;
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
                Record.Segments.Clear();
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
                Record.Segments.Clear();
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
                CurrentNodeIndex = Record.Nodes.Count - 1;
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
                Record.Segments.Clear();
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
            Record.Segments.Clear();
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
                LoadingDialog.Begin("正在求解");
            }));

            // 开启计算线程
            new Thread(delegate ()
            {
                //double actualCarSpeed = Record.CarSpeed * (1 - CongestionFactor);
                VRPSolver.Solve(x, y, d, c, m, WTime, WDis, WCar, GenerationCount, Record.CarSpeed, Record.NodeStayTime, OnSolveFinish, OnSolveError);
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

                // 保存路径
                for (int i = 0; i < paths.Length; ++i)
                {
                    Record.Paths.Add(new ObservableCollection<int>());
                    for (int j = 0; j < paths[i].Length; ++j)
                    {
                        Record.Paths[i].Add(paths[i][j]);
                    }
                }

                // 生成路径
                Record.GenerateSegments();

                // 绘制路径
                //Segments.Clear();
                //for (int i = 0; i < paths.Length; ++i)
                //{
                //    Record.Paths.Add(new ObservableCollection<int>());
                //    if (paths[i].Length > 0)
                //    {
                //        Brush brush = Util.RandomColorBrush();
                //        Point last = new Point(Record.Nodes[0].X, Record.Nodes[0].Y);
                //        for (int j = 0; j < paths[i].Length; ++j)
                //        {
                //            Record.Paths[i].Add(paths[i][j]);
                //            Segments.Add(new Segment { X1 = last.X, Y1 = last.Y, X2 = Record.Nodes[paths[i][j]].X, Y2 = Record.Nodes[paths[i][j]].Y, Stroke = brush });
                //            last = new Point(Record.Nodes[paths[i][j]].X, Record.Nodes[paths[i][j]].Y);
                //        }
                //        Segments.Add(new Segment { X1 = last.X, Y1 = last.Y, X2 = Record.Nodes[0].X, Y2 = Record.Nodes[0].Y, Stroke = brush });
                //    }
                //}

                // 保存结果
                try
                {
                    Record.CreateTime = DateTime.Now;
                    Record.Name = Record.CreateTime.ToString("yyyy-MM-dd-HH-mm-ss");
                    GlobalData.AddRecord(Record);
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
            SelectionChangedEventArgs args = (SelectionChangedEventArgs)parameter;
            ComboBox comboBox = (ComboBox)args.Source;
            switch (comboBox.SelectedIndex)
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
                        Record.Segments.Clear();
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
                Record.Segments.Clear();
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
            if (!(values[0] is double))
            {
                return 0;
            }

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
            if (!(values[0] is double))
            {
                return 0;
            }

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
            //int selectedIndex = (int)values[1];
            int selectedIndex = -1;
            if (values[1] is int)
            {
                selectedIndex = (int)values[1];
            }
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