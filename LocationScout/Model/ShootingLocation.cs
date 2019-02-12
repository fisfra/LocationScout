using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LocationScout.Model
{
    public class ShootingLocation : GPSLocationBase
    {
        #region attributes

        // navigation properties
        //public List<ParkingLocation> ParkingLocations { get; set; }     
        public virtual ICollection<ParkingLocation> ParkingLocations { get; set; }
        //public List<SubjectLocation> SubjectLocations { get; set; }
        public virtual ICollection<SubjectLocation> SubjectLocations { get; set; }
        //public List<Photo> Photos { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        #endregion

        #region constructors
        public ShootingLocation()
        { 
        }
        #endregion
    }
}
