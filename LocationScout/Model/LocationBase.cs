using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class LocationBase
    {
        #region attributes
        private GPSCoordinates _coordinates;
        #endregion

        #region constructors
        public LocationBase(GPSCoordinates coordinates)
        {
            _coordinates = coordinates;
        }
        #endregion

    }
}
