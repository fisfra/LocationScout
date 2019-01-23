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
        internal static E_DBReturnCode ReadAllCountries(out List<Country> allCountries, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            allCountries = new List<Country>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    allCountries = db.Countries.Include(c => c.Areas.Select(a => a.SubAreas)).Include(c => c.SubAreas).Include(c => c.SubjectLocations).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode ReadCountry(long id, out Country foundCountry, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            foundCountry = new Country();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var found = db.Countries.Where(c => c.Id == id).Include(c => c.Areas.Select(a => a.SubAreas)).Include(c => c.SubAreas).Include(c=> c.SubjectLocations).ToList();

                    if (found.Count == 1)
                    {
                        foundCountry = found[0];
                    }

                    else if (found.Count > 0)
                    {
                        // should not find more than one country for an Id
                        throw new Exception("Inconsitent data in database in PersistenceManager:ReadCountry.");
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode AddPhotoPlace(List<long> subjectLocationIds, List<long> parkingLocationIds, List<byte[]> photosAsByteArray, string shootingLocationName, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            
            try
            {
                using (var db = new LocationScoutContext())
                {
                    // get subject locations from database
                    List<SubjectLocation> subjectLocationsFromDB = new List<SubjectLocation>();
                    foreach (var id in subjectLocationIds)
                    {
                        subjectLocationsFromDB.Add(db.SubjectLocations.FirstOrDefault(o => o.Id == id));
                     }

                    // get parking locations from database
                    List<ParkingLocation> parkingLocationsFromDB = new List<ParkingLocation>();
                    foreach (var id in parkingLocationIds)
                    {
                        parkingLocationsFromDB.Add(db.ParkingLocations.FirstOrDefault(o => o.Id == id));
                    }

                    // create the new shooting location
                    ShootingLocation newShootingLocation = new ShootingLocation()
                    {
                        Name = shootingLocationName
                    };

                    // add photos to shooting location and set navigation property in photos
                    foreach (var ba in photosAsByteArray)
                    {
                        newShootingLocation.Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = newShootingLocation });
                    }

                    // set navigation properties for subject location
                    foreach (var subjectLocationFromDB in subjectLocationsFromDB)
                    {
                        subjectLocationFromDB.ShootLocations.Add(newShootingLocation);
                    }

                    // set navgation properties for parking location
                    foreach (var parkingLocationFromDB in parkingLocationsFromDB)
                    {
                        parkingLocationFromDB.ShootingLocations.Add(newShootingLocation);
                    }

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

        internal static E_DBReturnCode EditSubjectLocationName(long subjectLocationId, string newSubjectLocationName, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var subjectLocationFromDB = db.Countries.FirstOrDefault(o => o.Id == subjectLocationId);
                    if (subjectLocationFromDB == null) throw new Exception("Inconsistent database values - Id of SubjectLocation.");

                    subjectLocationFromDB.Name = newSubjectLocationName;

                    db.Entry(subjectLocationFromDB).State = EntityState.Modified;
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

        internal static E_DBReturnCode ReadAllShootingLocations(long shootingLocationId, out List<ShootingLocation> shootingLocationsFound, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            shootingLocationsFound = null;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    /*
                    photoPlacesFound = db.PhotoPlaces.Include(pp => pp.PlaceSubjectLocation)
                                    .Include(pp => pp.ParkingLocations.Select(pl => pl.ShootingLocations.Select(sl => sl.Photos)))
                                    .Include(pp => pp.PlaceSubjectLocation.SubjectCountry)
                                    .Include(pp => pp.PlaceSubjectLocation.SubjectArea)
                                    .Include(pp => pp.PlaceSubjectLocation.SubjectSubArea).ToList();*/


                    // get the table data including all joint tables
                    shootingLocationsFound = db.ShootingLocations.Include(sl => sl.Photos)
                                                                 .Include(sl => sl.SubjectLocations)
                                                                 .Include(sl => sl.ParkingLocations).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode SmartAddCountry(string countryName, string areaName, string subAreaName, string subjectLocationName, GPSCoordinates subjectLocationCoordinates, out string errorMesssage)
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
                    var subjectLocationFromDB = db.SubjectLocations.FirstOrDefault(o => o.Name == subjectLocationName);

                    // create new objects or take the once from DB
                    var country = (countryFromDB == null) ? new Country() { Name = countryName, Areas = new List<Area>(), SubAreas = new List<SubArea>() } : countryFromDB;
                    var area = (areaFromDB == null) ? new Area() { Name = areaName, Countries = new List<Country>(), SubAreas = new List<SubArea>() } : areaFromDB;
                    var subArea = (subAreaFromDB == null) ? new SubArea() { Name = subAreaName, Countries = new List<Country>(), Areas = new List<Area>() } : subAreaFromDB;
                    var subjectLocation = (subjectLocationFromDB == null) ? new SubjectLocation() { Name = subjectLocationName, Country = country, Area = area, SubArea = subArea,
                                                                                                    Coordinates = subjectLocationCoordinates} : subjectLocationFromDB;

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

                    // add relation, if new subject location
                    if (subjectLocationFromDB == null)
                    {
                        db.SubjectLocations.Add(subjectLocation);
                    }

                    // if at least one is new...
                    if ( (countryFromDB == null) || (areaFromDB == null) || (subAreaFromDB == null) || (subjectLocationFromDB == null) )
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
