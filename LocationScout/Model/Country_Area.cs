using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class Country_Area
    {
        #region attributes
        internal string CountryName { get; set; }
        internal string AreaName { get; set; }
        #endregion

        #region constructors
        public Country_Area(string countryName, string areaName)
        {
            CountryName = countryName;
            AreaName = areaName;
        }
        #endregion
    }
}
