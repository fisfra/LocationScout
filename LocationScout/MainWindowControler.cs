using System;
using System.Collections.Generic;
using LocationScout.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocationScout
{
    class MainWindowControler
    {
        #region attributes
        private MainWindow _window;
        private SettingTabControler _settingControler;
        #endregion


        #region contructors
        public MainWindowControler(MainWindow window)
        {
            _window = window;
            _settingControler = new SettingTabControler(window);
        }
        #endregion

        #region methods
        internal void HandleClose()
        {
            _window.Close();
        }

        internal void HandleAdd()
        {
            _settingControler.Add();
        }
        #endregion  
    }
}
