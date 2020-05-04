using LeYun.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LeYun.ViewModel.Dlg
{
    class NodeInfoDlgViewModel : DlgViewModelBase
    {
        public string Title { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Demand { get; set; }
    }

    class XValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val < 0)
                {
                    return new ValidationResult(false, "X坐标不能小于0");
                }
                else if (val > GlobalData.MaxNodeX)
                {
                    return new ValidationResult(false, "X坐标超出范围");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }

    class YValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val < 0)
                {
                    return new ValidationResult(false, "Y坐标不能小于0");
                }
                else if (val > GlobalData.MaxNodeY)
                {
                    return new ValidationResult(false, "Y坐标超出范围");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }

    class DemandValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                double val = double.Parse((string)value);
                if (val < 0)
                {
                    return new ValidationResult(false, "需求量不能小于0");
                }
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "请输入浮点数");
            }
        }
    }

    class XRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "(0 - " + GlobalData.MaxNodeX.ToString() + ")";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class YRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "(0 - " + GlobalData.MaxNodeY.ToString() + ")";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
