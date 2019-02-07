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
        private static readonly string c_googleMapsPlaceUrl = "https://www.google.com/maps/place/";
        private static readonly string c_googleMapsSearchUrl = "https://www.google.com/maps/search/";
        private static readonly string c_chrome_exe = "chrome.exe";
        #endregion

        public static void Go(GPSCoordinates coordinates)
        {
            var latitude = coordinates.Latitude;
            var longitude = coordinates.Longitude;

            var cpLat = GPSCoordinatesHelper.GetPosition(latitude, E_CoordinateType.Latitude);
            var cpLong = GPSCoordinatesHelper.GetPosition(longitude, E_CoordinateType.Longitude);

            var url = c_googleMapsPlaceUrl + new GPSCoordinatesHelper(latitude, cpLat).ToGoogleMapsString() + "+" + new GPSCoordinatesHelper(longitude, cpLong).ToGoogleMapsString();

            Process.Start(c_chrome_exe, url);
        }

        public static void Search(string text)
        {
            var url = c_googleMapsSearchUrl + text + "/";
            Process.Start(c_chrome_exe, url);
        }
    }
}
