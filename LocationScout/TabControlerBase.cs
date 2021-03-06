﻿using LocationScout.DataAccess;
using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using WPFUserControl;

namespace LocationScout
{
    internal abstract class TabControlerBase : ControlerBase
    {
        #region enums
        protected enum E_Mode { add, edit };
        protected enum E_EditMode { no_edit, edit_country, edit_area, edit_subarea, edit_subjectlocation, edit_shootinglocation, edit_parkinglocation };
        #endregion

        #region attributes
        private E_Mode _currentMode;

        private List<Area> _allAreas;
        private List<SubArea> _allSubAreas;
        private List<SubjectLocation> _allSubjectLocations;

        protected MainWindowControler MainControler { get; private set; }
        protected E_EditMode CurrentEditMode { get; set; }

        public abstract AutoCompleteTextBox CountryControl { get; }
        public abstract AutoCompleteTextBox AreaControl { get; }
        public abstract AutoCompleteTextBox SubAreaControl { get; }
        public abstract AutoCompleteTextBox SubjectLocationControl { get; }
        public abstract TextBox SubjectLocationLatitudeControl { get; }
        public abstract TextBox SubjectLocationLongitudeControl { get; }

        protected abstract Button EditButton { get; }
        protected abstract Button AddButton { get; }
        protected abstract Button DeleteButton { get; }
        protected abstract E_EditMode DoEdit();
        protected abstract void SaveEditChanges();
        protected abstract void CheckGPSPaste(TextBox textbox, List<string> coordinates);
        internal abstract void ReloadAndRefreshControls();
        #endregion

        #region constructors
        public TabControlerBase(MainWindow window, MainWindowControler mainControler) : base(window)
        {
            MainControler = mainControler;

            // register events
            CountryControl.Leaving += CountryControl_Leaving;
            AreaControl.Leaving += AreaControl_Leaving;
            AreaControl.LeavingViaShift += AreaControl_Leaving_LeavingViaShift;
            SubAreaControl.Leaving += SubAreaControl_Leaving;
            SubAreaControl.LeavingViaShift += SubAreaControl_LeavingViaShift;
            SubjectLocationControl.Leaving += SubjectLocationControl_Leaving;
            SubjectLocationControl.LeavingViaShift += SubjectLocationControl_LeavingViaShift;

            _currentMode = E_Mode.add;
        }
        #endregion

        #region methods

        #region internal methods
        internal virtual void HandleClipboardChange(string clipboardText)
        {
            // get focussed control (even if the whole window is not in focus)
            IInputElement focusedControl = FocusManager.GetFocusedElement(Window);

            // textbox focused
            if (focusedControl is TextBox textbox)
            {
                var coordinates = AnalyzeGPSText(clipboardText);

                if (coordinates.Count == 1)
                {
                    textbox.Text = clipboardText;
                }

                else if (coordinates.Count == 2)
                {
                    // override in derived class
                    CheckGPSPaste(textbox, coordinates);
                }
            }
        }

        internal virtual void RefreshAllObjectsFromDB(bool fullrefresh = true)
        {
            // base class might use parameter
            if (fullrefresh)
            {
                RefreshAllAreasFromDB();
                RefreshAllSubAreasFromDB();
                RefreshAllSubjectLocationsFromDB();
            }
        }

        internal virtual void Edit()
        {
            // currently in "add" mode...
            if (_currentMode == E_Mode.add)
            {
                // check if editing is possible and switch controls
                CurrentEditMode = DoEdit();

                // edit is possible
                if (CurrentEditMode != E_EditMode.no_edit)
                {
                    // update button state / label
                    UpdateButtons();

                    // switch to edit mode
                    _currentMode = E_Mode.edit;
                }
            }
            else
            {
                // set control state
                ResetControlState();

                // save the changes to the database
                SaveEditChanges();

                // ask maincontroler to reload data from database and refresh all controls
                // maincontroler needs to take care of this since it not only settings tab
                MainControler.ReloadAndRefreshControls();

                // reset the modes
                _currentMode = E_Mode.add;
                CurrentEditMode = E_EditMode.no_edit;
            }
        }
        #endregion

