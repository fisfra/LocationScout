using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUserControl;

namespace LocationScout
{
    internal class LocationTabControler : TabControlerBase
    {
        #region attributes
        private MainWindow _window;
        private MainWindowControler _mainControler;

        public override AutoCompleteTextBox CountryControl { get { return _window.LocationCountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return _window.LocationAreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return _window.LocationSubAreaControl; } }
        #endregion

        #region contructors
        public LocationTabControler(MainWindowControler mainControler) : base (mainControler.Window)
        {
            _mainControler = mainControler;

            _window = _mainControler.Window;

            _window.LocationCountryControl.Leaving += CountryControl_Leaving;
            _window.LocationAreaControl.Leaving += AreaControl_Leaving;
            _window.LocationAreaControl.LeavingViaShift += AreaControl_LeavingViaShift;
            _window.LocationSubAreaControl.Leaving += SubAreaControl_Leaving;
        }
        #endregion

        #region methods
        private void SubAreaControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.LocationNameTextBox.Focus();
        }

        private void AreaControl_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.LocationCountryControl.SetFocus();
        }
     
        internal void Add()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
