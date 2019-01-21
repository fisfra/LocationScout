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

            foreach (PhotoPlace photoPlace in photoPlaces)
            {
                var newDisplayItem = new LocationListerDisplayItem()
                {
                    CountryName = photoPlace.PlaceSubjectLocation.SubjectCountry.Name,
                    LocationName = photoPlace.PlaceSubjectLocation.LocationName,
                    AreaName = photoPlace.PlaceSubjectLocation.SubjectArea.Name,
                    SubAreaName = photoPlace.PlaceSubjectLocation.SubjectSubArea.Name,
                    SubjectLatitude = photoPlace.PlaceSubjectLocation.Coordinates.Latitude,
                    SubjectLongitude = photoPlace.PlaceSubjectLocation.Coordinates.Longitude,
                    ShootingLocation1_1_Photos = new ObservableCollection<byte[]>(),
                    Tag = photoPlace
                };


                var photos = photoPlace.ParkingLocations.ElementAtOrDefault(0)?.ShootingLocations.ElementAtOrDefault(0).Photos;
                if (photos != null)
                {
                    foreach (var photo in photos)
                    {
                        newDisplayItem.ShootingLocation1_1_Photos.Add(photo.ImageBytes);
                    }
                }

                AllDisplayItems.Add(newDisplayItem);

            }
        }

        private void SetSubjectLocation(SubjectLocation sl)
        {
            CurrentDisplayItem.CountryName = sl.SubjectCountry.Name;
            CurrentDisplayItem.LocationName = sl.LocationName;
            CurrentDisplayItem.AreaName = sl.SubjectArea.Name;
            CurrentDisplayItem.SubAreaName = sl.SubjectSubArea.Name;
            CurrentDisplayItem.SubjectLatitude = sl.Coordinates.Latitude;
            CurrentDisplayItem.SubjectLongitude = sl.Coordinates.Longitude;
        }

        private void SetParkingLocation_1(ParkingLocation pl)
        {

            CurrentDisplayItem.ParkingLocation1_GPS = pl.Coordinates;
            CurrentDisplayItem.ParkingLocation1_Latitude = pl.Coordinates.Latitude;
            CurrentDisplayItem.ParkingLocation1_Longitude = pl.Coordinates.Longitude;
        }

        private void SetParkingLocation_2(ParkingLocation pl)
        {

            CurrentDisplayItem.ParkingLocation2_GPS = pl.Coordinates;
            CurrentDisplayItem.ParkingLocation2_Latitude = pl.Coordinates.Latitude;
            CurrentDisplayItem.ParkingLocation2_Longitude = pl.Coordinates.Longitude;
        }

        private void SetShootingLocations_1(List<ShootingLocation> sl)
        {
            CurrentDisplayItem.ShootingLocation1_1_GPS = (sl.Count > 0) ? sl[0].Coordinates : null;
            CurrentDisplayItem.ShootingLocation1_1_Latitude = (sl.Count > 0) ? sl[0].Coordinates.Latitude : null;
            CurrentDisplayItem.ShootingLocation1_1_Longitude = (sl.Count > 0) ? sl[0].Coordinates.Longitude : null;

            CurrentDisplayItem.ShootingLocation1_2_GPS = (sl.Count > 1) ? sl[1].Coordinates : null;
            CurrentDisplayItem.ShootingLocation1_2_Latitude = (sl.Count > 1) ? sl[1].Coordinates.Latitude : null;
            CurrentDisplayItem.ShootingLocation1_2_Longitude = (sl.Count > 1) ? sl[1].Coordinates.Longitude : null;
        }

        private void SetShootingLocations_2(List<ShootingLocation> sl)
        {
            CurrentDisplayItem.ShootingLocation2_1_GPS = (sl.Count > 0) ? sl[0].Coordinates : null;
            CurrentDisplayItem.ShootingLocation2_1_Latitude = (sl.Count > 0) ? sl[0].Coordinates.Latitude : null;
            CurrentDisplayItem.ShootingLocation2_1_Longitude = (sl.Count > 0) ? sl[0].Coordinates.Longitude : null;

            CurrentDisplayItem.ShootingLocation2_2_GPS = (sl.Count > 1) ? sl[1].Coordinates : null;
            CurrentDisplayItem.ShootingLocation2_2_Latitude = (sl.Count > 1) ? sl[1].Coordinates.Latitude : null;
            CurrentDisplayItem.ShootingLocation2_2_Longitude = (sl.Count > 1) ? sl[1].Coordinates.Longitude : null;
        }

        private void SetPhoto_1(List<ShootingLocation> sl)
        {
            if (sl.Count > 0)
            {
                var photos = sl[0].Photos;
                if (photos != null)
                {
                    foreach (var photo in photos)
                    {
                        CurrentDisplayItem.ShootingLocation1_1_Photos.Add(photo.ImageBytes);
                    }

                    CurrentDisplayItem.CurrentPhoto = photos.Count > 0 ? ImageTools.LoadImage(photos[0].ImageBytes) : null;
                }
            }

            if (sl.Count > 1)
            {
                var photos = sl[1].Photos;
                if (photos != null)
                {
                    foreach (var photo in photos)
                    {
                        CurrentDisplayItem.ShootingLocation1_2_Photos.Add(photo.ImageBytes);
                    }
                }
            }
        }

        private void SetPhoto_2(List<ShootingLocation> sl)
        {
            if (sl.Count > 0)
            {
                var photos = sl[0].Photos;
                if (photos != null)
                {
                    foreach (var photo in photos)
                    {
                        CurrentDisplayItem.ShootingLocation2_1_Photos.Add(photo.ImageBytes);
                    }
                }
            }

            if (sl.Count > 1)
            {
                var photos = sl[1].Photos;
                if (photos != null)
                {
                    foreach (var photo in photos)
                    {
                        CurrentDisplayItem.ShootingLocation2_2_Photos.Add(photo.ImageBytes);
                    }
                }
            }
        }


        internal void HandleSelectionChanged()
        {
            var selectedItem = _listerWindow.LocationListView.SelectedItem as LocationListerDisplayItem;
            if (selectedItem?.Tag is PhotoPlace photoPlace)
            {
                // subject location
                SetSubjectLocation(photoPlace.PlaceSubjectLocation);

                // parking location 1 (and associated shooting locations)
                if (photoPlace.ParkingLocations.Count > 0)
                {
                    var pl = photoPlace.ParkingLocations[0];

                    SetParkingLocation_1(pl);
                    SetShootingLocations_1(pl.ShootingLocations);
                    SetPhoto_1(pl.ShootingLocations);
                }

                // parking location 2 (and associated shooting locations)
                if (photoPlace.ParkingLocations.Count > 1)
                {
                    var pl = photoPlace.ParkingLocations[1];

                    SetParkingLocation_2(pl);
                    SetShootingLocations_2(pl.ShootingLocations);
                    SetPhoto_2(pl.ShootingLocations);
                }

                // tag
                CurrentDisplayItem.Tag = photoPlace;
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
