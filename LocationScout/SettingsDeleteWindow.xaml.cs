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
        #region attributes
        public SettingDisplayItem SettingDisplayItem {get; set;}
        #endregion

        public SettingsDeleteWindow(SettingDisplayItem settingDisplayItem)
        {
            SettingDisplayItem = settingDisplayItem;

            InitializeComponent();

            this.DataContext = SettingDisplayItem;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
