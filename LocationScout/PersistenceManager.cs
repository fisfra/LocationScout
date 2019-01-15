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
        #region constants
        // *** begin testing -------------------------------------------------------
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
        // *** end testing ---------------------------------------------------------
        #endregion

        #region attributes
        #endregion

        #region constructores
        public PersistenceManager()
        {
        }
        #endregion

        #region methods
        public static List<Country> ReadAllCountries()
        {
            // to be replace with database reading
            return ForTesting_ReadAllCountries();
        }

        public static void AddCountry(Country newCountry)
        {
            // to do
        }

        public static List<Area> ReadAllAreas()
        {
            // to be replace with database reading
            return ForTesting_ReadAllAreas();
        }

        public static List<SubArea> ReadAllSubareas()
        {
            // to be replace with database reading
            return ForTesting_ReadAllSubareas();
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
        private static List<SubArea> ForTesting_ReadAllSubareas()
        {
            return new List<SubArea>()
            {
                new SubArea(c_subareaDolomites),
                new SubArea(c_subareaSouthTyrol),
                new SubArea(c_subareaValDOrcia),
                new SubArea(c_subareaAllgaeuerAlps),
                new SubArea(c_subareaBerechtesgadenerLand)
            };
        }

        private static List<Area> ForTesting_ReadAllAreas()
        {
            return new List<Area>()
            {
                new Area(c_areaAlps, new List<SubArea>()
                                     {
                                        new SubArea(c_subareaDolomites),
                                        new SubArea(c_subareaSouthTyrol),
                                        new SubArea(c_subareaAllgaeuerAlps),
                                        new SubArea(c_subareaBerechtesgadenerLand)
                                     }
                ),
                new Area(c_areaTuscany, new List<SubArea>()
                                     {
                                        new SubArea(c_subareaValDOrcia)
                                     }
                ),
                new Area(c_areaNorthernSea, null)
            };
        }

        private static List<Country> ForTesting_ReadAllCountries()
        {
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
