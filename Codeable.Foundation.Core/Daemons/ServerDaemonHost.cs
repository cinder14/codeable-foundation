using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.Daemons;
using Codeable.Foundation.Common;

namespace Codeable.Foundation.Core.Daemons
{
    public class ServerDaemonHost : IDaemonHost
    {
        public ServerDaemonHost(IFoundation iFoundation)
        {
        }

        #region IDaemonHost Members

        public string DaemonHostName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        #endregion
    }
}
