using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Daemons
{
    public interface IDaemon
    {
        string InstanceName { get; }
        DaemonConfig Config { get; }
        bool IsExecuting { get; }
        bool IsOnDemand { get; }
        int IntervalMilliSeconds { get; }
        DateTime? LastExecuteStartTime { get; }
        DateTime? LastExecuteEndTime { get; }
    }
}
