using LocationScout.Lister;
using LocationScout.Model;
using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout
{
    internal class ListerControler : ControlerBase
    {
        #region attributes
        public ObservableCollection<LocationListerDisplayItem> AllDisplayItems { get; set; }
        private LocationListerWindow _listerWindow;
        #endregion

        #region constructor
        public ListerControler(MainWindow window) : base(window)
        {
            _listerWindow = new LocationListerWindow();
            AllDisplayItems = new ObservableCollection<LocationListerDisplayItem>();

            _listerWindow.LocationListView.ItemsSource = AllDisplayItems;
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
                    SubjectLongitude = photoPlace.PlaceSubjectLocation.Coordinates.Longitude
                });
            }
        }
        #endregion
    }
}
