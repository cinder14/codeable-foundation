using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Codeable.Foundation.Common.System
{
    public interface ILogger
    {
        void Write(object message);
        void Write(object message, string category);
        void Write(object message, string category, int eventId);
        void Write(object message, string category, int eventId, int priority);
        void Write(object message, string category, int eventId, int priority, TraceEventType severity);
    }
}
