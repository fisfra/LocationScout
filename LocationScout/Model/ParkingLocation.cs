using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class ParkingLocation : LocationBase
    {
        #region attributes
        private List<ShootingLocation> _allShootingLocations;
        #endregion

        public ParkingLocation(GPSCoordinates coordinates, List<ShootingLocation> allShootingLocations) : base(coordinates)
        {
            _allShootingLocations = allShootingLocations;
        }
    }
}
