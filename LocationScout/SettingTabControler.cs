using LocationScout.DataAccess;
using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using WPFUserControl;

namespace LocationScout
{
    internal class SettingTabControler : TabControlerBase
    {
        #region enums
        private enum E_Mode { add, edit};
        private enum E_EditMode { no_edit, edit_country, edit_area, edit_subarea};
        #endregion

        #region attributes
        private MainWindowControler _mainControler;
        private E_Mode _currentMode;
        private E_EditMode _currentEditMode;

        public override AutoCompleteTextBox CountryControl { get { return Window.SettingsCountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return Window.SettingsAreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return Window.SettingsSubAreaControl; } }
        #endregion

        #region constructors
        public SettingTabControler(MainWindowControler mainControler, MainWindow window) : base(window)
        {            
            _mainControler = mainControler;
            _currentMode = E_Mode.add;

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
            PersistenceManager.E_DBReturnCode success = DataAccessAdapter.SmartAddCountry(countryName, areaName, subAreaName, out string errorMessage);

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

        internal void Delete()
        {
            // check which controls have text
            var hasCountryText = !string.IsNullOrEmpty(Window.SettingsCountryControl.GetCurrentText());
            var hasAreaText = !string.IsNullOrEmpty(Window.SettingsAreaControl.GetCurrentText());
            var hasSubAreaText = !string.IsNullOrEmpty(Window.SettingsSubAreaControl.GetCurrentText());

            // only country control has text, so edit country
            if (hasCountryText && !hasAreaText && !hasSubAreaText)
            {
                var countryFromUI = Window.SettingsCountryControl.GetCurrentObject() as Country;

                if (PersistenceManager.ReadCountry(countryFromUI.Id, out Country countryFromDB, out string errorMessage) == PersistenceManager.E_DBReturnCode.error)
                {
                    ShowMessage("Error reading country information." + errorMessage, E_MessageType.error);
                }

                var countryAreaCount = countryFromDB.Areas.Count;
                var countrySubAreaCount = countryFromDB.SubAreas.Count;
                //var t = countryFromDB.SubjectLocations

                var settingDisplayItem = new ViewModel.SettingDisplayItem()
                {
                    CountryAreaCountToDelete = countryAreaCount,
                    CountrySubAreaCountToDelete = countrySubAreaCount,
                    CountryPhotoPlaceCountToDelete = -1,
                    AreaSubAreaCountToDelete = -1,
                    AreaPhotoPlaceCountToDelete = -1,
                    SubAreaPhotoPlaceCountToDelete = -1
                };

                SettingsDeleteWindow deletingWindow = new SettingsDeleteWindow(settingDisplayItem);

                deletingWindow.ShowDialog();
            }

            // country and area control have text, so edit area
            else if (hasCountryText && hasAreaText && !hasSubAreaText)
            {

            }

            // all controls have text, so edit subarea
            else if (hasSubAreaText && hasAreaText && hasSubAreaText)
            {

            }
        }

        private void AfterDBWriteSteps(PersistenceManager.E_DBReturnCode success, string errorMessage)
        {
            switch (success)
            {
                case PersistenceManager.E_DBReturnCode.no_error:
                {
                    // read the countries again from database and update _allCountries
                    if (_mainControler.RefreshAllCountries(out errorMessage) == PersistenceManager.E_DBReturnCode.error)
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

                case PersistenceManager.E_DBReturnCode.error:
                    ShowMessage("Error writing data.\n" + errorMessage, E_MessageType.error);
                    break;

                case PersistenceManager.E_DBReturnCode.already_existing:
                    ShowMessage("Already existing in database.\n" + errorMessage, E_MessageType.info);
                    break;

                default:
                    // unknown enum
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        internal void Edit()
        {
            // currently in "add" mode...
            if (_currentMode == E_Mode.add)
            {
                // check if editing is possible and switch controls
                _currentEditMode = DoEdit();

                // edit is possible
                if (_currentEditMode != E_EditMode.no_edit)
                {
                    // change button label
                    Window.SettingsEditButton.Content = "Save";

                    // switch to edit mode
                    _currentMode = E_Mode.edit;
                }
            }
            else
            {
                // set control state
                Window.SettingsCountryControlEdit.Visibility = Visibility.Hidden;
                Window.SettingsCountryControl.Visibility = Visibility.Visible;
                Window.SettingsAreaControlEdit.Visibility = Visibility.Hidden;
                Window.SettingsAreaControl.Visibility = Visibility.Visible;
                Window.SettingsSubAreaControlEdit.Visibility = Visibility.Hidden;
                Window.SettingsSubAreaControl.Visibility = Visibility.Visible;
                Window.SettingsEditButton.Content = "Edit";
                Window.SettingsCountryControl.IsEnabled = true;
                Window.SettingsAreaControl.IsEnabled = true;
                Window.SettingsSubAreaControl.IsEnabled = true;

                // save the changes to the database
                SaveEditChanges();

                // refresh the controls with the changed text
                if (_mainControler.RefreshAllCountries(out string errorMessage) == PersistenceManager.E_DBReturnCode.error)
                {
                    ShowMessage("Error reading countries from database." + errorMessage, E_MessageType.error);
                }
                _mainControler.RefreshCountryControls();

                // clear the controls
                Window.SettingsCountryControl.ClearText();
                Window.SettingsAreaControl.ClearText();
                Window.SettingsSubAreaControl.ClearText();

                // reset the modes
                _currentMode = E_Mode.add;
                _currentEditMode = E_EditMode.no_edit;
            }
        }

        private void SaveEditChanges()
        {
            switch (_currentEditMode)
            {
                case E_EditMode.no_edit:
                    Debug.Assert(false);
                    throw new Exception("Wrong enum value in SettingTabControler::SaveEditChanges()");

                case E_EditMode.edit_country:
                    SaveEditedCountryName();
                    break;

                case E_EditMode.edit_area:
                    SaveEditedAreaName();
                    break;

                case E_EditMode.edit_subarea:
                    SaveEditedSubAreaName();
                    break;

                default:
                    Debug.Assert(false);
                    throw new Exception("Unknown enum value in SettingTabControler::SaveEditChanges()");
            }
        }

        private void SwitchEditModeCountry()
        {
            // hide the autocomplete textbox and show the edit textbox
            Window.SettingsCountryControlEdit.Visibility = Visibility.Visible;
            Window.SettingsCountryControl.Visibility = Visibility.Hidden;

            // disable textboxes not edited
            Window.SettingsAreaControl.IsEnabled = false;
            Window.SettingsSubAreaControl.IsEnabled = false;

            // set text to edit control
            var editedText = Window.SettingsCountryControl.GetCurrentText();
            Window.SettingsCountryControlEdit.Document.Blocks.Clear();
            Window.SettingsCountryControlEdit.Document.Blocks.Add(new Paragraph(new Run(editedText)));
        }

        private void SaveEditedAreaName()
        {
            // save the edits
            var area = Window.SettingsAreaControl.GetCurrentObject() as Area;
            var newAreaName = new TextRange(Window.SettingsAreaControlEdit.Document.ContentStart, Window.SettingsAreaControlEdit.Document.ContentEnd).Text;
            newAreaName = newAreaName.TrimEnd('\r', '\n');

            // write only to database if there was a change
            if (area.Name != newAreaName)
            {

                if (DataAccessAdapter.EditAreaName(area.Id, newAreaName, out string errorMessage) == PersistenceManager.E_DBReturnCode.no_error)
                {
                    ShowMessage("Area name successfully changed.", E_MessageType.info);
                }
                else
                {
                    ShowMessage("Error editing area name." + errorMessage, E_MessageType.error);
                }
            }

            else
            {
                ShowMessage("No change done.", E_MessageType.info);
            }
        }

        private void SaveEditedSubAreaName()
        {
            // save the edits
            var subArea = Window.SettingsSubAreaControl.GetCurrentObject() as SubArea;
            var newSubAreaName = new TextRange(Window.SettingsSubAreaControlEdit.Document.ContentStart, Window.SettingsSubAreaControlEdit.Document.ContentEnd).Text;
            newSubAreaName = newSubAreaName.TrimEnd('\r', '\n');

            // write only to database if there was a change
            if (subArea.Name != newSubAreaName)
            {

                if (DataAccessAdapter.EditSubAreaName(subArea.Id, newSubAreaName, out string errorMessage) == PersistenceManager.E_DBReturnCode.no_error)
                {
                    ShowMessage("Subarea name successfully changed.", E_MessageType.info);
                }
                else
                {
                    ShowMessage("Error editing Subarea name." + errorMessage, E_MessageType.error);
                }
            }

            else
            {
                ShowMessage("No change done.", E_MessageType.info);
            }
        }

        private void SaveEditedCountryName()
        {
            // save the edits
            var country = Window.SettingsCountryControl.GetCurrentObject() as Country;
            var newCountryName = new TextRange(Window.SettingsCountryControlEdit.Document.ContentStart, Window.SettingsCountryControlEdit.Document.ContentEnd).Text;
            newCountryName = newCountryName.TrimEnd('\r', '\n');

            // write only to database if there was a change
            if (country.Name != newCountryName)
            {

                if (DataAccessAdapter.EditCountryName(country.Id, newCountryName, out string errorMessage) == PersistenceManager.E_DBReturnCode.no_error)
                {
                    ShowMessage("Country name successfully changed.", E_MessageType.info);
                }
                else
                {
                    ShowMessage("Error editing country name." + errorMessage, E_MessageType.error);
                }
            }

            else
            {
                ShowMessage("No change done.", E_MessageType.info);
            }
        }

        private E_EditMode DoEdit()
        {
            // return the edit mode
            E_EditMode editmode = E_EditMode.no_edit;

            // check which controls have text
            var hasCountryText = !string.IsNullOrEmpty(Window.SettingsCountryControl.GetCurrentText());
            var hasAreaText = !string.IsNullOrEmpty(Window.SettingsAreaControl.GetCurrentText());
            var hasSubAreaText = !string.IsNullOrEmpty(Window.SettingsSubAreaControl.GetCurrentText());

            // only country control has text, so edit country
            if (hasCountryText && !hasAreaText && !hasSubAreaText)
            {
                SwitchEditModeCountry();

                editmode = E_EditMode.edit_country;
            }

            // country and area control have text, so edit area
            else if (hasCountryText && hasAreaText && !hasSubAreaText)
            {
                // hide the autocomplete textbox and show the edit textbox
                Window.SettingsAreaControlEdit.Visibility = Visibility.Visible;
                Window.SettingsAreaControl.Visibility = Visibility.Hidden;

                // disable textboxes not edited
                Window.SettingsCountryControl.IsEnabled = false;
                Window.SettingsSubAreaControl.IsEnabled = false;

                // set text to edit control
                var editedText = Window.SettingsAreaControl.GetCurrentText();
                Window.SettingsAreaControlEdit.Document.Blocks.Clear();
                Window.SettingsAreaControlEdit.Document.Blocks.Add(new Paragraph(new Run(editedText)));

                editmode = E_EditMode.edit_area;
            }

            // all controls have text, so edit subarea
            else if (hasSubAreaText && hasAreaText && hasSubAreaText)
            {
                // hide the autocomplete textbox and show the edit textbox
                Window.SettingsSubAreaControlEdit.Visibility = Visibility.Visible;
                Window.SettingsSubAreaControl.Visibility = Visibility.Hidden;

                // disable textboxes not edited
                Window.SettingsCountryControl.IsEnabled = false;
                Window.SettingsAreaControl.IsEnabled = false;

                // set text to edit control
                var editedText = Window.SettingsSubAreaControl.GetCurrentText();
                Window.SettingsSubAreaControlEdit.Document.Blocks.Clear();
                Window.SettingsSubAreaControlEdit.Document.Blocks.Add(new Paragraph(new Run(editedText)));

                editmode = E_EditMode.edit_subarea;
            }

            return editmode;
        }

        private void SubAreaControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Window.SettingsAddButton.Focus();
        }
        #endregion        
    }
}