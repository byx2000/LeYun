using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LeYun.Model
{
    static class Util
    {
        // 随机数种子
        static long tick = DateTime.Now.Ticks;

        // 随机数产生器
        static public Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

        // 生成随机画刷
        public static Brush RandomColorBrush()
        {
            int R = ran.Next(255);
            int G = ran.Next(255);
            int B = ran.Next(255);
            B = (R + G > 400) ? R + G - 400 : B;
            B = (B > 255) ? 255 : B;
            return new SolidColorBrush(Color.FromRgb((byte)R, (byte)G, (byte)B));
        }
    }
}
