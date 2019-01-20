using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    [ComplexType]
    public class GPSCoordinates
    {
        #region attributes
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        #endregion

        #region contructors
        public GPSCoordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        #endregion
    }
}
