using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.System;
using System.Diagnostics;

namespace Codeable.Foundation.Core.System
{
    /// <summary>
    /// Provides tracing for any caller that is invoked with a method wrapper. Caller or Caller is being traced.
    /// </summary>
    [DebuggerStepThrough]
    public partial class WrappedMethodTracer : ITracer
    {
        #region ITracer Members

        public int CurrentDepth { get; set; }

        public virtual IDisposable StartTrace(string operation)
        {
            return new DebugTrace(this, 2, Category.Trace, operation);
        }
        public virtual IDisposable StartTrace(string operation, string identifier)
        {
            return new DebugTrace(this, 2, Category.Trace, operation, identifier);
        }


        #endregion

    }
}
