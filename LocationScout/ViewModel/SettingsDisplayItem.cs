using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.ViewModel
{
    public class SettingsDisplayItem : BaseObservableObject
    {
        #region attributes
        private string _countryName;
        private string _areaName;
        private string _subareaName;
        private string _subjectLocationName;
        private double? _subjectLocationLatitude;
        private double? _subjectLocationLongitude;
        private bool _autoPasteFromClipboard = true;

        public string CountryName
        {
            get => _countryName;
            set
            {
                _countryName = value;
                OnPropertyChanged();
            }
        }
        public string AreaName
        {
            get => _areaName;
            set
            {
                _areaName = value;
                OnPropertyChanged();
            }
        }
        public string SubAreaName
        {
            get => _subareaName;
            set
            {
                _subareaName = value;
                OnPropertyChanged();
            }
        }
        public string SubjectLocationName
        {
            get => _subjectLocationName;
            set
            {
                _subjectLocationName = value;
                OnPropertyChanged();
            }
        }
        public double? SubjectLocationLatitude
        {
            get => _subjectLocationLatitude;
            set
            {
                _subjectLocationLatitude = value;
                OnPropertyChanged();
            }
        }
        public double? SubjectLocationLongitude
        {
            get => _subjectLocationLongitude;
            set
            {
                _subjectLocationLongitude = value;
                OnPropertyChanged();
            }
        }
        public bool AutoPasteFromClipboard
        {
            get => _autoPasteFromClipboard;
            set
            {
                _autoPasteFromClipboard = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region constructor
        public SettingsDisplayItem()
        {
        }
        #endregion

        #region methods
        public virtual void Reset()
        {
            CountryName = string.Empty;
            AreaName = string.Empty;
            SubAreaName = string.Empty;
            SubjectLocationName = string.Empty;
            SubjectLocationLatitude = null;
            SubjectLocationLongitude = null;
        }
        #endregion
    }
}
