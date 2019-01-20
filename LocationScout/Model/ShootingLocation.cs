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
        public ParkingLocation ParkingLocation { get; set; }      
        public List<Photo> Photos { get; set; }
        #endregion

        #region constructors
        public ShootingLocation()
        { 
        }
        #endregion
    }
}
