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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EditChangeSelectionWindow : Window
    {
        public enum EMode { edit, change };

        public EditChangeSelectionWindow(EMode mode)
        {
            InitializeComponent();

            switch (mode)
            {
                case EMode.edit:
                    Title = "Select edit";
                    ObjectSelectionGroupBox.Header = "Select objects to edit";
                    EditChangeButton.Content = "Edit";
                    break;
                case EMode.change:
                    Title = "Select change";
                    ObjectSelectionGroupBox.Header = "Select objects to change";
                    EditChangeButton.Content = "Change";
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    throw new Exception("Unknown enum value in EditChangeSelectionWindow::EditChangeSelectionWindow()");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
