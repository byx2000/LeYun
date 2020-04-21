using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LeYun.ViewModel.Dlg
{
    class RunParamSetDlgViewModel : ViewModelBase
    {
        public double CarSpeed { get; set; }
        public Collection<ValidationRule> CarSpeedValidationRules { get; set; } = new Collection<ValidationRule>();
        public double NodeStayTime { get; set; }

        public bool IsCancel { get; private set; }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public RunParamSetDlgViewModel()
        {
            IsCancel = true;
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
            CarSpeedValidationRules.Add(new CarSpeedValidationRule());
        }

        private void Cancel(object obj)
        {
            IsCancel = true;
            ((Window)obj).Close();
        }

        private void Ok(object obj)
        {
            IsCancel = false;
            ((Window)obj).Close();
        }
    }

    class CarSpeedValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val <= 0)
                {
                    return new ValidationResult(false, "速度不能小于等于0");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }

    class NodeStayTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val < 0)
                {
                    return new ValidationResult(false, "时间不能小于0");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }
}
