﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace InterviewTest.App.View.Converter
{
    public class ZeroToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue != 0;
            }
            if (value is double doubleValue)
            {
                return doubleValue != 0.0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
