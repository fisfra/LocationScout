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
    internal class ListerControler : ControlerBase
    {
        #region constants
        private string c_googleMapsUrl = "https://www.google.com/maps/place/";
        private string c_chrome_exe = "chrome.exe";
        #endregion

        #region attributes
        public ObservableCollection<LocationListerDisplayItem> AllDisplayItems { get; set; }
        public LocationListerDisplayItem CurrentDisplayItem { get; set; }
        private LocationListerWindow _listerWindow;
        private LocationTabControler _locationTabControler;
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
            // testing
            Console.WriteLine(Window.Location_CountryControl.SelectKey("Germany") ? "Germany" : "No Germany");
        }

        private void ReadData()
        {
            var success = DataAccess.DataAccessAdapter.ReadAllShootingLocations(out List<ShootingLocation> shootingLocations, out string errorMessage);

            foreach (var shootingLocation in shootingLocations)
            {
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

                AllDisplayItems.Add(newDisplayItem);                
            }
        }

        internal void HandleSelectionChanged()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem is LocationListerDisplayItem displayItem)
            {
                CurrentDisplayItem.SubjectLocationName = displayItem.SubjectLocationName;
                CurrentDisplayItem.SubjectLocationLatitude = displayItem.SubjectLocationLatitude;
                CurrentDisplayItem.SubjectLocationLongitude = displayItem.SubjectLocationLongitude;

                CurrentDisplayItem.ParkingLocationName = displayItem.ParkingLocationName;
                CurrentDisplayItem.ParkingLocationLatitude = displayItem.ParkingLocationLatitude;
                CurrentDisplayItem.ParkingLocationLongitude = displayItem.ParkingLocationLongitude;

                CurrentDisplayItem.Photo_1 = displayItem.Photo_1;
                CurrentDisplayItem.Photo_2 = displayItem.Photo_2;
                CurrentDisplayItem.Photo_3 = displayItem.Photo_3;

                CurrentDisplayItem.Tag = displayItem.Tag;
            }
        }

        internal void HandleGoogleMaps()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem?.Tag is ShootingLocation shootingLocation)
            {
                var latitude = shootingLocation.SubjectLocations.ElementAt(0)?.Coordinates?.Latitude;
                var longitude = shootingLocation.SubjectLocations.ElementAt(0)?.Coordinates?.Longitude;

                if ((latitude != null) && (longitude != null))
                {

                    var cpLat = GPSCoordinatesHelper.GetPosition(latitude, E_CoordinateType.Latitude);
                    var cpLong = GPSCoordinatesHelper.GetPosition(longitude, E_CoordinateType.Longitude);

                    var url = c_googleMapsUrl + new GPSCoordinatesHelper(latitude, cpLat).ToGoogleMapsString() + "+" + new GPSCoordinatesHelper(longitude, cpLong).ToGoogleMapsString();

                    Process.Start(c_chrome_exe, url);
                }
            }
        }

        internal void HandleClose()
        {
            _listerWindow.Close();
        }
        #endregion
    }
}
