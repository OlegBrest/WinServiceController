using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using WinServiceController.Models;
using WinServiceController.Utils;
using WinServiceController.Views;

namespace WinServiceController.ViewModels
{
    public class MainVM : NotifyUIBase
    {
        #region variables
        AppDBContext dBContext = new AppDBContext();

        public System.Timers.Timer timer = new System.Timers.Timer(500);

        #region Services
        private List<ServiceControllerValuesModel> _serviceControllerValuesDB = new List<ServiceControllerValuesModel>();
        public List<ServiceControllerValuesModel> serviceControllerValuesDB
        {
            get { return _serviceControllerValuesDB; }
            set
            {
                _serviceControllerValuesDB = value;
                onPropertyChanged(nameof(serviceControllerValuesDB));
            }
        }

        private ServiceControllerValuesModel _selectedServiceItem;
        public ServiceControllerValuesModel SelectedServiceItem
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
        #endregion

        #region Processes
        private ObservableCollection<ProcessessModel> _processValuesDB = new ObservableCollection<ProcessessModel>();
        public ObservableCollection<ProcessessModel> processValuesDB
        {
            get { return _processValuesDB; }
            set
            {
                _processValuesDB = value;
                onPropertyChanged(nameof(processValuesDB));
            }
        }

        private ProcessessModel _selectedProcessItem;
        public ProcessessModel SelectedProcessItem
        {
            get { return _selectedProcessItem; }
            set
            {
                _selectedProcessItem = value;
                onPropertyChanged(nameof(SelectedProcessItem));
            }
        }
        #endregion
        #region Tracks
        private ObservableCollection<TrackedItemsModel> _trackedItemsDB = new ObservableCollection<TrackedItemsModel>();
        public ObservableCollection<TrackedItemsModel> trackedItemsDB
        {
            get { return _trackedItemsDB; }
            set
            {
                _trackedItemsDB = value; onPropertyChanged(nameof(trackedItemsDB));
            }
        }
        private TrackedItemsModel _selectedTrackedItem;
        public TrackedItemsModel SelectedTrackedItem
        {
            get => _selectedTrackedItem;
            set
            {
                _selectedTrackedItem = value;
                onPropertyChanged(nameof(SelectedTrackedItem));
            }
        }
        #endregion

        #endregion

        public MainVM()
        {
            dBContext.Database.EnsureCreated();
            DBLoad();
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            //FillServiceArray(ServiceController.GetServices());
        }

        private void DBLoad()
        {
            dBContext = new AppDBContext();
            //trackedItemsDB.Clear();
            dBContext.TrackedItems.Load();
            trackedItemsDB = dBContext.TrackedItems.Local.ToObservableCollection();
        }
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            FillServiceArray(ServiceController.GetServices());
            FillProcessArray(Process.GetProcesses());

            CheckTracks();
            //App.Current.Dispatcher.Invoke((Action)delegate
            //{

