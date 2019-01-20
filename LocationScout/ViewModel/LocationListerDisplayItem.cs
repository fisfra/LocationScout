using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.ViewModel
{
    public class LocationListerDisplayItem
    {
        #region attributes
        public string LocationName { get; set; }
        public string CountryName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public double SubjectLatitude { get; set; }
        public double SubjectLongitude { get; set; }
        #endregion
    }

    #region enums
    // nested types not supported by XAML, therefore not part of a class
    public enum E_CoordinateType { Longitude, Latitude };
    #endregion
}
