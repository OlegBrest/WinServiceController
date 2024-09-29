using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinServiceController.Utils;
using System.ServiceProcess;

namespace WinServiceController.Models
{
    public class TrackedItemsModel : NotifyUIBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        private int _Id;
        public int Id
        {
            get => _Id;
            set
            {
                _Id = value; onPropertyChanged(nameof(Id));
            }
        }

        private string _Type = string.Empty;
        public string Type
        {
            get => _Type;
            set
            {
                _Type = value; onPropertyChanged(nameof(Type));
            }
        }

        private string _Title = string.Empty;
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value; onPropertyChanged(nameof(Title));
            }
        }

        private string _Description=string.Empty;
        public string Description
        {
            get => _Description;
            set
            {
                _Description = value; onPropertyChanged(nameof(Description));
            }
        }

        private string _FilePath = string.Empty;
        public string FilePath
        {
            get => _FilePath;
            set
            {
                _FilePath = value; onPropertyChanged(nameof(FilePath));
            }
        }

        private ServiceControllerStatus _Status;
        public ServiceControllerStatus Status
        {
            get { return _Status; }
            set
            {
                _Status = value; onPropertyChanged(nameof(Status));
            }
        }

        private bool _KeepRunned = false;
        public bool KeepRunned
        {
            get => _KeepRunned;
            set
            {
                _KeepRunned = value; onPropertyChanged(nameof(KeepRunned));
            }
        }

        private bool _KeepStopped = false;
        public bool KeepStopped
        {
            get => _KeepStopped;
            set
            {
                _KeepStopped = value; onPropertyChanged(nameof(KeepStopped));
            }
        }

        private string _CustomControl = string.Empty;
        public string CustomControl
        {
            get => _CustomControl;
            set { _CustomControl = value; onPropertyChanged(nameof(CustomControl));}
        }
    }
}