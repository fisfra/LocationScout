using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LocationScout.Model
{
    public class ShootingLocation : LocationBase
    {
        #region attributes
        public ParkingLocation ParkingLocation { get; set; }

        public List<byte[]> LocationPhotos { get; set; }
        #endregion

        #region constructors
        public ShootingLocation()
        { 
        }
        #endregion
    }
}
