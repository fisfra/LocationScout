using LocationScout.DataAccess;
using LocationScout.Model;
using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFUserControl;
using static LocationScout.DataAccess.PersistenceManager;

namespace LocationScout
{
    internal class LocationTabControler : TabControlerBase
    {
        #region attributes
        private MainWindowControler _mainControler;

        public override AutoCompleteTextBox CountryControl { get { return Window.LocationCountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return Window.LocationAreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return Window.LocationSubAreaControl; } }
        #endregion

        #region contructors
        public LocationTabControler(MainWindowControler mainControler, MainWindow window) : base (window)
        {
            _mainControler = mainControler;


            Window.LocationCountryControl.Leaving += CountryControl_Leaving;
            Window.LocationAreaControl.Leaving += AreaControl_Leaving;
            Window.LocationAreaControl.LeavingViaShift += AreaControl_LeavingViaShift;
            Window.LocationSubAreaControl.Leaving += SubAreaControl_Leaving;
        }
        #endregion

        #region methods
        private void SubAreaControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Window.LocationNameTextBox.Focus();
        }

        private void AreaControl_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Window.LocationCountryControl.SetFocus();
        }
     
        internal void Add()
        {
            // get Ids from UI
            var countryId = (Window.LocationCountryControl.GetCurrentObject() as Country).Id;
            var areaId = (Window.LocationAreaControl.GetCurrentObject() as Area).Id;
            var subAreaId = (Window.LocationSubAreaControl.GetCurrentObject() as SubArea).Id;

            // db operations might take a while
            Mouse.OverrideCursor = Cursors.Wait;

            // add country to database
            E_DBReturnCode success = DataAccessAdapter.SmartAddPhotoPlace(countryId, areaId, subAreaId, Window.LocationViewModel, out string errorMessage);

            // refresh or error handling
            // AfterDBWriteSteps(success, errorMessage);

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and reset focus
            // _window.SettingsCountryControl.ClearText();
            // _window.SettingsAreaControl.ClearText();
            // _window.SettingsSubAreaControl.ClearText();
            // _window.SettingsCountryControl.SetFocus();
        }


        #endregion
    }
}
