using LocationScout.Model;
using LocationScout.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using WPFUserControl;
using System.Linq;
using LocationScout.DataAccess;
using System.Windows.Input;
using static LocationScout.DataAccess.PersistenceManager;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows;

namespace LocationScout
{
    internal class LocationTabControler : TabControlerBase
    {
        #region enums
        public enum E_PhotoNumber { photo_1, photo_2, photo_3 };
        #endregion

        #region attributes        
        public override AutoCompleteTextBox CountryControl { get { return Window.Location_CountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return Window.Location_AreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return Window.Location_SubAreaControl; } }
        public override AutoCompleteTextBox SubjectLocationControl { get { return Window.Location_SubjectLocationControl; } }
        public override TextBox SubjectLocationLatitudeControl { get { return Window.Location_SubjectLocationLatituteTextBox; } }
        public override TextBox SubjectLocationLongitudeControl { get { return Window.Location_SubjectLocationLongitudeTextBox; } }

        protected override Button EditButton { get { return Window.Location_EditButton; } }
        protected override Button AddButton { get { return Window.Location_AddButton; } }
        protected override Button DeleteButton { get { return Window.Location_DeleteButton; } }
        protected override Label StatusLabel { get { return Window.StatusLabel; } }

        public LocationDisplayItem DisplayItem { get; set; }

        private List<ShootingLocation> _allShootingLocations;

        private List<ParkingLocation> _allParkingLocations;

        private LocationListerWindow _listerWindow;
        #endregion

        #region contructors
        public LocationTabControler(MainWindowControler mainControler, MainWindow window) : base(window, mainControler)
        {
            DisplayItem = new LocationDisplayItem();

            Window.LocationContentGrid.DataContext = DisplayItem;
            Window.ShootingLocationControl.Leaving += ShootingLocationNameControl_Leaving;
            Window.ParkingLocationControl.Leaving += ParkingLocationControl_Leaving;

            RefreshShootingLocationsFromDB();
            RefreshParkingLocationsFromDB();

            _listerWindow = new LocationListerWindow(base.Window, this);
        }
        #endregion

        #region methods

        #region internal methods
        internal void CloseListerWindow()
        {
            _listerWindow?.Close();
        }

        internal void Clear()
        {
            ClearUI();
            Window.Location_CountryControl.SetFocus(); 
        }

        internal override void ReloadAndRefreshControls()
        {
            ClearUI();

            RefreshShootLocationControl();
            RefreshParkingLocationControl();
        }

        internal override void RefreshAllObjectsFromDB(bool fullrefresh = true)
        {
            base.RefreshAllObjectsFromDB();

            RefreshShootingLocationsFromDB();
            RefreshParkingLocationsFromDB();
        }

        internal void ShowLister()
        {
            if (_listerWindow.Visibility != Visibility.Visible) _listerWindow.Show();
        }        

