using LocationScout.DataAccess;
using LocationScout.Model;
using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    internal class SettingTabControler : TabControlerBase
    {
        #region enums
        private enum E_Mode { add, edit};
        private enum E_EditMode { no_edit, edit_country, edit_area, edit_subarea, edit_subjectlocation};
        #endregion

        #region attributes
        private E_Mode _currentMode;
        private E_EditMode _currentEditMode;

        public override AutoCompleteTextBox CountryControl { get { return Window.SettingsCountryControl; } }
        //public override AutoCompleteTextBox CountryControl { get { return Window.SettingsCountryControl_New; } }
        public override AutoCompleteTextBox AreaControl { get { return Window.SettingsAreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return Window.SettingsSubAreaControl; } }
        public override AutoCompleteTextBox SubjectLocationControl { get { return Window.SettingsSubjectLocationControl; } }
        public override TextBox SubjectLocationLatitudeControl { get { return Window.SettingsSubjectLocationLatitute; } }
        public override TextBox SubjectLocationLongitudeControl { get { return Window.SettingsSubjectLocationLongitude; } }

        public SettingsDisplayItem DisplayItem { get; set; }
        #endregion

        #region constructors
        public SettingTabControler(MainWindowControler mainControler, MainWindow window) : base(window, mainControler)
        {            
            _currentMode = E_Mode.add;
            DisplayItem = new SettingsDisplayItem();

            Window.Settings_MaintainLocationGrid.DataContext = DisplayItem;

            // Test
            //Window.TestControl.AddObject("Test-1", "Test-1-Object");
            //Window.TestControl.AddObject("Test-2", "Test-2-Object");
        }
        #endregion

        #region methods      
        internal void Add()
        {
            // get values from UI
            string countryName = Window.SettingsCountryControl.GetCurrentText();
            string areaName = Window.SettingsAreaControl.GetCurrentText();
            string subAreaName = Window.SettingsSubAreaControl.GetCurrentText();
            string subjectLocationName = Window.SettingsSubjectLocationControl.GetCurrentText();

            // db operations might take a while
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            // add country to database
            PersistenceManager.E_DBReturnCode success = DataAccessAdapter.SmartAddCountry(DisplayItem, out string errorMessage);

            // show success or error message
            EvaluateDatabaseMessages(success, errorMessage);

            // reload data from database and refresh controls
            MainControler.ReloadAndRefreshControls();

            // reset cursor
            Mouse.OverrideCursor = null;

            Window.SettingsCountryControl.SetFocus();
        }

        public void ReloadAndRefreshControls()
        {
            Window.SettingsCountryControl.ClearText();
            Window.SettingsAreaControl.ClearText();
            Window.SettingsSubAreaControl.ClearText();
            Window.SettingsSubjectLocationControl.ClearText();
            Window.SettingsSubjectLocationLatitute.Text = string.Empty;
            Window.SettingsSubjectLocationLongitude.Text = string.Empty;
        }

        internal void Delete()
        {
            SettingsDeleteWindow deletingWindow = new SettingsDeleteWindow(Window);
            deletingWindow.ShowDialog();
        }

        private void EvaluateDatabaseMessages(PersistenceManager.E_DBReturnCode success, string errorMessage)
        {
            switch (success)
            {
                case PersistenceManager.E_DBReturnCode.no_error:
                    break;                

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
                Window.SettingsSubjectLocationControlEdit.Visibility = Visibility.Hidden;
                Window.SettingsSubjectLocationControl.Visibility = Visibility.Visible;
                Window.SettingsEditButton.Content = "Edit";
                Window.SettingsCountryControl.IsEnabled = true;
                Window.SettingsAreaControl.IsEnabled = true;
                Window.SettingsSubAreaControl.IsEnabled = true;
                Window.SettingsSubjectLocationControl.IsEnabled = true;
                Window.SettingsSubjectLocationLatitute.IsEnabled = true;
                Window.SettingsSubjectLocationLongitude.IsEnabled = true;

                // save the changes to the database
                SaveEditChanges();

                // reload data from database and refresh controls
                ReloadAndRefreshControls();

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
                case E_EditMode.edit_subjectlocation:
                    SaveEditedSubjectLocation();
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
            Window.SettingsSubjectLocationControl.IsEnabled = false;
            Window.SettingsSubjectLocationLatitute.IsEnabled = false;
            Window.SettingsSubjectLocationLongitude.IsEnabled = false;

            // set text to edit control
            var editedText = Window.SettingsCountryControl.GetCurrentText();
            Window.SettingsCountryControlEdit.Document.Blocks.Clear();
            Window.SettingsCountryControlEdit.Document.Blocks.Add(new Paragraph(new Run(editedText)));
        }

        private void SwitchEditModeArea()
        {
            // hide the autocomplete textbox and show the edit textbox
            Window.SettingsAreaControlEdit.Visibility = Visibility.Visible;
            Window.SettingsAreaControl.Visibility = Visibility.Hidden;

            // disable textboxes not edited
            Window.SettingsCountryControl.IsEnabled = false;
            Window.SettingsSubAreaControl.IsEnabled = false;
            Window.SettingsSubjectLocationControl.IsEnabled = false;
            Window.SettingsSubjectLocationLatitute.IsEnabled = false;
            Window.SettingsSubjectLocationLongitude.IsEnabled = false;

            // set text to edit control
            var editedText = Window.SettingsAreaControl.GetCurrentText();
            Window.SettingsAreaControlEdit.Document.Blocks.Clear();
            Window.SettingsAreaControlEdit.Document.Blocks.Add(new Paragraph(new Run(editedText)));
        }

        private void SwitchEditModeSubArea()
        {
            // hide the autocomplete textbox and show the edit textbox
            Window.SettingsSubAreaControlEdit.Visibility = Visibility.Visible;
            Window.SettingsSubAreaControl.Visibility = Visibility.Hidden;

            // disable textboxes not edited
            Window.SettingsCountryControl.IsEnabled = false;
            Window.SettingsAreaControl.IsEnabled = false;
            Window.SettingsSubjectLocationControl.IsEnabled = false;
            Window.SettingsSubjectLocationLatitute.IsEnabled = false;
            Window.SettingsSubjectLocationLongitude.IsEnabled = false;

            // set text to edit control
            var editedText = Window.SettingsSubAreaControl.GetCurrentText();
            Window.SettingsSubAreaControlEdit.Document.Blocks.Clear();
            Window.SettingsSubAreaControlEdit.Document.Blocks.Add(new Paragraph(new Run(editedText)));
        }

        private void SwitchEditModeSubjectLocation()
        {
            // hide the autocomplete textbox and show the edit textbox
            Window.SettingsSubjectLocationControlEdit.Visibility = Visibility.Visible;
            Window.SettingsSubjectLocationControl.Visibility = Visibility.Hidden;

            // disable textboxes not edited
            Window.SettingsCountryControl.IsEnabled = false;
            Window.SettingsAreaControl.IsEnabled = false;
            Window.SettingsSubAreaControl.IsEnabled = false;

            // set text to edit control
            var editedText = Window.SettingsSubjectLocationControl.GetCurrentText();
            Window.SettingsSubjectLocationControlEdit.Document.Blocks.Clear();
            Window.SettingsSubjectLocationControlEdit.Document.Blocks.Add(new Paragraph(new Run(editedText)));
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

        private void SaveEditedSubjectLocation()
        {
            // save the edits
            var subLocation = Window.SettingsSubjectLocationControl.GetCurrentObject() as SubjectLocation;
            var newSubjectLocationName = new TextRange(Window.SettingsSubjectLocationControlEdit.Document.ContentStart, Window.SettingsSubjectLocationControlEdit.Document.ContentEnd).Text;
            newSubjectLocationName = newSubjectLocationName.TrimEnd('\r', '\n');
            var newSubjectLocationGPS = new GPSCoordinates(DisplayItem.SubjectLocationLatitude, DisplayItem.SubjectLocationLongitude);

            // write only to database if there was a change
            if ( (subLocation.Name != newSubjectLocationName) || (subLocation.Coordinates.Latitude == newSubjectLocationGPS.Latitude) || (subLocation.Coordinates.Longitude == newSubjectLocationGPS.Longitude))
            {
                if (DataAccessAdapter.EditSubjectLocationName_GPS(subLocation.Id, newSubjectLocationName, newSubjectLocationGPS, out string errorMessage) == PersistenceManager.E_DBReturnCode.no_error)
                {
                    ShowMessage("Subject location data successfully changed.", E_MessageType.info);
                }
                else
                {
                    ShowMessage("Error editing Subject location data." + errorMessage, E_MessageType.error);
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
            //var newCountryName = new TextRange(Window.SettingsCountryControlEdit.Document.ContentStart, Window.SettingsCountryControlEdit.Document.ContentEnd).Text;
            //newCountryName = newCountryName.TrimEnd('\r', '\n');
            var newCountryName = DisplayItem.CountryName;

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

            // check which controls have text and objects (are in DB)
            var hasCountryText = !string.IsNullOrEmpty(Window.SettingsCountryControl.GetCurrentText());
            var countryInDB = Window.SettingsCountryControl.GetCurrentObject() != null;

            var hasAreaText = !string.IsNullOrEmpty(Window.SettingsAreaControl.GetCurrentText());
            var areaInDB = Window.SettingsAreaControl.GetCurrentObject() != null;

            var hasSubAreaText = !string.IsNullOrEmpty(Window.SettingsSubAreaControl.GetCurrentText());
            var subAreaInDB = Window.SettingsSubAreaControl.GetCurrentObject() != null;

            var hasSubjectLocationText = !string.IsNullOrEmpty(Window.SettingsSubjectLocationControl.GetCurrentText());
            var subjectLocationInDB = Window.SettingsSubjectLocationControl.GetCurrentObject() != null;

            // only country control has text, so edit country
            if (hasCountryText && !hasAreaText && !hasSubAreaText && !hasSubjectLocationText)
            {
                // edit only saved values
                if (countryInDB)
                {
                    SwitchEditModeCountry();

                    editmode = E_EditMode.edit_country;
                }
            }

            // country and area control have text, so edit area
            else if (hasCountryText && hasAreaText && !hasSubAreaText & !hasSubjectLocationText)
            {
                // edit only saved values
                if (countryInDB && areaInDB)
                {
                    SwitchEditModeArea();

                    editmode = E_EditMode.edit_area;
                }
            }

            // country, area and subarea control have text, so edit subarea
            else if (hasSubAreaText && hasAreaText && hasSubAreaText && !hasSubjectLocationText)
            {
                // edit only saved values
                if (countryInDB && areaInDB && subAreaInDB)
                {
                    SwitchEditModeSubArea();

                    editmode = E_EditMode.edit_subarea;
                }
            }

            // all controls have text, so edit subject location
            else if (hasSubAreaText && hasAreaText && hasSubAreaText && hasSubjectLocationText)
            {
                // edit only saved values
                if (countryInDB && areaInDB && subAreaInDB && subjectLocationInDB)
                {
                    SwitchEditModeSubjectLocation();

                    editmode = E_EditMode.edit_subjectlocation;
                }
            }

            // editing is not possible - probably user wants to edit values that are not added to DB yet
            else
            {
                ShowMessage("Editing not possible (you might have entered new values)", E_MessageType.info);
            }

            return editmode;
        }

        protected override void SubjectLocationControl_Leaving(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            base.SubjectLocationControl_Leaving(sender, e);

            DisplayItem.SubjectLocationLatitude = (e.Object as SubjectLocation)?.Coordinates?.Latitude;
            DisplayItem.SubjectLocationLongitude = (e.Object as SubjectLocation)?.Coordinates?.Longitude;

            Window.SettingsSubjectLocationLatitute.Focus();
            Window.SettingsSubjectLocationLatitute.CaretIndex = Window.SettingsSubjectLocationLatitute.Text.Length;
        }

        protected override void SetCountryDisplayItem(string countryName)
        {
            DisplayItem.CountryName = countryName;
        }

        protected override void SetAreaDisplayItem(string areaName)
        {
            DisplayItem.AreaName = areaName;
        }

        protected override void SetSubAreaDisplayItem(string subAreaName)
        {
            DisplayItem.SubAreaName = subAreaName;
        }

        protected override void SetSubjectLocationDisplayItem(string subjectLocationName)
        {
            DisplayItem.SubjectLocationName = subjectLocationName;
        }

        protected override void SubAreaControl_Leaving(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            base.SubAreaControl_Leaving(sender, e);
            
            //DisplayItem.SubjectLocationLatitude = (Window.SettingsSubjectLocationControl.GetCurrentObject() as SubjectLocation)?.Coordinates?.Latitude;
            //DisplayItem.SubjectLocationLongitude = (Window.SettingsSubjectLocationControl.GetCurrentObject() as SubjectLocation)?.Coordinates?.Longitude;
        }
        
        #endregion        
    }
}