using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using WinServiceController.ViewModels;

namespace WinServiceController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainVM datacontext;
        public MainWindow()
        {
            //InitializeComponent();
            datacontext = (MainVM) this.DataContext;
        }

        private void ServicesDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            datacontext = (MainVM)this.DataContext;
            //(sender as DataGrid).ContextMenu = datacontext.RibbonServicesContextMenu;
        }

        private void RibbonMenuOnItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}