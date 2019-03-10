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
using System.Windows.Shapes;

namespace LocationScout
{
    /// <summary>
    /// Interaction logic for SettingsDeleteWindow.xaml
    /// </summary>
    public partial class SettingsDeleteWindow : Window
    {
        #region enums
        public enum EDeleteTarget { Country, Area, Subarea, None };
        #endregion

        #region attributes
        public EDeleteTarget DeleteTarget { get; set; }

        private SettingsDeleteWindowControler _controler;

        public SettingsDeleteDisplayItem DisplayItem { get; set; }
        #endregion

        public SettingsDeleteWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            DisplayItem = new SettingsDeleteDisplayItem();
            this.DataContext = DisplayItem;

            _controler = new SettingsDeleteWindowControler(this, mainWindow);

            _controler.InitializeDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleDeleteButton();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _controler.HandleCancelButton();
        }
    }
}
