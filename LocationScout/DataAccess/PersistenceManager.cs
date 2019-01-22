using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using LocationScout.ViewModel;

namespace LocationScout.DataAccess
{
    internal class PersistenceManager
    {
        #region enums
        public enum E_DBReturnCode { no_error, error, already_existing};
        #endregion

        #region attributes
        #endregion

        #region constructores
        public PersistenceManager()
        {
        }
        #endregion

        #region methods
        internal static void ReadAllSubareas(out List<string> subareaNames)
        {
            TestDataGenerator.ReadAllSubareas(out subareaNames);
        }

        internal static void ReadAllAreas(out List<string> areaNames)
        {
            TestDataGenerator.ReadAllAreas(out areaNames);
        }

        internal static E_DBReturnCode ReadAllCountries(out List<Country> allCountries, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            allCountries = new List<Country>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    allCountries = db.Countries.Include(c => c.Areas.Select(a => a.SubAreas)).Include(c => c.SubAreas).ToList();
                    //allCountries = db.Countries.Including(c => c.Areas).ThenIncluding(c => c.SubAreas).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode SmartAddPhotoPlace(long countryId, long areaId, long subAreaId, string locationName, GPSCoordinates subjectGPS,
                                                          GPSCoordinates shooting1ParkingGPS, GPSCoordinates shooting1_1GPS, List<byte[]> shooting1_1Photos,
                                                          GPSCoordinates shooting1_2GPS, List<byte[]> shooting1_2Photos, GPSCoordinates shooting2ParkingGPS, 
                                                          GPSCoordinates shooting2_1GPS, List<byte[]> shooting2_1Photos, GPSCoordinates shooting2_2GPS,
                                                          List<byte[]> shooting2_2Photos, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var countryFromDB = db.Countries.FirstOrDefault(o => o.Id == countryId);
                    if (countryFromDB == null) throw new Exception("Inconsistent database values - Id of Country.");

                    var areaFromDB = db.Areas.FirstOrDefault(o => o.Id == areaId);
                    if (areaFromDB == null) throw new Exception("Inconsistent database values - Id of Area.");

                    var subAreaFromDB = db.SubAreas.FirstOrDefault(o => o.Id == subAreaId);
                    if (subAreaFromDB == null) throw new Exception("Inconsistent database values - Id of SubArea.");

                    // set parking locations later
                    var subjectLocation = new SubjectLocation() { SubjectCountry = countryFromDB,
                                                                  SubjectArea = areaFromDB,
                                                                  SubjectSubArea = subAreaFromDB,
                                                                  Coordinates = subjectGPS,
                                                                  LocationName = locationName };

                    // set photoplace and shootlinglocation later
                    var parkingLocation1 = new ParkingLocation() { Coordinates = shooting1ParkingGPS };
                    var parkingLocation2 = new ParkingLocation() { Coordinates = shooting2ParkingGPS };

                    // set photoplace and locationphotos later
                    var shootingLocation1_1 = new ShootingLocation() { Coordinates = shooting1_1GPS };
                    var shootingLocation1_2 = new ShootingLocation() { Coordinates = shooting1_2GPS };
                    var shootingLocation2_1 = new ShootingLocation() { Coordinates = shooting2_1GPS };
                    var shootingLocation2_2 = new ShootingLocation() { Coordinates = shooting2_2GPS };

                    // photos
                    var shootingLocation1_1_Photos = new List<Photo>();
                    foreach (var ba in shooting1_1Photos)
                    {
                        shootingLocation1_1_Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = shootingLocation1_1 });
                    }
                    var shootingLocation1_2_Photos = new List<Photo>();
                    foreach (var ba in shooting1_2Photos)
                    {
                        shootingLocation1_2_Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = shootingLocation1_2 });
                    }
                    var shootingLocation2_1_Photos = new List<Photo>();
                    foreach (var ba in shooting2_1Photos)
                    {
                        shootingLocation2_1_Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = shootingLocation2_1 });
                    }
                    var shootingLocation2_2_Photos = new List<Photo>();
                    foreach (var ba in shooting2_2Photos)
                    {
                        shootingLocation2_2_Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = shootingLocation2_2 });
                    }

                    // set dependant attributes
                    shootingLocation1_1.Photos = shootingLocation1_1_Photos;
                    shootingLocation1_2.Photos = shootingLocation1_2_Photos;
                    shootingLocation2_1.Photos = shootingLocation2_1_Photos;
                    shootingLocation2_2.Photos = shootingLocation2_2_Photos;
                    subjectLocation.ParkingLocations = new List<ParkingLocation>() { parkingLocation1, parkingLocation2 };
                    parkingLocation1.ShootingLocations = new List<ShootingLocation>() { shootingLocation1_1, shootingLocation1_2 };
                    parkingLocation2.ShootingLocations = new List<ShootingLocation>() { shootingLocation2_1, shootingLocation2_2 };

                    // photoplace
                    var photoplace = new PhotoPlace() { PlaceSubjectLocation = subjectLocation, ParkingLocations = new List<ParkingLocation>() { parkingLocation1, parkingLocation2 } };

                    // set depedant attributes
                    parkingLocation1.PhotoPlace = photoplace;
                    parkingLocation2.PhotoPlace = photoplace;

                    // set the database attributes     
                    foreach (var photo in shootingLocation1_1_Photos)
                    {
                        db.Photos.Add(photo);
                    }
                    foreach (var photo in shootingLocation1_2_Photos)
                    {
                        db.Photos.Add(photo);
                    }
                    foreach (var photo in shootingLocation2_1_Photos)
                    {
                        db.Photos.Add(photo);
                    }
                    foreach (var photo in shootingLocation2_2_Photos)
                    {
                        db.Photos.Add(photo);
                    }
                    db.PhotoPlaces.Add(photoplace);
                    db.ParkingLocations.Add(parkingLocation1);
                    db.ParkingLocations.Add(parkingLocation2);
                    db.ShootingLocations.Add(shootingLocation1_1);
                    db.ShootingLocations.Add(shootingLocation1_2);
                    db.ShootingLocations.Add(shootingLocation2_1);
                    db.ShootingLocations.Add(shootingLocation2_2);
                    db.SubjectLocations.Add(subjectLocation);
                    
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode EditSubAreaName(long subAreaId, string newSubAreaName, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var subAreaFromDB = db.SubAreas.FirstOrDefault(o => o.Id == subAreaId);
                    if (subAreaFromDB == null) throw new Exception("Inconsistent database values - Id of subAreaId.");

                    subAreaFromDB.Name = newSubAreaName;

                    db.Entry(subAreaFromDB).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode EditAreaName(long areaId, string newAreaName, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var AreaFromDB = db.Areas.FirstOrDefault(o => o.Id == areaId);
                    if (AreaFromDB == null) throw new Exception("Inconsistent database values - Id of areaId.");

                    AreaFromDB.Name = newAreaName;

                    db.Entry(AreaFromDB).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode EditCountryName(long countryId, string newCountryName, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var countryFromDB = db.Countries.FirstOrDefault(o => o.Id == countryId);
                    if (countryFromDB == null) throw new Exception("Inconsistent database values - Id of Country.");

                    countryFromDB.Name = newCountryName;

                    db.Entry(countryFromDB).State = EntityState.Modified;
                    db.SaveChanges();
                }            
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode ReadAllPhotoPlaces(long photoPlaceId, out List<PhotoPlace> photoPlacesFound, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            photoPlacesFound = null;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    // get the table data including all joint tables
                    photoPlacesFound = db.PhotoPlaces.Include(pp => pp.PlaceSubjectLocation)
                                                        .Include(pp => pp.ParkingLocations.Select(pl => pl.ShootingLocations.Select(sl => sl.Photos)))
                                                        .Include(pp => pp.PlaceSubjectLocation.SubjectCountry)
                                                        .Include(pp => pp.PlaceSubjectLocation.SubjectArea)
                                                        .Include(pp => pp.PlaceSubjectLocation.SubjectSubArea).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }


        internal static E_DBReturnCode SmartAddCountry(string countryName, string areaName, string subAreaName, out string errorMesssage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMesssage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    // first check if country / area / subarea already exist in the DB
                    var countryFromDB = db.Countries.FirstOrDefault(o => o.Name == countryName);
                    var areaFromDB = db.Areas.FirstOrDefault(o => o.Name == areaName);
                    var subAreaFromDB = db.SubAreas.FirstOrDefault(o => o.Name == subAreaName);

                    // create new objects or take the once from DB
                    var country = (countryFromDB == null) ? new Country() { Name = countryName, Areas = new List<Area>(), SubAreas = new List<SubArea>() } : countryFromDB;
                    var area = (areaFromDB == null) ? new Area() { Name = areaName, Countries = new List<Country>(), SubAreas = new List<SubArea>() } : areaFromDB;
                    var subArea = (subAreaFromDB == null) ? new SubArea() { Name = subAreaName, Countries = new List<Country>(), Areas = new List<Area>() } : subAreaFromDB;

                    // add relations, if new country
                    if (countryFromDB == null)
                    {
                        country.Areas.Add(area);
                        country.SubAreas.Add(subArea);
                        db.Countries.Add(country);
                    }

                    // add relations, if new area
                    if (areaFromDB == null)
                    {
                        area.Countries.Add(country);
                        area.SubAreas.Add(subArea);
                        db.Areas.Add(area);
                    }

                    // add relations, if new subarea
                    if (subAreaFromDB == null)
                    {
                        subArea.Areas.Add(area);
                        subArea.Countries.Add(country);
                        db.SubAreas.Add(subArea);
                    }


                    // if at least one is new...
                    if ( (countryFromDB == null) || (areaFromDB == null) || (subAreaFromDB == null) )
                    {
                        // ... save the changes to the DB
                        db.SaveChanges();
                    }
                    // all existing
                    else
                    {
                        success = E_DBReturnCode.already_existing;
                    }
                }
            }
            catch (Exception e)
            {
                errorMesssage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }


        internal static string BuildDBErrorMessages(Exception e)
        {
            var errorMessage = e.Message;
            errorMessage += (e.InnerException != null) ? ("\n" + e.InnerException.Message) : string.Empty;
            errorMessage += (e.InnerException != null) && (e.InnerException?.InnerException != null) ? ("\n" + e.InnerException.InnerException.Message) : string.Empty;

            return errorMessage;
        }
        #endregion
    }
}
