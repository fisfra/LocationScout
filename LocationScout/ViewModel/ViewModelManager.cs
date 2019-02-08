using LocationScout.Model;
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
            listerDisplayItem.ParkingLocationLongitude = locationDisplayItem.ParkingLocationLongitude;

            listerDisplayItem.ShootingLocationName = locationDisplayItem.ShootingLocationName;
            listerDisplayItem.ShootingLocationLatitude = locationDisplayItem.ShootingLocationLatitude;
            listerDisplayItem.ShootingLocationLongitude = locationDisplayItem.ShootingLocationLongitude;

            listerDisplayItem.Photo_1 = locationDisplayItem.Photo_1;
            listerDisplayItem.Photo_2 = locationDisplayItem.Photo_2;
            listerDisplayItem.Photo_3 = locationDisplayItem.Photo_3;
        }

        internal static LocationListerDisplayItem ConvertToListerDisplayItem(LocationDisplayItem locationDisplayItem, ShootingLocation shootingLocation)
        {
            return new LocationListerDisplayItem()
            {
                CountryName = locationDisplayItem.CountryName,
                AreaName = locationDisplayItem.AreaName,
                SubAreaName = locationDisplayItem.SubAreaName,
                SubjectLocationName = locationDisplayItem.SubjectLocationName,
                SubjectLocationLatitude = locationDisplayItem.SubjectLocationLatitude,
                SubjectLocationLongitude = locationDisplayItem.ShootingLocationLongitude,
                ShootingLocationName = locationDisplayItem.ShootingLocationName,
                ShootingLocationLatitude = locationDisplayItem.ShootingLocationLatitude,
                ShootingLocationLongitude = locationDisplayItem.ShootingLocationLongitude,
                Photo_1 = locationDisplayItem.Photo_1,
                Photo_2 = locationDisplayItem.Photo_2,
                Photo_3 = locationDisplayItem.Photo_3,
                ParkingLocationName = locationDisplayItem.ParkingLocationName,
                ParkingLocationLatitude = locationDisplayItem.ParkingLocationLatitude,
                ParkingLocationLongitude = locationDisplayItem.ParkingLocationLongitude,
                Tag = shootingLocation
            };
        }

        internal static void SetLocationListerDisplayItem(LocationListerDisplayItem source, ref LocationListerDisplayItem target)
        {
            target.CountryName = source.CountryName;
            target.AreaName = source.AreaName;
            target.SubAreaName = source.SubAreaName;
            target.SubjectLocationName = source.SubjectLocationName;
            target.SubjectLocationLatitude = source.SubjectLocationLatitude;
            target.SubjectLocationLongitude = source.ShootingLocationLongitude;
            target.ShootingLocationName = source.ShootingLocationName;
            target.ShootingLocationLatitude = source.ShootingLocationLatitude;
            target.ShootingLocationLongitude = source.ShootingLocationLongitude;
            target.Photo_1 = source.Photo_1;
            target.Photo_2 = source.Photo_2;
            target.Photo_3 = source.Photo_3;
            target.ParkingLocationName = source.ParkingLocationName;
            target.ParkingLocationLatitude = source.ParkingLocationLatitude;
            target.ParkingLocationLongitude = source.ParkingLocationLongitude;
            target.Tag = source.Tag;
        }
    }
}
