using LocationScout.DataAccess;
using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static LocationScout.DataAccess.PersistenceManager;

namespace LocationScout
{
    internal class SettingTabControler
    {
        #region enum
        enum E_MessageType { success, info, error };
        #endregion

        #region attributes
        private MainWindow _window;
        private MainWindowControler _mainControler;

        private ViewModel.MaintainCountries _maintainCountries;
        private System.Windows.Threading.DispatcherTimer _dispatcherTimer;
        #endregion

        #region constructors
        public SettingTabControler(MainWindow window, MainWindowControler mainControler)
        {
            _window = window;
            _mainControler = mainControler;

            _window.SE_CountriesACTB.Leaving += CountriesACTB_Leaving;
            _window.SE_AreasACTB.Leaving += AreasACTB_Leaving;
            _window.SE_AreasACTB.LeavingViaShift += AreasACTB_LeavingViaShift;
            _window.SE_SubAreasACTB.Leaving += SubAreasACTB_Leaving;
            _window.SE_SubAreasACTB.LeavingViaShift += SubAreasACTB_LeavingViaShift;

            _maintainCountries = new ViewModel.MaintainCountries();
        }
        #endregion

        #region methods
        internal void Add()
        {
            // get values form UI
            string countryName = _window.SE_CountriesACTB.GetCurrentText();
            string areaName = _window.SE_AreasACTB.GetCurrentText();
            string subAreaName = _window.SE_SubAreasACTB.GetCurrentText();

            // db operations might take a while
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            // add country to database
            E_DBReturnCode success = DataAccessAdapter.SmartAddCountry(countryName, areaName, subAreaName, out string errorMessage);

            // refresh or error handling
            AfterDBWriteSteps(success, errorMessage);

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and reset focus
            _window.SE_CountriesACTB.ClearText();
            _window.SE_AreasACTB.ClearText();
            _window.SE_SubAreasACTB.ClearText();
            _window.SE_CountriesACTB.SetFocus();
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

        private void ShowMessage(string text, E_MessageType type)
        {
            switch (type)
            {
                case E_MessageType.success:
                    SetMessage(text);
                    break;
                case E_MessageType.info:
                    SetMessage(text);
                    break;
                case E_MessageType.error:
                    MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        private void SetMessage(string text)
        {
            _window.StatusLabel.Content = text;

            _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _dispatcherTimer.Stop();

            _window.StatusLabel.Content = string.Empty;
        }

        private void AreasACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            var areaName = e.Text;
            var area = e.Object as Area;

            // known area (== null is a new area)
            if (area != null)
            {
                // remove the odl results
                _window.SE_SubAreasACTB.ClearSearchPool();

                // remember the selected area
                _maintainCountries.SelectedArea = area;

                // only take the subareas which are also assigned to the current country
                // an area can contain subareas outside an assigned country (e.g. Germany - Alps - Berner Alps --> not in Germany)
                var relevantSubAreas = (_maintainCountries.SelectedCountry != null) ? _mainControler.GetAllSubAreas(_maintainCountries.SelectedCountry.Id, area.Id) : null;
                if (relevantSubAreas != null)
                {
                    foreach (var sa in relevantSubAreas)
                    {
                        _window.SE_SubAreasACTB.AddObject(sa.Name, sa);
                    }
                }
            }

            _window.SE_SubAreasACTB.SetFocus(); 
        }

        private void CountriesACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            var country = e.Object as Country;

            // remember the currently selected country (null for new countries)
            _maintainCountries.SelectedCountry = country;

            // known country (== null is a new country)
            if (country != null)
            {
                _window.SE_AreasACTB.ClearSearchPool();

                _maintainCountries.SelectedCountry = country;
                foreach (var a in _maintainCountries.SelectedCountry.Areas)
                {
                    _window.SE_AreasACTB.AddObject(a.Name, a);
                }
            }

            _window.SE_AreasACTB.SetFocus(); 
        }

        private void AreasACTB_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {            
            _window.SE_CountriesACTB.SetFocus();
        }

        private void SubAreasACTB_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.SE_AreasACTB.SetFocus();
        }

        private void SubAreasACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {


            _window.SettingsAddButton.Focus();
        }
        #endregion
    }
}
