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
    public class ShootingLocation : LocationBase
    {
        #region attributes
        public string Name { get; set; }

        // navigation properties
        public List<ParkingLocation> ParkingLocations { get; set; }     
        public List<SubjectLocation> SubjectLocations { get; set; }
        public List<Photo> Photos { get; set; }
        #endregion

        #region constructors
        public ShootingLocation()
        { 
        }
        #endregion
    }
}
