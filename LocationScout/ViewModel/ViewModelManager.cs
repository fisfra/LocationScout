using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.ViewModel
{
    internal class ViewModelManager
    {
        internal static void SynchronizeDisplayItems(LocationDisplayItem locationDisplayItem, LocationListerDisplayItem listerDisplayItem)
        {
            listerDisplayItem.ParkingLocationName = locationDisplayItem.ParkingLocationName;
            listerDisplayItem.ParkingLocationLatitude = locationDisplayItem.ParkingLocationLatitude;
            locationDisplayItem.ParkingLocationLongitude = locationDisplayItem.ParkingLocationLongitude;

            listerDisplayItem.ShootingLocationName = locationDisplayItem.ShootingLocationName;
            listerDisplayItem.ShootingLocationLatitude = locationDisplayItem.ShootingLocationLatitude;
            listerDisplayItem.ShootingLocationLongitude = locationDisplayItem.ShootingLocationLongitude;
        }
    }
}
