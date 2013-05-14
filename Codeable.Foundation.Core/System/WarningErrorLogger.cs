using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Codeable.Foundation.Common.System;

namespace Codeable.Foundation.Core.System
{
    public partial class WarningErrorLogger : ILogger
    {
        #region ILogger Members

        [DebuggerNonUserCode]
        public virtual void Write(object message)
        {
            
        }
        [DebuggerNonUserCode]
        public virtual void Write(object message, string category)
        {
            if (Category.Warning.Equals(category, StringComparison.OrdinalIgnoreCase) || Category.Error.Equals(category, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine(Assumptions.LOG_PREFIX + message, category);
            }
        }
        [DebuggerNonUserCode]
        public virtual void Write(object message, string category, int eventId)
        {
            if (Category.Warning.Equals(category, StringComparison.OrdinalIgnoreCase) || Category.Error.Equals(category, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine(string.Format(Assumptions.LOG_PREFIX + "EventID: {0}. Message: {1}", eventId, message), category);
            }
        }
        [DebuggerNonUserCode]
        public virtual void Write(object message, string category, int eventId, int priority)
        {
            if (Category.Warning.Equals(category, StringComparison.OrdinalIgnoreCase) || Category.Error.Equals(category, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine(string.Format(Assumptions.LOG_PREFIX + "EventID: {0}. Priority: {1}. Message: {2}", eventId, priority, message), category);
            }
        }
        [DebuggerNonUserCode]
        public virtual void Write(object message, string category, int eventId, int priority, global::System.Diagnostics.TraceEventType severity)
        {
            if (Category.Warning.Equals(category, StringComparison.OrdinalIgnoreCase) || Category.Error.Equals(category, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine(string.Format(Assumptions.LOG_PREFIX + "Severity: {3}. EventID: {0}. Priority: {1}. Message: {2}", eventId, priority, message, severity.ToString()), category);
            }
        }

        #endregion
    }
}
