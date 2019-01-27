using LocationScout.DataAccess;
using LocationScout.Model;
using LocationScout.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFUserControl;
using static LocationScout.DataAccess.PersistenceManager;

namespace LocationScout
{
    internal class LocationTabControler : TabControlerBase
    {
        #region attributes
        
        public override AutoCompleteTextBox CountryControl { get { return Window.LocationCountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return Window.LocationAreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return Window.LocationSubAreaControl; } }
        public override AutoCompleteTextBox SubjectLocationControl { get {return new AutoCompleteTextBox(); } }
        #endregion

        #region contructors
        public LocationTabControler(MainWindowControler mainControler, MainWindow window) : base (window, mainControler)
        {
        }
        #endregion

        #region methods
        
        private void AreaControl_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Window.LocationCountryControl.SetFocus();
        }
     
        internal void Add()
        {
            /*
            // get Ids from UI
            var countryId = (Window.LocationCountryControl.GetCurrentObject() as Country).Id;
            var areaId = (Window.LocationAreaControl.GetCurrentObject() as Area).Id;
            var subAreaId = (Window.LocationSubAreaControl.GetCurrentObject() as SubArea).Id;

            // db operations might take a while
            Mouse.OverrideCursor = Cursors.Wait;

            // add country to database
            E_DBReturnCode success = DataAccessAdapter.AddShootingLocation(countryId, areaId, subAreaId, Window.LocationViewModel, out string errorMessage);

            // refresh or error handling
            // AfterDBWriteSteps(success, errorMessage);

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and reset focus
            // _window.SettingsCountryControl.ClearText();
            // _window.SettingsAreaControl.ClearText();
            // _window.SettingsSubAreaControl.ClearText();
            // _window.SettingsCountryControl.SetFocus();*/
        }

        internal void LoadPhoto_2_2()
        {
            LoadPhoto(Window.LocationViewModel.ShootingLocation2_2_Photos);
        }

        internal void LoadPhoto_2_1()
        {
            LoadPhoto(Window.LocationViewModel.ShootingLocation2_1_Photos);
        }

        internal void LoadPhoto_1_2()
        {
            LoadPhoto(Window.LocationViewModel.ShootingLocation1_2_Photos);
        }

        internal void LoadPhoto_1_1()
        {
            LoadPhoto(Window.LocationViewModel.ShootingLocation1_1_Photos);
        }

        private void LoadPhoto(ObservableCollection<byte[]> photos)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;

                try
                {
                    var byteArray = ImageTools.FileToByteArray(fileName);
                    photos.Add(byteArray); ;
                }
                catch (Exception e)
                {
                    ShowMessage("Error loading file." + e.Message, E_MessageType.error);
                }
            }
        }
       
        #endregion
    }
}
