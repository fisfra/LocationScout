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
using System.Windows.Shapes;

namespace LocationScout.Lister
{
    /// <summary>
    /// Interaction logic for LocationListerWindow.xaml
    /// </summary>
    public partial class LocationListerWindow : Window
    {
        #region attributes
        private ListerControler _controler;
        #endregion

        #region constructors
        public LocationListerWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            _controler = new ListerControler(mainWindow, this);
        }
        #endregion

        #region methods
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleClose();
        }

        private void GoogleMaps_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleGoogleMaps();
        }


        private void ParkingLocation1_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ParkingLocation2_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ParkingShootingLocation1_1_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ParkingShootingLocation1_2_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ParkingShootingLocation2_1_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ParkingShootingLocation2_2_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LocationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _controler.HandleSelectionChanged();
        } 
        #endregion
    }
}
