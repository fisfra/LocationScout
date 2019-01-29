﻿using LocationScout.Model;
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
using LocationScout.Lister;

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
        internal void ShowLister()
        {
            LocationListerWindow window = new LocationListerWindow(base.Window);
            window.Show();
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

        internal void Add()
        {
            // 
            string errorMessage = string.Empty;

            // data from UI
            var subjectLocation = Window.Location_SubjectLocationControl.GetCurrentObject() as SubjectLocation;
            var parkingLocation = Window.ParkingLocationControl.GetCurrentObject() as ParkingLocation;
            
            var shootingLocationName = DisplayItem.ShootingLocationName;

            // db operations might take a while
            Mouse.OverrideCursor = Cursors.Wait;

            // set parking location (if new parking location, initial value is -1)
            long parkingLocationId = parkingLocation != null ? parkingLocation.Id :- 1;

            // new parking location (object is null)
            if (parkingLocation == null)
            {
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
            var success = DataAccessAdapter.SmartAddPhotoLocation(DisplayItem, subjectLocation.Id, parkingLocationId, out errorMessage);

            switch (success)
            {
                case E_DBReturnCode.no_error:
                    ShowMessage("Photo Location successfully added", E_MessageType.success);
                    break;
                case E_DBReturnCode.error:
                    ShowMessage("Error adding PhotoLocation to database\n" + errorMessage, E_MessageType.error);
                    break;
                case E_DBReturnCode.already_existing:
                    // to do
                    break;
                default:
                    Debug.Assert(false);
                    throw new Exception("Unknown enum value in LocationTabControler::Add");
            }

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and refresh controls
            Window.Location_CountryControl.ClearText();
            Window.Location_AreaControl.ClearText();
            Window.Location_SubAreaControl.ClearText();
            Window.Location_SubjectLocationControl.ClearText();
            DisplayItem.Reset();
            RefreshShootingLocationsFromDB();
            RefreshParkingLocationsFromDB();       
            RefreshControl(_allShootingLocations?.OfType<Location>()?.ToList(), Window.ShootingLocationControl);
            RefreshControl(_allParkingLocations?.OfType<Location>()?.ToList(), Window.ParkingLocationControl);

            // reset cursor
            Mouse.OverrideCursor = null;

            Window.Location_CountryControl.SetFocus();
        }

        private long AddParkingLocation(string parkingLocationName, GPSCoordinates parkingLocationGPS, out string errorMessage)
        {
            long newId = -1;

            // write to database (newId == -1 in case of error, so no additional error handling required)
            DataAccessAdapter.AddParkingLocation(parkingLocationName, parkingLocationGPS, out newId, out errorMessage);

            return newId;
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
            DisplayItem.SubjectLocationName = (e.Object as SubjectLocation)?.Name;
            DisplayItem.SubjectLocationLatitude = (e.Object as SubjectLocation)?.Coordinates?.Latitude;
            DisplayItem.SubjectLocationLongitude = (e.Object as SubjectLocation)?.Coordinates?.Longitude;

            //
            RefreshControl(_allShootingLocations?.OfType<Location>()?.ToList(), Window.ShootingLocationControl);

            // 
            Window.ShootingLocationControl.SetFocus();
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
            RefreshControl(_allParkingLocations?.OfType<Location>()?.ToList(), Window.ParkingLocationControl);

            Window.ShootingLocationLatitudeTextbox.Focus();
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
