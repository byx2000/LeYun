using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.Model
{
    class NodeCollection : ObservableCollection<Node>
    {
        private const string NodeDataFileFlag = "NodeData";

        public new void Add(Node node)
        {
            node.ID = Count;
            base.Add(node);
        }

        public void SaveToFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.WriteLine(NodeDataFileFlag);
                    sw.WriteLine(Count.ToString());
                    for (int i = 0; i < Count; ++i)
                    {
                        sw.WriteLine(this[i].X.ToString());
                        sw.WriteLine(this[i].Y.ToString());
                        sw.WriteLine(this[i].Demand.ToString());
                    }
                }
            }
        }

        public void ReadFromFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    if (sr.ReadLine() != NodeDataFileFlag)
                    {
                        throw new Exception("文件格式错误！");
                    }

                    Clear();
                    int cnt = int.Parse(sr.ReadLine());
                    for (int i = 0; i < cnt; ++i)
                    {
                        double x = double.Parse(sr.ReadLine());
                        double y = double.Parse(sr.ReadLine());
                        double d = double.Parse(sr.ReadLine());
                        Add(new Node { X = x, Y = y, Demand = d});
                    }
                }
            }
        }
    }
}
