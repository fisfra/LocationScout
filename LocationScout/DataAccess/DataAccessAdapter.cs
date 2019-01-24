using LocationScout.Model;
using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LocationScout.DataAccess.PersistenceManager;

namespace LocationScout.DataAccess
{
    internal class DataAccessAdapter
    {
        internal static E_DBReturnCode ReadAllCountries(out List<Country> allCountries, out string errorMessage)
        {
            return PersistenceManager.ReadAllCountries(out allCountries, out errorMessage);
        }

        internal static E_DBReturnCode ReadCountry(long id, out Country foundCountry, out string errorMessage)
        {
            return PersistenceManager.ReadCountry(id, out foundCountry, out errorMessage);
        }

        internal static E_DBReturnCode SmartAddCountry(SettingsDisplayItem displayItem, out string errorMessage)
        {
            var countryName = displayItem.CountryName;
            var areaName = displayItem.AreaName;
            var subAreaName = displayItem.SubAreaName;
            var subjectLocationName = displayItem.SubjectLocationName;
            double? subjectLocationNameLatitude = displayItem.SubjectLocationLatitude;
            double? subjectLocationNameLongitude = displayItem.SubjectLocationLongitude;
            GPSCoordinates subjectLocationCoordinates = new GPSCoordinates(subjectLocationNameLatitude, subjectLocationNameLongitude);

            return PersistenceManager.SmartAddCountry(countryName, areaName, subAreaName, subjectLocationName, subjectLocationCoordinates, out errorMessage);
        }

        internal static E_DBReturnCode AddShootingLocation(List<long> subjectLocationIds, List<long> parkingLocationIds, List<byte[]> photosAsByteArray, string shootingLocationName, out string errorMessage)
        {
            /*
            var locationName = ldi.LocationName;
            var subjectGPS = new GPSCoordinates(ldi.SubjectLatitude, ldi.SubjectLongitude);

            var shooting1ParkingGPS = new GPSCoordinates(ldi.ShootingLocation1_Parking_Latitude, ldi.ShootingLocation1_Parking_Longitude);
            var shooting1_1GPS = new GPSCoordinates(ldi.ShootingLocation1_1_Latitude, ldi.ShootingLocation1_1_Longitude);
            var shooting1_2GPS = new GPSCoordinates(ldi.ShootingLocation1_2_Latitude, ldi.ShootingLocation1_2_Longitude);

            var shooting2ParkingGPS = new GPSCoordinates(ldi.ShootingLocation2_Parking_Latitude, ldi.ShootingLocation2_Parking_Longitude);
            var shooting2_1GPS = new GPSCoordinates(ldi.ShootingLocation2_1_Latitude, ldi.ShootingLocation2_1_Longitude);
            var shooting2_2GPS = new GPSCoordinates(ldi.ShootingLocation2_2_Latitude, ldi.ShootingLocation2_2_Longitude);

            var shooting1_1Photos = ldi.ShootingLocation1_1_Photos.ToList();
            var shooting1_2Photos = ldi.ShootingLocation1_2_Photos.ToList();
            var shooting2_1Photos = ldi.ShootingLocation2_1_Photos.ToList();
            var shooting2_2Photos = ldi.ShootingLocation1_2_Photos.ToList();*/

            return PersistenceManager.AddPhotoPlace(subjectLocationIds, parkingLocationIds, photosAsByteArray, shootingLocationName, out errorMessage);
        }

        internal static E_DBReturnCode EditCountryName(long id, string name, out string errorMessage)
        {
            return PersistenceManager.EditCountryName(id, name, out errorMessage);
        }

        internal static E_DBReturnCode EditAreaName(long id, string name, out string errorMessage)
        {
            return PersistenceManager.EditAreaName(id, name, out errorMessage);
        }

        internal static E_DBReturnCode EditSubAreaName(long id, string name, out string errorMessage)
        {
            return PersistenceManager.EditSubAreaName(id, name, out errorMessage);
        }

        internal static E_DBReturnCode EditSubjectLocationName_GPS(long id, string name, GPSCoordinates coordinates, out string errorMessage)
        {
            return PersistenceManager.EditSubjectLocationName_GPS(id, name, coordinates, out errorMessage);
        }

        internal static E_DBReturnCode ReadShootingLocation(long id, out List<ShootingLocation> shootingLocations, out string errorMessage)
        {
            return PersistenceManager.ReadAllShootingLocations(id, out shootingLocations, out errorMessage);
        }
    }
}
