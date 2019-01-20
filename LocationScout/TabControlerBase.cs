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
        public abstract AutoCompleteTextBox CountryControl { get; }
        public abstract AutoCompleteTextBox AreaControl { get; }
        public abstract AutoCompleteTextBox SubAreaControl { get; }
        #endregion

        #region constructors
        public TabControlerBase(MainWindow window) : base (window)
        {
        }
        #endregion

        #region methods
        protected virtual void AreaControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            // get the subareas and let the base class do the work
            var countrySubAreas = (CountryControl.GetCurrentObject() as Country)?.SubAreas;
            var areaSubAreas = (e.Object as Area)?.Subareas;

            // valid subareas
            if ( (areaSubAreas != null) && (countrySubAreas != null) )
            {
                // get the subareas belong to both selected area and selected country
                if (GetCountrySubAreas(areaSubAreas, countrySubAreas) is List<SubArea> matchingSubareas)
                {
                    // refresh control
                    RefreshSubAreaConrol(matchingSubareas, SubAreaControl);
                }
            }

            // set focus to the respective text box
            SubAreaControl.SetFocus();
        }

        protected virtual void CountryControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            if (e.Object is Country country)
            {
                // areas belonging to country
                RefreshAreaControl(country.Areas, AreaControl);

                // clear old subarea text
                SubAreaControl.ClearText();
            }

            // set focus to the respective text box
            AreaControl.SetFocus();
        }

        protected virtual void AreaControl_Leaving_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            CountryControl.SetFocus();
        }

        protected virtual void SubAreaControl_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            AreaControl.SetFocus();
        }

        private void RefreshSubAreaConrol(List<SubArea> subAreas, AutoCompleteTextBox textBox)
        {
            if (subAreas != null) 
            {
                // clear old subareas and entered text
                textBox.ClearSearchPool();
                textBox.ClearText();

                foreach (var subArea in subAreas)
                {
                    textBox.AddObject(subArea.Name, subArea);
                }
            }
        }

        private void RefreshAreaControl(List<Area> areas, AutoCompleteTextBox textBox)
        {
            // there is a country in the respective textbox
            if (areas != null)
            {
                // clear the old areas and entered text
                textBox.ClearSearchPool();
                textBox.ClearText();

                // add the respective areas of the country
                foreach (var a in areas)
                {
                    textBox.AddObject(a.Name, a);
                }
            }
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
