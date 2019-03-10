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

        internal static E_DBReturnCode ReadAllAreas(out List<Area> allAreas, out string errorMessage)
        {
            return PersistenceManager.ReadAllAreas(out allAreas, out errorMessage);
        }

        internal static E_DBReturnCode ReadAllSubAreas(out List<SubArea> allSubAreas, out string errorMessage)
        {
            return PersistenceManager.ReadAllSubAreas(out allSubAreas, out errorMessage);
        }

        internal static E_DBReturnCode ReadAllSubjectLocations(out List<SubjectLocation> allSubjectLocation, out string errorMessage)
        {
            return PersistenceManager.ReadAllSubjectLocations(out allSubjectLocation, out errorMessage);
        }

        internal static E_DBReturnCode ReadAllShootingLocations(out List<ShootingLocation> allShootingLocation, out string errorMessage)
        {
            return PersistenceManager.ReadAllShootingLocations(out allShootingLocation, out errorMessage);
        }

        internal static E_DBReturnCode ReadAllParkingLocations(out List<ParkingLocation> allParkingLocation, out string errorMessage)
        {
            return PersistenceManager.ReadAllParkingLocations(out allParkingLocation, out errorMessage);
        }

        internal static E_DBReturnCode ReadCountryById(long id, out Country foundCountry, out string errorMessage)
        {
            return PersistenceManager.ReadCountryById(id, out foundCountry, out errorMessage);
        }

        internal static E_DBReturnCode ReadAreaById(long id, out Area foundArea, out string errorMessage)
        {
            return PersistenceManager.ReadAreaById(id, out foundArea, out errorMessage);
        }

        internal static E_DBReturnCode ReadSubAreaById(long id, out SubArea foundSubArea, out string errorMessage)
        {
            return PersistenceManager.ReadSubAreaById(id, out foundSubArea, out errorMessage);
        }

        internal static E_DBReturnCode ReadShootingLocationByName(string name, out ShootingLocation foundShootingLocation, out string errorMessage)
        {
            return PersistenceManager.ReadShootingLocationByName(name, out foundShootingLocation, out errorMessage);
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

            // some consistency checks
            if (string.IsNullOrEmpty(countryName)) throw new Exception("Country must be set when adding a country (DataAccessAdapter::SmartAddCountry).\n");
            if (!string.IsNullOrEmpty(subAreaName) && string.IsNullOrEmpty(areaName)) throw new Exception("Area name must be set if SubArea name is set (DataAccessAdapter::SmartAddCountry\n");

            return PersistenceManager.SmartAddCountry(countryName, areaName, subAreaName, subjectLocationName, subjectLocationCoordinates, out errorMessage);
        }

        internal static E_DBReturnCode AddParkingLocation(string parkingLocationName, GPSCoordinates parkingLocationCoordinates, out long id, out string errorMessage)
        {
            return PersistenceManager.AddParkingLocation(parkingLocationName, parkingLocationCoordinates, out id, out errorMessage);
        }

        internal static E_DBReturnCode SmartAddShootingLocation(LocationDisplayItem displayItem, long subjectLocationId, long parkingLocationId, out string errorMessage)
        {
            var photosAsByteArray = new List<byte[]>();
            if (displayItem.Photo_1 != null) photosAsByteArray.Add(ImageTools.BitmapImageToByteArray(displayItem.Photo_1));
            if (displayItem.Photo_2 != null) photosAsByteArray.Add(ImageTools.BitmapImageToByteArray(displayItem.Photo_2));
            if (displayItem.Photo_3 != null) photosAsByteArray.Add(ImageTools.BitmapImageToByteArray(displayItem.Photo_3));

            var shootingLocationName = displayItem.ShootingLocationName;
            var shootingLocationGPS = new GPSCoordinates(displayItem.ShootingLocationLatitude, displayItem.ShootingLocationLongitude);

            return PersistenceManager.SmartAddShootingLocation(subjectLocationId, parkingLocationId, photosAsByteArray, shootingLocationName, shootingLocationGPS, out errorMessage);
        }

        internal static E_DBReturnCode DeleteCountryById(long id, out string errorMessage)
        {
            return PersistenceManager.DeleteCountryById(id, out errorMessage);
        }

        internal static E_DBReturnCode DeleteAreaById(long id, out string errorMessage)
        {
            return PersistenceManager.DeleteAreaById(id, out errorMessage);
        }

        internal static E_DBReturnCode DeleteSubAreaById(long id, out string errorMessage)
        {
            return PersistenceManager.DeleteSubAreaById(id, out errorMessage);
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

        internal static E_DBReturnCode ReadAllShootingLocations(long id, out List<ShootingLocation> shootingLocations, out string errorMessage)
        {
            return PersistenceManager.ReadAllShootingLocations(id, out shootingLocations, out errorMessage);
        }

        internal static E_DBReturnCode EditParkingLocationName_GPS(long id, string name, GPSCoordinates coordinates, out string errorMessage)
        {
            return PersistenceManager.EditParkingLocationName_GPS(id, name, coordinates, out errorMessage);
        }

        internal static E_DBReturnCode EditShootingLocation(long id, string name, GPSCoordinates coordinates, List<byte[]> photosAsByteArray, out string errorMessage)
        {
            return PersistenceManager.EditShootingLocation(id, name, coordinates, photosAsByteArray, out errorMessage);
        }

        internal static E_DBReturnCode DeleteShootingLocationById(long id, out string errorMessage)
        {
            return PersistenceManager.DeleteShootingLocationById(id, out errorMessage);
        }

        internal static E_DBReturnCode BackupDatabase(out string errorMessage)
        {
            return PersistenceManager.BackupDatabase(out errorMessage);
        }

        internal static E_DBReturnCode RestoreDatabase(out string errorMessage, string fullPathDatabaseToRestore)
        {
            return PersistenceManager.RestoreDatabase(out errorMessage, fullPathDatabaseToRestore);
        }
    }
}
