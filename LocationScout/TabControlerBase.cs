using LocationScout.DataAccess;
using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFUserControl;

namespace LocationScout
{
    internal abstract class TabControlerBase : ControlerBase
    {
        #region enum
        #endregion

        #region attributes
        private List<Area> _allAreas;
        private List<SubArea> _allSubAreas;
        private List<SubjectLocation> _allSubjectLocations;

        protected MainWindowControler MainControler { get; private set; }

        public abstract AutoCompleteTextBox CountryControl { get; }
        public abstract AutoCompleteTextBox AreaControl { get; }
        public abstract AutoCompleteTextBox SubAreaControl { get; }
        public abstract AutoCompleteTextBox SubjectLocationControl { get; }
        #endregion

        #region constructors
        public TabControlerBase(MainWindow window, MainWindowControler mainControler) : base(window)
        {
            MainControler = mainControler;

            CountryControl.Leaving += CountryControl_Leaving;
            AreaControl.Leaving += AreaControl_Leaving;
            AreaControl.LeavingViaShift += AreaControl_Leaving_LeavingViaShift;
            SubAreaControl.Leaving += SubAreaControl_Leaving;
            SubAreaControl.LeavingViaShift += SubAreaControl_LeavingViaShift;
            SubjectLocationControl.Leaving += SubjectLocationControl_Leaving;
        }
        #endregion

        #region methods
        public void RefreshAllObjectsFromDB()
        {
            RefreshAllAreasFromDB();
            RefreshAllSubAreasFromDB();
            RefreshAllSubjectLocationsFromDB();
        }

        private void RefreshAllAreasFromDB()
        {
            var success = DataAccessAdapter.ReadAllAreas(out _allAreas, out string errorMessage);
            if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading areas" + errorMessage, E_MessageType.error);
        }

        private void RefreshAllSubAreasFromDB()
        {
            var success = DataAccessAdapter.ReadAllSubAreas(out _allSubAreas, out string errorMessage);
            if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading subareas" + errorMessage, E_MessageType.error);
        }

        private void RefreshAllSubjectLocationsFromDB()
        {
            var success = DataAccessAdapter.ReadAllSubjectLocations(out _allSubjectLocations, out string errorMessage);
            if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading subject locations" + errorMessage, E_MessageType.error);
        }

        protected virtual void CountryControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            // if area control is ready only, only area of the assigned country can be selected
            if (AreaControl.ListBoxReadOnly)
            {
                RefreshControl((e.Object as Country).Areas.OfType<Location>().ToList(), AreaControl);
            }
            // otherwise all existing areas can be selected (or new onces can be entered)
            else
            {
                RefreshControl(_allAreas.OfType<Location>().ToList(), AreaControl);
            }

            // clear old subarea text
            SubAreaControl.ClearText();

            // derived class can set the view model property
            // take country name from country object (if in database) or from UI text
            SetCountryDisplayItem(e.Object as Country != null ? (e.Object as Country).Name : CountryControl.GetCurrentText());

            // set focus to the respective text box
            AreaControl.SetFocus();
        }

        protected virtual void AreaControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            if (SubAreaControl.ListBoxReadOnly)
            {
                // get the subareas and let the base class do the work
                var countrySubAreas = (CountryControl.GetCurrentObject() as Country)?.SubAreas;
                var areaSubAreas = (e.Object as Area)?.SubAreas;

                // valid subareas
                if ((areaSubAreas != null) && (countrySubAreas != null))
                {
                    // get the subareas belong to both selected area and selected country
                    if (GetCountrySubAreas(areaSubAreas, countrySubAreas) is List<SubArea> matchingSubareas)
                    {
                        // refresh control
                        RefreshControl(matchingSubareas.OfType<Location>().ToList(), SubAreaControl);
                    }
                }
            }
            else
            {
                RefreshControl(_allSubAreas.OfType<Location>().ToList(), SubAreaControl);
            }

            // derived class can set the view model property
            // take area name from area object (if in database) or from UI text
            SetAreaDisplayItem(e.Object is Area area? area.Name : AreaControl.GetCurrentText());

            // set focus to the respective text box
            SubAreaControl.SetFocus();
        }
        
        protected virtual void AreaControl_Leaving_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            CountryControl.SetFocus();
        }

        protected virtual void SubAreaControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            if (SubjectLocationControl.ListBoxReadOnly)
            {
                // get the country / area / subarea names from UI
                var countryName = Window.SettingsCountryControl.GetCurrentText();
                var areaName = Window.SettingsAreaControl.GetCurrentText();
                var subAreaName = Window.SettingsSubAreaControl.GetCurrentText();

                // get country object from UI
                if (Window.SettingsCountryControl.GetCurrentObject() is Country country)
                {
                    // find the corresponding subject location (need to match country, area and subarea)
                    var subjectLocations = country.SubjectLocations.Where(c => c.Country.Name == countryName && c.Area.Name == areaName && c.SubArea.Name == subAreaName).ToList();

                    // valid subject Locations
                    if (subjectLocations != null)
                    {
                        // refresh control
                        RefreshControl(subjectLocations.OfType<Location>().ToList(), SubjectLocationControl);
                    }
                }
            }
            else
            {
                RefreshControl(_allSubjectLocations.OfType<Location>().ToList(), SubjectLocationControl);
            }

            // take subarea name from subarea object (if in database) or from UI text
            SetSubAreaDisplayItem(e.Object is SubArea subArea ? subArea.Name : SubAreaControl.GetCurrentText());

            SubjectLocationControl.SetFocus();
        }

        protected virtual void SubAreaControl_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            AreaControl.SetFocus();
        }

        protected virtual void SubjectLocationControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            // take subject location name from subject location object (if in database) or from UI text
            SetSubjectLocationDisplayItem(e.Object is SubjectLocation subjectLocation ? subjectLocation.Name : SubjectLocationControl.GetCurrentText());
        }

        protected virtual void SetCountryDisplayItem(string countryName)
        {
            // overrride if want to set the view model attributes for databinding
        }

        protected virtual void SetAreaDisplayItem(string areaName)
        {
            // overrride if want to set the view model attributes for databinding
        }

        protected virtual void SetSubAreaDisplayItem(string subAreaName)
        {
            // overrride if want to set the view model attributes for databinding
        }

        protected virtual void SetSubjectLocationDisplayItem(string subjectLocationName)
        {
            // overrride if want to set the view model attributes for databinding
        }

        private List<SubArea> GetCountrySubAreas(List<SubArea> subAreas1, List<SubArea> subAreas2)
        {
            // intersects the two list and return the result list containing intersecting areas of both lists
            return subAreas1.Select(a => a)
                            .Intersect(subAreas2.Select(b => new SubArea { Id = b.Id, Areas = b.Areas, Name = b.Name, Countries = b.Countries }))
                            .ToList();
        }
        #endregion
    }
}
