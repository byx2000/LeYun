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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LeYun.View.Dlg
{
    /// <summary>
    /// AdDlg.xaml 的交互逻辑
    /// </summary>
    public partial class AdDlg : Window
    {
        DispatcherTimer timer;
        int cnt = 5;

        public AdDlg()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += OnTimer;
            timer.Start();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            if (cnt == 0)
            {
                timer.Stop();
                Close();
            }
            else
            {
                cnt--;
                time.Text = "跳过(" + cnt.ToString() + "s)";
            }
        }

        private void skip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            Close();
        }
    }
}
