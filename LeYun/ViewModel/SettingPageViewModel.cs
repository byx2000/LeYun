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

        // 构造函数
        public SettingPageViewModel()
        {
            ChangeRecordLocationCommand = new DelegateCommand(ChangeRecordLocation);
            ActivateCommand = new DelegateCommand(Activate, CanActivate);
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
                GlobalData.IsActive = true;
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

    class DemoDurationValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val <= 0)
                {
                    return new ValidationResult(false, "演示时长必须大于0");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }

    class NodeMaxXValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val <= 0)
                {
                    return new ValidationResult(false, "X坐标最大值必须大于0");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }

    class NodeMaxYValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val <= 0)
                {
                    return new ValidationResult(false, "Y坐标最大值必须大于0");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }

    class ErrorTipVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
