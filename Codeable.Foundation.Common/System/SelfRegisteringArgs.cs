using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.System;

namespace Codeable.Foundation.Common
{
    public class SelfRegisteringArgs : EventArgs
    {
        public SelfRegisteringArgs(IDynamicallySelfRegisterWithUnity selfRegisterer)
        {
            this.SelfRegisterer = selfRegisterer;
        }

        public IDynamicallySelfRegisterWithUnity SelfRegisterer { get; private set; }
        public bool Cancel { get; set; }
    }
    
}
