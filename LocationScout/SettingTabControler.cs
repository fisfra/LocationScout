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

            _maintainCountries = new ViewModel.MaintainCountries();
        }
        #endregion

        #region methods
        internal void Add()
        {
            bool newCountryEntered = (_window.CountriesACTB.GetCurrentObject() == null);
            bool newAreaEntered = (_window.AreasACTB.GetCurrentObject() == null);
            bool newSubareaEntered = (_window.SubAreasACTB.GetCurrentObject() == null);

            string countryName = _window.CountriesACTB.GetCurrentText();
            string areaName = _window.AreasACTB.GetCurrentText();
            string subAreaName = _window.SubAreasACTB.GetCurrentText();

            if (newCountryEntered)
            {
                SubArea newSubarea = new SubArea() { Name = subAreaName };
                Area newArea = new Area() { Name = areaName, Subareas = new List<SubArea>() { newSubarea } };
                Country newCountry = new Country() { Name = countryName, Areas = new List<Area>() { newArea } };
                newArea.Countries = new List<Country>() { newCountry };
                newSubarea.Areas = new List<Area>() { newArea };

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (!DataAccessAdapter.AddCountry(newCountry, out string errorMessage))
                {
                    MessageBox.Show("Error writing data.\n" + errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Mouse.OverrideCursor = null;
            }

            else if (newAreaEntered)
            {
                Country existingCountry = _window.CountriesACTB.GetCurrentObject() as Country;
                SubArea newSubarea = new SubArea() { Name = subAreaName };
                Area newArea = new Area() { Name = areaName, Subareas = new List<SubArea>() { newSubarea } };
                newArea.Countries = new List<Country>() { existingCountry };
                
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (!DataAccessAdapter.AddAreaToCountry(existingCountry, newArea, out string errorMessage))
                {
                    MessageBox.Show("Error writing data.\n" + errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Mouse.OverrideCursor = null;
            }

            else if (newSubareaEntered)
            {
                Area existingArea = _window.AreasACTB.GetCurrentObject() as Area;
                SubArea newSubArea = new SubArea() { Name = subAreaName };
                newSubArea.Areas = new List<Area>() { existingArea };

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (!DataAccessAdapter.AddSubAreaToArea(existingArea, newSubArea, out string errorMessage))
                {
                    MessageBox.Show("Error writing data.\n" + errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Mouse.OverrideCursor = null;
            }
        }

        private bool AreaExists(string areaName)
        {
            return false;
            //return (PersistenceManager.ReadAllAreas().Find(o => o.Name == areaName) != null);
        }

        private bool SubareaExists(string subareaName)
        {
            return false;
            //return (PersistenceManager.ReadAllSubareas().Find(o => o.Name == subareaName) != null);
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
        #endregion
    }
}
