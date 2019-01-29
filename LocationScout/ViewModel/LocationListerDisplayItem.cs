using System.Windows.Media.Imaging;

namespace LocationScout.ViewModel
{
    public class LocationListerDisplayItem : BaseObservableObject
    {
        #region attributes
        private string _shootingLocationName;
        private string _countryName;
        private string _areaName;
        private string _subAreaName;

        private string _subjectLocationName;
        private double? _subjectLocationLatitude;
        private double? _subjectLocationLongitude;
        private string _parkingLocationName;
        private double? _parkingLocationLatitude;
        private double? _parkingLocationLongitude;
        private BitmapImage _photo_1;
        private BitmapImage _photo_2;
        private BitmapImage _photo_3;

        private object _tag;

        public string ShootingLocationName
        {
            get { return _shootingLocationName; }
            set
            {
                _shootingLocationName = value;
                OnPropertyChanged();
            }
        }
        public string CountryName
        {
            get { return _countryName; }
            set
            {
                _countryName = value;
                OnPropertyChanged();
            }
        }
        public string AreaName
        {
            get { return _areaName; }
            set
            {
                _areaName = value;
                OnPropertyChanged();
            }
        }
        public string SubAreaName
        {
            get { return _subAreaName; }
            set
            {
                _subAreaName = value;
                OnPropertyChanged();
            }
        }

        public string SubjectLocationName
        {
            get { return _subjectLocationName; }
            set
            {
                _subjectLocationName = value;
                OnPropertyChanged();
            }
        }
        public double? SubjectLocationLatitude
        {
            get { return _subjectLocationLatitude; }
            set
            {
                _subjectLocationLatitude = value;
                OnPropertyChanged();
            }
        }
        public double? SubjectLocationLongitude
        {
            get { return _subjectLocationLongitude; }
            set
            {
                _subjectLocationLongitude = value;
                OnPropertyChanged();
            }
        }
        public string ParkingLocationName
        {
            get { return _parkingLocationName; }
            set
            {
                _parkingLocationName = value;
                OnPropertyChanged();
            }
        }
        public double? ParkingLocationLatitude
        {
            get { return _parkingLocationLatitude; }
            set
            {
                _parkingLocationLatitude = value;
                OnPropertyChanged();
            }
        }
        public double? ParkingLocationLongitude
        {
            get { return _parkingLocationLongitude; }
            set
            {
                _parkingLocationLongitude = value;
                OnPropertyChanged();
            }
        }
        public BitmapImage Photo_1
        {
            get { return _photo_1; }
            set
            {
                _photo_1 = value;
                OnPropertyChanged();
            }
        }
        public BitmapImage Photo_2
        {
            get { return _photo_2; }
            set
            {
                _photo_2 = value;
                OnPropertyChanged();
            }
        }
        public BitmapImage Photo_3
        {
            get { return _photo_3; }
            set
            {
                _photo_3 = value;
                OnPropertyChanged();
            }
        }

        public object Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public LocationListerDisplayItem()
        {
        }
    }

    #region enums
    // nested types not supported by XAML, therefore not part of a class
    public enum E_CoordinateType { Longitude, Latitude };
    #endregion
}
