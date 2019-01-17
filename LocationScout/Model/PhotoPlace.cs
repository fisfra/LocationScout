using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class PhotoPlace
    {
        #region attributes
        [Key]
        public long Id { get; set; }
        public List<ParkingLocation> ParkingLocations { get; set; }
        #endregion

        #region constructors   
        public PhotoPlace()
        { 
        }
        #endregion
    }
}
