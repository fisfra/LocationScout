using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static LocationScout.ViewModel.GPSCoordinatesHelper;

namespace LocationScout.ViewModel
{
    public class GPSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // convert parameter
            double doubleValue = System.Convert.ToDouble(value);
            var coordinateType = (E_CoordinateType)parameter;

            // determinate coordinate type (E, W, S, N)
            CoordinatesPosition cp;
            switch (coordinateType)
            {
                case E_CoordinateType.Longitude:
                    cp = (doubleValue > 0) ? CoordinatesPosition.E : CoordinatesPosition.W;
                    break;
                case E_CoordinateType.Latitude:
                    cp = (doubleValue < 0) ? CoordinatesPosition.N : CoordinatesPosition.S;
                    break;
                default:
                    Debug.Assert(false);
                    throw (new Exception("Unknown enum value in GPSConverter::Convert"));
            }

            // return the converted string value
            return new GPSCoordinatesHelper(doubleValue, cp).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
