using LeYun.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace LeYun.ViewModel.Dlg
{
    class CarDetailDlgViewModel : ViewModelBase
    {
        public int ID { get; set; }
        public double WeightLimit { get; set; }
        public double Weight { get; set; }
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
        public double DisLimit { get; set; }
        public double Dis { get; set; }
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
        public double Time { get; set; }
        public double TotalTime { get; set; }
        public double TimeRate { get { return Time / TotalTime; } }
        public int NodeCount { get; set; }
        public int TotalNodeCount { get; set; }
        public double NodeRate { get { return (double)NodeCount / TotalNodeCount; } }
        public List<Node> NodeList { get; set; }
        public Node Start { get; set; }

        public List<Node> AllNode
        {
            get
            {
                List<Node> nodes = new List<Node>();

                if (NodeList.Count > 0)
                {
                    nodes.Add(Start);
                    for (int i = 0; i < NodeList.Count; ++i)
                    {
                        nodes.Add(NodeList[i]);
                    }
                }
              
                return nodes;
            }
        }

        public List<Segment> Segments
        {
            get
            {
                List<Segment> segments = new List<Segment>();
                List<Node> nodes = AllNode;
                if (nodes.Count > 1)
                {
                    for (int i = 1; i < nodes.Count; ++i)
                    {
                        segments.Add(new Segment { X1 = nodes[i - 1].X, Y1 = nodes[i - 1].Y, X2 = nodes[i].X, Y2 = nodes[i].Y, Stroke = Brushes.Black });
                    }
                    segments.Add(new Segment { X1 = nodes[nodes.Count - 1].X, Y1 = nodes[nodes.Count - 1].Y, X2 = nodes[0].X, Y2 = nodes[0].Y, Stroke = Brushes.Black });
                }
                return segments;
            }
        }
    }

    class Times100 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
