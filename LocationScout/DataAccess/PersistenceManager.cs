using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;
using LocationScout.ViewModel;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;

namespace LocationScout.DataAccess
{
    internal class PersistenceManager
    {
        #region nested classes
        internal class StoredProcedureParameter
        {
            public StoredProcedureParameter(string name, SqlDbType type, object value)
            {
                Name = name;
                Type = type;
                Value = value;
            }

            public string Name { set; get; }
            public SqlDbType Type { set; get; }
            public object Value { set; get; }
        }
        #endregion

        #region constants
        private const string c_backupStoredProcedureName = "BackupLocationScout";
        private const string c_restoreStoredProcedureName = "RestoreLocationScout";
        private const string c_connectionStringName = "LocationScout";
        private const string c_backupPathParameterName = "@BackupPath";
        private const string c_backupNameParameterName = "@BackupFileName";
        private const string c_backupPathParameterValue = @"C:\Users\fisfra\OneDrive\Documents\SQL-Backup\";
        private const string c_backupNameParameterValue = "LocationScout";
        #endregion

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
                    db.Configuration.LazyLoadingEnabled = false;

                    allCountries = db.Countries.Include(c => c.Areas.Select(a => a.SubAreas))
                                               .Include(c => c.SubAreas.Select(sa => sa.SubjectLocation.Select(sl => sl.ShootingLocations.Select(sh => sh.ParkingLocations))))
                                               .Include(c => c.SubjectLocations.Select(sl => sl.ShootingLocations.Select(sh => sh.ParkingLocations)))
                                               .Include(c => c.SubjectLocations.Select(sl => sl.ShootingLocations.Select(sh => sh.Photos)))
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
                    db.Configuration.LazyLoadingEnabled = false;

                    allAreas = db.Areas
                                 .Include(a => a.Countries)
                                 .Include(a => a.SubAreas)
                                 .Include(a => a.SubjectLocations)
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

