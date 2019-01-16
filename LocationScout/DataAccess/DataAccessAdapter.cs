using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.DataAccess
{
    internal class DataAccessAdapter
    {
        /*
        internal static List<Area> ReadAllAreas()
        {
            
            return PersistenceManager.ReadAllAreas();
        }*/

        internal static bool ReadAllCountries(out List<Country> allCountries, out string errorMessage)
        {
            return PersistenceManager.ReadAllCountries(out allCountries, out errorMessage);
        }

        internal static bool AddCountry(Country countryToAdd, out string errorMessage)
        {
            return PersistenceManager.AddCountry(countryToAdd, out errorMessage);
        }

        internal static bool UpdateCountry(Country countryToUpdate, out string errorMessage)
        {
            return PersistenceManager.UpdateCountry(countryToUpdate, out errorMessage);
        }

        internal static bool AddAreaToCountry(Country updatedCountry, Area newArea, out string errorMessage)
        {
            if (!PersistenceManager.AddArea(newArea, out errorMessage))
            {
                return false;
            }

            return PersistenceManager.AddAreaCountry(updatedCountry, newArea, out errorMessage);
        }

        internal static bool AddSubAreaToArea(Area updatedArea, SubArea newSubArea, out string errorMessage)
        {
            if (!PersistenceManager.AddSubArea(newSubArea, out errorMessage))
            {
                return false;
            }

            return true;
            // to do
            //return PersistenceManager.AddAreaCountry(updatedCountry, newArea, out errorMessage);
        }


        internal static bool AddArea(Area newArea, out string errorMessage)
        {
            return PersistenceManager.AddArea(newArea, out errorMessage);
        }

        private static List<Area> GetAreasByCountryName(string countryName, List<Area> allAreas, List<SubArea> subAreas)
        {
            return null;
        }
    }
}
