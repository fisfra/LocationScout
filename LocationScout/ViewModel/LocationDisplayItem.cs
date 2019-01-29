using System.Windows.Media.Imaging;

namespace LocationScout.ViewModel
{
    public class LocationDisplayItem : SettingsDisplayItem
    {
        #region attributes
        private string _shootingLocationName;
        private double? _shootingLocationLatitude;
        private double? _shootingLocationLongitude;
        private string _parkingLocationName;
        private double? _parkingLocationLatitude;
        private double? _parkingLocationLongitude;
        private int _existingShootingLocationsCount;
        private BitmapImage _photo_1;
        private BitmapImage _photo_2;
        private BitmapImage _photo_3;

        public string ShootingLocationName
        {
            get { return _shootingLocationName; }
            set
            {
                _shootingLocationName = value;
                OnPropertyChanged();
            }
        }
        public double? ShootingLocationLatitude
        {
            get { return _shootingLocationLatitude; }
            set
            {
                _shootingLocationLatitude = value;
                OnPropertyChanged();
            }
        }
        public double? ShootingLocationLongitude
        {
            get { return _shootingLocationLongitude; }
            set
            {
                _shootingLocationLongitude = value;
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
        public int ExistingShootingLocationsCount
        {
            get { return _existingShootingLocationsCount; }
            set
            {
                _existingShootingLocationsCount = value;
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
        #endregion

        #region constructor
        public LocationDisplayItem()
        {
        }
        #endregion

        #region methods
        public override void Reset()
        {
            base.Reset();

            ShootingLocationName = string.Empty;
            ShootingLocationLatitude = null;
            ShootingLocationLongitude = null;
            ParkingLocationName = string.Empty;
            ParkingLocationLatitude = null;
            ParkingLocationLongitude = null;
            ExistingShootingLocationsCount = 0;
        }
        #endregion
    }
}
