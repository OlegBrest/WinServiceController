using SQLitePCL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WinServiceController.Utils;

namespace WinServiceController.Models
{
    public class ProcessessModel : NotifyUIBase
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int processId
        {
            get
            {
                int retval = -1;
                try
                {
                    retval = _proc.Id;
                }
                catch { }
                return retval;
            }
        }

        private Process _proc = new Process();
        public Process proc
        {
            get { return _proc; }
            set { _proc = value; onPropertyChanged(nameof(proc)); }
        }

        private string _ProcessName = string.Empty;
        public string MainModuleName { get => (_mainModule == null ? "" : _mainModule.ModuleName); }
        public string ProcessName
        {
            get { return _ProcessName; }
            set
            {
                _ProcessName = value;
                onPropertyChanged(nameof(ProcessName));
            }
        }
        public string FullFilePath { get => (_mainModule == null ? "" : _mainModule.FileName); }

        public string startTime
        {
            get
            {
                DateTime retval = DateTime.Now;
                try
                {
                    retval = _proc.StartTime;
                }
                catch { }
                return retval.ToString("u");
            }
        }
        public int TotalModulesCount
        {
            get
            {
                int retval = 1;
                try
                {
                    retval = _proc.Modules.Count;
                }
                catch { }
                return retval;
            }
        }

        private ProcessModule? _mainModule = null;



        public ProcessessModel(Process proc)
        {
            _proc = proc;
            try
            {
                _mainModule = _proc.MainModule;
            }
            catch { }
            ProcessName = _proc.ProcessName;
        }

    }
}
