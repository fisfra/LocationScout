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
        public ListerControler(MainWindow window) : base(window)
        {
            _listerWindow = new LocationListerWindow(this);
            AllDisplayItems = new ObservableCollection<LocationListerDisplayItem>();
            CurrentDisplayItem = new LocationListerDisplayItem();

            _listerWindow.LocationListView.ItemsSource = AllDisplayItems;
            _listerWindow.PhotoPlaceGrid.DataContext = CurrentDisplayItem;
        }

        internal void Show()
        {
            ReadData();

            _listerWindow.Show();
        }

        private void ReadData()
        {
            var success = DataAccess.DataAccessAdapter.ReadPhotoPlace(4, out List<PhotoPlace> photoPlaces, out string errorMessage);

            foreach (var photoPlace in photoPlaces)
            {
                AllDisplayItems.Add(new LocationListerDisplayItem()
                {
                    CountryName = photoPlace.PlaceSubjectLocation.SubjectCountry.Name,
                    LocationName = photoPlace.PlaceSubjectLocation.LocationName,
                    AreaName = photoPlace.PlaceSubjectLocation.SubjectArea.Name,
                    SubAreaName = photoPlace.PlaceSubjectLocation.SubjectSubArea.Name,
                    SubjectLatitude = photoPlace.PlaceSubjectLocation.Coordinates.Latitude,
                    SubjectLongitude = photoPlace.PlaceSubjectLocation.Coordinates.Longitude,
                    Tag = photoPlace
                });
            }
        }

        internal void HandleSelectionChanged()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem?.Tag is PhotoPlace photoPlace)
            {
                // subject location
                CurrentDisplayItem.CountryName = photoPlace.PlaceSubjectLocation.SubjectCountry.Name;
                CurrentDisplayItem.LocationName = photoPlace.PlaceSubjectLocation.LocationName;
                CurrentDisplayItem.AreaName = photoPlace.PlaceSubjectLocation.SubjectArea.Name;
                CurrentDisplayItem.SubAreaName = photoPlace.PlaceSubjectLocation.SubjectSubArea.Name;
                CurrentDisplayItem.SubjectLatitude = photoPlace.PlaceSubjectLocation.Coordinates.Latitude;
                CurrentDisplayItem.SubjectLongitude = photoPlace.PlaceSubjectLocation.Coordinates.Longitude;

                // parking location 1 (and associated shooting locations)
                if (photoPlace.ParkingLocations.Count > 0)
                {
                    var pl = photoPlace.ParkingLocations[0];

                    CurrentDisplayItem.ParkingLocation1_GPS = pl.Coordinates;
                    CurrentDisplayItem.ParkingLocation1_Latitude = pl.Coordinates.Latitude;
                    CurrentDisplayItem.ParkingLocation1_Longitude = pl.Coordinates.Longitude;

                    CurrentDisplayItem.ShootingLocation1_1_GPS = (pl.ShootingLocations.Count > 0) ? pl.ShootingLocations[0].Coordinates : null;
                    CurrentDisplayItem.ShootingLocation1_1_Latitude = (pl.ShootingLocations.Count > 0) ? pl.ShootingLocations[0].Coordinates.Latitude : null;
                    CurrentDisplayItem.ShootingLocation1_1_Longitude = (pl.ShootingLocations.Count > 0) ? pl.ShootingLocations[0].Coordinates.Longitude : null;

                    CurrentDisplayItem.ShootingLocation1_2_GPS = (pl.ShootingLocations.Count > 1) ? pl.ShootingLocations[1].Coordinates : null;
                    CurrentDisplayItem.ShootingLocation1_2_Latitude = (pl.ShootingLocations.Count > 1) ? pl.ShootingLocations[1].Coordinates.Latitude : null;
                    CurrentDisplayItem.ShootingLocation1_2_Longitude = (pl.ShootingLocations.Count > 1) ? pl.ShootingLocations[1].Coordinates.Longitude : null;
                }

                // parking location 2 (and associated shooting locations)
                if (photoPlace.ParkingLocations.Count > 1)
                {
                    var pl = photoPlace.ParkingLocations[1];

                    CurrentDisplayItem.ParkingLocation2_GPS = pl.Coordinates;
                    CurrentDisplayItem.ParkingLocation2_Latitude = pl.Coordinates.Latitude;
                    CurrentDisplayItem.ParkingLocation2_Longitude = pl.Coordinates.Longitude;

                    CurrentDisplayItem.ShootingLocation2_1_GPS = (pl.ShootingLocations.Count > 0) ? pl.ShootingLocations[0].Coordinates : null;
                    CurrentDisplayItem.ShootingLocation2_1_Latitude = (pl.ShootingLocations.Count > 0) ? pl.ShootingLocations[0].Coordinates.Latitude : null;
                    CurrentDisplayItem.ShootingLocation2_1_Longitude = (pl.ShootingLocations.Count > 0) ? pl.ShootingLocations[0].Coordinates.Longitude : null;

                    CurrentDisplayItem.ShootingLocation2_2_GPS = (pl.ShootingLocations.Count > 1) ? pl.ShootingLocations[1].Coordinates : null;
                    CurrentDisplayItem.ShootingLocation2_2_Latitude = (pl.ShootingLocations.Count > 1) ? pl.ShootingLocations[1].Coordinates.Latitude : null;
                    CurrentDisplayItem.ShootingLocation2_2_Longitude = (pl.ShootingLocations.Count > 1) ? pl.ShootingLocations[1].Coordinates.Longitude : null;
                }

                // tag
                CurrentDisplayItem.Tag = photoPlace;

                if (CurrentDisplayItem.ShootingLocation1_1_Photos.Count > 0)
                {
                    try
                    {
                        _listerWindow.PreviewImage.Source = ImageTools.LoadImage(CurrentDisplayItem.ShootingLocation1_1_Photos[0]);
                    }
                    catch (Exception e)
                    {
                        base.ShowMessage("Error showing preview image. " + e.Message, E_MessageType.error);
                    }
                }
            }
        }

        internal void HandleGoogleMaps()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem?.Tag is PhotoPlace photoPlace)
            {
                var latitude = photoPlace.PlaceSubjectLocation.Coordinates.Latitude;
                var longitude = photoPlace.PlaceSubjectLocation.Coordinates.Longitude;

                var cpLat = GPSCoordinatesHelper.GetPosition(latitude, E_CoordinateType.Latitude);
                var cpLong = GPSCoordinatesHelper.GetPosition(longitude, E_CoordinateType.Longitude);

                var url = c_googleMapsUrl + new GPSCoordinatesHelper(latitude, cpLat).ToGoogleMapsString() + "+" + new GPSCoordinatesHelper(longitude, cpLong).ToGoogleMapsString();

                Process.Start(c_chrome_exe, url);
            }
        }

        internal void HandleClose()
        {
            _listerWindow.Close();
        }
        #endregion
    }
}
