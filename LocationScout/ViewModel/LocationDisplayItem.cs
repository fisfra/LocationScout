﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.ViewModel
{
    public class LocationDisplayItem : BaseObservableObject
    {
        #region attributes
        private string _locationName;

        private double _subjectLatitude;
        private double _subjectLongitude;

        private double _shootinglocation1_parking_latitude;
        private double _shootinglocation1_parking_longitude;
        private double _shootinglocation1_1_latitude;
        private double _shootinglocation1_1_longitude;
        private double _shootinglocation1_2_latitude;
        private double _shootinglocation1_2_longitude;

        private double _shootinglocation2_parking_latitude;
        private double _shootinglocation2_parking_longitude;
        private double _shootinglocation2_1_latitude;
        private double _shootinglocation2_1_longitude;        
        private double _shootinglocation2_2_latitude;
        private double _shootinglocation2_2_longitude;

        public string LocationName
        {
            get { return _locationName; }
            set
            {
                _locationName = value;
                OnPropertyChanged();
            }
        }

        public double SubjectLatitude
        {
            get { return _subjectLatitude; }
            set
            {
                _subjectLatitude = value;
                OnPropertyChanged();
            }
        }
        public double SubjectLongitude
        {
            get { return _subjectLongitude; }
            set
            {
                _subjectLongitude = value;
                OnPropertyChanged();
            }
        }

        public double ShootingLocation1_Parking_Latitude
        {
            get { return _shootinglocation1_parking_latitude; }
            set
            {
                _shootinglocation1_parking_latitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation1_Parking_Longitude
        {
            get { return _shootinglocation1_parking_longitude; }
            set
            {
                _shootinglocation1_parking_longitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation1_1_Latitude
        {
            get { return _shootinglocation1_1_latitude; }
            set
            {
                _shootinglocation1_1_latitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation1_1_Longitude
        {
            get { return _shootinglocation1_1_longitude; }
            set
            {
                _shootinglocation1_1_longitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation1_2_Latitude
        {
            get { return _shootinglocation1_2_latitude; }
            set
            {
                _shootinglocation1_2_latitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation1_2_Longitude
        {
            get { return _shootinglocation1_2_longitude; }
            set
            {
                _shootinglocation1_2_longitude = value;
                OnPropertyChanged();
            }
        }       

        public double ShootingLocation2_Parking_Latitude
        {
            get { return _shootinglocation2_parking_latitude; }
            set
            {
                _shootinglocation2_parking_latitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation2_Parking_Longitude
        {
            get { return _shootinglocation2_parking_longitude; }
            set
            {
                _shootinglocation2_parking_longitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation2_1_Latitude
        {
            get { return _shootinglocation2_1_latitude; }
            set
            {
                _shootinglocation2_1_latitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation2_1_Longitude
        {
            get { return _shootinglocation2_1_longitude; }
            set
            {
                _shootinglocation2_1_longitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation2_2_Latitude
        {
            get { return _shootinglocation2_2_latitude; }
            set
            {
                _shootinglocation2_2_latitude = value;
                OnPropertyChanged();
            }
        }
        public double ShootingLocation2_2_Longitude
        {
            get { return _shootinglocation2_2_longitude; }
            set
            {
                _shootinglocation2_2_longitude = value;
                OnPropertyChanged();
            }
        }       
        #endregion
    }
}