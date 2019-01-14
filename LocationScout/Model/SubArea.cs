using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class SubArea
    {
        #region attributes
        private Country country;
        private Area _area;
        private string subareaName;
        #endregion

        #region constructors
        public SubArea(Country country, Area area, string subareaName)
        {
            this.country = country;
            _area = area;
            this.subareaName = subareaName;
        }
        #endregion
    }
}
