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

        public LocationDisplayItem DisplayItem { get; set; }

        private List<ShootingLocation> _allShootingLocations;
        private List<ParkingLocation> _allParkingLocations;
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
        }
        #endregion

        #region methods
        private void AreaControl_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Window.Location_CountryControl.SetFocus();
        }

        private void RefreshShootingLocationsFromDB()
        {
            var success = DataAccessAdapter.ReadAllShootingLocations(out _allShootingLocations, out string errorMessage);
            if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading shooting locations" + errorMessage, E_MessageType.error);
        }

        private void RefreshParkingLocationsFromDB()
        {
            var success = DataAccessAdapter.ReadAllParkingLocations(out _allParkingLocations, out string errorMessage);
            if (success == PersistenceManager.E_DBReturnCode.error) ShowMessage("Error reading Parking locations" + errorMessage, E_MessageType.error);
        }

        internal void Add()
        {
            // data from UI
            var subjectLocation = Window.Location_SubjectLocationControl.GetCurrentObject() as SubjectLocation;
            var parkingLocation = Window.ParkingLocationControl.GetCurrentObject() as ParkingLocation;
            var shootingLocationName = DisplayItem.ShootingLocationName;

            // db operations might take a while
            Mouse.OverrideCursor = Cursors.Wait;

            // add country to database
            E_DBReturnCode success = DataAccessAdapter.AddPhotoLocation(new List<long>() { subjectLocation.Id }, new List<long>() { parkingLocation.Id }, null, shootingLocationName, out string errorMessages);

            // refresh or error handling
            // AfterDBWriteSteps(success, errorMessage);

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and reset focus
            // _window.SettingsCountryControl.ClearText();
            // _window.SettingsAreaControl.ClearText();
            // _window.SettingsSubAreaControl.ClearText();
            // _window.SettingsCountryControl.SetFocus();*/
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
                    break;
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

        internal void HandleGoogleMapsSubjectLocation()
        {
            var gps = new GPSCoordinates(DisplayItem.SubjectLocationLatitude, DisplayItem.SubjectLocationLongitude);
            GoogleMapsHelper.GoGoogleMap(gps);
        }        

        protected override void SubjectLocationControl_Leaving(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            // base class call
            base.SubjectLocationControl_Leaving(sender, e);

            // set the view model
            DisplayItem.SubjectLocationLatitude = (e.Object as SubjectLocation)?.Coordinates?.Latitude;
            DisplayItem.SubjectLocationLongitude = (e.Object as SubjectLocation)?.Coordinates?.Longitude;

            // get the subject location object from UI
            var selectedSubjectLocation = Window.Location_SubjectLocationControl.GetCurrentObject() as SubjectLocation;

            // if the subject location has already shooting locations
            List<ShootingLocation> existingShootingLocations = selectedSubjectLocation != null ? selectedSubjectLocation.ShootLocations : _allShootingLocations;
            DisplayItem.ExistingShootingLocationsName = existingShootingLocations != null ? existingShootingLocations.Count : 0;

            //
            RefreshControl(_allShootingLocations?.OfType<Location>()?.ToList(), Window.ShootingLocationControl);

            // 
            Window.ShootingLocationControl.SetFocus();
        }

        private void ShootingLocationNameControl_Leaving(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            DisplayItem.ShootingLocationName = e.Object is ShootingLocation shootingLocation ? shootingLocation.Name : Window.ShootingLocationControl.GetCurrentText();

            Window.ShootingLocationLatitudeTextbox.Focus();
        }

        private void ParkingLocationControl_Leaving(object sender, AutoCompleteTextBoxControlEventArgs e)
        {
            DisplayItem.ParkingLocationName = e.Object is ParkingLocation parkingLocation ? parkingLocation.Name : Window.ParkingLocationControl.GetCurrentText();

            Window.ParkingLocationLatitudeTextbox.Focus();
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
        #endregion
    }
}
