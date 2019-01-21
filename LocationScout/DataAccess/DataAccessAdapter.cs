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

        internal static E_DBReturnCode SmartAddCountry(string countryName, string areaName, string subAreaName, out string errorMessage)
        {
            return PersistenceManager.SmartAddCountry(countryName, areaName, subAreaName, out errorMessage);
        }

        internal static E_DBReturnCode SmartAddPhotoPlace(long countryId, long areaId, long subAreaId, LocationDisplayItem ldi, out string errorMessage)
        {
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
            var shooting2_2Photos = ldi.ShootingLocation1_2_Photos.ToList();

            return PersistenceManager.SmartAddPhotoPlace(countryId, areaId, subAreaId, locationName, subjectGPS,
                                                        shooting1ParkingGPS, shooting1_1GPS, shooting1_1Photos, shooting1_2GPS, shooting1_2Photos,
                                                        shooting2ParkingGPS, shooting2_1GPS, shooting2_1Photos, shooting2_2GPS, shooting2_2Photos, out errorMessage);
        }

        internal static E_DBReturnCode EditCountryName(long countryId, string newCountryName, out string errorMessage)
        {
            return PersistenceManager.EditCountryName(countryId, newCountryName, out errorMessage);
        }

        internal static E_DBReturnCode ReadPhotoPlace(long photoPlaceId, out List<PhotoPlace> photoPlaces, out string errorMessage)
        {
            return PersistenceManager.ReadAllPhotoPlaces(photoPlaceId, out photoPlaces, out errorMessage);
        }
    }
}
