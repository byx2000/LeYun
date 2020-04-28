using LeYun.Model;
using LeYun.View.Dlg;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;

namespace LeYun.ViewModel
{
    class SettingPageViewModel : ViewModelBase
    {
        // 命令
        public DelegateCommand ChangeRecordLocationCommand { get; }
        public DelegateCommand ActivateCommand { get; }
        public DelegateCommand ChangeCoordRangeCommand { get; }

        // 当前记录保存路径
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

        // 当前激活状态
        private bool isActive = GlobalData.IsActive;
        public bool IsActive
        {
            get { return isActive; }
            set 
            { 
                isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        // 当前密钥
        public string ActivateKey { get; set; } = GlobalData.ActivateKey;

        // X坐标最大值
        private string maxNodeX = GlobalData.MaxNodeX.ToString();
        public string MaxNodeX 
        { 
            get { return maxNodeX; }
            set
            {
                maxNodeX = value;
                RaisePropertyChanged("MaxNodeX");
                IsChangeXRangeButtonEnable = true;
            }
        }

        // Y坐标最大值
        private string maxNodeY = GlobalData.MaxNodeY.ToString();
        public string MaxNodeY
        {
            get { return maxNodeY; }
            set
            {
                maxNodeY = value;
                RaisePropertyChanged("MaxNodeY");
                IsChangeYRangeButtonEnable = true;
            }
        }

        // 改变X范围按钮状态
        private bool isChangeXRangeButtonEnable = false;
        public bool IsChangeXRangeButtonEnable
        {
            get { return isChangeXRangeButtonEnable; }
            set 
            { 
                isChangeXRangeButtonEnable = value;
                RaisePropertyChanged("IsChangeXRangeButtonEnable");
            }
        }

        // 改变Y范围按钮状态
        private bool isChangeYRangeButtonEnable = false;
        public bool IsChangeYRangeButtonEnable
        {
            get { return isChangeYRangeButtonEnable; }
            set
            {
                isChangeYRangeButtonEnable = value;
                RaisePropertyChanged("IsChangeYRangeButtonEnable");
            }
        }

        // 构造函数
        public SettingPageViewModel()
        {
            ChangeRecordLocationCommand = new DelegateCommand(ChangeRecordLocation);
            ActivateCommand = new DelegateCommand(Activate, CanActivate);
            ChangeCoordRangeCommand = new DelegateCommand(ChangeCoordRange);
        }

        // 更改坐标范围
        private void ChangeCoordRange(object obj)
        {
            try
            {         
                // 修改X范围
                if ((string)obj == "X")
                {
                    // 验证输入
                    double val = double.Parse(MaxNodeX);
                    if (val <= 0)
                    {
                        throw new Exception("X坐标最大值不能小于等于0");
                    }
                    // 写入配置文件
                    GlobalData.WriteConfiguration(GlobalData.MaxNodeXKey, val.ToString());
                    // 更新全局数据
                    GlobalData.MaxNodeX = val;
                    // 禁用修改按钮
                    IsChangeXRangeButtonEnable = false;
                }
                // 修改Y范围
                else if ((string)obj == "Y")
                {
                    // 验证输入
                    double val = double.Parse(MaxNodeY);
                    if (val <= 0)
                    {
                        throw new Exception("Y坐标最大值不能小于等于0");
                    }
                    // 写入配置文件
                    GlobalData.WriteConfiguration(GlobalData.MaxNodeYKey, val.ToString());
                    // 更新全局数据
                    GlobalData.MaxNodeY = val;
                    // 禁用修改按钮
                    IsChangeYRangeButtonEnable = false;
                }
            }
            catch (Exception e)
            {
                SystemSounds.Beep.Play();
                MsgBox.Show("修改失败！\n" + e.Message);
                return;
            }
            
            // 修改成功
            SystemSounds.Beep.Play();
            MsgBox.Show("修改成功！");
        }

        // 判断是否能激活
        private bool CanActivate(object arg)
        {
            return !IsActive;
        }

        // 激活
        private void Activate(object obj)
        {
            if ((string)obj == "123-456-789")
            {
                // 更新配置文件
                try
                {
                    GlobalData.IsActive = true;
                    GlobalData.WriteConfiguration(GlobalData.ActiveStateKey, "true");
                    GlobalData.ActivateKey = ActivateKey;
                    GlobalData.WriteConfiguration(GlobalData.ActivateKeyKey, ActivateKey);
                }
                catch (Exception e)
                {
                    SystemSounds.Beep.Play();
                    MsgBox.Show("激活失败！\n" + e.Message);
                    return;
                }

                IsActive = true;

                SystemSounds.Beep.Play();
                MsgBox.Show("激活成功！");
                
            }
            else
            {
                SystemSounds.Beep.Play();
                MsgBox.Show("激活码错误！\n激活码形式如下：XXX-XXX-XXX");
            }
        }

        // 改变历史记录存储位置
        private void ChangeRecordLocation(object obj)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "选择历史记录存储位置";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 移动记录文件
                    DirectoryInfo dir = new DirectoryInfo(GlobalData.RecordPath);
                    FileInfo[] files = dir.GetFiles();
                    for (int i = 0; i < files.Length; ++i)
                    {
                        if (files[i].Extension == ".rec")
                        {
                            files[i].MoveTo(dlg.SelectedPath + "/" + files[i].Name);
                        }                        
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

    class ActiveVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class KeyTextReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class VersionTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "正式版";
            }
            else
            {
                return "试用版";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ActivateButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "已激活";
            }
            else
            {
                return "激活";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class LineWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness(0, 0, 0, (double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
