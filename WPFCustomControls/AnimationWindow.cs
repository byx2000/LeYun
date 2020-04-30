using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFCustomControls
{
    public class AnimationWindow : Window
    {
        static AnimationWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationWindow), new FrameworkPropertyMetadata(typeof(AnimationWindow)));
        }

        // 构造函数
        public AnimationWindow()
        {
            Closing += AnimationWindow_Closing;
        }

        private void AnimationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBox.Show("123");
        }
    }
}
