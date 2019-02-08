using LocationScout.Model;
using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace LocationScout
{
    internal class ListerControler : ControlerBase
    {
        #region constants
        #endregion

        #region attributes
        public ObservableCollection<LocationListerDisplayItem> AllDisplayItems { get; set; }
        public LocationListerDisplayItem CurrentDisplayItem { get; set; }
        private LocationListerWindow _listerWindow;
        private LocationTabControler _locationTabControler;

        protected override Label StatusLabel { get { return _listerWindow.StatusLabel; } }
        #endregion

        #region constructor
        public ListerControler(MainWindow mainWindow, LocationListerWindow listerWindow, LocationTabControler locationTabControler) : base(mainWindow)
        {
            AllDisplayItems = new ObservableCollection<LocationListerDisplayItem>();
            CurrentDisplayItem = new LocationListerDisplayItem();

            _locationTabControler = locationTabControler;
            _listerWindow = listerWindow;

            _listerWindow.LocationListView.ItemsSource = AllDisplayItems;
            _listerWindow.PhotoPlaceGrid.DataContext = CurrentDisplayItem;

            ReadData();
        }

        internal void HandleEdit()
        {
            // country
            Window.Location_CountryControl.SelectKey(CurrentDisplayItem.CountryName);
            _locationTabControler.DisplayItem.CountryName = CurrentDisplayItem.CountryName;
            var country = Window.Location_CountryControl.GetCurrentObject() as Country;

            // area
            RefreshControl(country?.Areas?.OfType<Location>().ToList(), Window.Location_AreaControl);
            Window.Location_AreaControl.SelectKey(CurrentDisplayItem.AreaName);
            _locationTabControler.DisplayItem.AreaName = CurrentDisplayItem.AreaName;
            var area = Window.Location_AreaControl.GetCurrentObject() as Area;

            // subarea
            RefreshControl(area?.SubAreas?.OfType<Location>().ToList(), Window.Location_SubAreaControl);
            Window.Location_SubAreaControl.SelectKey(CurrentDisplayItem.SubAreaName);
            _locationTabControler.DisplayItem.SubAreaName = CurrentDisplayItem.SubAreaName;
            var subArea = Window.Location_SubAreaControl.GetCurrentObject() as SubArea;
            var subjectLocations = country.SubjectLocations.Where(c => c.Country.Name == CurrentDisplayItem.CountryName && 
                                                                  c.Area.Name == CurrentDisplayItem.AreaName && c.SubArea.Name == CurrentDisplayItem.SubAreaName).ToList();

            // subject location
            RefreshControl(subjectLocations?.OfType<Location>().ToList(), Window.Location_SubjectLocationControl);
            Window.Location_SubjectLocationControl.SelectKey(CurrentDisplayItem.SubjectLocationName);
            _locationTabControler.DisplayItem.SubjectLocationName = CurrentDisplayItem.SubjectLocationName;
            var subjectLocation = Window.Location_SubjectLocationControl.GetCurrentObject() as SubjectLocation;
            _locationTabControler.DisplayItem.SubjectLocationLatitude = subjectLocation.Coordinates.Latitude;
            _locationTabControler.DisplayItem.SubjectLocationLongitude = subjectLocation.Coordinates.Longitude;

            // shooting location
            RefreshControl(subjectLocation?.ShootLocations?.OfType<Location>().ToList(), Window.ShootingLocationControl);
            Window.ShootingLocationControl.SelectKey(CurrentDisplayItem.ShootingLocationName);
            _locationTabControler.DisplayItem.ShootingLocationName = CurrentDisplayItem.ShootingLocationName;
            var shootingLocation = Window.ShootingLocationControl.GetCurrentObject() as ShootingLocation;
            _locationTabControler.DisplayItem.ShootingLocationLatitude = shootingLocation.Coordinates.Latitude;
            _locationTabControler.DisplayItem.ShootingLocationLongitude = shootingLocation.Coordinates.Longitude;
            _locationTabControler.DisplayItem.Photo_1 = ImageTools.ByteArrayToBitmapImage(shootingLocation?.Photos?.ElementAtOrDefault(0)?.ImageBytes);
            _locationTabControler.DisplayItem.Photo_2 = ImageTools.ByteArrayToBitmapImage(shootingLocation?.Photos?.ElementAtOrDefault(1)?.ImageBytes);
            _locationTabControler.DisplayItem.Photo_3 = ImageTools.ByteArrayToBitmapImage(shootingLocation?.Photos?.ElementAtOrDefault(2)?.ImageBytes);

            // parking location
            RefreshControl(shootingLocation?.ParkingLocations?.OfType<Location>().ToList(), Window.ParkingLocationControl);
            Window.ParkingLocationControl.SelectKey(CurrentDisplayItem.ParkingLocationName);
            _locationTabControler.DisplayItem.ParkingLocationName = CurrentDisplayItem.ParkingLocationName;
            var parkingLocation = Window.ParkingLocationControl.GetCurrentObject() as ParkingLocation;
            _locationTabControler.DisplayItem.ParkingLocationLatitude = parkingLocation.Coordinates.Latitude;
            _locationTabControler.DisplayItem.ParkingLocationLongitude = parkingLocation.Coordinates.Longitude;


            var shootingLocationId = shootingLocation.Id;

            var di = AllDisplayItems.FirstOrDefault(d => (d.Tag as ShootingLocation).Id == shootingLocationId);
            di.ShootingLocationName = shootingLocation.Name;
        }

        internal void HandleDelete()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem is LocationListerDisplayItem displayItem)
            {
                if (DataAccess.DataAccessAdapter.DeleteShootingLocationById((displayItem.Tag as ShootingLocation).Id, out string errorMessagse) == DataAccess.PersistenceManager.E_DBReturnCode.error)
                {
                    ShowMessage(errorMessagse, E_MessageType.error);
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    AllDisplayItems.Remove(displayItem);
                    CurrentDisplayItem.Reset();

                    _locationTabControler.RefreshAllObjectsFromDB();
                    _locationTabControler.ReloadAndRefreshControls();

                    Mouse.OverrideCursor = null;

                    ShowMessage("Shooting location successfully deleted.", E_MessageType.success);
                }
            }
        }

        private void ReadData()
        {
            // read all shooting locations
            if (DataAccess.DataAccessAdapter.ReadAllShootingLocations(out List<ShootingLocation> shootingLocations, out string errorMessage) == DataAccess.PersistenceManager.E_DBReturnCode.no_error)
            {
                // loop throw dhoow locations
                foreach (var shootingLocation in shootingLocations)
                {
                    // create view model
                    var newDisplayItem = new LocationListerDisplayItem()
                    {
                        ParkingLocationName = shootingLocation.ParkingLocations.ElementAtOrDefault(0)?.Name,
                        ParkingLocationLatitude = shootingLocation.ParkingLocations.ElementAtOrDefault(0)?.Coordinates?.Latitude,
                        ParkingLocationLongitude = shootingLocation.ParkingLocations.ElementAtOrDefault(0)?.Coordinates?.Longitude,
                        ShootingLocationName = shootingLocation.Name,
                        ShootingLocationLatitude = shootingLocation.Coordinates.Latitude,
                        ShootingLocationLongitude = shootingLocation.Coordinates.Longitude,
                        Photo_1 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(0)?.ImageBytes),
                        Photo_2 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(1)?.ImageBytes),
                        Photo_3 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(2)?.ImageBytes),
                        Tag = shootingLocation
                    };

                    foreach (var subjectLocation in shootingLocation.SubjectLocations)
                    {
                        newDisplayItem.CountryName = subjectLocation.Country.Name;
                        newDisplayItem.SubjectLocationName = subjectLocation.Name;
                        newDisplayItem.AreaName = subjectLocation.Area.Name;
                        newDisplayItem.SubAreaName = subjectLocation.SubArea.Name;
                        newDisplayItem.SubjectLocationLatitude = subjectLocation.Coordinates.Latitude;
                        newDisplayItem.SubjectLocationLongitude = subjectLocation.Coordinates.Longitude;
                    }

                    newDisplayItem.Photo_1 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(0)?.ImageBytes);
                    newDisplayItem.Photo_2 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(1)?.ImageBytes);
                    newDisplayItem.Photo_3 = ImageTools.ByteArrayToBitmapImage(shootingLocation.Photos?.ElementAtOrDefault(2)?.ImageBytes);

                    // add to list which is bound to list view
                    AllDisplayItems.Add(newDisplayItem);
                }
            }

            // error reading the data
            else
            {
                ShowMessage(errorMessage, E_MessageType.error);
            }
        }

        internal void HandleSelectionChanged()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem is LocationListerDisplayItem displayItem)
            {
                CurrentDisplayItem.CountryName = displayItem.CountryName;
                CurrentDisplayItem.AreaName = displayItem.AreaName;
                CurrentDisplayItem.SubAreaName = displayItem.SubAreaName;

                CurrentDisplayItem.SubjectLocationName = displayItem.SubjectLocationName;
                CurrentDisplayItem.SubjectLocationLatitude = displayItem.SubjectLocationLatitude;
                CurrentDisplayItem.SubjectLocationLongitude = displayItem.SubjectLocationLongitude;

                CurrentDisplayItem.ShootingLocationName = displayItem.ShootingLocationName;
                CurrentDisplayItem.ShootingLocationLatitude = displayItem.ShootingLocationLatitude;
                CurrentDisplayItem.ShootingLocationLongitude = displayItem.ShootingLocationLongitude;

                CurrentDisplayItem.ParkingLocationName = displayItem.ParkingLocationName;
                CurrentDisplayItem.ParkingLocationLatitude = displayItem.ParkingLocationLatitude;
                CurrentDisplayItem.ParkingLocationLongitude = displayItem.ParkingLocationLongitude;

                CurrentDisplayItem.Photo_1 = displayItem.Photo_1;
                CurrentDisplayItem.Photo_2 = displayItem.Photo_2;
                CurrentDisplayItem.Photo_3 = displayItem.Photo_3;

                CurrentDisplayItem.Tag = displayItem.Tag;
            }
        }

        internal void HandleShootingLocationGoogleMaps()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem?.Tag is ShootingLocation shootingLocation)
            {
                if (shootingLocation?.Coordinates is GPSCoordinates coordinates)
                {
                    GoogleMapsHelper.Go(coordinates);
                }
            }
        }

        internal void HandleSubjectLocationGoogleMaps()
        {
            var coordinates = new GPSCoordinates(CurrentDisplayItem.SubjectLocationLatitude, CurrentDisplayItem.SubjectLocationLongitude);

            if (coordinates != null)
            {
                GoogleMapsHelper.Go(coordinates);
            }
        }

        internal void HandleParkingLocationGoogleMaps()
        {
            var coordinates = new GPSCoordinates(CurrentDisplayItem.ParkingLocationLatitude, CurrentDisplayItem.ParkingLocationLongitude);

            if (coordinates != null)
            {
                GoogleMapsHelper.Go(coordinates);
            }
        }

        internal void HandleClose()
        {
            // just hide the window to be able to show it again
            _listerWindow.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion
    }
}
    