using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WinServiceController.Models;
using WinServiceController.Utils;

namespace WinServiceController.ViewModels
{
    public class MainVM : NotifyUIBase
    {

        private List <ServiceControllerValuesDB> _serviceControllerValuesDB = new List <ServiceControllerValuesDB>();

        public List<ServiceControllerValuesDB> serviceControllerValuesDB
        {
            get { return _serviceControllerValuesDB; }
            set 
            { 
                _serviceControllerValuesDB = value;
                onPropertyChanged(nameof(serviceControllerValuesDB));
            }
        }
        public MainVM()
        {
            fillServiceArray (ServiceController.GetServices());
        }

        private void fillServiceArray(ServiceController[] serviceControllers  ) 
        {
            serviceControllerValuesDB = new List<ServiceControllerValuesDB>();
            int i = 0;
            foreach ( ServiceController serviceController in serviceControllers ) 
            {
                //serviceControllerValuesDB[i] = new ServiceControllerValuesDB (serviceController,i);
                serviceControllerValuesDB.Add(new ServiceControllerValuesDB(serviceController, i));
                i++;
            }

        }
    }
}
