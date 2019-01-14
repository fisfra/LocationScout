using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class GPSCoordinates : IEquatable<GPSCoordinates>
    {
        #region attributes
        private double _latitude;
        private double _longitude;
        #endregion

        #region operators
        public override bool Equals(object obj)
        {
            return Equals(obj as GPSCoordinates);
        }

        public bool Equals(GPSCoordinates other)
        {
            return other != null &&
                   _latitude == other._latitude &&
                   _longitude == other._longitude;
        }

        public override int GetHashCode()
        {
            var hashCode = -615568823;
            hashCode = hashCode * -1521134295 + _latitude.GetHashCode();
            hashCode = hashCode * -1521134295 + _longitude.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(GPSCoordinates coordinates1, GPSCoordinates coordinates2)
        {
            return EqualityComparer<GPSCoordinates>.Default.Equals(coordinates1, coordinates2);
        }

        public static bool operator !=(GPSCoordinates coordinates1, GPSCoordinates coordinates2)
        {
            return !(coordinates1 == coordinates2);
        }
        #endregion

        #region contructors
        public GPSCoordinates(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }
        #endregion
    }
}
