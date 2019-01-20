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
        private MainWindowControler _mainControler;

        public override AutoCompleteTextBox CountryControl { get { return Window.SettingsCountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return Window.SettingsAreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return Window.SettingsSubAreaControl; } }
        #endregion

        #region constructors
        public SettingTabControler(MainWindowControler mainControler, MainWindow window) : base(window)
        {            
            _mainControler = mainControler;

            Window.SettingsCountryControl.Leaving += CountryControl_Leaving;
            Window.SettingsAreaControl.Leaving += AreaControl_Leaving;
            Window.SettingsAreaControl.LeavingViaShift += AreaControl_Leaving_LeavingViaShift;
            Window.SettingsSubAreaControl.Leaving += SubAreaControl_Leaving;
            Window.SettingsSubAreaControl.LeavingViaShift += SubAreaControl_LeavingViaShift;
        }
        #endregion

        #region methods
        internal void Add()
        {
            // get values from UI
            string countryName = Window.SettingsCountryControl.GetCurrentText();
            string areaName = Window.SettingsAreaControl.GetCurrentText();
            string subAreaName = Window.SettingsSubAreaControl.GetCurrentText();

            // db operations might take a while
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            // add country to database
            E_DBReturnCode success = DataAccessAdapter.SmartAddCountry(countryName, areaName, subAreaName, out string errorMessage);

            // refresh or error handling
            AfterDBWriteSteps(success, errorMessage);

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and reset focus
            Window.SettingsCountryControl.ClearText();
            Window.SettingsAreaControl.ClearText();
            Window.SettingsSubAreaControl.ClearText();
            Window.SettingsCountryControl.SetFocus();
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
            Window.SettingsAddButton.Focus();
        }
        #endregion
    }
}
