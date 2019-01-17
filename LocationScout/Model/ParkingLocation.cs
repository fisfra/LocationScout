using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class ParkingLocation : LocationBase
    {
        #region attributes
        public PhotoPlace PhotoPlace { get; set; }
        public List<ShootingLocation> ShootingLocations { get; set; }
        #endregion

        public ParkingLocation()
        {
        }
    }
}
