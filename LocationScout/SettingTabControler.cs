using LocationScout.DataAccess;
using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFUserControl;
using static LocationScout.DataAccess.PersistenceManager;

namespace LocationScout
{
    internal class SettingTabControler : TabControlerBase
    {
        #region attributes
        private MainWindow _window;
        private MainWindowControler _mainControler;

        public override AutoCompleteTextBox CountryControl { get { return _window.SettingsCountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return _window.SettingsAreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return _window.SettingsSubAreaControl; } }
        #endregion

        #region constructors
        public SettingTabControler(MainWindowControler mainControler) : base(mainControler.Window)
        {            
            _mainControler = mainControler;
            _window = _mainControler.Window;

            _window.SettingsCountryControl.Leaving += CountryControl_Leaving;
            _window.SettingsAreaControl.Leaving += AreaControl_Leaving;
            _window.SettingsAreaControl.LeavingViaShift += AreaControl_Leaving_LeavingViaShift;
            _window.SettingsSubAreaControl.Leaving += SubAreaControl_Leaving;
            _window.SettingsSubAreaControl.LeavingViaShift += SubAreaControl_LeavingViaShift;
        }
        #endregion

        #region methods
        internal void Add()
        {
            // get values form UI
            string countryName = _window.SettingsCountryControl.GetCurrentText();
            string areaName = _window.SettingsAreaControl.GetCurrentText();
            string subAreaName = _window.SettingsSubAreaControl.GetCurrentText();

            // db operations might take a while
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            // add country to database
            E_DBReturnCode success = DataAccessAdapter.SmartAddCountry(countryName, areaName, subAreaName, out string errorMessage);

            // refresh or error handling
            AfterDBWriteSteps(success, errorMessage);

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and reset focus
            _window.SettingsCountryControl.ClearText();
            _window.SettingsAreaControl.ClearText();
            _window.SettingsSubAreaControl.ClearText();
            _window.SettingsCountryControl.SetFocus();
        }

        private void AfterDBWriteSteps(E_DBReturnCode success, string errorMessage)
        {
            switch (success)
            {
                case E_DBReturnCode.no_error:
                {
                    // read the countries again from database and update _allCountries
                    if (_mainControler.RefreshAllCountries(out errorMessage) == E_DBReturnCode.error)
                    {
                        ShowMessage("Error re-reading data.\n" + errorMessage, E_MessageType.error);
                    }
                    else
                    {
                        _mainControler.RefreshCountryControls();
                        ShowMessage("Added to database.", E_MessageType.success);
                    }

                    break;
                }

                case E_DBReturnCode.error:
                    ShowMessage("Error writing data.\n" + errorMessage, E_MessageType.error);
                    break;

                case E_DBReturnCode.already_existing:
                    ShowMessage("Already existing in database.\n" + errorMessage, E_MessageType.info);
                    break;

                default:
                    // unknown enum
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        private void SubAreaControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.SettingsAddButton.Focus();
        }
        #endregion
    }
}
