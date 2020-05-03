using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace LeYun.Model
{
    class AnimatableValue : Animatable
    {
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(AnimatableValue), new PropertyMetadata(0.0d));

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