        internal void Add()
        {
            // error message
            string errorMessage = string.Empty;

            // data from UI
            var subjectLocation = Window.Location_SubjectLocationControl.GetCurrentObject() as SubjectLocation;
            var parkingLocation = Window.ParkingLocationControl.GetCurrentObject() as ParkingLocation;
            
            var shootingLocationName = DisplayItem.ShootingLocationName;

            // db operations might take a while
            Mouse.OverrideCursor = Cursors.Wait;

            // set parking location (if new parking location, initial value is -1)
            long parkingLocationId = parkingLocation != null ? parkingLocation.Id :- 1;

            // create new parking location if necessary
            if ( (parkingLocation == null) && (!string.IsNullOrEmpty(DisplayItem.ParkingLocationName)))
            {
                // get name of parking location from UI
                var parkingLocationName = DisplayItem.ParkingLocationName;

                var parkingLocationGPS = new GPSCoordinates(DisplayItem.ParkingLocationLatitude, DisplayItem.ParkingLocationLongitude);

                parkingLocationId = AddParkingLocation(parkingLocationName, parkingLocationGPS, out errorMessage);
                if (parkingLocationId == -1)
                {
                    ShowMessage("Error writing parking locations to database\n" + errorMessage, E_MessageType.error);
                    return;
                }
            }

            // add country to database
            var success = DataAccessAdapter.SmartAddShootingLocation(DisplayItem, subjectLocation.Id, parkingLocationId, out errorMessage);

            switch (success)
            {
                case E_DBReturnCode.no_error:
                    // update the lister view
                    AddListerDisplayItem(shootingLocationName);
                    ShowMessage("Photo Location successfully added", E_MessageType.success);
                    break;
                case E_DBReturnCode.error:
                    ShowMessage("Error adding PhotoLocation to database\n" + errorMessage, E_MessageType.error);
                    break;
                case E_DBReturnCode.already_existing:
                    ShowMessage("PhotoLocation already existing\n" + errorMessage, E_MessageType.info);
                    break;
                default:
                    Debug.Assert(false);
                    throw new Exception("Unknown enum value in LocationTabControler::Add");
            }

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and refresh controls
            ClearUI();
            RefreshShootingLocationsFromDB();
            RefreshParkingLocationsFromDB();
            RefreshShootLocationControl();
            RefreshParkingLocationControl();

            // reset cursor
            Mouse.OverrideCursor = null;

            Window.Location_CountryControl.SetFocus();
        }

        internal void HandleParkingLocationControlEditLostFocus()
        {
            DisplayItem.ParkingLocationName = GetTextFromRichEditControl(Window.ParkingLocationControlEdit);
        }

        internal void HandleShootingLocationControlEditLostFocus()
        {
            DisplayItem.ShootingLocationName = GetTextFromRichEditControl(Window.ShootingLocationControlEdit);
        }

        internal void HandleRemove(E_PhotoNumber photoNumber)
        {
            switch (photoNumber)
            {
                case E_PhotoNumber.photo_1:
                    DisplayItem.Photo_1 = null;
                    break;
                case E_PhotoNumber.photo_2:
                    DisplayItem.Photo_2 = null;
                    break;
                case E_PhotoNumber.photo_3:
                    DisplayItem.Photo_3 = null;
                    break;
                default:
                    Debug.Assert(false);
                    throw new Exception("Unkown enum value E_PhotoNumber in LocationTabControler::HandleRemove");
            }
        }

        internal void HandlePhotoUpload()
        {
            if (LoadPhoto() is BitmapImage newPhoto)
            {
                if (DisplayItem.Photo_1 == null)
                {
                    DisplayItem.Photo_1 = newPhoto;
                }

                else if (DisplayItem.Photo_2 == null)
                {
                    DisplayItem.Photo_2 = newPhoto;
                }

                else if (DisplayItem.Photo_3 == null)
                {
                    DisplayItem.Photo_3 = newPhoto;
                }
            }
        }

        internal void HandleGoogleMapsSubjectLocation()
        {
            var gps = new GPSCoordinates(DisplayItem.SubjectLocationLatitude, DisplayItem.SubjectLocationLongitude);
            GoogleMapsHelper.Go(gps);
        }        
        #endregion

        #region protected methods
        protected override void CheckGPSPaste(TextBox textbox, List<string> coordinates)
        {
            if ( (textbox.Name == Window.ShootingLocationLatitudeTextBox.Name) || (textbox.Name == Window.ShootingLocationLongitudeTextBox.Name))
            {
                Window.ShootingLocationLatitudeTextBox.Text = coordinates[0];
                Window.ShootingLocationLongitudeTextBox.Text = coordinates[1];
            }

            else if ((textbox.Name == Window.ParkingLocationLatitudeTextBox.Name) || (textbox.Name == Window.ParkingLocationLongitudeTextBox.Name))
            {
                Window.ParkingLocationLatitudeTextBox.Text = coordinates[0];
                Window.ParkingLocationLongitudeTextBox.Text = coordinates[1];
            }
        }

