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
    class RunParamSetDlgViewModel : DlgViewModelBase
    {
        public double CarSpeed { get; set; }
        public double NodeStayTime { get; set; }
        public double CongestionFactor { get; set; }
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
