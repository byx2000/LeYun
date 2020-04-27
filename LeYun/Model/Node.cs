using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.Model
{
    class Node : ViewModelBase
    {
        // 编号
        private int id;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                RaisePropertyChanged("ID");
            }
        }

        // X坐标
        private double x;
        public double X
        {
            get { return x; }
            set 
            { 
                x = value;
                RaisePropertyChanged("X");
            }
        }

        // Y坐标
        private double y;
        public double Y
        {
            get { return y; }
            set 
            { 
                y = value;
                RaisePropertyChanged("Y");
            }
        }

        // 需求量
        private double demand;
        public double Demand
        {
            get { return demand; }
            set 
            { 
                demand = value;
                RaisePropertyChanged("Demand");
            }
        }

        // 距离
        public double Distance(Node n)
        {
            return Math.Sqrt((X - n.X) * (X - n.X) + (Y - n.Y) * (Y - n.Y));
        }
    }
}
