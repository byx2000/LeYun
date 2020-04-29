using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LeYun.Model
{
    class CarRuntimeInfo : ViewModelBase
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

        // 线条颜色
        private Brush lineBrush;
        public Brush LineBrush 
        { 
            get { return lineBrush; } 
            set
            {
                lineBrush = value;
                RaisePropertyChanged("LineBrush");
            }
        }

        // 是否完成配送任务
        private bool isFinished;
        public bool IsFinished
        {
            get { return isFinished; }
            set 
            { 
                isFinished = value;
                RaisePropertyChanged("IsFinished");
            }
        }

        // 完成百分比
        private double completedPercent;
        public double CompletedPercent
        {
            get { return completedPercent; }
            set 
            { 
                completedPercent = value;
                RaisePropertyChanged("CompletedPercent");
            }
        }

    }
}
