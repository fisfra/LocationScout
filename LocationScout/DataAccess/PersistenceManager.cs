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

        internal static bool AddSubArea(SubArea newSubArea, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
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


        internal static bool UpdateCountry(Country updatedCountry, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var country = db.Countries.SingleOrDefault(o => o.Name == updatedCountry.Name);
                    country.Areas = updatedCountry.Areas;
                    db.Entry(country).State = EntityState.Modified;

                    var result = db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                errorMessage = BuildDBErrorMessages(e);
                success = false;
            }

            return success;
        }

        internal static bool AddAreaCountry(Country updatedCountry, Area newArea, out string errorMessage)
        {
            bool success = true;
            errorMessage = string.Empty;

            try
            {
                using (var db = new LocationScoutContext())
                {
                    var country = db.Countries.SingleOrDefault(o => o.Name == updatedCountry.Name);
                    country.Areas.Add(newArea);
                    db.Entry(country).State = EntityState.Modified;

                    var result = db.SaveChanges();
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
