using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    [ComplexType]
    public class GPSCoordinates : IEquatable<GPSCoordinates>
    {
        #region attributes
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        #endregion

        #region contructors
        public GPSCoordinates(double? latitude, double? longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public GPSCoordinates()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GPSCoordinates);
        }

        public bool Equals(GPSCoordinates other)
        {
            return other != null &&
                   EqualityComparer<double?>.Default.Equals(Latitude, other.Latitude) &&
                   EqualityComparer<double?>.Default.Equals(Longitude, other.Longitude);
        }

        public override int GetHashCode()
        {
            var hashCode = -1416534245;
            hashCode = hashCode * -1521134295 + EqualityComparer<double?>.Default.GetHashCode(Latitude);
            hashCode = hashCode * -1521134295 + EqualityComparer<double?>.Default.GetHashCode(Longitude);
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
    }
}
