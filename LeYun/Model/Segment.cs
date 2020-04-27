using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        private Brush stroke = Brushes.Black;
        public Brush Stroke 
        { 
            get { return stroke; }
            set
            {
                stroke = value;
                RaisePropertyChanged("Stroke");
            }
        }

        // 动画参数
        private double xFrom, yFrom, xTo, yTo, second;

        // 动画完成时的回调函数
        public delegate void OnAnimationFinishHandler();
        public OnAnimationFinishHandler OnAnimationFinish { get; set; }

        // 设置动画
        public void SetAnimation(double xFrom, double yFrom, double xTo, double yTo, double second)
        {
            this.xFrom = xFrom;
            this.yFrom = yFrom;
            this.xTo = xTo;
            this.yTo = yTo;
            this.second = second;
        }

        // 开始播放动画
        public void BeginAnimation()
        {
            double frame = 60;
            double xDelta = xTo - xFrom;
            double yDelta = yTo - yFrom;
            int count = (int)(second * frame);

            new Thread(delegate ()
            {
                X1 = xFrom;
                Y1 = yFrom;
                X2 = X1;
                Y2 = Y1;
                for (int i = 0; i < count; ++i)
                {
                    X2 += xDelta / count;
                    Y2 += yDelta / count;
                    Thread.Sleep((int)(1000 / frame));
                }
                if (OnAnimationFinish != null)
                {
                    OnAnimationFinish();
                }
            }).Start();
        }

        //public void BeginAnimation(Point from, Point to, double second, OnFinish onFinish = null)
        //{
        //    X1 = X2 = from.X;
        //    Y1 = Y2 = from.Y;
        //    double frame = 60;
        //    double xDelta = to.X - from.X;
        //    double yDelta = to.Y - from.Y;
        //    int count = (int)(second * frame);

        //    new Thread(delegate ()
        //    {
        //        for (int i = 0; i < count; ++i)
        //        {
        //            X2 += xDelta / count;
        //            Y2 += yDelta / count;
        //            Thread.Sleep(16);
        //        }
        //        if (onFinish != null)
        //        {
        //            onFinish();
        //        }
        //    }).Start();    
        //}
    }
}
