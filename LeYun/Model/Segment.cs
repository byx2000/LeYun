using LeYun.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LeYun.Model
{
    class Segment : Animatable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(Segment), new PropertyMetadata(0.0d));



        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(Segment), new PropertyMetadata(0.0d));



        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(Segment), new PropertyMetadata(0.0d));



        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(Segment), new PropertyMetadata(0.0d));

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
        public double XAnimationFrom { get; set; }
        public double YAnimationFrom { get; set; }
        public double XAnimationTo { get; set; }
        public double YAnimationTo { get; set; }
        public double Duration { get; set; } // 秒
        public double Delay { get; set; } // 秒

        // 动画完成时的回调函数
        public delegate void OnAnimationFinishHandler();
        public OnAnimationFinishHandler AnimationCompleted { get; set; }

        // 开始播放动画
        public void BeginAnimation()
        {
            DoubleAnimation xAnim = new DoubleAnimation();
            xAnim.From = XAnimationFrom;
            xAnim.To = XAnimationTo;
            xAnim.Duration = new Duration(TimeSpan.FromSeconds(Duration));
            xAnim.BeginTime = TimeSpan.FromSeconds(Delay);
            xAnim.Completed += delegate (object sender, EventArgs e)
            {
                if (AnimationCompleted != null)
                {
                    AnimationCompleted();
                }
            };

            DoubleAnimation yAnim = new DoubleAnimation();
            yAnim.From = YAnimationFrom;
            yAnim.To = YAnimationTo;
            yAnim.Duration = new Duration(TimeSpan.FromSeconds(Duration));
            yAnim.BeginTime = TimeSpan.FromSeconds(Delay);

            X1 = X2 = XAnimationFrom;
            Y1 = Y2 = YAnimationFrom;
            BeginAnimation(X2Property, xAnim);
            BeginAnimation(Y2Property, yAnim);
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
