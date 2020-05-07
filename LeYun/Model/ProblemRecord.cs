using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LeYun.Model
{
    class ProblemRecord : ViewModelBase, ICloneable
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


        // 复制
        public object Clone()
        {
            ProblemRecord record = new ProblemRecord();
            record.CreateTime = CreateTime;
            record.Name = (string)Name.Clone();
            record.CarSpeed = CarSpeed;
            record.NodeStayTime = NodeStayTime;
            record.Cars = (CarCollection)Cars.Clone();
            record.Nodes = (NodeCollection)Nodes.Clone();

            // 复制路径
            for (int i = 0; i < Paths.Count; ++i)
            {
                record.Paths.Add(new ObservableCollection<int>());
                for (int j = 0; j < Paths[i].Count; ++j)
                {
                    record.Paths[i].Add(Paths[i][j]);
                }
            }

            // 复制线条
            for (int i = 0; i < Segments.Count; ++i)
            {
                record.Segments.Add((Segment)Segments[i].Clone());
            }

            return record;
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

        // 获取使用车辆数
        public int GetUseCarCount()
        {
            int cnt = 0;
            for (int i = 0; i < Paths.Count; ++i)
            {
                if (Paths[i].Count > 0)
                {
                    cnt++;
                }
            }
            return cnt;
        }

        // 获取车辆载重
        public double GetCarWeight(int iCar)
        {
            double w = 0;
            for (int i = 0; i < Paths[iCar].Count; ++i)
            {
                int iNode = Paths[iCar][i];
                w += Nodes[iNode].Demand;
            }
            return w;
        }

        // 获取车辆满载率
        public double GetCarLoadRate(int iCar)
        {
            if (Cars[iCar].WeightLimit == 0)
            {
                return 0;
            }
            return GetCarWeight(iCar) / Cars[iCar].WeightLimit;
        }

        // 获取总满载率
        public double GetTotalLoadRate()
        {
            double r = 0;
            int cnt = 0;
            for (int i = 0; i < Paths.Count; ++i)
            {
                if (Paths[i].Count > 0)
                {
                    cnt++;
                    r += GetCarLoadRate(i);
                }
            }
            return r / cnt;
        }

        // 获取车辆行驶里程
        public double GetCarDistance(int iCar)
        {
            Node last = Nodes[0];
            double dis = 0;
            for (int i = 0; i < Paths[iCar].Count; ++i)
            {
                int iNode = Paths[iCar][i];
                dis += last.Distance(Nodes[iNode]);
                last = Nodes[iNode];
            }
            dis += last.Distance(Nodes[0]);
            return dis;
        }

        // 获取车辆运行时间
        public double GetCarTime(int iCar)
        {
            return GetCarDistance(iCar) / CarSpeed * 60 + Paths[iCar].Count * NodeStayTime;
        }

        // 获取车辆配送路径
        public List<Node> GetCarPath(int iCar)
        {
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < Paths[iCar].Count; ++i)
            {
                int iNode = Paths[iCar][i];
                nodes.Add(new Node { ID = Nodes[iNode].ID, X = Nodes[iNode].X, Y = Nodes[iNode].Y, Demand = Nodes[iNode].Demand });
            }
            return nodes;
        }

        // 获取总运行里程
        public double GetTotalDistance()
        {
            double dis = 0;
            for (int i = 0; i < Cars.Count; ++i)
            {
                dis += GetCarDistance(i);
            }
            return dis;
        }

        // 获取配送总时间
        public double GetTotalTime()
        {
            double time = 0;
            for (int i = 0; i < Cars.Count; ++i)
            {
                time = Math.Max(time, GetCarTime(i));
            }
            return time;
        }

        // 获取用时最多的车辆编号
        public int GetSlowestCarIndex()
        {
            double maxTime = -1;
            int index = -1;
            for (int i = 0; i < Paths.Count; ++i)
            {
                double t = GetCarTime(i);
                if (t > maxTime)
                {
                    maxTime = t;
                    index = i;
                }
            }
            return index;
        }

        // 获取节点服务时间（分钟）
        public double GetNodeServedTime(int iNode)
        {
            for (int iCar = 0; iCar < Paths.Count; ++iCar)
            {
                bool found = false;
                for (int i = 0; i < Paths[iCar].Count; ++i)
                {
                    if (Paths[iCar][i] == iNode)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    double time = 0;
                    Node last = Nodes[0];
                    for (int i = 0; i < Paths[iCar].Count; ++i)
                    {
                        int index = Paths[iCar][i];
                        time += last.Distance(Nodes[index]) / CarSpeed * 60;
                        if (index == iNode)
                        {
                            return time;
                        }
                        else
                        {
                            time += NodeStayTime;
                        }
                        last = Nodes[index];
                    }
                }
            }

            return 0;
        }

        // 计算所有信息
        public void GenerateAllInfo()
        {
            TotalTime = GetTotalTime();
            TotalDis = GetTotalDistance();
            UseCarCount = GetUseCarCount();
            TotalLoadRate = GetTotalLoadRate();

            for (int iCar = 0; iCar < Paths.Count; ++iCar)
            {
                Cars[iCar].Dis = GetCarDistance(iCar);
                Cars[iCar].Weight = GetCarWeight(iCar);
                Cars[iCar].Path = GetCarPath(iCar);
                Cars[iCar].Time = GetCarTime(iCar);
            }

            for (int iNode = 1; iNode < Nodes.Count; ++iNode)
            {
                Nodes[iNode].ServedTime = GetNodeServedTime(iNode);
            }
        }

        // 计算路径线条
        public void GenerateSegments()
        {
            Segments.Clear();
            for (int i = 0; i < Paths.Count; ++i)
            {
                if (Paths[i].Count > 0)
                {
                    Brush brush = Util.RandomColorBrush();
                    Point last = new Point(Nodes[0].X, Nodes[0].Y);
                    for (int j = 0; j < Paths[i].Count; ++j)
                    {
                        Segments.Add(new Segment { X1 = last.X, Y1 = last.Y, X2 = Nodes[Paths[i][j]].X, Y2 = Nodes[Paths[i][j]].Y, Stroke = brush });
                        last = new Point(Nodes[Paths[i][j]].X, Nodes[Paths[i][j]].Y);
                    }
                    Segments.Add(new Segment { X1 = last.X, Y1 = last.Y, X2 = Nodes[0].X, Y2 = Nodes[0].Y, Stroke = brush });
                }
            }
        }
    }
}
