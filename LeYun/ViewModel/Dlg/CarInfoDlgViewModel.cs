using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LeYun.ViewModel.Dlg
{
    class CarInfoDlgViewModel : ViewModelBase
    {
        public string Title { get; set; }

        public double WeightLimit { get; set; }
        public double DisLimit { get; set; }

        public bool IsCancel { get; private set; }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public CarInfoDlgViewModel()
        {
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
            IsCancel = true;
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

    class WeightLimitValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val < 0)
                {
                    return new ValidationResult(false, "最大载重不能小于0");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }

    class DisLimitValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val < 0)
                {
                    return new ValidationResult(false, "最大里程不能小于0");
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
