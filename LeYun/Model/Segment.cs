using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LeYun.Model
{
    class Segment : ViewModelBase
    {
        private double x1;
        public double X1 
        { 
            get { return x1; }
            set
            {
                x1 = value;
                RaisePropertyChanged("X1");
            }
        }

        private double y1;
        public double Y1 
        { 
            get { return y1; }
            set
            {
                y1 = value;
                RaisePropertyChanged("Y1");
            }
        }

        private double x2;
        public double X2 
        { 
            get { return x2; }
            set
            {
                x2 = value;
                RaisePropertyChanged("X2");
            }
        }

        private double y2;
        public double Y2 
        { 
            get { return y2; }
            set
            {
                y2 = value;
                RaisePropertyChanged("Y2");
            }
        }

        private Brush stroke;
        public Brush Stroke 
        { 
            get { return stroke; }
            set
            {
                stroke = value;
                RaisePropertyChanged("Stroke");
            }
        }
    }
}
