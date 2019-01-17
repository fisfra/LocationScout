using LocationScout.DataAccess;
using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LocationScout
{
    internal class SettingTabControler
    {
        #region attributes
        private MainWindow _window;

        private List<Country> _allCountries;
        private ViewModel.MaintainCountries _maintainCountries;

        private System.Windows.Threading.DispatcherTimer _dispatcherTimer;
        #endregion

        #region constructors
        public SettingTabControler(MainWindow window)
        {
            _window = window;

            if (!DataAccessAdapter.ReadAllCountries(out _allCountries, out string errorMessage))
            {
                MessageBox.Show("Error reading saved data.\n" + errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            FillCountryACTB();

            _window.CountriesACTB.Leaving += CountriesACTB_Leaving;
            _window.AreasACTB.Leaving += AreasACTB_Leaving;
            _window.AreasACTB.LeavingViaShift += AreasACTB_LeavingViaShift;
            _window.SubAreasACTB.Leaving += SubAreasACTB_Leaving;
            _window.SubAreasACTB.LeavingViaShift += SubAreasACTB_LeavingViaShift;

            _maintainCountries = new ViewModel.MaintainCountries();
        }


        #endregion

        #region methods
        internal void Add()
        {
            // get values form UI
            string countryName = _window.CountriesACTB.GetCurrentText();
            string areaName = _window.AreasACTB.GetCurrentText();
            string subAreaName = _window.SubAreasACTB.GetCurrentText();

            // db operations might take a while
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            // add country to database
            var success = DataAccessAdapter.SmartAddCountry(countryName, areaName, subAreaName, out string errorMessage);

            // refresh or error handling
            AfterDBWriteSteps(success, errorMessage);

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and reset focus
            _window.CountriesACTB.ClearText();
            _window.AreasACTB.ClearText();
            _window.SubAreasACTB.ClearText();
            _window.CountriesACTB.SetFocus();
        }

        private void AfterDBWriteSteps(bool success, string errorMessage)
        {
            // no error
            if (success)
            {
                // read the countries again from database and update _allCountries
                success = DataAccessAdapter.ReadAllCountries(out _allCountries, out errorMessage);

                // set (error) message
                if (success)
                {
                    FillCountryACTB();
                    SetMessage("Sucessfully added");
                }
                else
                {
                    ErrorMessage("Error re-reading data.\n" + errorMessage);
                }
            }

            // error
            else
            {
                MessageBox.Show("Error writing data.\n" + errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ErrorMessage(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void FillCountryACTB()
        {
            if (_allCountries != null)
            {
                _window.CountriesACTB.ClearSearchPool();

                foreach (var country in _allCountries)
                {
                    _window.CountriesACTB.AddObject(country.Name, country);
                }
            }
        }

        private void AreasACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            var areaName = e.Text;
            var area = e.Object as Area;

            // known area (== null is a new area)
            if (area != null)
            {
                _window.SubAreasACTB.ClearSearchPool();

                foreach (var sa in area.Subareas)
                {
                    _window.SubAreasACTB.AddObject(sa.Name, sa);
                }
            }

            _window.SubAreasACTB.SetFocus(); 
        }

        private void CountriesACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            var country = e.Object as Country;

            // known country (== null is a new country)
            if (country != null)
            {
                _window.AreasACTB.ClearSearchPool();

                _maintainCountries.SelectedCountry = country;
                foreach (var a in _maintainCountries.SelectedCountry.Areas)
                {
                    _window.AreasACTB.AddObject(a.Name, a);
                }
            }

            _window.AreasACTB.SetFocus(); 
        }

        private void AreasACTB_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.CountriesACTB.SetFocus();
        }

        private void SubAreasACTB_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.AreasACTB.SetFocus();
        }

        private void SubAreasACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.AddButton.Focus();
        }        
        #endregion
    }
}
