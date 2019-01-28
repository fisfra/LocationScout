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

        internal static E_DBReturnCode AddPhotoLocation(List<long> subjectLocationIds, List<long> parkingLocationIds, List<byte[]> photosAsByteArray, string shootingLocationName, out string errorMessage)
        {
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
