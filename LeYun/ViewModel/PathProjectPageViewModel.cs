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
        public DelegateCommand SaveResultCommand { get; }
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
        
        // 构造函数
        public PathProjectPageViewModel()
        {
            SaveResultCommand = new DelegateCommand(SaveResult);
            AddCarCommand = new DelegateCommand(AddCar);
            EditCarCommand = new DelegateCommand(EditCar);
            AddNodeCommand = new DelegateCommand(AddNode);
            EditNodeCommand = new DelegateCommand(EditNode);
            ClearCommand = new DelegateCommand(Clear, CanClear);
            SolveCommand = new DelegateCommand(Solve, CanSolve);
            ModeChangeCommand = new DelegateCommand(ModeChange);
            SetAlgoParamCommand = new DelegateCommand(SetAlgoParam, CanSetAlgoParam);
            ImportNodesFromFileCommand = new DelegateCommand(ImportNodesFromFile);
            ImportCarsFromFileCommand = new DelegateCommand(ImportCarsFromFile);
            ShowAddCarPopupCommand = new DelegateCommand(ShowAddCarPopup);
            ShowAddNodePopupCommand = new DelegateCommand(ShowAddNodePopup);
            SetRunParamCommand = new DelegateCommand(SetRunParam);
            MouseAddNodeCommand = new DelegateCommand(MouseAddNode);
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

        // 解析车辆数据文件
        private bool LoadCarsInfo(string path)
        {
            // 打开文件
            try
            {
                StreamReader reader = new StreamReader(path);
                string txt = reader.ReadToEnd();
                char[] sep = { ' ', '\t', '\n' };
                string[] input = txt.Split(sep);

                int index = 0;

                int numCar = Convert.ToInt32(input[index++]);
                double[] capacity = new double[numCar];
                double[] disLimit = new double[numCar];
                for (int i = 0; i < numCar; ++i)
                {
                    capacity[i] = Convert.ToDouble(input[index++]);
                    disLimit[i] = Convert.ToDouble(input[index++]);
                    Record.Cars.Add(new Car { WeightLimit = capacity[i], DisLimit = disLimit[i] });
                }                

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        // 从文件导入车辆信息
        private void ImportCarsFromFile(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "车辆数据文件 (.txt)|*.txt|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                Record.Cars.Clear();
                Segments.Clear();
                if (!LoadCarsInfo(dlg.FileName))
                {
                    MessageBox.Show("车辆数据文件读取失败！");
                }
            }
        }

        // 解析节点数据文件
        public bool LoadNodesInfo(string path)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                string txt = reader.ReadToEnd();
                char[] sep = { ' ', '\t', '\n' };
                string[] input = txt.Split(sep);

                int index = 0;

                int numNode = Convert.ToInt32(input[index++]);
                double[] x = new double[numNode];
                double[] y = new double[numNode];
                double[] demand = new double[numNode];
                for (int i = 0; i < numNode; ++i)
                {
                    x[i] = Convert.ToDouble(input[index++]);
                    y[i] = Convert.ToDouble(input[index++]);
                    demand[i] = Convert.ToDouble(input[index++]);
                    Record.Nodes.Add(new Node { X = x[i], Y = y[i], Demand = demand[i] });
                }               

                return true;
            }
            catch (Exception)
            {
                Record.Nodes.Clear();
                return false;
            }
        }

        // 从文件导入节点信息
        private void ImportNodesFromFile(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "节点数据文件 (.txt)|*.txt|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                Record.Nodes.Clear();
                Segments.Clear();
                if (!LoadNodesInfo(dlg.FileName))
                {
                    MessageBox.Show("节点数据文件读取失败！");
                }
            }
        }

        // 判断是否能执行求解操作
        private bool CanSolve(object arg)
        {
            return Record.Nodes.Count > 0 && Record.Cars.Count > 0;
        }

        // 判断是否能执行清空操作
        private bool CanClear(object arg)
        {
            return Record.Nodes.Count > 0 || Record.Cars.Count > 0;
        }

        // 保存结果
        private void SaveResult(object parameter)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
        }

        // 添加车辆
        private void AddCar(object parameter)
        {
            CarInfoDlg dlg = new CarInfoDlg();
            CarInfoDlgViewModel viewModel = new CarInfoDlgViewModel() { Title = "添加车辆" };
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
            CarInfoDlgViewModel viewModel = new CarInfoDlgViewModel() { Title = "编辑车辆", WeightLimit = Record.Cars[index].WeightLimit, DisLimit = Record.Cars[index].DisLimit };
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
            NodeInfoDlgViewModel viewModel = new NodeInfoDlgViewModel() { Title = "添加节点" };

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

            NodeInfoDlg dlg = new NodeInfoDlg();
            NodeInfoDlgViewModel viewModel = new NodeInfoDlgViewModel() { Title = "编辑节点", X = Record.Nodes[index].X, Y = Record.Nodes[index].Y, Demand = Record.Nodes[index].Demand };
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
                MessageBox.Show("求解失败！");
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
                //ProblemRecord record = GetProblemRecord();
                try
                {
                    //record.SaveToFile(GlobalData.RecordPath + record.Name + ".txt");
                    Record.CreateTime = DateTime.Now;
                    Record.Name = record.CreateTime.ToString("yyyy-MM-dd-HH-mm-ss");
                    Record.SaveToFile(GlobalData.RecordPath + record.Name + ".txt");
                }
                catch (Exception e)
                {
                    MessageBox.Show("保存求解记录失败！\n" + e.Message);
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
            else
            {
                return (int)obj == 2;
            }
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

    class NodeButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            if (index == 0)
            {
                return Brushes.Red;
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0x38, 0x7c, 0xdf));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}