using LocationScout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFUserControl;

namespace LocationScout
{
    abstract public class ControlerBase
    {
        #region enum
        protected enum E_MessageType { success, info, error };
        private System.Windows.Threading.DispatcherTimer _dispatcherTimer;
        #endregion

        #region attributes
        protected MainWindow Window { get; private set; }

        abstract protected Label StatusLabel { get; }
        #endregion

        #region
        public ControlerBase(MainWindow window)
        {
            Window = window;
        }
        #endregion

        #region methods
        protected void ShowMessage(string text, E_MessageType type)
        {
            switch (type)
            {
                case E_MessageType.success:
                    SetMessage(text);
                    break;
                case E_MessageType.info:
                    SetMessage(text);
                    break;
                case E_MessageType.error:
                    MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        private void SetMessage(string text)
        {
            //Window.StatusLabel.Content = text;
            StatusLabel.Content = text;

            if (_dispatcherTimer == null)
            {
                _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            }
            else
            {
                _dispatcherTimer.Start();
            }
            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _dispatcherTimer.Stop();

            //Window.StatusLabel.Content = string.Empty;
            StatusLabel.Content = string.Empty;
        }

        public void RefreshControl(List<Location> locations, AutoCompleteTextBox textBox)
        {
            if (locations != null)
            {
                // clear old locations and entered text
                textBox.ClearSearchPool();
                textBox.ClearText();

                foreach (var location in locations)
                {
                    textBox.AddObject(location.Name, location);
                }
            }
        }
        #endregion
    }
}
