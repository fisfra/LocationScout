using LocationScout.Model;
using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout
{
    public class GoogleMapsHelper
    {
        #region constants
        private static readonly string c_googleMapsUrl = "https://www.google.com/maps/place/";
        private static readonly string c_chrome_exe = "chrome.exe";
        #endregion

        public static void GoGoogleMap(GPSCoordinates coordinates)
        {
            var latitude = coordinates.Latitude;
            var longitude = coordinates.Longitude;

            var cpLat = GPSCoordinatesHelper.GetPosition(latitude, E_CoordinateType.Latitude);
            var cpLong = GPSCoordinatesHelper.GetPosition(longitude, E_CoordinateType.Longitude);

            var url = c_googleMapsUrl + new GPSCoordinatesHelper(latitude, cpLat).ToGoogleMapsString() + "+" + new GPSCoordinatesHelper(longitude, cpLong).ToGoogleMapsString();

            Process.Start(c_chrome_exe, url);
        }
    }
}