            //});
        }
        private void CheckTracks()
        {
            foreach (TrackedItemsModel trackItem in trackedItemsDB)
            {
                if (trackItem.Status == ServiceControllerStatus.Running)
                {
                    if (trackItem.Type.Equals("Service"))
                    {
                        ServiceControllerValuesModel? service = serviceControllerValuesDB.FirstOrDefault(sr => (sr.ServiceName == trackItem.Title));
                        if (service != null)
                        {
                            if ((service.Status != ServiceControllerStatus.Running) && trackItem.KeepRunned)
                            {
                                try
                                {
                                    ServiceController sc = new ServiceController(service.ServiceName);
                                    sc.Start();
                                }
                                catch (Exception ex)
                                {
                                    // MessageBox.Show(ex.Message);
                                }
                            }
                            else if ((service.Status != ServiceControllerStatus.Stopped) && trackItem.KeepStopped)
                            {
                                try
                                {
                                    ServiceController sc = new ServiceController(service.ServiceName);
                                    sc.Stop();
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show(ex.Message);
                                }
                            }
                        }
                    }
                    if (trackItem.Type.Equals("File"))
                    {
                        ProcessessModel? process = processValuesDB.FirstOrDefault(pr => (pr.FullFilePath == trackItem.FilePath));
                        if (process != null)
                        {
                            if (trackItem.KeepStopped)
                            {
                                try
                                {
                                    Process.GetProcessById(process.processId).Kill();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                        else if (trackItem.KeepRunned)
                        {
                            Process prc = new Process();
                            prc.StartInfo.FileName = trackItem.FilePath;
                            prc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                            prc.Start();
                        }
                    }
                }
            }

        }

        #region Services
        private void FillServiceArray(ServiceController[] serviceControllers)
        {
            if (serviceControllerValuesDB == null) serviceControllerValuesDB = new List<ServiceControllerValuesModel>();
            foreach (ServiceController serviceController in serviceControllers)
            {
                AddService(serviceController);
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
                ServiceControllerValuesModel serviceController = serviceControllerValuesDB[indx];
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
                serviceControllerValuesDB.Add(new ServiceControllerValuesModel(value, i == -1 ? ++maxId : i));
            }
            else
            {
                ServiceController sc = new ServiceController(value.ServiceName);
                serviceControllerValuesDB[findedID].Status = sc.Status;
                serviceControllerValuesDB[findedID].serviceStartMode = sc.StartType;
                serviceControllerValuesDB[findedID].CanStop = sc.CanStop;
                if (SelectedServiceItem != null)
                {
                    if ((SelectedServiceItem.ServiceName == sc.ServiceName) && (SelectedServiceItem.DisplayName == sc.DisplayName))
                    {
                        SelectedServiceItem.Status = sc.Status;
                        SelectedServiceItem.CanStop = sc.CanStop;
                        SelectedServiceItem.serviceStartMode = sc.StartType;
                        ServiceOnEnabled = SelectedServiceItem == null ? (false) : (SelectedServiceItem.Status == ServiceControllerStatus.Stopped);
                        ServiceOffEnabled = SelectedServiceItem == null ? (false) : ((SelectedServiceItem.Status == ServiceControllerStatus.Running) && (SelectedServiceItem.CanStop));
                        //App.Current.Dispatcher.Invoke((Action)delegate
                        //   {
                        //       RibbonServicesContextMenu.ItemsSource = _RibbonMenuServicesItems;
                        //   });
                    }
                }
            }
            onPropertyChanged(nameof(serviceControllerValuesDB));
        }
        #endregion

        #region Processes
        private void FillProcessArray(Process[] processes)
        {
            if (processValuesDB == null) processValuesDB = new ObservableCollection<ProcessessModel>();
            List<ProcessessModel> listToDel = new List<ProcessessModel>();
            foreach (ProcessessModel proc in processValuesDB)
            {
                if (processes.FirstOrDefault(pr => (pr.Id == proc.processId)) == null)
                {
                    listToDel.Add(proc);
                }
            }
            foreach (ProcessessModel delitem in listToDel)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    processValuesDB.Remove(delitem);
                });

            }
            foreach (Process process in processes)
            {
                AddProcess(process);
            }
            onPropertyChanged(nameof(processValuesDB));
        }
        public void AddProcess(Process value, int i = -1)
        {
            int findedIndex = -1;
            string processName = value.ProcessName;
            string ProcessPath = string.Empty;
            try
            {
                ProcessPath = value.MainModule.FileName;
            }
            catch { }

            if (ProcessPath != string.Empty)
            {
                ProcessessModel process = null;
                try
                {
                    process = processValuesDB.FirstOrDefault(pr => (pr.ProcessName.Equals(processName) && pr.FullFilePath.Equals(ProcessPath)));
                }
                catch { }

                if (process == null)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        processValuesDB.Add(new ProcessessModel(value));
                    });
                }
            }
        }
        #endregion

        #region ServicesContextMenu

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
                try
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        ribbonMenuOnItem.IsEnabled = ServiceOnEnabled;
                        ribbonMenuOffItem.IsEnabled = ServiceOffEnabled;
                    });
                }
                catch
                { //TODO 
                }
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
                try
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        ribbonMenuOnItem.IsEnabled = ServiceOnEnabled;
                        ribbonMenuOffItem.IsEnabled = ServiceOffEnabled;
                    });
                }
                catch
                { //TODO 
                }
                onPropertyChanged("ServiceOffEnabled");
            }
        }
        private RibbonMenuItem ribbonMenuOffItem = new RibbonMenuItem();
        private RibbonMenuItem ribbonMenuOnItem = new RibbonMenuItem();
        private RibbonMenuItem ribbonMenuTrackItem = new RibbonMenuItem();

        private ObservableCollection<RibbonMenuItem> _RibbonMenuServicesItems;
        public ObservableCollection<RibbonMenuItem> RibbonMenuServicesItems
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

        private void CreateServicesContextMenu()
        {
            if (_RibbonMenuServicesItems == null)
            {
                _RibbonMenuServicesItems = new ObservableCollection<RibbonMenuItem>();
                RibbonMenuServicesItems.Clear();

                ribbonMenuOnItem.Header = "Запустить";
                ribbonMenuOnItem.Command = RibbonMenuOnItem_Click;

                ribbonMenuOffItem.Header = "Остановить";
                ribbonMenuOffItem.Command = RibbonMenuOffItem_Click;

                ribbonMenuTrackItem.Header = "Отслеживать";
                ribbonMenuTrackItem.Command = RibbonMenuTrackItem_Click;

                RibbonMenuServicesItems.Add(ribbonMenuOnItem);
                RibbonMenuServicesItems.Add(ribbonMenuOffItem);
                RibbonMenuServicesItems.Add(ribbonMenuTrackItem);
            }
            else
            {
                ribbonMenuOnItem.IsEnabled = ServiceOnEnabled;
                ribbonMenuOffItem.IsEnabled = ServiceOffEnabled;
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

        public DelegateCommand RibbonMenuTrackItem_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    try
                    {
                        TrackedItemsModel trackedItemModel = new TrackedItemsModel();
                        trackedItemModel.Type = obj.ToString();
                        if (trackedItemModel.Type.Equals("Service"))
                        {
                            trackedItemModel.Title = SelectedServiceItem.ServiceName;
                            trackedItemModel.Description = SelectedServiceItem.DisplayName;
                            trackedItemModel.Status = SelectedServiceItem.Status;
                        }
                        else if (trackedItemModel.Type.Equals("File"))
                        {
                            trackedItemModel.Title = SelectedProcessItem.MainModuleName;
                            trackedItemModel.FilePath = SelectedProcessItem.FullFilePath;
                            trackedItemModel.Status = ServiceControllerStatus.Stopped;
                        }
                        TrackAdderWindow trackAdderWindow = new TrackAdderWindow(trackedItemModel, true);
                        if (trackAdderWindow.ShowDialog() == true)
                        {
                            dBContext.TrackedItems.Add(trackedItemModel);
                            try
                            {
                                dBContext.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            DBLoad();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }
        #endregion

        private DelegateCommand trackDataGrid_MouseDoubleClick;
        public ICommand TrackDataGrid_MouseDoubleClick => trackDataGrid_MouseDoubleClick ??= new DelegateCommand(PerformtrackDataGrid_MouseDoubleClick);

        private void PerformtrackDataGrid_MouseDoubleClick(object commandParameter)
        {
            TrackAdderWindow trackAdderWindow = new TrackAdderWindow(SelectedTrackedItem, false);
            if (trackAdderWindow.ShowDialog() == true)
            {
                try
                {
                    if (trackAdderWindow.needToDelete)
                    {
                        dBContext.TrackedItems.Remove(SelectedTrackedItem);
                    }
                    dBContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DBLoad();
            }
        }
    }
}
