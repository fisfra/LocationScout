using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocationScout
{
    internal class SettingTabControler
    {
        #region attributes
        private MainWindow _window;

        private List<Country> _allCountries;
        private ViewModel.MaintainCountries _maintainCountries;
        #endregion

        #region constructors
        public SettingTabControler(MainWindow window)
        {
            _window = window;

            try
            {
                _allCountries = PersistenceManager.ReadCountries();                
            }
            catch (Exception e)
            {
                MessageBox.Show("Error reading saved data.\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            FillCountryACTB();

            _window.CountriesACTB.Leaving += CountriesACTB_Leaving;
            _window.AreasACTB.Leaving += AreasACTB_Leaving;
            _window.AreasACTB.LeavingViaShift += AreasACTB_LeavingViaShift;

            _maintainCountries = new ViewModel.MaintainCountries();
        }
        #endregion

        #region methods
        internal void Add()
        {
            string countryName = _window.CountriesACTB.GetCurrentText();
            string areaName = _window.AreasACTB.GetCurrentText();
            string subareaName = _window.SubAreasACTB.GetCurrentText();
        }

        private void FillCountryACTB()
        {
            if (_allCountries != null)
            {
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
                foreach (var sa in area.AllSubareas)
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
                _maintainCountries.SelectedCountry = country;
                foreach (var a in _maintainCountries.SelectedCountry.AllAreas)
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
        #endregion
    }
}
