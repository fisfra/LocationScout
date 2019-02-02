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
    internal class SettingTabControler  : TabControlerBase
    {
        #region enums
        #endregion

        #region attributes
        public override AutoCompleteTextBox CountryControl { get { return Window.Settings_CountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return Window.Settings_AreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return Window.Settings_SubAreaControl; } }
        public override AutoCompleteTextBox SubjectLocationControl { get { return Window.Settings_SubjectLocationControl; } }
        public override TextBox SubjectLocationLatitudeControl { get { return Window.Settings_SubjectLocationLatituteTextBox; } }
        public override TextBox SubjectLocationLongitudeControl { get { return Window.Settings_SubjectLocationLongitudeTextBox; } }

        protected override Button EditButton { get { return Window.Settings_EditButton; } }
        protected override Button AddButton { get { return Window.Settings_AddButton; } }
        protected override Button DeleteButton { get { return Window.Settings_DeleteButton; } }

        public SettingsDisplayItem DisplayItem { get; set; }
        #endregion

        #region constructors
        public SettingTabControler(MainWindowControler mainControler, MainWindow window) : base(window, mainControler)
        {            
            DisplayItem = new SettingsDisplayItem();

            Window.Settings_MaintainLocationGrid.DataContext = DisplayItem;
        }
        #endregion

        #region methods      
        internal void Add()
        {
            // if the autocomplete textboxes are not left via TAB but by clicking directly on a button,
            // the display item is not udpated - so do this here as a workaround
            UpdateDisplayItem();

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

            Window.Settings_CountryControl.SetFocus();
        }

        private void UpdateDisplayItem()
        {
            DisplayItem.CountryName = Window.Settings_CountryControl.GetCurrentText();
            DisplayItem.AreaName = Window.Settings_AreaControl.GetCurrentText();
            DisplayItem.SubAreaName = Window.Settings_SubAreaControl.GetCurrentText();
            DisplayItem.SubjectLocationName = Window.Settings_SubjectLocationControl.GetCurrentText();
        }

        public override void ReloadAndRefreshControls()
        {
            Window.Settings_CountryControl.ClearText();
            Window.Settings_AreaControl.ClearText();
            Window.Settings_SubAreaControl.ClearText();
            Window.Settings_SubjectLocationControl.ClearText();
            Window.Settings_SubjectLocationLatituteTextBox.Text = string.Empty;
            Window.Settings_SubjectLocationLongitudeTextBox.Text = string.Empty;
        }

        internal void HandleSettingsAreaControlEditLostFocus()
        {
            DisplayItem.AreaName = GetTextFromRichEditControl(Window.Settings_AreaControlEdit);
        }

        internal void HandleSettingsCountryControlEditLostFocus()
        {
            DisplayItem.CountryName = GetTextFromRichEditControl(Window.Settings_CountryControlEdit);
        }

        internal void HandleSettingsSubjectLocationControlEditLostFocus()
        {
            DisplayItem.SubjectLocationName = GetTextFromRichEditControl(Window.Settings_SubjectLocationControlEdit);
        }

        internal void HandleSettingsSubAreaControlEditLostFocus()
        {
            DisplayItem.SubAreaName = GetTextFromRichEditControl(Window.Settings_SubAreaControlEdit);
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

        protected override void ResetControlState()
        {
            base.ResetControlState();

            Window.Settings_CountryControlEdit.Visibility = Visibility.Hidden;
            Window.Settings_CountryControl.Visibility = Visibility.Visible;
            Window.Settings_AreaControlEdit.Visibility = Visibility.Hidden;
            Window.Settings_AreaControl.Visibility = Visibility.Visible;
            Window.Settings_SubAreaControlEdit.Visibility = Visibility.Hidden;
            Window.Settings_SubAreaControl.Visibility = Visibility.Visible;
            Window.Settings_SubjectLocationControlEdit.Visibility = Visibility.Hidden;
            Window.Settings_SubjectLocationControl.Visibility = Visibility.Visible;
            Window.Settings_CountryControl.IsEnabled = true;
            Window.Settings_AreaControl.IsEnabled = true;
            Window.Settings_SubAreaControl.IsEnabled = true;
            Window.Settings_SubjectLocationControl.IsEnabled = true;
            Window.Settings_SubjectLocationLatituteTextBox.IsEnabled = true;
            Window.Settings_SubjectLocationLongitudeTextBox.IsEnabled = true;
        }

        protected override void SaveEditChanges()
        {
            switch (CurrentEditMode)
            {
                case E_EditMode.no_edit:
                case E_EditMode.edit_shootinglocation:
                case E_EditMode.edit_parkinglocation:
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
            var controlsToDisable = new List<Control>()
            {
                Window.Settings_AreaControl,
                Window.Settings_SubAreaControl,
                Window.Settings_SubjectLocationControl,
                Window.Settings_SubjectLocationLatituteTextBox,
                Window.Settings_SubjectLocationLongitudeTextBox
            };

            SwitchEditModeShootingLocation(Window.Settings_CountryControl, Window.Settings_CountryControlEdit, controlsToDisable);
        }

        private void SwitchEditModeArea()
        {
            var controlsToDisable = new List<Control>()
            {
                Window.Settings_CountryControl,
                Window.Settings_SubAreaControl,
                Window.Settings_SubjectLocationControl,
                Window.Settings_SubjectLocationLatituteTextBox,
                Window.Settings_SubjectLocationLongitudeTextBox
            };

            SwitchEditModeShootingLocation(Window.Settings_AreaControl, Window.Settings_AreaControlEdit, controlsToDisable);
        }

        private void SwitchEditModeSubArea()
        {
            var controlsToDisable = new List<Control>()
            {
                Window.Settings_CountryControl,
                Window.Settings_AreaControl,
                Window.Settings_SubjectLocationControl,
                Window.Settings_SubjectLocationLatituteTextBox,
                Window.Settings_SubjectLocationLongitudeTextBox
            };

            SwitchEditModeShootingLocation(Window.Settings_SubAreaControl, Window.Settings_SubAreaControlEdit, controlsToDisable);
        }

        private void SwitchEditModeSubjectLocation()
        {
            var controlsToDisable = new List<Control>()
            {
                Window.Settings_CountryControl,
                Window.Settings_AreaControl,
                Window.Settings_SubAreaControl
            };

            SwitchEditModeShootingLocation(Window.Settings_SubjectLocationControl, Window.Settings_SubjectLocationControlEdit, controlsToDisable);
        }

        private void SaveEditedAreaName()
        {
            // save the edits
            var area = Window.Settings_AreaControl.GetCurrentObject() as Area;
            var newAreaName = DisplayItem.AreaName;

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
            var subArea = Window.Settings_SubAreaControl.GetCurrentObject() as SubArea;
            var newSubAreaName = DisplayItem.SubAreaName;

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
            var subLocation = Window.Settings_SubjectLocationControl.GetCurrentObject() as SubjectLocation;
            var newSubjectLocationName = DisplayItem.SubjectLocationName;
            var newSubjectLocationGPS = new GPSCoordinates(DisplayItem.SubjectLocationLatitude, DisplayItem.SubjectLocationLongitude);

            // write only to database if there was a change
            if ( (subLocation.Name != newSubjectLocationName) || (subLocation.Coordinates.Latitude != newSubjectLocationGPS.Latitude) || (subLocation.Coordinates.Longitude != newSubjectLocationGPS.Longitude))
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
            var country = Window.Settings_CountryControl.GetCurrentObject() as Country;
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

        protected override E_EditMode DoEdit()
        {
            // return the edit mode
            E_EditMode editmode = E_EditMode.no_edit;

            // check which controls have text and objects (are in DB)
            var hasCountryText = !string.IsNullOrEmpty(Window.Settings_CountryControl.GetCurrentText());
            var countryInDB = Window.Settings_CountryControl.GetCurrentObject() != null;

            var hasAreaText = !string.IsNullOrEmpty(Window.Settings_AreaControl.GetCurrentText());
            var areaInDB = Window.Settings_AreaControl.GetCurrentObject() != null;

            var hasSubAreaText = !string.IsNullOrEmpty(Window.Settings_SubAreaControl.GetCurrentText());
            var subAreaInDB = Window.Settings_SubAreaControl.GetCurrentObject() != null;

            var hasSubjectLocationText = !string.IsNullOrEmpty(Window.Settings_SubjectLocationControl.GetCurrentText());
            var subjectLocationInDB = Window.Settings_SubjectLocationControl.GetCurrentObject() != null;

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

            Window.Settings_SubjectLocationLatituteTextBox.Focus();
            Window.Settings_SubjectLocationLatituteTextBox.CaretIndex = Window.Settings_SubjectLocationLatituteTextBox.Text.Length;
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