using System.Windows.Media.Imaging;

namespace LocationScout.ViewModel
{
    public class LocationListerDisplayItem : LocationDisplayItem
    {
        #region attributes
        private object _tag;

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

        public override void Reset()
        {
            base.Reset();

            Tag = null;
        }

    }

    #region enums
    // nested types not supported by XAML, therefore not part of a class
    public enum E_CoordinateType { Longitude, Latitude };
    #endregion


}
