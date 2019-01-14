using System;
using System.Collections.Generic;
using LocationScout.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout
{
    class MainWindowControler
    {
        #region attributes
        private MainWindow _window;

        private List<Country> _allCountries;
        private List<Area> _allAreas;
        private List<SubArea> _allSubareas;
        #endregion


        #region contructors
        public MainWindowControler(MainWindow window)
        {
            _window = window;
        }
        #endregion

        #region methods
        internal void HandleClose()
        {
            _window.Close();
        }

        internal void HandleAdd()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
