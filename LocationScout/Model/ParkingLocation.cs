using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
        public class ParkingLocation : GPSLocationBase
        {
            #region attributes
            // navigation properties
            public List<ShootingLocation> ShootingLocations { get; set; }
            #endregion

            public ParkingLocation()
            {
            }
        }
}
