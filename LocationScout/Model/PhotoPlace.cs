using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class PhotoPlace
    {
        #region attributes
        private SubjectLocation _subjectLocation;
        private List<ParkingLocation> _allParkingLocations;
        #endregion

        #region constructors   
        public PhotoPlace(SubjectLocation subjectLocation, List<ParkingLocation> allParkingLocations)
        {
            _subjectLocation = subjectLocation;
            _allParkingLocations = allParkingLocations;
        }
        #endregion
    }
}
