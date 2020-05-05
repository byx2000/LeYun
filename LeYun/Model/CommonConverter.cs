using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LeYun.Model.CommonConverter
{
    class EmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //class ComplexConverter : IValueConverter
    //{
    //    public IValueConverter ResultConverter { get; set; } = new EmptyConverter();

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return ResultConverter.Convert(value, targetType, parameter, culture);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    class BoolAndConverter : IMultiValueConverter
    {
        public IValueConverter ResultConverter { get; set; } = new EmptyConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;
            for (int i = 0; i < values.Length; ++i)
            {
                result = result && (bool)values[i];
            }
            return ResultConverter.Convert(result, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BoolOrConverter : IMultiValueConverter
    {
        public IValueConverter ResultConverter { get; set; } = new EmptyConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            for (int i = 0; i < values.Length; ++i)
            {
                result = result || (bool)values[i];
            }
            return ResultConverter.Convert(result, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BoolNotConverter : IValueConverter
    {
        public IValueConverter ResultConverter { get; set; } = new EmptyConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ResultConverter.Convert(!(bool)value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = (bool)value;
            if (isVisible)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
