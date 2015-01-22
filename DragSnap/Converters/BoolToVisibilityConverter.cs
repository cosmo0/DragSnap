namespace DragSnap.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts a boolean value to a visibility value
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to a visibility
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The target type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>The converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts back a visibility to a boolean
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The target type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>The converted value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Visibility)value)
            {
                case Visibility.Collapsed:
                case Visibility.Hidden:
                    return false;
                case Visibility.Visible:
                    return true;
                default:
                    return false;
            }
        }
    }
}