        internal static E_DBReturnCode ReadAllSubAreas(out List<SubArea> allSubAreas, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            allSubAreas = new List<SubArea>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    allSubAreas = db.SubAreas.Include(sa => sa.Countries)
                                             .Include(sa => sa.Areas)
                                             .Include(sa => sa.SubjectLocation)
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

        internal static E_DBReturnCode ReadShootingLocationByName(string name, out ShootingLocation foundShootingLocation, out string errorMessage, LocationScoutContext db = null)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;
            foundShootingLocation = null;

            // databased contact that is used
            // might be a new one or and existing one depending on the parameter
            LocationScoutContext dbUsed = null;

            try
            {
                // cannot use "using" here since the context might be needed later
                // take existing context or create new one
                dbUsed = db ?? new LocationScoutContext();

                // query
                var found = dbUsed.ShootingLocations.Where(s => s.Name == name)
                                                    .Include(s => s.ParkingLocations)
                                                    .Include(s => s.SubjectLocations.Select(sl => sl.ShootingLocations.Select(sh => sh.ParkingLocations)))
                                                    .Include(s => s.SubjectLocations.Select(sl => sl.ShootingLocations.Select(sh => sh.Photos)))
                                                    .Include(s => s.Photos)
                                                    .ToList();

                // should be exactly one result or no result (unique name)
                if (found.Count == 1)
                {
                    foundShootingLocation = found[0];
                }

                // more then one result found
                else if (found.Count > 1)
                {
                    // should not find more than one country 
                    throw new Exception("Inconsistent data in database in PersistenceManager:ReadShootingLocationByName.");
                }

                // dispose only if it was newly created
                if (db == null) dbUsed.Dispose();

            }
            catch (Exception e)
            {
                // dispose only if it was newly created
                if (db == null) dbUsed.Dispose();

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
                    allSubjectLocations = db.SubjectLocations.Include(s => s.Country)
                                                             .Include(s => s.SubArea)
                                                             .Include(s => s.Area)
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

        internal static E_DBReturnCode RestoreDatabase(out string errorMessage, string fullPathDatabaseToRestore)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Master"].ConnectionString))
                {
                    // open the connection
                    connection.Open();

                    // check if there are sessions that lock the database (works probably not if there are more than one session)
                    string getSessionIdSQL = "SELECT request_session_id FROM sys.dm_tran_locks WHERE resource_database_id = DB_ID('" + c_connectionStringName + "')";
                    SqlCommand getSessionIdCommand = new SqlCommand(getSessionIdSQL, connection);
                    object o = getSessionIdCommand.ExecuteScalar();

                    // if there are sessions, kill the session
                    if (int.TryParse(o.ToString(), out int sessionId))
                    {
                        string killSessionSQL = "KILL " + sessionId.ToString();
                        SqlCommand killSessionCommand = new SqlCommand(killSessionSQL, connection);
                        killSessionCommand.ExecuteNonQuery();
                    }

                    // use the master database to start the restore
                    string UseMaster = "USE master";
                    SqlCommand UseMasterCommand = new SqlCommand(UseMaster, connection);
                    UseMasterCommand.ExecuteNonQuery();

                    // (not sure what this is for)
                    string Alter1 = @"ALTER DATABASE [" + c_connectionStringName + "] SET Single_User WITH Rollback Immediate";
                    SqlCommand Alter1Cmd = new SqlCommand(Alter1, connection);
                    Alter1Cmd.ExecuteNonQuery();

                    // do the restore
                    string Restore = string.Format("RESTORE DATABASE [" + c_connectionStringName + "] FROM DISK='{0}' WITH REPLACE", fullPathDatabaseToRestore);
                    SqlCommand RestoreCmd = new SqlCommand(Restore, connection);
                    RestoreCmd.ExecuteNonQuery();

                    // // (not sure what this is for)
                    string Alter2 = @"ALTER DATABASE [" + c_connectionStringName + "] SET Multi_User";
                    SqlCommand Alter2Cmd = new SqlCommand(Alter2, connection);
                    Alter2Cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode BackupDatabase(out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                // create a date-time string
                var datetime = DateTime.Today.Year.ToString("D4");
                datetime += "-" + DateTime.Today.Month.ToString("D2");
                datetime += "-" + DateTime.Today.Day.ToString("D2");
                datetime += "-" + DateTime.Now.ToLongTimeString();
                datetime = datetime.Replace(':', '-');

                // build the filename of the backup file
                var filename = c_backupNameParameterValue + "-" + datetime;

                // file and pathname are parameter for the stored procedure
                List<StoredProcedureParameter> parameters = new List<StoredProcedureParameter>()
                {
                    new StoredProcedureParameter(c_backupPathParameterName, SqlDbType.NVarChar, c_backupPathParameterValue),
                    new StoredProcedureParameter(c_backupNameParameterName, SqlDbType.NVarChar, filename),
                };

                // call the stored procedure
                CallStoredProedure(c_backupStoredProcedureName, parameters);
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        private static void CallStoredProedure(string storedProecedureName, List<StoredProcedureParameter> parameters = null)
        {
            // get the connection String
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[c_connectionStringName].ConnectionString;

            // open a new connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // open the database connection
                connection.Open();

                // create the SQL command for the stored procedure
                SqlCommand cmd = new SqlCommand(storedProecedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // add parameters if there are any
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter.Name, parameter.Type).Value = parameter.Value;
                    }
                }

                // execute the stored procedure
                cmd.ExecuteNonQuery();

                // close the database connection
                connection.Close();
            }
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
                    allParkingLocations = db.ParkingLocations.Include(p => p.ShootingLocations)
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
                    var found = db.Countries.Where(c => c.Id == id).Include(c => c.Areas.Select(a => a.SubAreas))
                                                                   .Include(c => c.SubAreas)
                                                                   .Include(c=> c.SubjectLocations)
                                                                   .ToList();

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

        internal static E_DBReturnCode EditShootingLocation(long id, string name, GPSCoordinates coordinates, List<byte[]> photosAsByteArray, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            // only take the three first photo
            if (photosAsByteArray?.Count > 3)
            {
                throw new Exception("Maximum number of photos is exceeded in PersistenceManager::EditShootingLocation");
            }

            try
            {
                using (var db = new LocationScoutContext())
                {
                    // get the shooting location from the database
                    var shootingLocationFromDB = db.ShootingLocations.FirstOrDefault(o => o.Id == id);
                    if (shootingLocationFromDB == null) throw new Exception("Inconsistent database values - Id of ShootingLocation.");

                    // set name and coordinates
                    shootingLocationFromDB.Name = name;
                    shootingLocationFromDB.Coordinates = coordinates;

                    // there are photos
                    if (photosAsByteArray != null)
                    {
                        // there are no photos
                        if (photosAsByteArray?.Count == 0)
                        {
                            // try to delete (do this with a different database context)
                            if (DeleteAllPhotosOfShootingLocation(id, out errorMessage) != E_DBReturnCode.no_error)
                            {
                                throw new Exception(errorMessage);
                            }
                        }

                        else if (photosAsByteArray.Count < shootingLocationFromDB?.Photos?.Count)
                        {
                            for (var i = photosAsByteArray.Count; i < shootingLocationFromDB.Photos.Count; i++)
                            {
                                var photoId = shootingLocationFromDB.Photos.ElementAt(i).Id;
                                DeletePhoto(photoId, out errorMessage);
                            }
                        }

                        // update the photos with the new photos
                        for (var i = 0; i < photosAsByteArray.Count; i++)
                        {
                            shootingLocationFromDB.Photos.Add(new Photo() { ImageBytes = photosAsByteArray[i], ShootingLocation = shootingLocationFromDB });
                        }
                    }

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

        internal static E_DBReturnCode DeletePhoto(long id, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var photoToRemove = db.Photos.FirstOrDefault(p => p.Id == id);
                    if (photoToRemove == null) throw new Exception("Photo not found in PersistenceManager::DeletePhoto - Id:" + id.ToString());

                    db.Photos.Remove(photoToRemove);
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode DeleteAllPhotosOfShootingLocation(long shootingLocationId, out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    // get the shooting location from the database
                    var shootingLocationFromDB = db.ShootingLocations.FirstOrDefault(o => o.Id == shootingLocationId);
                    if (shootingLocationFromDB == null) throw new Exception("Shooting Location not found in PersistenceManager::DeleteAllPhotosOfShootingLocation - Id:" + shootingLocationId.ToString());

                    if (shootingLocationFromDB.Photos != null)
                    {
                        var photoToDelete = shootingLocationFromDB.Photos;
                        db.Photos.RemoveRange(photoToDelete);

                        db.SaveChanges();
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
                    var found = db.Areas.Where(a => a.Id == id).Include(a => a.Countries)
                                                               .Include(a => a.SubAreas)
                                                               .Include(a => a.SubjectLocations)
                                                               .ToList();

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
                    var found = db.SubAreas.Where(s => s.Id == id).Include(s => s.Countries)
                                                                  .Include(s => s.Areas)
                                                                  .Include(s => s.SubjectLocation)
                                                                  .ToList();

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

        private static ShootingLocation GetNewOrExisitingShootingLocation(LocationScoutContext db, string oldName, GPSCoordinates oldGPS, out bool isNewShootingLocation)
        {
            // falg showing if it is a new or old (= from database) shooting location
            isNewShootingLocation = false;

            // read from database by name
            switch (ReadShootingLocationByName(oldName, out ShootingLocation shootingLocation, out string errorMessage, db))
            {
                // no error
                case E_DBReturnCode.no_error:
                    // not found in database
                    if (shootingLocation == null)
                    {
                        // set flag and create a new one
                        isNewShootingLocation = true;
                        shootingLocation = new ShootingLocation() { Name = oldName, Coordinates = oldGPS };
                    }
                    break;

                // error
                case E_DBReturnCode.error:
                    throw new Exception(errorMessage);

                // invalid return code
                default:
                    Debug.Assert(false);
                    throw new Exception("Invalid return code E_DBReturnCode in PersistenceManager::SmartAddPhotoPlace");
            }

            // consistency check - if existing shooting location with same name, GPS should be equal too
            if (!isNewShootingLocation && shootingLocation.Coordinates != oldGPS)
            {
                throw new Exception("Inconsistent data in database - PersistenceManager::GetNewOrExisitingShootingLocation");
            }

            return shootingLocation;
        }

        private static bool HasParkingLocation(ShootingLocation shootingLocation, ParkingLocation parkingLocation)
        {
            if ( (shootingLocation == null) || (parkingLocation == null)|| (shootingLocation?.ParkingLocations == null) )
            {
                return false;
            }

            return shootingLocation.ParkingLocations.FirstOrDefault(p => p.Id == parkingLocation.Id) != null;
        }

        private static bool HasSubjectLocation(ShootingLocation shootingLocation, SubjectLocation subjectLocation)
        {
            if ((shootingLocation == null) || (subjectLocation == null) || (shootingLocation?.SubjectLocations == null))
            {
                return false;
            }

            return shootingLocation.SubjectLocations.FirstOrDefault(s => s.Id == subjectLocation.Id) != null;
        }

        private static bool HasShootingLocation(SubjectLocation subjectLocation, ShootingLocation shootingLocation)
        {
            if ((subjectLocation == null) || (shootingLocation == null) || (subjectLocation?.ShootingLocations == null))
            {
                return false;
            }

            return subjectLocation.ShootingLocations.FirstOrDefault(s => s.Id == shootingLocation.Id) != null;
        }

        private static bool HasShootingLocation(ParkingLocation parkingLocation, ShootingLocation shootingLocation)
        {
            if ((parkingLocation == null) || (shootingLocation == null) || (parkingLocation?.ShootingLocations == null))
            {
                return false;
            }

            return parkingLocation.ShootingLocations.FirstOrDefault(p => p.Id == shootingLocation.Id) != null;
        }

        private static bool PhotoExists(ShootingLocation shootingLocation, byte[] photosAsByteArray)
        {
            bool exists = false;

            if (shootingLocation?.Photos != null)
            {
                foreach (var photo in shootingLocation.Photos)
                {
                    if (photo.ImageBytes.SequenceEqual(photosAsByteArray))
                    {
                        exists = true;
                        break;
                    }
                }
            }

            return exists;
        }

        internal static E_DBReturnCode SmartAddShootingLocation(long subjectLocationId, long parkingLocationId, List<byte[]> photosAsByteArray, string shootingLocationName, 
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

                    // get parking location from database if parkingLocationId != 1 (meaning there was a valid parking location entered in UI)
                    var parkingLocationFromDB = (parkingLocationId != -1) ? db.ParkingLocations.FirstOrDefault(o => o.Id == parkingLocationId) : null;
                    if ( (parkingLocationFromDB == null) && (parkingLocationId != -1))throw new Exception("Invalid Parking Location Id in PersistenceManager::SmartAddPhotoPlace");

                    // check if a shooting location with the same name exists already
                    var shootingLocation = GetNewOrExisitingShootingLocation(db, shootingLocationName, shootingLocationGPS, out bool isNewShootingLocation);

                    // add parking location if necessary
                    if ((parkingLocationId != -1) && (!HasParkingLocation(shootingLocation, parkingLocationFromDB)))
                    {
                        if (shootingLocation.ParkingLocations == null) shootingLocation.ParkingLocations = new List<ParkingLocation>();
                        shootingLocation.ParkingLocations.Add(parkingLocationFromDB);
                    }

                    // add subject location if necessary
                    if (!HasSubjectLocation(shootingLocation, subjectLocationFromDB))
                    {
                        if (shootingLocation.SubjectLocations == null) shootingLocation.SubjectLocations = new List<SubjectLocation>();
                        shootingLocation.SubjectLocations.Add(subjectLocationFromDB);
                    }

                    // add photos to shooting location and set navigation property in photos
                    foreach (var ba in photosAsByteArray)
                    {
                        if (isNewShootingLocation)
                        {
                            if (shootingLocation.Photos == null) shootingLocation.Photos = new List<Photo>();
                            shootingLocation.Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = shootingLocation });
                        }
                        else
                        {
                            if (!PhotoExists(shootingLocation, ba))
                            {
                                shootingLocation.Photos.Add(new Photo() { ImageBytes = ba, ShootingLocation = shootingLocation });
                            }
                        }
                    }

                    // only add if it is a new shooting location
                    if (isNewShootingLocation) db.ShootingLocations.Add(shootingLocation);

                    // add shooting location to subject location
                    if (!HasShootingLocation(subjectLocationFromDB, shootingLocation))
                    {
                        if (subjectLocationFromDB.ShootingLocations == null) subjectLocationFromDB.ShootingLocations = new List<ShootingLocation>();
                        subjectLocationFromDB.ShootingLocations.Add(shootingLocation);
                    }

                    // add shooting location to parking location
                    if ((parkingLocationId != -1) && !HasShootingLocation(parkingLocationFromDB, shootingLocation))
                    {
                        if (parkingLocationFromDB.ShootingLocations == null) parkingLocationFromDB.ShootingLocations = new List<ShootingLocation>();
                        parkingLocationFromDB.ShootingLocations.Add(shootingLocation);
                    }

                    // apply changes check if change was done
                    db.Entry(shootingLocation).State = isNewShootingLocation ? EntityState.Added : EntityState.Modified;
                    success = (db.SaveChanges() == 0) ? E_DBReturnCode.already_existing : E_DBReturnCode.no_error;
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

        internal static E_DBReturnCode SmartAddCountry(string countryName, string areaName, string subAreaName, string subjectLocationName, GPSCoordinates subjectLocationCoordinates, out string errorMessage)
        {
            Debug.Assert(!string.IsNullOrEmpty(countryName));

            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

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
                errorMessage = BuildDBErrorMessages(e);
                success = E_DBReturnCode.error;
            }

            return success;
        }

        internal static E_DBReturnCode Seriales(out string errorMessage)
        {
            E_DBReturnCode success = E_DBReturnCode.no_error;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
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
