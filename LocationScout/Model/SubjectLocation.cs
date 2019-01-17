using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class SubjectLocation : LocationBase
    {
        #region attributes
        public Country SubjectCountry {get; set;}
        public Area SubjectArea {get; set;}
        public SubArea SubjectSubArea { get; set; }
        public string LocationName { get; set; }
        public List<ParkingLocation> ParkingLocations { get; set; }
        #endregion

        #region constructors
        public SubjectLocation()
        {
        }
        #endregion
    }
}
