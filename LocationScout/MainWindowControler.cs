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
using LocationScout.ViewModel;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace LocationScout
{
    internal class MainWindowControler : ControlerBase
    {
        #region attributes
        private SettingTabControler _settingControler;
        private LocationTabControler _locationControler;
        private ListerControler _listerControler;
        private List<Country> _allCountries;

        public List<Country> AllCountries { get { return _allCountries; } }
        #endregion

        #region contructors
        public MainWindowControler(MainWindow window) : base (window)
        {
            _settingControler = new SettingTabControler(this, window);
            _locationControler = new LocationTabControler(this, window);
            _listerControler = new ListerControler(window);

            ReloadAndRefreshControls();
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

        internal void LoadPhoto_1_1()
        {
            _locationControler.LoadPhoto_1_1();
        }

        internal void LoadPhoto_1_2()
        {
            _locationControler.LoadPhoto_1_2();
        }

        internal void LoadPhoto_2_1()
        {
            _locationControler.LoadPhoto_2_1();
        }

        internal void HandleLocationShow()
        {
            _listerControler.Show();
        }

        internal void LoadPhoto_2_2()
        {
            _locationControler.LoadPhoto_2_2();
        }

        internal void Edit()
        {
            _settingControler.Edit();
        }

        internal void Delete()
        {
            _settingControler.Delete();
        }

        private void RefreshAllCountriesFromDB()
        {
            var success = DataAccessAdapter.ReadAllCountries(out _allCountries, out string errorMessage);
            if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading countries" + errorMessage, E_MessageType.error);
        }

        private void RefreshCountryControls()
        {
            RefreshControl(_allCountries.OfType<Location>().ToList(), Window.LocationCountryControl);
            RefreshControl(_allCountries.OfType<Location>().ToList(), Window.SettingsCountryControl);
        }

        public void ReloadAndRefreshControls()
        {
            // read from database
            RefreshAllCountriesFromDB();
            _settingControler.RefreshAllObjectsFromDB();

            // refresh the country control
            RefreshCountryControls();
            _settingControler.ReloadAndRefreshControls();
        }
        #endregion
    }
}
