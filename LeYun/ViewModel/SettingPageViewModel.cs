using LeYun.Model;
using LeYun.View.Dlg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace LeYun.ViewModel
{
    class SettingPageViewModel : ViewModelBase
    {
        public DelegateCommand ChangeRecordLocationCommand { get; }

        private string currentRecordPath = GlobalData.RecordPath;
        public string CurrentRecordPath 
        { 
            get { return currentRecordPath; } 
            set
            {
                currentRecordPath = value;
                RaisePropertyChanged("CurrentRecordPath");
            }
        }

        public SettingPageViewModel()
        {
            ChangeRecordLocationCommand = new DelegateCommand(ChangeRecordLocation);
        }

        // 改变历史记录存储位置
        private void ChangeRecordLocation(object obj)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 移动记录文件
                    DirectoryInfo dir = new DirectoryInfo(GlobalData.RecordPath);
                    FileInfo[] files = dir.GetFiles();
                    for (int i = 0; i < files.Length; ++i)
                    {
                        files[i].MoveTo(dlg.SelectedPath + "/" + files[i].Name);
                    }

                    // 更新配置文件
                    GlobalData.RecordPath = dlg.SelectedPath;
                    GlobalData.WriteConfiguration(GlobalData.RecordPathKey, dlg.SelectedPath);
                    CurrentRecordPath = dlg.SelectedPath;

                    // 成功提示
                    SystemSounds.Beep.Play();
                    MsgBox.Show("设置成功！");
                }
                catch (Exception e)
                {
                    MsgBox.Show("设置失败！\n" + e.Message);
                }
            }
        }
    }
}
