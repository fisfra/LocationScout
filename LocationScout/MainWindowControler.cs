using System;
using System.Collections.Generic;
using LocationScout.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LocationScout.DataAccess;

namespace LocationScout
{
    class MainWindowControler
    {
        #region attributes
        private MainWindow _window;
        private SettingTabControler _settingControler;
        private LocationTabControler _locationControler;

        private List<Country> _allCountries;

        public List<Country> AllCountries { get => _allCountries; set => _allCountries = value; }
        #endregion

        #region contructors
        public MainWindowControler(MainWindow window)
        {
            _window = window;
            _settingControler = new SettingTabControler(window, this);
            _locationControler = new LocationTabControler(window);

            if (!RefreshAllCountries(out string errorMessage))
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
            _window.Close();
        }

        internal void HandleSettingAdd()
        {
            _settingControler.Add();
        }

        internal void HandleLocationAdd()
        {
            _locationControler.Add();
        }

        internal bool RefreshAllCountries(out string errorMessage)
        {
            return DataAccessAdapter.ReadAllCountries(out _allCountries, out errorMessage);
        }

        internal void RefreshCountryControls()
        {
            if (_allCountries != null)
            {
                _window.SE_CountriesACTB.ClearSearchPool();
                _window.SL_CountriesACTB.ClearSearchPool();

                foreach (var country in _allCountries)
                {
                    _window.SE_CountriesACTB.AddObject(country.Name, country);
                    _window.SL_CountriesACTB.AddObject(country.Name, country);
                }
            }
        }
        #endregion
    }
}
