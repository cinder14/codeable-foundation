using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Daemons
{
    public interface IDaemonTask
    {
        void Execute(IFoundation iFoundation);
        DaemonSynchronizationPolicy SynchronizationPolicy { get; }
        string DaemonName { get; }
    }
}
