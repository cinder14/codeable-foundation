using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.System;
using System.Diagnostics;

namespace Codeable.Foundation.Core.System
{
    /// <summary>
    /// Provides tracing for standard method calls (caller is being traced)
    /// </summary>
    [DebuggerStepThrough]
    public partial class EmptyTracer : ITracer
    {
        private EmptyTrace _emptyTrace = new EmptyTrace();

        #region ITracer Members

        public int CurrentDepth { get; set; }

        public virtual IDisposable StartTrace(string operation)
        {
            return _emptyTrace;
        }
        public virtual IDisposable StartTrace(string operation, string identifier)
        {
            return _emptyTrace;
        }

        #endregion

    }
}
