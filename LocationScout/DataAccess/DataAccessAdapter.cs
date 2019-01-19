using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LocationScout.DataAccess.PersistenceManager;

namespace LocationScout.DataAccess
{
    internal class DataAccessAdapter
    {
        internal static E_DBReturnCode ReadAllCountries(out List<Country> allCountries, out string errorMessage)
        {
            return PersistenceManager.ReadAllCountries(out allCountries, out errorMessage);
        }

        internal static E_DBReturnCode SmartAddCountry(string countryName, string areaName, string subAreaName, out string errorMessage)
        {
            return PersistenceManager.SmartAddCountry(countryName, areaName, subAreaName, out errorMessage);
        }

        private static List<Area> GetAreasByCountryName(string countryName, List<Area> allAreas, List<SubArea> subAreas)
        {
            return null;
        }
    }
}
