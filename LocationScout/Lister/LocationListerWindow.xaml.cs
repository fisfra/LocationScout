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

namespace LocationScout
{
    /// <summary>
    /// Interaction logic for LocationListerWindow.xaml
    /// </summary>
    public partial class LocationListerWindow : ExtendedWindow
    {
        #region attributes
        private ListerControler _controler;
        #endregion

        #region constructors
        internal LocationListerWindow(MainWindow mainWindow, LocationTabControler locationTabControler)
        {
            InitializeComponent();

            _controler = new ListerControler(mainWindow, this, locationTabControler);
        }
        #endregion

        #region methods
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleClose();
        }

        private void ShootingLocationGoogleMaps_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleShootingLocationGoogleMaps();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleEdit();
        }

        private void LocationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _controler.HandleSelectionChanged();
        }

        private void SubjectLocationGoogleMapsButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleSubjectLocationGoogleMaps();
        }

        private void ParkingLocationGoogleMapsButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleParkingLocationGoogleMaps();
        }        
        #endregion
    }
}
