using LeYun.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using WPFCustomControls;

namespace LeYun.View.Dlg
{
    /// <summary>
    /// ActivateDlg.xaml 的交互逻辑
    /// </summary>
    public partial class ActivateDlg : Dialog
    {
        public ActivateDlg()
        {
            InitializeComponent();
        }

        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            if (key.Text == "123-456-789")
            {
                GlobalData.IsActive = true;
                Close();
                SystemSounds.Beep.Play();
                MsgBox.Show("激活成功！");
            }
            else
            {
                SystemSounds.Beep.Play();
                MsgBox.Show("注册码错误！");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
