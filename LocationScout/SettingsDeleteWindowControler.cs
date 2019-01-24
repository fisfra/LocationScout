﻿using LocationScout.DataAccess;
using LocationScout.Model;
using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout
{
    public class SettingsDeleteWindowControler : ControlerBase
    {
        #region attributes
        private SettingsDeleteWindow _deleteWindow;
        #endregion

        #region constructor
        public SettingsDeleteWindowControler(SettingsDeleteWindow deleteWindow, MainWindow window) : base(window)
        {
            _deleteWindow = deleteWindow;
        }
        #endregion

        #region methods
        internal void InitializeDialog()
        {
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
            if (hasCountryText && !hasAreaText && !hasSubAreaText)
            {
                if (countryInDB)
                {
                    SetDeleteDialogCountryInformation(_deleteWindow.DisplayItem);

                    _deleteWindow.AreaRadioButton.IsEnabled = false;
                    _deleteWindow.AreaTextBlock.IsEnabled = false;
                    _deleteWindow.SubAreaTextRadioButton.IsEnabled = false;
                    _deleteWindow.SubAreaTextBlock.IsEnabled = false;
                }
            }

            // country and area control have text, so edit area
            else if (hasCountryText && hasAreaText && !hasSubAreaText)
            {
                if (countryInDB)
                {
                    SetDeleteDialogCountryInformation(_deleteWindow.DisplayItem);

                    _deleteWindow.SubAreaTextRadioButton.IsEnabled = false;
                    _deleteWindow.SubAreaTextBlock.IsEnabled = false;
                }
                else
                {
                    _deleteWindow.CountryRadioButton.IsEnabled = false;
                    _deleteWindow.CountryTextBlock.IsEnabled = false;
                }

                if (areaInDB)
                {
                    SetDeleteDialogAreaInformation(_deleteWindow.DisplayItem);

                    _deleteWindow.SubAreaTextRadioButton.IsEnabled = false;
                    _deleteWindow.SubAreaTextBlock.IsEnabled = false;
                }
                else
                {
                    _deleteWindow.AreaRadioButton.IsEnabled = false;
                    _deleteWindow.AreaTextBlock.IsEnabled = false;
                }
            }

            // all controls have text, so edit subarea
            else if (hasSubAreaText && hasAreaText && hasSubAreaText)
            {
                if (countryInDB)
                {
                    SetDeleteDialogCountryInformation(_deleteWindow.DisplayItem);
                }

                if (areaInDB)
                {
                    SetDeleteDialogAreaInformation(_deleteWindow.DisplayItem);
                }

                if (subAreaInDB)
                {
                    SetDeleteDialogSubAreaInformation(_deleteWindow.DisplayItem);
                }
            }
        }

        private void SetDeleteDialogCountryInformation(SettingsDeleteDisplayItem displayItem)
        {
            // get country from UI
            if (Window.SettingsCountryControl.GetCurrentObject() is Country countryFromUI)
            {
                // get count of deleted areas / subareas
                var countryAreaCount = 0;
                var countrySubAreaCount = 0;

                // loop through all areas
                foreach (var area in countryFromUI.Areas)
                {
                    // read addtional area informtion from database
                    var success = PersistenceManager.ReadArea(area.Id, out Area areaFullInformation, out string errorMessage);
                    if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading area information." + errorMessage, E_MessageType.error);

                    // area has one one country, so it can be deleted
                    if (areaFullInformation.Countries.Count == 1) countryAreaCount++;

                    // loop through all subareas of current area read from database with full information
                    foreach (var subarea in areaFullInformation.SubAreas)
                    {
                        // read additonal subarea information from database
                        success = PersistenceManager.ReadSubArea(subarea.Id, out SubArea subAreaFullInformation, out errorMessage);
                        if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading subarea information." + errorMessage, E_MessageType.error);

                        // subarea has only one area, so it can be deleted
                        if (subAreaFullInformation.Areas.Count == 1) countrySubAreaCount++;
                    }

                    // only delete areas that have no further countries
                    if (areaFullInformation.Countries.Count == 1)
                    {
                        countryAreaCount++;
                    }
                }

                // subject location has only one country
                var countrySubjectLocationCount = countryFromUI.SubjectLocations.Count;

                displayItem.CountryAreaCountToDelete = countryAreaCount;
                displayItem.CountrySubAreaCountToDelete = countrySubAreaCount;
                displayItem.CountrySubjectLocationCountToDelete = countrySubjectLocationCount;
            }
        }

        private void SetDeleteDialogAreaInformation(SettingsDeleteDisplayItem displayItem)
        {
            var areaFromUI = Window.SettingsAreaControl.GetCurrentObject() as Area;

            if (PersistenceManager.ReadArea(areaFromUI.Id, out Area areaFromDB, out string errorMessage) == PersistenceManager.E_DBReturnCode.error)
            {
                ShowMessage("Error reading area information." + errorMessage, E_MessageType.error);
            }

            // additional information (countries assigned to areas) need to be read from database
            var areaSubAreaCount = areaFromDB.SubAreas.Count;
            foreach (var sa in areaFromDB.SubAreas)
            {
                if (PersistenceManager.ReadSubArea(sa.Id, out SubArea subAreaFromDB, out errorMessage) == PersistenceManager.E_DBReturnCode.error)
                {
                    ShowMessage("Error reading subarea information." + errorMessage, E_MessageType.error);
                }

                // onll delete subareas that have no further countries
                if (subAreaFromDB.Countries.Count > 1) areaSubAreaCount--;
            }

            // subject location has only one country
            var countrySubjectLocationCount = areaFromDB.SubjectLocations.Count;

            displayItem.AreaSubAreaCountToDelete = areaSubAreaCount;
            displayItem.AreaPhotoPlaceCountToDelete = countrySubjectLocationCount;
        }

        private void SetDeleteDialogSubAreaInformation(SettingsDeleteDisplayItem displayItem)
        {
            var subAreaFromUI = Window.SettingsSubAreaControl.GetCurrentObject() as SubArea;

            if (PersistenceManager.ReadSubArea(subAreaFromUI.Id, out SubArea subAreaFromDB, out string errorMessage) == PersistenceManager.E_DBReturnCode.error)
            {
                ShowMessage("Error reading subarea information." + errorMessage, E_MessageType.error);
            }

            // subject location has only one country
            var countrySubjectLocationCount = subAreaFromDB.SubjectLocation.Count;

            displayItem.SubAreaPhotoPlaceCountToDelete = countrySubjectLocationCount;
        }
        #endregion
    }
}