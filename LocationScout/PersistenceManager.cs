using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout
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
        public static List<Country> ReadCountries()
        {
            // to be replace with database reading
            return ForTesting_FillData();
        }

        private static List<Area> GetAreaNamesByCountryName(string countryName, List<Country_Area> allCountryAreas, List<Area_Subarea> allAreaSubareas)
        {
            var foundAreas = new List<Area>();

            foreach (var ca in allCountryAreas)
            {
                if (ca.CountryName == countryName)
                {
                    foundAreas.Add(new Area(ca.AreaName, GetSubareaNamesByAreaName(ca.AreaName, allAreaSubareas)));
                }
            }

            return foundAreas;
        }

        private static List<SubArea> GetSubareaNamesByAreaName(string areaName, List<Area_Subarea> allAreaSubareas)
        {
            var foundSubareaNames = new List<SubArea>();

            foreach (var arsa in allAreaSubareas)
            {
                if (arsa.AreaName == areaName)
                {
                    foundSubareaNames.Add(new SubArea(arsa.SubareaName));
                }
            }

            return foundSubareaNames;
        }
        #endregion

        // *** Testing only ***
        private static List<Country> ForTesting_FillData()
        {
            // countries
            const string c_countryItaly = "Italy";
            const string c_countryGermany = "Germany";

            // areas
            const string c_areaAlps = "Alps";
            const string c_areaTuscany = "Tuscany";
            const string c_areaNorthernSea = "Northern Sea";

            // subareas
            const string c_subareaDolomites = "Dolomites";
            const string c_subareaSouthTyrol = "South Tyrol";
            const string c_subareaValDOrcia = "Val d'Orcia";
            const string c_subareaAllgaeuerAlps = "Allgäuer Alpen";
            const string c_subareaBerechtesgadenerLand = "Berchtesgadener Land";

            var allAreaSubareas = new List<Area_Subarea>()
            {
                new Area_Subarea(c_areaAlps, c_subareaDolomites),
                new Area_Subarea(c_areaAlps, c_subareaSouthTyrol),
                new Area_Subarea(c_areaAlps, c_subareaAllgaeuerAlps),
                new Area_Subarea(c_areaAlps, c_subareaBerechtesgadenerLand),
                new Area_Subarea(c_areaTuscany, c_subareaValDOrcia)
            };


            var allCountryAreas = new List<Country_Area>()
            {
                new Country_Area(c_countryItaly, c_areaAlps),
                new Country_Area(c_countryItaly, c_areaTuscany),
                new Country_Area(c_countryGermany, c_areaAlps),
                new Country_Area(c_countryGermany, c_areaNorthernSea)
            };

            return new List<Country>()
            {
                new Country(c_countryGermany, GetAreaNamesByCountryName(c_countryGermany, allCountryAreas, allAreaSubareas)),
                new Country(c_countryItaly, GetAreaNamesByCountryName(c_countryItaly, allCountryAreas, allAreaSubareas))
            };
        }
    }


}
