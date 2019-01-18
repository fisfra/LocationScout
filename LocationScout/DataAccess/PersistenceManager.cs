using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace LocationScout.DataAccess
{
    internal class PersistenceManager
    {
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

        internal static bool ReadAllCountries(out List<Country> allCountries, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;
            allCountries = new List<Country>();

            try
            {
                using (var db = new LocationScoutContext())
                {
                    allCountries = db.Countries.Include(c => c.Areas.Select(a => a.Subareas)).Include(c => c.SubAreas).ToList();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = false;
            }

            return success;
        }

        internal static void ReadAllArea_Subareas(out List<Tuple<string, string>> areaNames_subareaNames)
        {
            TestDataGenerator.ReadAllArea_Subareas(out areaNames_subareaNames);
        }

        internal static void ReadAll_Country_Areas(out List<Tuple<string, string>> countryNames_areaNames)
        {
            TestDataGenerator.ReadAll_Country_Areas(out countryNames_areaNames);
        }

        internal static bool SmartAddCountry(string countryName, string areaName, string subAreaName, out string errorMesssage)
        {
            bool success = true;
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
                    var area = (areaFromDB == null) ? new Area() { Name = areaName, Countries = new List<Country>(), Subareas = new List<SubArea>() } : areaFromDB;
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
                        area.Subareas.Add(subArea);
                        db.Areas.Add(area);
                    }

                    // add relations, if new subarea
                    if (subAreaFromDB == null)
                    {
                        subArea.Areas.Add(area);
                        subArea.Countries.Add(country);
                        db.SubAreas.Add(subArea);
                    }

                    // save the changes to the DB
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                errorMesssage = BuildDBErrorMessages(e);
                success = false;
            }

            return success;
        }

        internal static bool GetArea(string areaName, out Area areaFromDB, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;
            areaFromDB = null;

            try
            {
                // ensure to all resources of the datacontext are disposed correctly
                using (var db = new LocationScoutContext())
                {
                    // get the existing area from database
                    areaFromDB = db.Areas.FirstOrDefault(o => o.Name == areaName);
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = false;
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
