using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout
{
    public class LocationTabControler
    {
        #region attributes
        private MainWindow _window;

        private ViewModel.Location _locationViewModel;
        #endregion

        #region contructors
        public LocationTabControler(MainWindow window)
        {
            _window = window;
            _locationViewModel = new ViewModel.Location();

            _window.SL_CountriesACTB.Leaving += SL_CountriesACTB_Leaving;
            _window.SL_AreaACTB.Leaving += SL_AreaACTB_Leaving;
            _window.SL_AreaACTB.LeavingViaShift += SL_AreaACTB_LeavingViaShift;
            _window.SL_SubAreaACTB.Leaving += SL_SubAreaACTB_Leaving;
        }
        #endregion

        #region methods
        private void SL_SubAreaACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.LocationNameTextBox.Focus();
        }

        private void SL_AreaACTB_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            _window.SL_CountriesACTB.SetFocus();
        }

        private void SL_AreaACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Area area = e.Object as Area;

            RefreshSubAreaACTB(area.Subareas);
           
            _window.SL_SubAreaACTB.ClearText();

            _window.SL_SubAreaACTB.SetFocus();            
        }

        private void SL_CountriesACTB_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Country country = e.Object as Country;

            RefreshAreaACTB(country.Areas);

            _window.SL_AreaACTB.SetFocus();
        }

        private void RefreshSubAreaACTB(List<SubArea> subAreas)
        {
                _window.SL_SubAreaACTB.ClearSearchPool();

                foreach (var subArea in subAreas)
                {
                    _window.SL_SubAreaACTB.AddObject(subArea.Name, subArea);
                }
        }
        
        private void RefreshAreaACTB(List<Area> areas)
        {
                _window.SL_AreaACTB.ClearSearchPool();

                foreach (var area in areas)
                {
                    _window.SL_AreaACTB.AddObject(area.Name, area);
                }
        }

        internal void Add()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
