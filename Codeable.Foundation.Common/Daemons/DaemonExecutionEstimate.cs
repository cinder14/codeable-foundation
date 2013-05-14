using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Daemons
{
    public class DaemonExecutionEstimate
    {
        public string Name { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime? NextScheduledStart { get; set; }
        public DateTime? LastExecutedStart { get; set; }
        public DateTime? LastExecutedEnd { get; set; }

        public bool IsRunning { get; set; }
    }
}
