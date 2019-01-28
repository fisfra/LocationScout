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

        internal void HandleLocationShow()
        {
            _listerControler.Show();
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
            RefreshControl(_allCountries.OfType<Location>().ToList(), Window.Location_CountryControl);
            RefreshControl(_allCountries.OfType<Location>().ToList(), Window.Settings_CountryControl);

            // testing
            RefreshControl(_allCountries.OfType<Location>().ToList(), Window.SettingsCountryControl_New);
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

        internal void HandleSettingsCountryControlListFocus()
        {
            _settingControler.HandleSettingsCountryControlListFocus();
        }

        internal void HandleSettingsAreaControlListFocus()
        {
            _settingControler.HandleSettingsAreaControlListFocus();
        }

        internal void HandleGoopleMapsSubjectLocation()
        {
            _locationControler.HandleGoogleMapsSubjectLocation();
        }

        internal void HandleSettingsSubAreaControlListFocus()
        {
            _settingControler.HandleSettingsSubAreaControlListFocus();
        }

        internal void HandlePhotoUpload()
        {
            _locationControler.HandlePhotoUpload();
        }

        internal void HandleRemovePhoto_1()
        {
            _locationControler.HandleRemove(LocationTabControler.E_PhotoNumber.photo_1);
        }

        internal void HandleRemovePhoto_2()
        {
            _locationControler.HandleRemove(LocationTabControler.E_PhotoNumber.photo_2);
        }

        internal void HandleRemovePhoto_3()
        {
            _locationControler.HandleRemove(LocationTabControler.E_PhotoNumber.photo_3);
        }

        internal void HandleSettingsSubjectLocationControlListFocus()
        {
            _settingControler.HandleSettingsSubjectLocationControlListFocus();
        }
        #endregion
    }
}
