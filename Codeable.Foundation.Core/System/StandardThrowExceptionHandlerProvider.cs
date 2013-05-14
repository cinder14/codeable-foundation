using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.System;

namespace Codeable.Foundation.Core.System
{
    public class StandardThrowExceptionHandlerProvider : IHandleExceptionProvider
    {
        public StandardThrowExceptionHandlerProvider(ILogger iLogger)
        {
            this.ILogger = iLogger;
        }
        protected virtual ILogger ILogger { get; set; }

        #region IHandleExceptionProvider Members

        public virtual string PolicyName { get; set; }

        public virtual IHandleException CreateHandler()
        {
            return new StandardThrowExceptionHandler(this.ILogger);
        }

        #endregion
    }
}
