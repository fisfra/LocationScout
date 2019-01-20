using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.ViewModel
{
    public class LocationListerDisplayItem : BaseObservableObject
    {
        #region attributes
        private string _locationName;
        private string _countryName;
        private string _areaName;
        private string _subAreaName;
        private double? _subjectLatitude;
        private double? _subjectLongitude;
        private double? _parkingLocation1_Latitude;
        private double? _parkingLocation1_Longitude;
        private GPSCoordinates _parkingLocation1_GPS;
        private double? _parkingLocation2_Latitude;
        private double? _parkingLocation2_Longitude;
        private GPSCoordinates _parkingLocation2_GPS;

        private double? _shootingLocation1_1_Latitude;
        private double? _shootingLocation1_1_Longitude;
        private GPSCoordinates _shootingLocation1_1_GPS;
        private double? _shootingLocation1_2_Latitude;
        private double? _shootingLocation1_2_Longitude;
        private GPSCoordinates _shootingLocation1_2_GPS;

        private double? _shootingLocation2_1_Latitude;
        private double? _shootingLocation2_1_Longitude;
        private GPSCoordinates _shootingLocation2_1_GPS;
        private double? _shootingLocation2_2_Latitude;
        private double? _shootingLocation2_2_Longitude;
        private GPSCoordinates _shootingLocation2_2_GPS;

        private object _tag;

        public string LocationName
        {
            get { return _locationName; }
            set
            {
                _locationName = value;
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
        public double? SubjectLatitude
        {
            get { return _subjectLatitude; }
            set
            {
                _subjectLatitude = value;
                OnPropertyChanged();
            }
        }
        public double? SubjectLongitude
        {
            get { return _subjectLongitude; }
            set
            {
                _subjectLongitude = value;
                OnPropertyChanged();
            }
        }

        public double? ParkingLocation1_Latitude
        {
            get { return _parkingLocation1_Latitude; }
            set
            {
                _parkingLocation1_Latitude = value;
                OnPropertyChanged();
            }
        }
        public double? ParkingLocation1_Longitude
        {
            get { return _parkingLocation1_Longitude; }
            set
            {
                _parkingLocation1_Longitude = value;
                OnPropertyChanged();
            }
        }
        public GPSCoordinates ParkingLocation1_GPS
        {
            get { return _parkingLocation1_GPS; }
            set
            {
                _parkingLocation1_GPS = value;
                OnPropertyChanged();
            }
        }
        public double? ParkingLocation2_Latitude
        {
            get { return _parkingLocation2_Latitude; }
            set
            {
                _parkingLocation2_Latitude = value;
                OnPropertyChanged();
            }
        }
        public double? ParkingLocation2_Longitude
        {
            get { return _parkingLocation2_Longitude; }
            set
            {
                _parkingLocation2_Longitude = value;
                OnPropertyChanged();
            }
        }
        public GPSCoordinates ParkingLocation2_GPS
        {
            get { return _parkingLocation2_GPS; }
            set
            {
                _parkingLocation2_GPS = value;
                OnPropertyChanged();
            }
        }

        public double? ShootingLocation1_1_Latitude
        {
            get { return _shootingLocation1_1_Latitude; }
            set
            {
                _shootingLocation1_1_Latitude = value;
                OnPropertyChanged();
            }
        }
        public double? ShootingLocation1_1_Longitude
        {
            get { return _shootingLocation1_1_Longitude; }
            set
            {
                _shootingLocation1_1_Longitude = value;
                OnPropertyChanged();
            }
        }
        public GPSCoordinates ShootingLocation1_1_GPS
        {
            get { return _shootingLocation1_1_GPS; }
            set
            {
                _shootingLocation1_1_GPS = value;
                OnPropertyChanged();
            }
        }
        public double? ShootingLocation1_2_Latitude
        {
            get { return _shootingLocation1_2_Latitude; }
            set
            {
                _shootingLocation1_2_Latitude = value;
                OnPropertyChanged();
            }
        }
        public double? ShootingLocation1_2_Longitude
        {
            get { return _shootingLocation1_2_Longitude; }
            set
            {
                _shootingLocation1_2_Longitude = value;
                OnPropertyChanged();
            }
        }
        public GPSCoordinates ShootingLocation1_2_GPS
        {
            get { return _shootingLocation1_2_GPS; }
            set
            {
                _shootingLocation1_2_GPS = value;
                OnPropertyChanged();
            }
        }

        public double? ShootingLocation2_1_Latitude
        {
            get { return _shootingLocation2_1_Latitude; }
            set
            {
                _shootingLocation2_1_Latitude = value;
                OnPropertyChanged();
            }
        }
        public double? ShootingLocation2_1_Longitude
        {
            get { return _shootingLocation2_1_Longitude; }
            set
            {
                _shootingLocation2_1_Longitude = value;
                OnPropertyChanged();
            }
        }
        public GPSCoordinates ShootingLocation2_1_GPS
        {
            get { return _shootingLocation2_1_GPS; }
            set
            {
                _shootingLocation2_1_GPS = value;
                OnPropertyChanged();
            }
        }
        public double? ShootingLocation2_2_Latitude
        {
            get { return _shootingLocation2_2_Latitude; }
            set
            {
                _shootingLocation2_2_Latitude = value;
                OnPropertyChanged();
            }
        }
        public double? ShootingLocation2_2_Longitude
        {
            get { return _shootingLocation2_2_Longitude; }
            set
            {
                _shootingLocation2_2_Longitude = value;
                OnPropertyChanged();
            }
        }
        public GPSCoordinates ShootingLocation2_2_GPS
        {
            get { return _shootingLocation2_2_GPS; }
            set
            {
                _shootingLocation2_2_GPS = value;
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
    }

    #region enums
    // nested types not supported by XAML, therefore not part of a class
    public enum E_CoordinateType { Longitude, Latitude };
    #endregion
}
