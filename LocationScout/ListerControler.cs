using LocationScout.Lister;
using LocationScout.Model;
using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout
{
    public class ListerControler : ControlerBase
    {
        #region constants
        private string c_googleMapsUrl = "https://www.google.com/maps/place/";
        private string c_chrome_exe = "chrome.exe";
        #endregion

        #region attributes
        public ObservableCollection<LocationListerDisplayItem> AllDisplayItems { get; set; }
        public LocationListerDisplayItem CurrentDisplayItem { get; set; }
        private LocationListerWindow _listerWindow;
        #endregion

        #region constructor
        public ListerControler(MainWindow mainWindow, LocationListerWindow listerWindow) : base(mainWindow)
        {
            AllDisplayItems = new ObservableCollection<LocationListerDisplayItem>();
            CurrentDisplayItem = new LocationListerDisplayItem();

            _listerWindow = listerWindow;

            _listerWindow.LocationListView.ItemsSource = AllDisplayItems;
            _listerWindow.PhotoPlaceGrid.DataContext = CurrentDisplayItem;

            ReadData();
        }

        private void ReadData()
        {
            var success = DataAccess.DataAccessAdapter.ReadAllShootingLocations(out List<ShootingLocation> shootingLocations, out string errorMessage);

            foreach (var shootingLocation in shootingLocations)
            {
                var newDisplayItem = new LocationListerDisplayItem()
                {
                    ParkingLocationName = shootingLocation.ParkingLocations[0].Name,
                    ShootingLocationName = shootingLocation.Name
                };

                foreach (var subjectLocation in shootingLocation.SubjectLocations)
                {
                    newDisplayItem.CountryName = subjectLocation.Country.Name;
                    newDisplayItem.SubjectLocationName = subjectLocation.Name;
                    newDisplayItem.AreaName = subjectLocation.Area.Name;
                    newDisplayItem.SubAreaName = subjectLocation.SubArea.Name;
                    newDisplayItem.SubjectLocationLatitude = subjectLocation.Coordinates.Latitude;
                    newDisplayItem.SubjectLocationLongitude = subjectLocation.Coordinates.Longitude;
                    newDisplayItem.Tag = shootingLocation;
                }

                AllDisplayItems.Add(newDisplayItem);                
            }
        }

        internal void HandleSelectionChanged()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem?.Tag is ShootingLocation shootingLocation)
            {
                CurrentDisplayItem.SubjectLocationName = selectedItem.SubjectLocationName;
                CurrentDisplayItem.SubjectLocationLatitude = selectedItem.SubjectLocationLatitude;
                CurrentDisplayItem.SubjectLocationLongitude = selectedItem.SubjectLocationLongitude;

                CurrentDisplayItem.ParkingLocationName = selectedItem.ParkingLocationName;
                CurrentDisplayItem.ParkingLocationLatitude = selectedItem.ParkingLocationLatitude;
                CurrentDisplayItem.ParkingLocationLongitude = selectedItem.ParkingLocationLongitude;

                CurrentDisplayItem.Photo_1 = selectedItem.Photo_1;
                CurrentDisplayItem.Photo_2 = selectedItem.Photo_2;
                CurrentDisplayItem.Photo_3 = selectedItem.Photo_3;

                // tag
                CurrentDisplayItem.Tag = shootingLocation;
            }
        }

        internal void HandleGoogleMaps()
        {
            /*
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem?.Tag is PhotoPlace photoPlace)
            {
                var latitude = photoPlace.PlaceSubjectLocation.Coordinates.Latitude;
                var longitude = photoPlace.PlaceSubjectLocation.Coordinates.Longitude;

                var cpLat = GPSCoordinatesHelper.GetPosition(latitude, E_CoordinateType.Latitude);
                var cpLong = GPSCoordinatesHelper.GetPosition(longitude, E_CoordinateType.Longitude);

                var url = c_googleMapsUrl + new GPSCoordinatesHelper(latitude, cpLat).ToGoogleMapsString() + "+" + new GPSCoordinatesHelper(longitude, cpLong).ToGoogleMapsString();

                Process.Start(c_chrome_exe, url);
            }*/
        }

        internal void HandleClose()
        {
            _listerWindow.Close();
        }
        #endregion
    }
}
