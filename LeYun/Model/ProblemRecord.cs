using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeYun.Model
{
    class ProblemRecord : ViewModelBase
    {
        private const string RecordDataFileFlag = "RecordData";

        // 创建时间
        private DateTime createTime;
        public DateTime CreateTime 
        { 
            get { return createTime; }
            set
            {
                createTime = value;
                RaisePropertyChanged("CreateTime");
            }
        }

        // 记录名称
        private string name;
        public string Name 
        { 
            get { return name; } 
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        // 文件名
        private string filename;
        public string Filename
        {
            get { return filename; }
            set
            {
                filename = value;
                RaisePropertyChanged("Filename");
            }
        }

        // 车辆平均速度(km/h)
        private double carSpeed = 50;
        public double CarSpeed 
        {
            get { return carSpeed; }
            set
            {
                carSpeed = value;
                RaisePropertyChanged("CarSpeed");
            }
        }

        // 配送点停留时间(min)
        private double nodeStayTime = 5;
        public double NodeStayTime 
        {
            get { return nodeStayTime; }
            set
            {
                nodeStayTime = value;
                RaisePropertyChanged("NodeStayTime");
            }
        }

        // 车辆列表
        private CarCollection cars = new CarCollection();
        public CarCollection Cars 
        { 
            get { return cars; }
            set
            {
                cars = value;
                RaisePropertyChanged("Cars");
            }
        }

        // 节点列表
        private NodeCollection nodes = new NodeCollection();
        public NodeCollection Nodes 
        { 
            get { return nodes; }
            set
            {
                nodes = value;
                RaisePropertyChanged("Nodes");
            }
        }

        // 配送路径
        private ObservableCollection<ObservableCollection<int>> paths = new ObservableCollection<ObservableCollection<int>>();
        public ObservableCollection<ObservableCollection<int>> Paths 
        { 
            get { return paths; }
            set
            {
                paths = value;
                RaisePropertyChanged("Paths");
            }
        }

        // 配送总时间
        private double totalTime;
        public double TotalTime
        {
            get { return totalTime; }
            set
            {
                totalTime = value;
                RaisePropertyChanged("TotalTime");
            }
        }

        // 配送总里程
        private double totalDis;
        public double TotalDis
        {
            get { return totalDis; }
            set
            {
                totalDis = value;
                RaisePropertyChanged("TotalDis");
            }
        }

        // 使用车辆数
        private int useCarCount;
        public int UseCarCount
        {
            get { return useCarCount; }
            set
            {
                useCarCount = value;
                RaisePropertyChanged("UseCarCount");
            }
        }

        // 总满载率
        private double totalLoadRate;
        public double TotalLoadRate
        {
            get { return totalLoadRate; }
            set
            {
                totalLoadRate = value;
                RaisePropertyChanged("TotalLoadRate");
            }
        }

        // 保存到文件
        public void SaveToFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    // 写入文件头
                    sw.WriteLine(RecordDataFileFlag);

                    // 写入创建时间
                    sw.WriteLine(CreateTime.ToString());

                    // 写入记录名称
                    sw.WriteLine(Name);

                    // 写入车辆速度
                    sw.WriteLine(CarSpeed);

                    // 写入配送点停留时间
                    sw.WriteLine(NodeStayTime);

                    // 写入节点信息
                    sw.WriteLine(Nodes.Count);
                    for (int i = 0; i < Nodes.Count; ++i)
                    {
                        sw.WriteLine(Nodes[i].X);
                        sw.WriteLine(Nodes[i].Y);
                        sw.WriteLine(Nodes[i].Demand);
                    }

                    //写入车辆信息
                    sw.WriteLine(Cars.Count);
                    for (int i = 0; i < Cars.Count; ++i)
                    {
                        sw.WriteLine(Cars[i].WeightLimit);
                        sw.WriteLine(Cars[i].DisLimit);
                    }

                    // 写入路径信息
                    sw.WriteLine(Paths.Count);
                    for (int i = 0; i < Paths.Count; ++i)
                    {
                        sw.WriteLine(Paths[i].Count);
                        for (int j = 0; j < Paths[i].Count; ++j)
                        {
                            sw.WriteLine(Paths[i][j]);
                        }
                    }
                }
            }
        }

        // 从文件读取
        public void ReadFromFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    // 读取文件头
                    if (sr.ReadLine() != RecordDataFileFlag)
                    {
                        throw new Exception("文件格式错误");
                    }

                    // 读取创建时间
                    CreateTime = DateTime.Parse(sr.ReadLine());

                    // 读取记录名称
                    Name = sr.ReadLine();

                    // 读取车辆速度
                    CarSpeed = double.Parse(sr.ReadLine());

                    // 读取配送点停留时间
                    NodeStayTime = double.Parse(sr.ReadLine());

                    // 读取节点信息
                    int nodeCount = int.Parse(sr.ReadLine());
                    for (int i = 0; i < nodeCount; ++i)
                    {
                        double x = double.Parse(sr.ReadLine());
                        double y = double.Parse(sr.ReadLine());
                        double d = double.Parse(sr.ReadLine());
                        Nodes.Add(new Node { X = x, Y = y, Demand = d });
                    }

                    // 读取车辆信息
                    int carCount = int.Parse(sr.ReadLine());
                    for (int i = 0; i < carCount; ++i)
                    {
                        double w = double.Parse(sr.ReadLine());
                        double d = double.Parse(sr.ReadLine());
                        Cars.Add(new Car { WeightLimit = w, DisLimit = d });
                    }

                    // 读取路径信息
                    int pathCount = int.Parse(sr.ReadLine());
                    for (int i = 0; i < pathCount; ++i)
                    {
                        Paths.Add(new ObservableCollection<int>());
                        int pathLength = int.Parse(sr.ReadLine());
                        for (int j = 0; j < pathLength; ++j)
                        {
                            Paths[i].Add(int.Parse(sr.ReadLine()));
                        }
                    }
                }
            }
        }
    }
}
