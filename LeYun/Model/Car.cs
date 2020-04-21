using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.Model
{
    class Car : ViewModelBase
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

		// 最大载重
		private double weightLimit;
		public double WeightLimit
		{
			get { return weightLimit; }
			set 
			{ 
				weightLimit = value;
				RaisePropertyChanged("WeightLimit");
				RaisePropertyChanged("WeightRate");
			}
		}

		// 最大里程
		private double disLimit;
		public double DisLimit
		{
			get { return disLimit; }
			set 
			{ 
				disLimit = value;
				RaisePropertyChanged("DisLimit");
				RaisePropertyChanged("DisRate");
			}
		}

		// 实际载重
		private double weight;
		public double Weight
		{
			get { return weight; }
			set 
			{ 
				weight = value;
				RaisePropertyChanged("Weight");
				RaisePropertyChanged("WeightRate");
			}
		}

		// 实际里程
		private double dis;
		public double Dis
		{
			get { return dis; }
			set
			{ 
				dis = value;
				RaisePropertyChanged("Dis");
				RaisePropertyChanged("DisRate");
			}
		}

		// 满载率
		public double LoadRate
		{
			get 
			{ 
				if (WeightLimit != 0)
				{
					return Weight / WeightLimit;
				}
				else
				{
					return 0;
				}
			}
		}

		// 里程使用率
		public double DisRate
		{
			get 
			{ 
				if (DisLimit != 0)
				{
					return Dis / DisLimit;
				}
				else
				{
					return 0;
				}
			}
		}

		// 运行时间
		private double time;
		public double Time
		{
			get { return time; }
			set
			{
				time = value;
				RaisePropertyChanged("Time");
			}
		}

		// 配送路径
		private List<Node> path = new List<Node>();
		public List<Node> Path 
		{ 
			get { return path; }
			set
			{
				path = value;
				RaisePropertyChanged("Path");
			}
		}
	}
}
