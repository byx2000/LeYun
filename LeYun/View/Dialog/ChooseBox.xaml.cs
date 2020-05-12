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

namespace LeYun.View
{
    /// <summary>
    /// ChooseBox.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseBox : Window
    {
        static bool choose;

        public static bool Show(string info, string title = "选择", string btn1Text = "确定", string btn2Text = "取消")
        {
            ChooseBox chooseBox = new ChooseBox(title, info, btn1Text, btn2Text);
            chooseBox.ShowDialog();
            return choose;
        }

        public ChooseBox(string title, string info, string btn1Text, string btn2Text)
        {
            InitializeComponent();
            titleText.Text = title;
            infoText.Text = info;
            okBtn.Content = btn1Text;
            cancelBtn.Content = btn2Text;
        }

        private void closeWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        private void ok_Click(object sender, RoutedEventArgs e)
        {
            choose = true;
            Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            choose = false;
            Close();
        }
    }
}
