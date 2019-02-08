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
        internal ListerControler Controler { get; private set; }
        #endregion

        #region constructors
        internal LocationListerWindow(MainWindow mainWindow, LocationTabControler locationTabControler)
        {
            InitializeComponent();

            Controler = new ListerControler(mainWindow, this, locationTabControler);
        }
        #endregion

        #region methods
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Controler.HandleClose();
        }

        private void ShootingLocationGoogleMaps_Click(object sender, RoutedEventArgs e)
        {
            Controler.HandleShootingLocationGoogleMaps();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Controler.HandleEdit();
        }

        private void LocationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Controler.HandleSelectionChanged();
        }

        private void SubjectLocationGoogleMapsButton_Click(object sender, RoutedEventArgs e)
        {
            Controler.HandleSubjectLocationGoogleMaps();
        }

        private void ParkingLocationGoogleMapsButton_Click(object sender, RoutedEventArgs e)
        {
            Controler.HandleParkingLocationGoogleMaps();
        }        
        #endregion
    }
}
