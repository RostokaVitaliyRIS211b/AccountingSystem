using Avalonia.Controls;

using ObjectsManager.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IProgressBarViewModel
    {
        public string Message { get; set; }

        public double Value { get; set; }

        public double Maximum { get; set; }

        public string Header { get; set; }

        public abstract Task StartBackup();

        public Window? Win {  get; set; }

        public Interaction<object?, string?> Save { get; }

        public abstract Task StopBackup();

        public bool IsBackupInProgress { get; protected set; }
    }
}
