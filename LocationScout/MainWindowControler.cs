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
        private List<Country> _allCountries;

        public List<Country> AllCountries { get { return _allCountries; } }
        #endregion

        #region contructors
        public MainWindowControler(MainWindow window) : base (window)
        {
            _settingControler = new SettingTabControler(this, window);
            _locationControler = new LocationTabControler(this, window);

            ReloadAndRefreshControls();

        }
        #endregion

        #region methods             
        internal void HandleClose()
        {
            Window.Close();
        }

        internal void HandleClipboardUpdate()
        {
            string clipboardText = Clipboard.GetText();

            _locationControler.HandleClipboardChange(clipboardText);
            _settingControler.HandleClipboardChange(clipboardText);
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
            _locationControler.ShowLister();
        }

        internal void HandleLocationClear()
        {
            _locationControler.Clear();
        }

        internal void SettingsEdit()
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
        }

        public void ReloadAndRefreshControls()
        {
            // read from database
            RefreshAllCountriesFromDB();
            _settingControler.RefreshAllObjectsFromDB();
            _locationControler.RefreshAllObjectsFromDB(false); // use parameter to avoid double refreshing

            // refresh the country control
            RefreshCountryControls();
            _settingControler.ReloadAndRefreshControls();
            _locationControler.ReloadAndRefreshControls();
        }

        internal void HandleSettingsCountryControlEditLostFocus()
        {
            _settingControler.HandleSettingsCountryControlEditLostFocus();
        }

        internal void HandleSettingsAreaControlEditLostFocus()
        {
            _settingControler.HandleSettingsAreaControlEditLostFocus();
        }

        internal void HandleGoopleMapsSubjectLocation()
        {
            _locationControler.HandleGoogleMapsSubjectLocation();
        }

        internal void HandleSettingsSubAreaControlEditLostFocus()
        {
            _settingControler.HandleSettingsSubAreaControlEditLostFocus();
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

        internal void HandleSettingsSubjectLocationControlEditLostFocus()
        {
            _settingControler.HandleSettingsSubjectLocationControlEditLostFocus();
        }

        internal void HandleSettingsSubjectLocationControlLostFocus()
        {
            _settingControler.HandleSettingsSubjectLocationControlLostFocus();
        }

        internal void HandleShootingLocationControlEditLostFocus()
        {
            _locationControler.HandleShootingLocationControlEditLostFocus();
        }

        internal void LocationEdit()
        {
            _locationControler.Edit();
        }

        internal void HandleSubjectLocationGoogleSearch()
        {
            _settingControler.SubjectLocationGoogleSearch();
        }

        internal void HandleParkingLocationControlEditLostFocus()
        {
            _locationControler.HandleParkingLocationControlEditLostFocus();
        }
        #endregion
    }
}
