using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Codeable.Foundation.Common.Daemons
{
    public interface IDaemonTask : IDisposable
    {
        void Execute(IFoundation iFoundation, CancellationToken token);
        DaemonSynchronizationPolicy SynchronizationPolicy { get; }
        string DaemonName { get; }
    }
}
