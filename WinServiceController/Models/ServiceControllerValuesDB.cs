using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WinServiceController.Utils;

namespace WinServiceController.Models
{
    public class ServiceControllerValuesDB : NotifyUIBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool CanPauseAndContinue { get; set; }
        public bool CanShutdown { get; set; }
        public bool CanStop { get; set; }
        private ServiceController[]? _DependentServices;
        public string DependentServices
        {
            get
            {
                return GetStringFromServiceController(_DependentServices);
            }
        }

        public string DisplayName { get; set; }
        public string MachineName { get; set; }
        private SafeHandle? _ServiceHandle;
        public string ServiceHandle
        {
            get
            {
                return (GetStringFromServiceHandle(_ServiceHandle));
            }
        }
        public string ServiceName { get; set; }

        private ServiceController[] _ServicesDependedOn;
        public string ServicesDependedOn
        {
            get
            {
                return GetStringFromServiceController(_ServicesDependedOn);
            }
        }

        public ServiceType ServiceType { get; set; }
        public ISite? Site { get; set; }
        private ServiceStartMode _StartType;
        public string StartType
        {
            get
            {
                string retval = "unknown";
                switch (_StartType)
                {
                    case (ServiceStartMode.Boot):
                        retval = "Драйвер, при загрузке";
                        break;
                    case (ServiceStartMode.Automatic):
                        retval = "Автоматически";
                        break;
                    case (ServiceStartMode.Disabled):
                        retval = "Отключено";
                        break;
                    case (ServiceStartMode.Manual):
                        retval = "Вручную";
                        break;
                    case (ServiceStartMode.System):
                        retval = "Система";
                        break;
                }
                return retval;
            }
        }

        private ServiceControllerStatus _Status;
        public ServiceControllerStatus Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                onPropertyChanged("Status");
            }
        }

        public ServiceControllerValuesDB(ServiceController scs, int i = -1)
        {
            import(scs, i);
        }

        public void import(ServiceController sc, int i = -1)
        {
            if (i == -1) Id++;
            else
            {
                Id = i;
            }
            CanPauseAndContinue = sc.CanPauseAndContinue;
            CanShutdown = sc.CanShutdown;
            CanStop = sc.CanStop;
            try
            {
                _DependentServices = sc.DependentServices;
            }
            catch
            {
                _DependentServices = null;
            }
            DisplayName = sc.DisplayName;
            MachineName = sc.MachineName;
            try
            {
                _ServiceHandle = sc.ServiceHandle;
            }
            catch
            {
                _ServiceHandle = null;
            }
            ServiceType = sc.ServiceType;
            ServiceName = sc.ServiceName;
            _ServicesDependedOn = sc.ServicesDependedOn;
            ServiceType = sc.ServiceType;
            Site = sc.Site;
            _StartType = sc.StartType;
            Status = sc.Status;
        }

        private string GetStringFromServiceController(ServiceController[]? scs)
        {
            string retval = "";
            if (scs != null)
            {
                int i = 0;
                foreach (ServiceController sc in scs)
                {
                    if (retval != "")
                    {
                        retval += "; ";
                        if ((i % 2) == 0)
                        {
                            retval += Environment.NewLine;
                        }
                    }
                    if (i < 20) retval += sc.DisplayName;
                    else
                    {
                        retval += Environment.NewLine;
                        retval += "...more than 20 services";
                        break;
                    }
                    i++;
                }
            }
            return retval;
        }

        private string GetStringFromServiceHandle(SafeHandle? serviceHandle)
        {
            string retval = "";
            if (serviceHandle != null)
            {
                retval = serviceHandle.DangerousGetHandle().ToString();
            }
            return retval;
        }
    }
}
