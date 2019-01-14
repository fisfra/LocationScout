using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout
{
    class MainWindowControler
    {
        #region attributes
        private MainWindow _window;
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
        #endregion
    }
}
