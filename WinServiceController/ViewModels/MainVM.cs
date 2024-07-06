using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using WinServiceController.Models;
using WinServiceController.Utils;

namespace WinServiceController.ViewModels
{
    public class MainVM : NotifyUIBase
    {

        private List<ServiceControllerValuesDB> _serviceControllerValuesDB = new List<ServiceControllerValuesDB>();

        public List<ServiceControllerValuesDB> serviceControllerValuesDB
        {
            get { return _serviceControllerValuesDB; }
            set
            {
                _serviceControllerValuesDB = value;
                onPropertyChanged(nameof(serviceControllerValuesDB));
            }
        }

        public System.Timers.Timer timer = new System.Timers.Timer(500);

        private ServiceControllerValuesDB _selectedServiceItem;
        public ServiceControllerValuesDB SelectedServiceItem
        {
            get { return _selectedServiceItem; }
            set
            {
                _selectedServiceItem = value;
                ServiceOnEnabled = SelectedServiceItem == null ? (false) : (SelectedServiceItem.Status == ServiceControllerStatus.Stopped);
                ServiceOffEnabled = SelectedServiceItem == null ? (false) : ((SelectedServiceItem.Status == ServiceControllerStatus.Running) && (SelectedServiceItem.CanStop));
                CreateServicesContextMenu();
                onPropertyChanged(nameof(SelectedServiceItem));
            }
        }
        public MainVM()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            //FillServiceArray(ServiceController.GetServices());
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            /*App.Current.Dispatcher.Invoke((Action)delegate
            {*/
            FillServiceArray(ServiceController.GetServices());
            //});
        }

        private void FillServiceArray(ServiceController[] serviceControllers)
        {
            if (serviceControllerValuesDB == null) serviceControllerValuesDB = new List<ServiceControllerValuesDB>();
            //int i = 0;
            foreach (ServiceController serviceController in serviceControllers)
            {
                //serviceControllerValuesDB[i] = new ServiceControllerValuesDB (serviceController,i);
                //serviceControllerValuesDB.Add(new ServiceControllerValuesDB(serviceController, i));
                AddService(serviceController);
                //i++;
            }
            onPropertyChanged(nameof(serviceControllerValuesDB));
        }

        public void AddService(ServiceController value, int i = -1)
        {
            int findedID = -1;
            int maxId = -1;
            string serviceName = value.ServiceName;
            string displayName = value.DisplayName;
            int servicesCount = serviceControllerValuesDB.Count;
            for (int indx = 0; indx < servicesCount; indx++)
            {
                ServiceControllerValuesDB serviceController = serviceControllerValuesDB[indx];
                if ((serviceController.ServiceName == serviceName) && (serviceController.DisplayName == displayName))
                {
                    findedID = indx;
                }
                if (serviceController.Id >= maxId)
                {
                    maxId = serviceController.Id;
                }
            }
            if (findedID == -1)
            {
                serviceControllerValuesDB.Add(new ServiceControllerValuesDB(value, i == -1 ? ++maxId : i));
            }
            else
            {
                ServiceController sc = new ServiceController(value.ServiceName);
                serviceControllerValuesDB[findedID].Status = sc.Status;
                serviceControllerValuesDB[findedID].CanStop = sc.CanStop;
                if (SelectedServiceItem != null)
                {
                    if ((SelectedServiceItem.ServiceName == sc.ServiceName) && (SelectedServiceItem.DisplayName == sc.DisplayName))
                    {
                        SelectedServiceItem.Status = sc.Status;
                        SelectedServiceItem.CanStop = sc.CanStop;
                        ServiceOnEnabled = SelectedServiceItem == null ? (false) : (SelectedServiceItem.Status == ServiceControllerStatus.Stopped);
                        ServiceOffEnabled = SelectedServiceItem == null ? (false) : ((SelectedServiceItem.Status == ServiceControllerStatus.Running) && (SelectedServiceItem.CanStop));
                        App.Current.Dispatcher.Invoke((Action)delegate
           {
               RibbonServicesContextMenu.ItemsSource = _RibbonMenuServicesItems;
           });

                    }
                }
            }
            onPropertyChanged(nameof(serviceControllerValuesDB));
        }

        #region ContextMenu


        private bool _ServiceOnEnabled;
        public bool ServiceOnEnabled
        {
            get
            {
                return _ServiceOnEnabled;
            }
            set
            {
                _ServiceOnEnabled = value;
                onPropertyChanged("ServiceOnEnabled");
            }
        }

        private bool _ServiceOffEnabled;
        public bool ServiceOffEnabled
        {
            get
            {
                return _ServiceOffEnabled;
            }
            set
            {
                _ServiceOffEnabled = value;
                onPropertyChanged("ServiceOffEnabled");
            }
        }
        private RibbonMenuItem ribbonMenuOffItem = new RibbonMenuItem();
        private RibbonMenuItem ribbonMenuOnItem = new RibbonMenuItem();

        private List<RibbonMenuItem> _RibbonMenuServicesItems;
        public List<RibbonMenuItem> RibbonMenuServicesItems
        {
            get
            {
                return _RibbonMenuServicesItems;
            }
            set
            {
                _RibbonMenuServicesItems = value;
                onPropertyChanged(nameof(RibbonMenuServicesItems));
            }
        }

        private RibbonContextMenu _RibbonServicesContextMenu;
        public RibbonContextMenu RibbonServicesContextMenu
        {
            get
            {
                //CreateServicesContextMenu();
                return _RibbonServicesContextMenu;
            }
            set
            {
                //_RibbonServicesContextMenu.ItemsSource = RibbonMenuServicesItems;
                _RibbonServicesContextMenu = value;
                onPropertyChanged(nameof(RibbonServicesContextMenu));
            }
        }

        private void CreateServicesContextMenu()
        {
            if (_RibbonMenuServicesItems == null)
            {
                _RibbonMenuServicesItems = new List<RibbonMenuItem>();
                _RibbonServicesContextMenu = new RibbonContextMenu();
                _RibbonMenuServicesItems.Clear();

                ribbonMenuOnItem.Header = "Запустить";
                ribbonMenuOnItem.IsEnabled = ServiceOnEnabled;

                ribbonMenuOffItem.Header = "Остановить";
                ribbonMenuOffItem.IsEnabled = ServiceOffEnabled;


                _RibbonMenuServicesItems.Add(ribbonMenuOnItem);
                _RibbonMenuServicesItems.Add(ribbonMenuOffItem);
                _RibbonServicesContextMenu.ItemsSource = _RibbonMenuServicesItems;
            }
            else
            {
                ribbonMenuOnItem.IsEnabled = ServiceOnEnabled;
                ribbonMenuOffItem.IsEnabled = ServiceOffEnabled;
                _RibbonServicesContextMenu.ItemsSource = _RibbonMenuServicesItems;
            }
        }

        public DelegateCommand RibbonMenuOnItem_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    try
                    {
                        ServiceController sc = new ServiceController(SelectedServiceItem.ServiceName);
                        sc.Start();
                        AddService(sc);
                        //FillServiceArray(ServiceController.GetServices());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });
            }
        }

        public DelegateCommand RibbonMenuOffItem_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    try
                    {
                        ServiceController sc = new ServiceController(SelectedServiceItem.ServiceName);
                        if (sc.CanStop)
                        {
                            sc.Stop();
                            AddService(sc);
                            //FillServiceArray(ServiceController.GetServices());
                        }
                        else MessageBox.Show("Службу нельзя остановить.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });
            }
        }
        #endregion


    }
}
