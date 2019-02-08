using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

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
                    allCountries = db.Countries.Include(c => c.Areas.Select(a => a.SubAreas))
                                               .Include(c => c.SubAreas.Select(sa => sa.SubjectLocation.Select(sl => sl.ShootLocations.Select(sh => sh.ParkingLocations))))
                                               .Include(c => c.SubjectLocations.Select(sl => sl.ShootLocations.Select(sh => sh.ParkingLocations)))
                                               .ToList();
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

        internal static E_DBReturnCode ReadShootingLocationByName(string name, out ShootingLocation foundShootingLocation, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            foundShootingLocation = new ShootingLocation();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var found = db.ShootingLocations.Where(s => s.Name == name)
                                                    .Include(s => s.ParkingLocations)
                                                    .Include(s => s.SubjectLocations.Select(sl => sl.ShootLocations.Select(sh => sh.ParkingLocations)))
                                                    .Include(s => s.SubjectLocations.Select(sl => sl.ShootLocations.Select(sh => sh.Photos)))
                                                    .Include(s => s.Photos)
                                                    .ToList();

                    if (found.Count == 1)
                    {
                        foundShootingLocation = found[0];
                    }

                    else if (found.Count > 0)
                    {
                        // should not find more than one country for an Id
                        throw new Exception("Inconsistent data in database in PersistenceManager:ReadShootingLocationByName.");
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
                    allShootingLocations = db.ShootingLocations.Include(s => s.ParkingLocations)
                                                               .Include(s => s.Photos)
                                                               .Include(s => s.SubjectLocations.Select(sl => sl.Area))
                                                               .Include(s => s.SubjectLocations.Select(sl => sl.SubArea))
                                                               .Include(s => s.SubjectLocations.Select(sl => sl.Country))
                                                               .ToList();
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

        internal static E_DBReturnCode DeleteShootingLocationById(long id, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var shootingLocationFromDB = db.ShootingLocations.FirstOrDefault(o => o.Id == id);
                    if (shootingLocationFromDB == null) throw new Exception("Inconsistent database values - Id of ShootingLocation.");

                    db.Entry(shootingLocationFromDB).State = EntityState.Deleted;
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
                        throw new Exception("Inconsistent data in database in PersistenceManager:ReadCountry.");
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

        internal static E_DBReturnCode EditShootingLocationName_GPS(long id, string name, GPSCoordinates coordinates, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var shootingLocationFromDB = db.ShootingLocations.FirstOrDefault(o => o.Id == id);
                    if (shootingLocationFromDB == null) throw new Exception("Inconsistent database values - Id of ShootingLocation.");

                    shootingLocationFromDB.Name = name;
                    shootingLocationFromDB.Coordinates = coordinates;

                    db.Entry(shootingLocationFromDB).State = EntityState.Modified;
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

        internal static E_DBReturnCode EditParkingLocationName_GPS(long id, string name, GPSCoordinates coordinates, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var parkingLocationFromDB = db.ParkingLocations.FirstOrDefault(o => o.Id == id);
                    if (parkingLocationFromDB == null) throw new Exception("Inconsistent database values - Id of ParkingLocation.");

                    parkingLocationFromDB.Name = name;
                    parkingLocationFromDB.Coordinates = coordinates;

                    db.Entry(parkingLocationFromDB).State = EntityState.Modified;
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
                    newShootingLocation.ParkingLocations = new List<ParkingLocation>();
                    newShootingLocation.ParkingLocations.Add(parkingLocationFromDB);
                    newShootingLocation.SubjectLocations = new List<SubjectLocation>();
                    newShootingLocation.SubjectLocations.Add(subjectLocationFromDB);

                    // add photos to shooting location and set navigation property in photos
                    foreach (var ba in photosAsByteArray)
                    {
                        if (newShootingLocation.Photos == null) newShootingLocation.Photos = new List<Photo>();
                        newShootingLocation.Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = newShootingLocation });
                    }
                    db.ShootingLocations.Add(newShootingLocation);

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
            Debug.Assert(!string.IsNullOrEmpty(countryName));

            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMesssage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    // first check if country / area / subarea already exist in the DB
                    var countryFromDB = db.Countries.Where(c => c.Name == countryName)
                                                    .Include(c => c.Areas)
                                                    .Include(c => c.SubAreas)
                                                    .Include(c => c.SubjectLocations)
                                                    .ToList()
                                                    .ElementAtOrDefault(0);

                    var areaFromDB = db.Areas.Where(a => a.Name == areaName)
                                       .Include(a => a.SubAreas)
                                       .Include(a => a.Countries)
                                       .Include(a => a.SubjectLocations)
                                       .ToList()
                                       .ElementAtOrDefault(0);

                    var subAreaFromDB = db.SubAreas.Where(s => s.Name == subAreaName)
                                                   .Include(s => s.Areas)
                                                   .Include(s => s.Countries)
                                                   .Include(s => s.SubjectLocation)
                                                   .ToList()
                                                   .ElementAtOrDefault(0);

                    var subjectLocationFromDB = db.SubjectLocations.Where(s => s.Name == subjectLocationName)
                                                                   .Include(s => s.Area)
                                                                   .Include(s => s.SubArea)
                                                                   .Include(s => s.Country)
                                                                   .ToList()
                                                                   .ElementAtOrDefault(0);
                    
                    // country: 
                    // * take object from database or create a new one (countryName must not be null or empty) 
                    // area / subarea / subject location:
                    // * take object from database if there is one
                    // * create a new object if the name was set
                    // * do nothing (object is null) if not in database, but there was no name set in UI
                    //   (this happens if you enter e. g. just a country,but no area)
                    // Futher consistency checks should be done in DatabaseAdapter
                    var country = (countryFromDB == null) ? 
                        new Country() { Name = countryName, Areas = new List<Area>(), SubAreas = new List<SubArea>() } : countryFromDB;

                    var area = ((areaFromDB == null) && (!string.IsNullOrEmpty(areaName))) ?
                        new Area() { Name = areaName, Countries = new List<Country>(), SubAreas = new List<SubArea>() } : areaFromDB;

                    var subArea = ((subAreaFromDB == null) && (!string.IsNullOrEmpty(subAreaName)))? 
                        new SubArea() { Name = subAreaName, Countries = new List<Country>(), Areas = new List<Area>() } : subAreaFromDB;

                    var subjectLocation = ((subjectLocationFromDB == null) && (!string.IsNullOrEmpty(subjectLocationName))) ? 
                        new SubjectLocation() { Name = subjectLocationName, Country = country, Area = area, SubArea = subArea,
                                                Coordinates = subjectLocationCoordinates} : subjectLocationFromDB;

                    bool changesToDB = false;

                    // adjust country
                    if (country != null)
                    {
                        // navigation property area
                        if (area != null)
                        {
                            if (country.Areas == null) country.Areas = new List<Area>();
                            if (!country.Areas.Contains(area)) country.Areas.Add(area);
                        }

                        // navigation property subarea
                        if (subArea != null)
                        {
                            if (country.SubAreas == null) country.SubAreas = new List<SubArea>();
                            if(!country.SubAreas.Contains(subArea)) country.SubAreas.Add(subArea);
                        }

                        // navigation property subject location
                        if (subjectLocation != null)
                        {
                            if (country.SubjectLocations == null) country.SubjectLocations = new List<SubjectLocation>();
                            if (!country.SubjectLocations.Contains(subjectLocation)) country.SubjectLocations.Add(subjectLocation);
                        }

                        // add to database if new country or navigation property added
                        if ((area != null) || (subArea != null) || (countryFromDB == null))
                        {
                            if (countryFromDB != null)
                            {
                                db.Entry(country).State = EntityState.Modified;
                            }
                            else
                            {
                                db.Countries.Add(country);
                            }

                            changesToDB = true;
                        }
                    }

                    // adjust area
                    if (area != null) 
                    {
                        // navigaton property subarea
                        if (subArea != null)
                        {
                            if (area.SubAreas == null) area.SubAreas = new List<SubArea>();
                            if (!area.SubAreas.Contains(subArea)) area.SubAreas.Add(subArea);
                        }

                        // navigation property country
                        if (country != null)
                        {
                            if (area.Countries == null) area.Countries = new List<Country>();
                            if (!area.Countries.Contains(country)) area.Countries.Add(country);
                        }

                        // add to database if new area or navigation property added
                        if ((subArea != null) || (country != null) || (areaFromDB == null))
                        {
                            if (areaFromDB != null)
                            {
                                db.Entry(area).State = EntityState.Modified;
                            }
                            else
                            {
                                db.Areas.Add(area);
                            }

                            changesToDB = true;
                        }
                    }

                    // adjust subarea
                    if (subArea != null)
                    {
                        // navigaton property area
                        if (area != null)
                        {
                            if (subArea.Areas == null) subArea.Areas = new List<Area>();
                            if (!subArea.Areas.Contains(area)) subArea.Areas.Add(area);
                        }

                        // navigation property country
                        if (country != null)
                        {
                            if (subArea.Countries == null) subArea.Countries = new List<Country>();
                            if (!subArea.Countries.Contains(country)) subArea.Countries.Add(country);
                        }

                        // add to database if subarea or navigation property added
                        if ((area != null) || (country != null) || (subAreaFromDB == null))
                        {
                            if (subAreaFromDB != null)
                            {
                                db.Entry(subArea).State = EntityState.Modified;
                            }
                            else
                            {
                                db.SubAreas.Add(subArea);
                            }

                            changesToDB = true;
                        }
                    }

                    // subject location
                    if (subjectLocation != null)
                    {
                        // navigaton property area
                        if (area != null)
                        {
                            if (subjectLocation.Area != area) subjectLocation.Area = area;
                        }

                        // navigaton property subarea
                        if (subArea != null)
                        {
                            if (subjectLocation.SubArea != area) subjectLocation.SubArea = subArea;
                        }

                        // navigaton property country
                        if (country != null)
                        {
                            if (subjectLocation.Country != country) subjectLocation.Country = country;
                        }

                        if (subjectLocationFromDB != null)
                        {
                            db.Entry(subjectLocation).State = EntityState.Modified;
                        }
                        else
                        {
                            db.SubjectLocations.Add(subjectLocation);
                        }

                        changesToDB = true;
                    }


                    // if at least one changes was applied...
                    if (changesToDB)
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