        protected override E_EditMode DoEdit()
        {
            // return the edit mode
            E_EditMode editmode = E_EditMode.no_edit;

            // check which controls have text and objects (are in DB)
            var hasShootingLocationText = !string.IsNullOrEmpty(Window.Location_SubjectLocationControl.GetCurrentText());
            var shootingLocationInDB = Window.Location_SubjectLocationControl.GetCurrentObject() != null;

            var hasParkingLocationText = !string.IsNullOrEmpty(Window.ParkingLocationControl.GetCurrentText());
            var parkingLocationInDB = Window.ParkingLocationControl.GetCurrentObject() != null;

            // only shooting location control has text, so edit shooting location
            if (hasShootingLocationText && !hasParkingLocationText)
            {
                // edit only saved values
                if (shootingLocationInDB)
                {
                    SwitchEditModeShootingLocation();

                    editmode = E_EditMode.edit_shootinglocation;
                }
            }

            // only parking locaton have text, so edit parking location
            else if (!hasShootingLocationText && hasParkingLocationText)
            {
                // edit only saved values
                if (parkingLocationInDB)
                {
                    SwitchEditModeParkingLocation();

                    editmode = E_EditMode.edit_parkinglocation;
                }
            }

            // both have text, so check focus
            else if (hasShootingLocationText && hasParkingLocationText)
            {
                if (shootingLocationInDB && !parkingLocationInDB) 
                {
                    SwitchEditModeShootingLocation();
                    editmode = E_EditMode.edit_shootinglocation;
                }

                else if (!shootingLocationInDB && parkingLocationInDB)
                {
                    SwitchEditModeParkingLocation();
                    editmode = E_EditMode.edit_parkinglocation;
                }

                else if (shootingLocationInDB && parkingLocationInDB)
                {
                    EditSelectionWindow window = new EditSelectionWindow();
                    window.ShowDialog();

                    if (window.DialogResult is true)
                    {
                        if (window.ShootingLocationRadioButton.IsChecked is true)
                        {
                            SwitchEditModeShootingLocation();
                            editmode = E_EditMode.edit_shootinglocation;
                        }
                        else if (window.ParkingLocationRadioButton.IsChecked is true)
                        {
                            SwitchEditModeParkingLocation();
                            editmode = E_EditMode.edit_parkinglocation;
                        }
                    }
                }
            }

            // editing is not possible - probably user wants to edit values that are not added to DB yet
            else
            {
                ShowMessage("Editing not possible (you might have entered new values)", E_MessageType.info);
            }


            return editmode;
        }

        protected override void SaveEditChanges()
        {
            switch (CurrentEditMode)
            {
                case E_EditMode.no_edit:
                case E_EditMode.edit_country:
                case E_EditMode.edit_area:
                case E_EditMode.edit_subarea:
                case E_EditMode.edit_subjectlocation:
                    Debug.Assert(false);
                    throw new Exception("Wrong enum value in LocationTabControler::SaveEditChanges()");

                case E_EditMode.edit_shootinglocation:
                    SaveEditedShootingLocation();
                    break;
                case E_EditMode.edit_parkinglocation:
                    SaveEditedParkingLocation();
                    break;
                default:
                    Debug.Assert(false);
                    throw new Exception("Unknown enum value in SettingTabControler::SaveEditChanges()"); ;
            }
        }

