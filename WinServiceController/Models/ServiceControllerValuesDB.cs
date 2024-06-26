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
        private ServiceController[] _DependentServices;
        public string DependentServices
        {
            get
            {
                return GetStringFromServiceController(_DependentServices);
            }
        }

        public string DisplayName { get; set; }
        public string MachineName { get; set; }
        public SafeHandle? ServiceHandle { get; set; }
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
        public ServiceStartMode StartType { get; set; }
        public ServiceControllerStatus Status { get; set; }

        public ServiceControllerValuesDB(ServiceController scs, int i)
        {
            import(scs, i);
        }

        public void import(ServiceController sc, int i = 0)
        {
            Id = i;
            CanPauseAndContinue = sc.CanPauseAndContinue;
            CanShutdown = sc.CanShutdown;
            CanStop = sc.CanStop;
            _DependentServices = sc.DependentServices;
            DisplayName = sc.DisplayName;
            MachineName = sc.MachineName;
            try
            {
                ServiceHandle = sc.ServiceHandle;
            }
            catch
            {
                ServiceHandle = null;
            }
            ServiceType = sc.ServiceType;
            ServiceName = sc.ServiceName;
            _ServicesDependedOn = sc.ServicesDependedOn;
            ServiceType = sc.ServiceType;
            Site = sc.Site;
            StartType = sc.StartType;
            Status = sc.Status;
        }

        private string GetStringFromServiceController(ServiceController[] scs)
        {
            string retval = "";
            int i = 0;
            foreach (ServiceController sc in scs)
            {
                if (retval != "")
                {
                    retval += ";";
                    if ((i % 2) == 0)
                    {
                        retval += Environment.NewLine;
                    }
                }
                if (i<20) retval += sc.DisplayName;
                else 
                {
                    retval += Environment.NewLine;
                    retval += "...more than 20 services";
                    break;
                }
                i++;
            }
            return retval;
        }
    }
}
