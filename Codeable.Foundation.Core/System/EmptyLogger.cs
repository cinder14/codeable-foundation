using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Codeable.Foundation.Common.System;

namespace Codeable.Foundation.Core.System
{
    public partial class EmptyLogger : ILogger
    {
        #region ILogger Members
        [DebuggerNonUserCode]
        public virtual void Write(object message)
        {
        }
        [DebuggerNonUserCode]
        public virtual void Write(object message, string category)
        {
        }
        [DebuggerNonUserCode]
        public virtual void Write(object message, string category, int eventId)
        {
        }
        [DebuggerNonUserCode]
        public virtual void Write(object message, string category, int eventId, int priority)
        {
        }
        [DebuggerNonUserCode]
        public virtual void Write(object message, string category, int eventId, int priority, global::System.Diagnostics.TraceEventType severity)
        {
        }

        #endregion
    }
}
