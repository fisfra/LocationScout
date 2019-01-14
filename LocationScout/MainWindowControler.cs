using System;
using System.Collections.Generic;
using LocationScout.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocationScout
{
    class MainWindowControler
    {
        #region attributes
        private MainWindow _window;

        private List<Country> _allCountries;
        #endregion


        #region contructors
        public MainWindowControler(MainWindow window)
        {
            _window = window;

            try
            {
                _allCountries = ReadCountries();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error reading saved data.\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            InitializeACTB();

            _window.CountriesACTB.Leaving += CountriesACTB_Leaving;
        }

        private void CountriesACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            var countryName = e.Object as string;

            var country = GetCountryByName(countryName);

            foreach (var a in country.AllAreas)
            {
                _window.AreasACTB.AddObject(a.Name, a);
            }

        }
        #endregion

        #region methods
        internal void HandleClose()
        {
            _window.Close();
        }

        internal void HandleAdd()
        {
            throw new NotImplementedException();
        }

        private void InitializeACTB()
        {
            if (_allCountries != null)
            {
                foreach (var country in _allCountries)
                {
                    _window.CountriesACTB.AddObject(country.Name, country);
                }
            }
        }

        private List<Country> ReadCountries()
        {
            // to be replace with database reading
            return ForTesting_FillData();
        }

        // *** Testing only ***
        private List<Country> ForTesting_FillData()
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

        private List<Area> GetAreaNamesByCountryName(string countryName, List<Country_Area> allCountryAreas, List<Area_Subarea> allAreaSubareas)
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

        private List<SubArea> GetSubareaNamesByAreaName(string areaName, List<Area_Subarea> allAreaSubareas)
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

        private Country GetCountryByName(string countryName)
        {
            Country foundCountry = null;

            foreach (var c in _allCountries)
            {
                if (c.Name == countryName)
                {
                    foundCountry = c;
                    break;
                }
            }

            return foundCountry;
        }
        #endregion
    }
}
