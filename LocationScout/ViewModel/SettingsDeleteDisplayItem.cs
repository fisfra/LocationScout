using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.ViewModel
{
    public class SettingsDeleteDisplayItem : BaseObservableObject
    {
        #region attributes
        private long _countryAreaCountToDelete;
        private long _countrySubAreaCountToDelete;
        private long _countryPhotoPlaceCountToDelete;
        private long _areaSubAreaCountToDelete;
        private long _areaPhotoPlaceCountToDelete;
        private long _subAreaPhotoPlaceCountToDelete;

        public long CountryAreaCountToDelete
        {
            get => _countryAreaCountToDelete;
            set
            {
                _countryAreaCountToDelete = value;
                OnPropertyChanged();
            }
        }
        public long CountrySubAreaCountToDelete
        {
            get => _countrySubAreaCountToDelete;
            set
            {
                _countrySubAreaCountToDelete = value;
                OnPropertyChanged();
            }
        }
        public long CountryPhotoPlaceCountToDelete
        {
            get => _countryPhotoPlaceCountToDelete;
            set
            {
                _countryPhotoPlaceCountToDelete = value;
                OnPropertyChanged();
            }
        }

        public long AreaSubAreaCountToDelete
        {
            get => _areaSubAreaCountToDelete;
            set
            {
                _areaSubAreaCountToDelete = value;
                OnPropertyChanged();
            }
        }
        public long AreaPhotoPlaceCountToDelete
        {
            get => _areaPhotoPlaceCountToDelete;
            set
            {
                _areaPhotoPlaceCountToDelete = value;
                OnPropertyChanged();
            }
        }

        public long SubAreaPhotoPlaceCountToDelete
        {
            get => _subAreaPhotoPlaceCountToDelete;
            set
            {
                _subAreaPhotoPlaceCountToDelete = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
