using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.DataAccess
{
    internal class TestDataGenerator
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

        internal static void ReadAllSubareas(out List<string> subareaNames)
        {
            subareaNames = new List<string>()
            {
                c_subareaDolomites,
                c_subareaSouthTyrol,
                c_subareaValDOrcia,
                c_subareaAllgaeuerAlps,
                c_subareaBerechtesgadenerLand
            };
        }

        internal static void ReadAllAreas(out List<string> areaNames)
        {
            areaNames = new List<string>()
            {
                c_areaAlps,
                c_areaTuscany,
                c_areaNorthernSea
            };
        }

        internal static void ReadAllCountries(out List<string> countryNames)
        {
            countryNames = new List<string>()
            {
                c_countryItaly,
                c_countryGermany
            };
        }

        internal static void ReadAllArea_Subareas(out List<Tuple<string, string>> areaNames_subareaNames)
        {
            areaNames_subareaNames = new List<Tuple<string,string>>()
            {
                new Tuple<string, string>(c_areaAlps, c_subareaDolomites),
                new Tuple<string,string>(c_areaAlps, c_subareaSouthTyrol),
                new Tuple<string,string>(c_areaAlps, c_subareaAllgaeuerAlps),
                new Tuple<string,string>(c_areaAlps, c_subareaBerechtesgadenerLand),
                new Tuple<string,string>(c_areaTuscany, c_subareaValDOrcia)
            };
        }

        internal static void ReadAll_Country_Areas(out List<Tuple<string, string>> countryNames_areaNames)
        {
            countryNames_areaNames = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(c_countryItaly, c_areaAlps),
                new Tuple<string,string>(c_countryItaly, c_areaTuscany),
                new Tuple<string,string>(c_countryGermany, c_areaAlps),
                new Tuple<string,string>(c_countryGermany, c_areaNorthernSea)
            };
        }
    }
}
