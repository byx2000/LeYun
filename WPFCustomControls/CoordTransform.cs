using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFCustomControls
{
    public static class CoordTransform
    {
        public const double D2R = (Math.PI / 180.0);

        public static Point PolarToCartesian(double angle, double radius)
        {
            double rad = angle * D2R;
            double x = radius * Math.Cos(rad);
            double y = radius * Math.Sin(rad);
            return new Point(x, y);
        }
    }
}
