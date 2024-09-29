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
using WinServiceController.Models;
using WinServiceController.ViewModels;

namespace WinServiceController.Views
{
    /// <summary>
    /// Логика взаимодействия для TrackAdderWindow.xaml
    /// </summary>
    public partial class TrackAdderWindow : Window
    {
        public bool needToDelete = false;
        public TrackAdderWindow(TrackedItemsModel _model, bool isNewElement)
        {
            InitializeComponent();
            ((TrackAdderVM)this.DataContext).winModel = _model;
            if (isNewElement)
            {
                OffDeletePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                OffDeletePanel.Visibility= Visibility.Visible;
            }
        }

        private void CancelButton_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OkButton_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void DeleteButton_click(object sender, RoutedEventArgs e)
        {
            needToDelete = true;
            this.DialogResult = true;
        }
    }
}
