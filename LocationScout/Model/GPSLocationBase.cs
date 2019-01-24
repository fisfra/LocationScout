using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LocationScout.Model
{
    public class GPSLocationBase : Location
    {
        #region attributes
        public GPSCoordinates Coordinates { get; set; }
        #endregion

        #region constructors
        public GPSLocationBase()
        {
        }
        #endregion
    }
}
