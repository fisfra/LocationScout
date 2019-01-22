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
    public partial class MainWindow : Window
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

        private void PhotoUpoad1_1Button_Click(object sender, RoutedEventArgs e)
        {
            _controler.LoadPhoto_1_1();
        }

        private void PhotoUpoad1_2Button_Click(object sender, RoutedEventArgs e)
        {
            _controler.LoadPhoto_1_2();
        }

        private void PhotoUpoad2_1Button_Click(object sender, RoutedEventArgs e)
        {
            _controler.LoadPhoto_2_1();
        }

        private void PhotoUpoad2_2Button_Click(object sender, RoutedEventArgs e)
        {
            _controler.LoadPhoto_2_2();
        }

        private void SettingsEditButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.Edit();
        }


        private void SettingsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.Delete();
        }
        #endregion
    }
}
