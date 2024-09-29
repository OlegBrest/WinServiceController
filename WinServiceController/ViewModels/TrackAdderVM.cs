using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinServiceController.Models;
using WinServiceController.Utils;

namespace WinServiceController.ViewModels
{
    public class TrackAdderVM : NotifyUIBase
    {
        private TrackedItemsModel _winModel = new TrackedItemsModel();
        public TrackedItemsModel winModel
        {
            get => _winModel;
            set
            {
                _winModel = value;onPropertyChanged(nameof(winModel));
            }
        }

        private DelegateCommand cancelButton_click;
        public ICommand CancelButton_click => cancelButton_click ??= new DelegateCommand(PerformCancelButton_click);

        private void PerformCancelButton_click(object commandParameter)
        {
            if (winModel.Status == System.ServiceProcess.ServiceControllerStatus.Running) winModel.Status = System.ServiceProcess.ServiceControllerStatus.Stopped;
            else winModel.Status = System.ServiceProcess.ServiceControllerStatus.Running;
        }
    }
}
