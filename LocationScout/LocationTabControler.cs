using LocationScout.DataAccess;
using LocationScout.Model;
using Microsoft.Win32;
using System;
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
        private MainWindowControler _mainControler;

        public override AutoCompleteTextBox CountryControl { get { return Window.LocationCountryControl; } }
        public override AutoCompleteTextBox AreaControl { get { return Window.LocationAreaControl; } }
        public override AutoCompleteTextBox SubAreaControl { get { return Window.LocationSubAreaControl; } }
        #endregion

        #region contructors
        public LocationTabControler(MainWindowControler mainControler, MainWindow window) : base (window)
        {
            _mainControler = mainControler;

            Window.LocationCountryControl.Leaving += CountryControl_Leaving;
            Window.LocationAreaControl.Leaving += AreaControl_Leaving;
            Window.LocationAreaControl.LeavingViaShift += AreaControl_LeavingViaShift;
            Window.LocationSubAreaControl.Leaving += SubAreaControl_Leaving;
        }
        #endregion

        #region methods
        private void SubAreaControl_Leaving(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Window.LocationNameTextBox.Focus();
        }

        private void AreaControl_LeavingViaShift(object sender, WPFUserControl.AutoCompleteTextBoxControlEventArgs e)
        {
            Window.LocationCountryControl.SetFocus();
        }
     
        internal void Add()
        {
            // get Ids from UI
            var countryId = (Window.LocationCountryControl.GetCurrentObject() as Country).Id;
            var areaId = (Window.LocationAreaControl.GetCurrentObject() as Area).Id;
            var subAreaId = (Window.LocationSubAreaControl.GetCurrentObject() as SubArea).Id;

            // db operations might take a while
            Mouse.OverrideCursor = Cursors.Wait;

            // add country to database
            E_DBReturnCode success = DataAccessAdapter.SmartAddPhotoPlace(countryId, areaId, subAreaId, Window.LocationViewModel, out string errorMessage);

            // refresh or error handling
            // AfterDBWriteSteps(success, errorMessage);

            // reset cursor
            Mouse.OverrideCursor = null;

            // clear the controls and reset focus
            // _window.SettingsCountryControl.ClearText();
            // _window.SettingsAreaControl.ClearText();
            // _window.SettingsSubAreaControl.ClearText();
            // _window.SettingsCountryControl.SetFocus();
        }

        internal void LoadPhoto()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;

                try
                {
                    var byteArray = FileToByteArray(fileName);
                    Window.LocationViewModel.ShootingLocation1_1_Photos.Add(byteArray);;
                }
                catch (Exception e)
                {
                    // to do
                }
            }
        }

        public static byte[] FileToByteArray(string fileName)
        {
            byte[] fileData = null;

            using (FileStream fs = File.OpenRead(fileName))
            {
                var binaryReader = new BinaryReader(fs);
                fileData = binaryReader.ReadBytes((int)fs.Length);
            }
            return fileData;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        #endregion
    }
}
