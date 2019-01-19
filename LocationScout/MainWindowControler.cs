using System;
using System.Collections.Generic;
using LocationScout.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LocationScout.DataAccess;
using static LocationScout.DataAccess.PersistenceManager;
using WPFUserControl;

namespace LocationScout
{
    public class MainWindowControler
    {
        #region attributes
        private MainWindow _window;
        private SettingTabControler _settingControler;
        private LocationTabControler _locationControler;

        private List<Country> _allCountries;

        public List<Country> AllCountries { get => _allCountries; set => _allCountries = value; }
        public MainWindow Window { get => _window; private set => _window = value; }
        #endregion

        #region contructors
        public MainWindowControler(MainWindow window)
        {
            Window = window;
            _settingControler = new SettingTabControler(this);
            _locationControler = new LocationTabControler(this);

            if (RefreshAllCountries(out string errorMessage) == E_DBReturnCode.error)
            {
                MessageBox.Show("Error reading saved data.\n" + errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                RefreshCountryControls();
            }
        }
        #endregion

        #region methods
        internal void HandleClose()
        {
            Window.Close();
        }

        internal void HandleSettingAdd()
        {
            _settingControler.Add();
        }

        internal void HandleLocationAdd()
        {
            _locationControler.Add();
        }

        internal E_DBReturnCode RefreshAllCountries(out string errorMessage)
        {
            return DataAccessAdapter.ReadAllCountries(out _allCountries, out errorMessage);
        }

        internal void RefreshCountryControls()
        {
            if (_allCountries != null)
            {
                Window.SettingsCountryControl.ClearSearchPool();
                Window.LocationCountryControl.ClearSearchPool();

                foreach (var country in _allCountries)
                {
                    Window.SettingsCountryControl.AddObject(country.Name, country);
                    Window.LocationCountryControl.AddObject(country.Name, country);
                }
            }
        }

        internal List<SubArea> GetAllSubAreas(long countryId, long areaId)
        {
            // result list
            List<SubArea> foundSubAreas = new List<SubArea>();

            // search for country in list of all countries
            var foundCountry = _allCountries.FirstOrDefault(o => o.Id == countryId);

            // found
            if (foundCountry != null)
            {
                // get subareas
                foundSubAreas = foundCountry.SubAreas;
            }
            return foundSubAreas;
        }
        #endregion
    }
}
