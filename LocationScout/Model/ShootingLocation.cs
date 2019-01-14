using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LocationScout.Model
{
    internal class ShootingLocation : LocationBase
    {
        #region attributes
        private List<BitmapImage> _locationPhotos;
        #endregion

        #region constructors
        public ShootingLocation(GPSCoordinates coordinates, List<BitmapImage> locationPhotos) : base(coordinates)
        {
            _locationPhotos = locationPhotos;
        }
        #endregion
    }
}
