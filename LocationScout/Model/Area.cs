using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class Area
    {
        #region
        private List<Country> _allCountries;
        private string _name;
        #endregion

        #region contructor
        public Area(List<Country> allCountries, string areaName)
        {
            _allCountries = allCountries;
            _name = areaName;
        }
        #endregion
    }
}
