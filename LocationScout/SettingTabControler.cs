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
using static LocationScout.DataAccess.PersistenceManager;

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
            E_DBReturnCode success = DataAccessAdapter.SmartAddCountry(countryName, areaName, subAreaName, out string errorMessage);

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

        internal void Edit()
        {
            if (_currentMode == E_Mode.add)
            {
                _currentEditMode = DoEdit();

                if (_currentEditMode != E_EditMode.no_edit)
                {
                    Window.SettingsEditButton.Content = "Save";
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
                if (_mainControler.RefreshAllCountries(out string errorMessage) == E_DBReturnCode.error)
                {
                    ShowMessage("Error reading countries from database." + errorMessage, E_MessageType.error);
                }
                _mainControler.RefreshCountryControls();

                Window.SettingsCountryControl.ClearText();
                Window.SettingsAreaControl.ClearText();
                Window.SettingsSubAreaControl.ClearText();

                // reset the mode
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
                    // to do
                    break;

                case E_EditMode.edit_subarea:
                    // to do
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

        private void SaveEditedCountryName()
        {
            // save the edits
            var country = Window.SettingsCountryControl.GetCurrentObject() as Country;
            var newCountryName = new TextRange(Window.SettingsCountryControlEdit.Document.ContentStart, Window.SettingsCountryControlEdit.Document.ContentEnd).Text;
            newCountryName = newCountryName.TrimEnd('\r', '\n');

            if (DataAccessAdapter.EditCountryName(country.Id, newCountryName, out string errorMessage) == E_DBReturnCode.no_error)
            {
                ShowMessage("Country name successfully changed.", E_MessageType.info);
            }
            else
            {
                ShowMessage("Error editing country name." + errorMessage, E_MessageType.error);
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