using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Daemons
{
    public interface IDaemonSynchronizationHandler
    {
        bool TryBeginDaemonTask(IDaemonHost host, IDaemonTask task);
        void ClearAllDaemonTasks(IDaemonHost host);
        bool EndDaemonTask(IDaemonHost host, IDaemonTask task);
    }
}
