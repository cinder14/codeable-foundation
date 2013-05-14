using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.Daemons;
using Codeable.Foundation.Common;

namespace Codeable.Foundation.Core.Daemons
{
    public class EmptyDaemonSynchronizationHandler : IDaemonSynchronizationHandler
    {
        public EmptyDaemonSynchronizationHandler(IFoundation iFoundation)
        {

        }

        #region IDaemonSynchronizationHandler Members

        public bool TryBeginDaemonTask(IDaemonHost host, IDaemonTask task)
        {
            return true;
        }
        public void ClearAllDaemonTasks(IDaemonHost host)
        {
        }
        public bool EndDaemonTask(IDaemonHost host, IDaemonTask task)
        {
            return true;
        }

        #endregion
    }
}
