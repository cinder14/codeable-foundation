using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.System;
using System.Security;

namespace Codeable.Foundation.Core.System
{
    public partial class StandardThrowExceptionHandler : IHandleException
    {
        public StandardThrowExceptionHandler(ILogger iLogger)
        {
            this.ILogger = iLogger;
        }

        public virtual string PolicyName { get; set; }
        protected virtual ILogger ILogger { get; set; }

        public virtual bool HandleException(Exception ex, out bool rethrowCurrent, out Exception replacedException)
        {
            this.LogException(ex);
            
            rethrowCurrent = true;
            replacedException = null;
            return true;
        }

        protected virtual void LogException(Exception ex)
        {
            string message = FormatException(ex);
            this.ILogger.Write(message, Category.Error);
        }
        protected virtual string FormatException(Exception ex)
        {
            return FormatException(ex, DateTime.Now.ToString("r"));
        }
        protected virtual string FormatException(Exception ex, string tag)
        {
            return CoreUtility.FormatException(ex, tag);
        }

    }
}
