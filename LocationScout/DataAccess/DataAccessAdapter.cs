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
        internal static bool ReadAllCountries(out List<Country> allCountries, out string errorMessage)
        {
            return PersistenceManager.ReadAllCountries(out allCountries, out errorMessage);
        }

        internal static bool AddCountry(Country countryToAdd, out string errorMessage)
        {
            return PersistenceManager.AddCountry(countryToAdd, out errorMessage);
        }

        internal static bool AddAreaToCountry(Country updatedCountry, Area newArea, SubArea newSubArea, out string errorMessage)
        {
            return PersistenceManager.AddAreaToCountry(updatedCountry, newArea, newSubArea, out errorMessage);
        }

        internal static bool AddSubAreaToArea(Area updatedArea, SubArea newSubArea, out string errorMessage)
        {
            return PersistenceManager.AddSubAreaToArea(updatedArea, newSubArea, out errorMessage);
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