        protected override void ResetControlState()
        {
            base.ResetControlState();

            Window.ShootingLocationControl.Visibility = Visibility.Visible;
            Window.ShootingLocationControlEdit.Visibility = Visibility.Hidden;            
            Window.ParkingLocationControl.Visibility = Visibility.Visible;
            Window.ParkingLocationControlEdit.Visibility = Visibility.Hidden;
            Window.Location_CountryControl.IsEnabled = true;
            Window.Location_AreaControl.IsEnabled = true;
            Window.Location_SubAreaControl.IsEnabled = true;
            Window.Location_SubjectLocationControl.IsEnabled = true;
            Window.Location_SubjectLocationLatituteTextBox.IsEnabled = false;
            Window.Location_SubjectLocationLongitudeTextBox.IsEnabled = false;
            Window.ShootingLocationControl.IsEnabled = true;
            Window.ShootingLocationLatitudeTextBox.IsEnabled = true;
            Window.ShootingLocationLongitudeTextBox.IsEnabled = true;
            Window.ParkingLocationControl.IsEnabled = true;
            Window.ParkingLocationLatitudeTextBox.IsEnabled = true;
            Window.ParkingLocationLongitudeTextBox.IsEnabled = true;
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

        protected override void SubjectLocationControl_Leaving(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            // base class call
            base.SubjectLocationControl_Leaving(sender, e);

            // set the view model of the coordinates
            // DisplayItem.SubjectLocationName = (e.Object as SubjectLocation)?.Name;
            DisplayItem.SubjectLocationLatitude = (e.Object as SubjectLocation)?.Coordinates?.Latitude;
            DisplayItem.SubjectLocationLongitude = (e.Object as SubjectLocation)?.Coordinates?.Longitude;

            //
            RefreshShootLocationControl();

            // 
            Window.ShootingLocationControl.SetFocus();
        }
        #endregion

        #region private methods
        private void ClearUI()
        {
            Window.Location_CountryControl.ClearText();
            Window.Location_AreaControl.ClearText();
            Window.Location_SubAreaControl.ClearText();
            Window.Location_SubjectLocationControl.ClearText();
            Window.ShootingLocationControl.ClearText();
            Window.ParkingLocationControl.ClearText();
            
            DisplayItem.Reset();
        }

        private void RefreshAllParkingLocationsFromDB()
        {
            throw new NotImplementedException();
        }

        private void RefreshAllShootingLocationsFromDB()
        {
            throw new NotImplementedException();
        }

        private void AreaControl_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Window.Location_CountryControl.SetFocus();
        }

        private void RefreshShootingLocationsFromDB()
        {
            var success = DataAccessAdapter.ReadAllShootingLocations(out _allShootingLocations, out string errorMessage);
            if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading shooting locations\n" + errorMessage, E_MessageType.error);
        }

        private void RefreshParkingLocationsFromDB()
        {
            var success = DataAccessAdapter.ReadAllParkingLocations(out _allParkingLocations, out string errorMessage);
            if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading parking locations\n" + errorMessage, E_MessageType.error);
        }

        private void RefreshShootLocationControl()
        {
            RefreshControl(_allShootingLocations?.OfType<Location>()?.ToList(), Window.ShootingLocationControl);
        }

        private void RefreshParkingLocationControl()
        {
            RefreshControl(_allParkingLocations?.OfType<Location>()?.ToList(), Window.ParkingLocationControl);
        }

        private void SwitchEditModeParkingLocation()
        {
            var controlsToDisable = new List<Control>()
            {
                Window.Location_CountryControl,
                Window.Location_AreaControl,
                Window.Location_SubAreaControl,
                Window.Location_SubjectLocationControl,
                Window.Location_SubjectLocationLatituteTextBox,
                Window.Location_SubjectLocationLongitudeTextBox,
                Window.ShootingLocationControl,
                Window.ShootingLocationLatitudeTextBox,
                Window.ShootingLocationLongitudeTextBox
            };

            SwitchEditModeShootingLocation(Window.ParkingLocationControl, Window.ParkingLocationControlEdit, controlsToDisable);
        }

        private void SwitchEditModeShootingLocation()
        {
            var controlsToDisable = new List<Control>()
            {
                Window.Location_CountryControl,
                Window.Location_AreaControl,
                Window.Location_SubAreaControl,
                Window.Location_SubjectLocationControl,
                Window.Location_SubjectLocationLatituteTextBox,
                Window.Location_SubjectLocationLongitudeTextBox,
                Window.ParkingLocationControl,
                Window.ParkingLocationLatitudeTextBox,
                Window.ParkingLocationLongitudeTextBox
            };

            SwitchEditModeShootingLocation(Window.ShootingLocationControl, Window.ShootingLocationControlEdit, controlsToDisable);
        }

