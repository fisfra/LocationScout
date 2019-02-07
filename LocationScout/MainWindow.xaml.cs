using LocationScout.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocationScout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ExtendedWindow
    {
        #region attributes
        private MainWindowControler _controler;

        public LocationDisplayItem LocationViewModel { get; set; }
        #endregion

        #region contructors
        public MainWindow()
        {
            InitializeComponent();

            LocationViewModel = new LocationDisplayItem();

            SetDataContext();

            // call after initilaze component
            _controler = new MainWindowControler(this);
        }
        #endregion

        #region methods   
        protected override void OnClipboardUpdate()
        {
            _controler.HandleClipboardUpdate();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleClose();
        }

        private void SettingAddButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleSettingAdd();
        }

        private void LocationButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleLocationAdd();
        }

        private void SetDataContext()
        {
            DataContext = LocationViewModel;
        }

        private void LocationButtonShow_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleLocationShow();
        }

        private void LocationButtonClear_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleLocationClear();
        }

        private void SettingsEditButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.SettingsEdit();
        }

        private void SettingsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.Delete();
        }

        private void SettingsSubjectLocationLatitute_GotFocus(object sender, RoutedEventArgs e)
        {
            Settings_SubjectLocationLatituteTextBox.SelectAll();
        }

        private void SettingsSubjectLocationLongitude_GotFocus(object sender, RoutedEventArgs e)
        {
            Settings_SubjectLocationLongitudeTextBox.SelectAll();
        }

        private void SubjectGPSGoogleMapsButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleGoopleMapsSubjectLocation();
        }

        private void SettingsCountryControlEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            _controler.HandleSettingsCountryControlEditLostFocus();
        }

        private void SettingsAreaControlEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            _controler.HandleSettingsAreaControlEditLostFocus();
        }

        private void SettingsSubAreaControlEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            _controler.HandleSettingsSubAreaControlEditLostFocus();
        }

        private void SettingsSubjectLocationControlEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            _controler.HandleSettingsSubjectLocationControlEditLostFocus();
        }

        private void PhotoUploadButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandlePhotoUpload();
        }

        private void RemovePhoto_1_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleRemovePhoto_1();
        }

        private void RemovePhoto_2_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleRemovePhoto_2();
        }

        private void RemovePhoto_3_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleRemovePhoto_3();
        }
    
        private void LocationButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            _controler.LocationEdit();
        }

        private void ShootingLocationControlEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            _controler.HandleShootingLocationControlEditLostFocus();
        }

        private void ParkingLocationControl_LostFocus(object sender, RoutedEventArgs e)
        {
            _controler.HandleParkingLocationControlEditLostFocus();
        } 
        #endregion

    }
}
