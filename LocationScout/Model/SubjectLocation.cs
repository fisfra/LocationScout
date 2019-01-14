using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class SubjectLocation : LocationBase
    {
        #region attributes
        private string _country;
        private string _area;
        private string _subArea;
        private string _locationName;
        #endregion

        #region constructors
        public SubjectLocation(GPSCoordinates coordinates, string country, string area, string subArea, string locationName) : base(coordinates)
        {
            _country = country;
            _area = area;
            _subArea = subArea;
            _locationName = locationName;
        }
        #endregion
    }
}
