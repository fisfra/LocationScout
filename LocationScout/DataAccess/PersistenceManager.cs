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
                    allCountries = db.Countries.Include(c => c.Areas.Select(a => a.Subareas)).ToList();
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

        internal static bool AddCountry(Country newCountry, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    db.Countries.Add(newCountry);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = false;
            }

            return success;
        }

        internal static bool AddArea(Area newArea, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    db.Areas.Add(newArea);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = false;
            }

            return success;
        }

        internal static bool AddSubAreaToArea(Area updatedArea, SubArea newSubArea, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;

            try
            {
                // ensure to all resources of the datacontext are disposed correctly
                using (var db = new LocationScoutContext())
                {
                    // get existing area from database
                    Area existingArea = db.Areas.FirstOrDefault(o => o.Name == updatedArea.Name);

                    // 
                    if (newSubArea.Areas == null) newSubArea.Areas = new List<Area>();
                    newSubArea.Areas.Add(existingArea);

                    //                 
                    db.SubAreas.Add(newSubArea);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = false;
            }

            return success;
        }

        internal static bool AddAreaToCountry(Country updatedCountry, Area newArea, SubArea newSubArea, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;

            try
            {
                // ensure to all resources of the datacontext are disposed correctly
                using (var db = new LocationScoutContext())
                {
                    // get the existing country from database
                    Country existingCountry = db.Countries.FirstOrDefault(o => o.Name == updatedCountry.Name);

                    // add the existing country to the new area
                    if (newArea.Countries == null) newArea.Countries = new List<Country>();
                    newArea.Countries.Add(existingCountry);

                    // add the new subarea to the new area
                    if (newArea.Subareas == null) newArea.Subareas = new List<SubArea>();
                    newArea.Subareas.Add(newSubArea);

                    // add the new area to the new subarea
                    if (newSubArea.Areas == null) newSubArea.Areas = new List<Area>();
                    newSubArea.Areas.Add(newArea);

                    // now add the new area and subarea to the database
                    db.SubAreas.Add(newSubArea);
                    db.Areas.Add(newArea);

                    // save the changes to the database
                    db.SaveChanges();
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