        private void SaveEditedParkingLocation()
        {
            // save the edits
            var parkingLocation = Window.ParkingLocationControl.GetCurrentObject() as ParkingLocation;
            var newParkingLocationName = DisplayItem.ParkingLocationName;
            var newParkingLocationGPS = new GPSCoordinates(DisplayItem.ParkingLocationLatitude, DisplayItem.ParkingLocationLongitude);

            // write only to database if there was a change
            if ((parkingLocation.Name != newParkingLocationName) || (parkingLocation.Coordinates.Latitude != newParkingLocationGPS.Latitude) || (parkingLocation.Coordinates.Longitude != newParkingLocationGPS.Longitude))
            {
                if (DataAccessAdapter.EditParkingLocationName_GPS(parkingLocation.Id, newParkingLocationName, newParkingLocationGPS, out string errorMessage) == PersistenceManager.E_DBReturnCode.no_error)
                {
                    ShowMessage("Parking location data successfully changed.", E_MessageType.info);

                    // synchronize the display items of the main window and the lister window
                    ViewModelManager.SynchronizeDisplayItems(DisplayItem, _listerWindow.Controler.CurrentDisplayItem);
                }
                else
                {
                    ShowMessage("Error editing Parking location data." + errorMessage, E_MessageType.error);
                }
            }
            else
            {
                ShowMessage("No change done.", E_MessageType.info);
            }
        }

        private void SaveEditedShootingLocation()
        {
            // save the edits
            var shootingLocation = Window.ShootingLocationControl.GetCurrentObject() as ShootingLocation;
            var newShootingLocationName = DisplayItem.ShootingLocationName;
            var newShootingLocationGPS = new GPSCoordinates(DisplayItem.ShootingLocationLatitude, DisplayItem.ShootingLocationLongitude);

            var newPhoto1 = ImageTools.BitmapImageToByteArray(DisplayItem.Photo_1);
            var newPhoto2 = ImageTools.BitmapImageToByteArray(DisplayItem.Photo_2);
            var newPhoto3 = ImageTools.BitmapImageToByteArray(DisplayItem.Photo_3);

            var oldPhoto1 = shootingLocation.Photos.ElementAtOrDefault(0)?.ImageBytes;
            var oldPhoto2 = shootingLocation.Photos.ElementAtOrDefault(1)?.ImageBytes;
            var oldPhoto3 = shootingLocation.Photos.ElementAtOrDefault(2)?.ImageBytes;


            // write only to database if there was a change
            if ((shootingLocation.Name != newShootingLocationName) || 
                (shootingLocation.Coordinates.Latitude != newShootingLocationGPS.Latitude) || 
                (shootingLocation.Coordinates.Longitude != newShootingLocationGPS.Longitude) ||
                (!oldPhoto1.SequenceEqual(newPhoto1)) ||
                (!oldPhoto2.SequenceEqual(newPhoto2)) ||
                (!oldPhoto3.SequenceEqual(newPhoto3)))
            {
                var newPhotosAsByteArray = new List<byte[]>();
                if (newPhoto1 != null) newPhotosAsByteArray.Add(newPhoto1);
                if (newPhoto2 != null) newPhotosAsByteArray.Add(newPhoto2);
                if (newPhoto3 != null) newPhotosAsByteArray.Add(newPhoto3);

                if (DataAccessAdapter.EditShootingLocation(shootingLocation.Id, newShootingLocationName, newShootingLocationGPS, newPhotosAsByteArray, out string errorMessage) == PersistenceManager.E_DBReturnCode.no_error)
                {
                    ShowMessage("Shooting location data successfully changed.", E_MessageType.info);

                    // synchronize the display items of the main window and the lister window
                    // do this for the current item (detail view) and the concerned item in the list view (AllDisplayItems)
                    ViewModelManager.SynchronizeDisplayItems(DisplayItem, _listerWindow.Controler.CurrentDisplayItem);
                    var selectedDisplayItem = _listerWindow.Controler.AllDisplayItems.FirstOrDefault(d => (d.Tag as ShootingLocation).Id == shootingLocation.Id);
                    ViewModelManager.SynchronizeDisplayItems(DisplayItem, selectedDisplayItem);
                }
                else
                {
                    ShowMessage("Error editing Shooting location data." + errorMessage, E_MessageType.error);
                }
            }
            else
            {
                ShowMessage("No change done.", E_MessageType.info);
            }
        }

