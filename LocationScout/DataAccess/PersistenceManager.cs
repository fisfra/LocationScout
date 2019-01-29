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

        internal static E_DBReturnCode ReadAllAreas(out List<Area> allAreas, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            allAreas = new List<Area>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    allAreas = db.Areas.Include(a => a.Countries).Include(a => a.SubAreas).Include(a => a.SubjectLocations).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode ReadAllSubAreas(out List<SubArea> allSubAreas, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            allSubAreas = new List<SubArea>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    allSubAreas = db.SubAreas.Include(sa => sa.Countries).Include(sa => sa.Areas).Include(sa => sa.SubjectLocation).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode ReadAllSubjectLocations(out List<SubjectLocation> allSubjectLocations, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            allSubjectLocations = new List<SubjectLocation>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    allSubjectLocations = db.SubjectLocations.Include(s => s.Country).Include(s => s.SubArea).Include(s => s.Area).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode ReadAllShootingLocations(out List<ShootingLocation> allShootingLocations, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            allShootingLocations = new List<ShootingLocation>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    allShootingLocations = db.ShootingLocations.Include(s => s.ParkingLocations).Include(s => s.Photos).Include(s => s.SubjectLocations).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode ReadAllParkingLocations(out List<ParkingLocation> allParkingLocations, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            allParkingLocations = new List<ParkingLocation>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    allParkingLocations = db.ParkingLocations.Include(p => p.ShootingLocations).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode ReadCountryById(long id, out Country foundCountry, out string errorMessage)
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

        internal static E_DBReturnCode ReadAreaById(long id, out Area foundArea, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            foundArea = new Area();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var found = db.Areas.Where(a => a.Id == id).Include(a => a.Countries).Include(a => a.SubAreas).Include(a => a.SubjectLocations).ToList();

                    if (found.Count == 1)
                    {
                        foundArea = found[0];
                    }

                    else if (found.Count > 0)
                    {
                        // should not find more than one country for an Id
                        throw new Exception("Inconsistent data in database in PersistenceManager:ReadArea.");
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

        internal static E_DBReturnCode ReadSubAreaById(long id, out SubArea foundSubArea, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            foundSubArea = new SubArea();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var found = db.SubAreas.Where(s => s.Id == id).Include(s => s.Countries).Include(s => s.Areas).Include(s => s.SubjectLocation).ToList();

                    if (found.Count == 1)
                    {
                        foundSubArea = found[0];
                    }

                    else if (found.Count > 0)
                    {
                        // should not find more than one country for an Id
                        throw new Exception("Inconsistent data in database in PersistenceManager:ReadSubArea.");
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

        internal static E_DBReturnCode SmartAddPhotoPlace(long subjectLocationId, long parkingLocationId, List<byte[]> photosAsByteArray, string shootingLocationName, 
                                                          GPSCoordinates shootingLocationGPS, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            
            try
            {
                using (var db = new LocationScoutContext())
                {
                    // get subject location from database
                    var subjectLocationFromDB = db.SubjectLocations.FirstOrDefault(o => o.Id == subjectLocationId);
                    if (subjectLocationFromDB == null) throw new Exception("Invalid Subject Location Id in PersistenceManager::SmartAddPhotoPlace");

                    // get parking location from database
                    var parkingLocationFromDB = db.ParkingLocations.FirstOrDefault(o => o.Id == parkingLocationId);
                    if (parkingLocationFromDB == null) throw new Exception("Invalid Parking Location Id in PersistenceManager::SmartAddPhotoPlace");

                    // create the new shooting location
                    ShootingLocation newShootingLocation = new ShootingLocation() { Name = shootingLocationName, Coordinates = shootingLocationGPS };

                    // add photos to shooting location and set navigation property in photos
                    foreach (var ba in photosAsByteArray)
                    {
                        if (newShootingLocation.Photos == null) newShootingLocation.Photos = new List<Photo>();
                        newShootingLocation.Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = newShootingLocation });
                    }
               
                    // add shooting location to subject location
                    if (subjectLocationFromDB.ShootLocations == null) subjectLocationFromDB.ShootLocations = new List<ShootingLocation>();
                    subjectLocationFromDB.ShootLocations.Add(newShootingLocation);

                    // add shootling location to parking location
                    if (parkingLocationFromDB.ShootingLocations == null) parkingLocationFromDB.ShootingLocations = new List<ShootingLocation>();
                    parkingLocationFromDB.ShootingLocations.Add(newShootingLocation);

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

        internal static E_DBReturnCode AddParkingLocation(string parkingLocationName, GPSCoordinates parkingLocationCoordinates, out long id, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            id = -1;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var newParkingLocation = new ParkingLocation() { Name = parkingLocationName, Coordinates = parkingLocationCoordinates };
                    db.ParkingLocations.Add(newParkingLocation);

                    db.SaveChanges();

                    id = newParkingLocation.Id;
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode EditSubAreaName(long id, string name, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var subAreaFromDB = db.SubAreas.FirstOrDefault(o => o.Id == id);
                    if (subAreaFromDB == null) throw new Exception("Inconsistent database values - Id of subAreaId.");

                    subAreaFromDB.Name = name;

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

        internal static E_DBReturnCode EditAreaName(long id, string name, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var AreaFromDB = db.Areas.FirstOrDefault(o => o.Id == id);
                    if (AreaFromDB == null) throw new Exception("Inconsistent database values - Id of areaId.");

                    AreaFromDB.Name = name;

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

        internal static E_DBReturnCode EditCountryName(long id, string name, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var countryFromDB = db.Countries.FirstOrDefault(o => o.Id == id);
                    if (countryFromDB == null) throw new Exception("Inconsistent database values - Id of Country.");

                    countryFromDB.Name = name;

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

        internal static E_DBReturnCode EditSubjectLocationName_GPS(long id, string name, GPSCoordinates coordinates, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var subjectLocationFromDB = db.SubjectLocations.FirstOrDefault(o => o.Id == id);
                    if (subjectLocationFromDB == null) throw new Exception("Inconsistent database values - Id of SubjectLocation.");

                    subjectLocationFromDB.Name = name;
                    subjectLocationFromDB.Coordinates = coordinates;

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

        internal static E_DBReturnCode ReadAllShootingLocations(long id, out List<ShootingLocation> shootingLocationsFound, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            shootingLocationsFound = null;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
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
