using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class Area_Subarea
    {
        #region attributes
        internal string AreaName { get; set; }
        internal string SubareaName { get; set; }
        #endregion

        #region constructors
        public Area_Subarea(string areaName, string subareaName)
        {
            AreaName = areaName;
            SubareaName = subareaName;
        }
        #endregion
    }
}