        #region protected methods
        protected virtual void UpdateButtons()
        {
            EditButton.Content = "Save";
            AddButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        protected virtual void CountryControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            // if area control is ready only, only area of the assigned country can be selected
            if (AreaControl.ListBoxReadOnly)
            {
                RefreshControl((e.Object as Country)?.Areas?.OfType<Location>().ToList(), AreaControl);
            }
            // otherwise all existing areas can be selected (or new onces can be entered)
            else
            {
                RefreshControl(_allAreas.OfType<Location>().ToList(), AreaControl);
            }

            // clear old text
            SubAreaControl.ClearText();
            SubjectLocationControl.ClearText();
            SubjectLocationLatitudeControl.Text = null;
            SubjectLocationLongitudeControl.Text = null;

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
                    if (GetCountrySubAreas(areaSubAreas.ToList(), countrySubAreas.ToList()) is List<SubArea> matchingSubareas)
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
                var countryName = CountryControl.GetCurrentText();
                var areaName = AreaControl.GetCurrentText();
                var subAreaName = SubAreaControl.GetCurrentText();

                // get country object from UI
                if (CountryControl.GetCurrentObject() is Country country)
                {
                    // find the corresponding subject location (need to match country, area and subarea)
                    var subjectLocations = country.SubjectLocations.Where(c => c.Country.Name == countryName && c.Area.Name == areaName && c.SubArea.Name == subAreaName).ToList();

                    // valid subject Locations
                    if (subjectLocations != null)
                    {
                        // refresh control
                        RefreshControl(subjectLocations?.OfType<Location>().ToList(), SubjectLocationControl);
                    }
                }
            }
            else
            {
                RefreshControl(_allSubjectLocations?.OfType<Location>().ToList(), SubjectLocationControl);
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
            // override if want to set the view model attributes for databinding
        }

        protected virtual void SwitchEditModeShootingLocation(AutoCompleteTextBox targetControl, RichTextBox targetEditControl, List<Control> otherControls)
        {
            // hide the autocomplete textbox and show the edit textbox
            targetEditControl.Visibility = Visibility.Visible;
            targetControl.Visibility = Visibility.Hidden;

            // disable the other controls
            DisableControls(otherControls);

            // set text to edit control
            var editedText = targetControl.GetCurrentText();
            targetEditControl.Document.Blocks.Clear();
            targetEditControl.Document.Blocks.Add(new Paragraph(new Run(editedText)));
        }

        protected virtual void DisableControls(List<Control> controls)
        {
            // disable controls that should not be edited
            foreach (var control in controls)
            {
                control.IsEnabled = false;
            }
        }

        protected virtual void ResetControlState()
        {
            EditButton.Content = "Edit";
            AddButton.IsEnabled = true;            
            DeleteButton.IsEnabled = true;
        }

        protected string GetTextFromRichEditControl(RichTextBox textbox)
        {
            var newText = new TextRange(textbox.Document.ContentStart, textbox.Document.ContentEnd).Text;
            newText = newText?.TrimEnd('\r', '\n');

            return newText;
        }

        protected virtual List<string> AnalyzeGPSText(string text)
        {
            var result = new List<string>();

            if (text != null)
            {
                // split text by blanks
                var split = text.Split(' ').ToList();

                // clean strings (remove spaces, comma, semicolon at beginning and end)
                for (var i = 0; i<split.Count; i++)
                {
                    split[i] = split[i].Trim(new char[] { ' ', ',', ';' });
                }


                // only valid if there are one or two text parts
                if ((split.Count == 1) || (split.Count == 2))
                {
                    // loop through split text
                    foreach (var coordinateAsText in split)
                    {
                        // check if it's a valid double value...
                        if (IsDouble(coordinateAsText))
                        {
                            //.. if so add to results
                            result.Add(coordinateAsText);
                        }
                    }
                }
            }

            return result;
        }

        protected bool IsDouble(string text)
        {
            return Double.TryParse(text, out double d);
        }
        #endregion

        #region private methods
        private List<SubArea> GetCountrySubAreas(List<SubArea> subAreas1, List<SubArea> subAreas2)
        {
            // intersects the two list and return the result list containing intersecting areas of both lists
            return subAreas1.Select(a => a)
                            .Intersect(subAreas2.Select(b => new SubArea { Id = b.Id, Areas = b.Areas, Name = b.Name, Countries = b.Countries }))
                            .ToList();
        }

        private void SubjectLocationControl_LeavingViaShift(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            SubAreaControl.SetFocus();
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
        #endregion

        #endregion
    }
}