        private long AddParkingLocation(string parkingLocationName, GPSCoordinates parkingLocationGPS, out string errorMessage)
        {
            long newId = -1;

            // write to database (newId == -1 in case of error, so no additional error handling required)
            DataAccessAdapter.AddParkingLocation(parkingLocationName, parkingLocationGPS, out newId, out errorMessage);

            return newId;
        }

        private void LoadPhoto(ObservableCollection<byte[]> photos)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;

                try
                {
                    var byteArray = ImageTools.FileToByteArray(fileName);
                    photos.Add(byteArray); ;
                }
                catch (Exception e)
                {
                    ShowMessage("Error loading file." + e.Message, E_MessageType.error);
                }
            }
        }

        private BitmapImage LoadPhoto()
        {
            BitmapImage loadedPhoto = null;

            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;

                try
                {
                    loadedPhoto = new BitmapImage(new Uri(fileName));
                }
                catch (Exception e)
                {
                    ShowMessage("Error loading file." + e.Message, E_MessageType.error);
                }
            }

            return loadedPhoto;
        }

        private void ShootingLocationNameControl_Leaving(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            // set the view model

            // existing shooting location
            if (e.Object is ShootingLocation shootingLocation)
            {
                DisplayItem.ShootingLocationName = shootingLocation.Name;
                DisplayItem.ShootingLocationLatitude = shootingLocation.Coordinates?.Latitude;
                DisplayItem.ShootingLocationLongitude = shootingLocation.Coordinates?.Longitude;

                // might be null if no photos exist
                DisplayItem.Photo_1 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(0)?.ImageBytes);
                DisplayItem.Photo_2 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(1)?.ImageBytes);
                DisplayItem.Photo_3 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(2)?.ImageBytes);
            }

            // new shooting location
            else
            {
                // other attributes set via databinding
                DisplayItem.ShootingLocationName = Window.ShootingLocationControl.GetCurrentText();
            }

            //
            RefreshParkingLocationControl();

            Window.ShootingLocationLatitudeTextBox.Focus();
        }

        private void ParkingLocationControl_Leaving(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            // existing parking location
            if (e.Object is ParkingLocation parkingLocation)
            {
                DisplayItem.ParkingLocationName = parkingLocation.Name;
                DisplayItem.ParkingLocationLatitude = parkingLocation.Coordinates.Latitude;
                DisplayItem.ParkingLocationLongitude = parkingLocation.Coordinates.Longitude;
            }

            // new parking location
            else
            {
                DisplayItem.ParkingLocationName = Window.ParkingLocationControl.GetCurrentText();
            }

            //
            DisplayItem.ParkingLocationName = e.Object is ParkingLocation pl ? pl.Name : Window.ParkingLocationControl.GetCurrentText();

            Window.ParkingLocationLatitudeTextBox.Focus();
        }

        private void AddListerDisplayItem(string shootingLocationName)
        {
            if (DataAccessAdapter.ReadShootingLocationByName(shootingLocationName, out ShootingLocation shootingLocation, out string errorMessage) == E_DBReturnCode.no_error)
            {
                // convert the location display item into an lister display Item
                var listerDisplayItem = ViewModelManager.ConvertToListerDisplayItem(DisplayItem, shootingLocation);

                // add the converted display item to the lister (so it is added in the list view of the window)
                _listerWindow.Controler.AllDisplayItems.Add(listerDisplayItem);
            }
            else
            {
                ShowMessage(errorMessage, E_MessageType.error);
            }
        }
        #endregion

        #endregion
    }
}
