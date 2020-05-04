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

namespace LeYun.View.Dlg
{
    /// <summary>
    /// LoadingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingDialog : Window
    {
        private static LoadingDialog loadingBox = null;

        public static void Begin(string info = "Loading...")
        {
            loadingBox = new LoadingDialog(info);
            loadingBox.ShowDialog();
        }

        public static void End()
        {
            if (loadingBox != null)
            {
                loadingBox.Close();
                loadingBox = null;
            }
        }

        public LoadingDialog(string info)
        {
            InitializeComponent();
            infoText.Text = info;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
