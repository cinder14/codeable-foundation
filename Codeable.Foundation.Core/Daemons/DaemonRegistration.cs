using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.Daemons;

namespace Codeable.Foundation.Core.Daemons
{
    public class DaemonRegistration
    {
        public IDaemonTask IDaemonTask { get; set; }
        public DaemonConfig DaemonConfig { get; set; }
        public bool AutoStart { get; set; }
    }
}
