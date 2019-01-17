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

        internal static bool AddCountry(string countryName, string areaName, string subAreaName, out string errorMessage)
        {
            // create all model objects and set the relations
            SubArea newSubarea = new SubArea() { Name = subAreaName };
            Area newArea = new Area() { Name = areaName, Subareas = new List<SubArea>() { newSubarea } };
            Country newCountry = new Country() { Name = countryName, Areas = new List<Area>() { newArea } };
            newArea.Countries = new List<Country>() { newCountry };
            newSubarea.Areas = new List<Area>() { newArea };

            // write to database
            return PersistenceManager.AddCountry(newCountry, out errorMessage);
        }

        internal static bool AddAreaToCountry(string countryName, string areaName, string subAreaName, out string errorMessage)
        {
            errorMessage = string.Empty;
            var success = true;

            // no error
            if (success)
            {
                // create all model objects and set the relations
                SubArea newSubArea = new SubArea() { Name = subAreaName, Areas = new List<Area>() };
                Area newArea = new Area() { Name = areaName, Subareas = new List<SubArea>(), Countries = new List<Country>()};          
                newArea.Subareas.Add(newSubArea);
                newSubArea.Areas.Add(newArea);

                // write to database
                success = PersistenceManager.AddAreaToCountry(countryName, newArea, newSubArea, out errorMessage);
            }

            return success;
        }

        internal static bool AddSubAreaToArea(string areaName, string subAreaName, out string errorMessage)
        {
            var success = true;

            // get the existing country from database
            success = PersistenceManager.GetArea(areaName, out Area existingArea, out errorMessage);

            // no error
            if (success)
            {
                // create all model objects and set the relations
                SubArea newSubArea = new SubArea() { Name = subAreaName, Areas = new List<Area>() };
                existingArea.Subareas.Add(newSubArea);
                newSubArea.Areas.Add(existingArea);

                // write to database
                success = PersistenceManager.AddSubAreaToArea(existingArea, newSubArea, out errorMessage);
            }

            return success;
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
