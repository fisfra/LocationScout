using LocationScout.Model;
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
            var cp = GPSCoordinatesHelper.GetPosition(doubleValue, coordinateType);

            // return the converted string value
            return new GPSCoordinatesHelper(doubleValue, cp).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GPSConverterComplex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "<not set>";

            if (value is GPSCoordinates gpsCoordinates)
            {
                var cpLat = GPSCoordinatesHelper.GetPosition(gpsCoordinates.Latitude, E_CoordinateType.Latitude);
                var cpLatString = new GPSCoordinatesHelper(gpsCoordinates.Latitude, cpLat).ToString();

                var cpLong = GPSCoordinatesHelper.GetPosition(gpsCoordinates.Longitude, E_CoordinateType.Longitude);
                var cpLongString = new GPSCoordinatesHelper(gpsCoordinates.Longitude, cpLong).ToString();

                result = cpLatString + " " + cpLongString;                     
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DeleteCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
