using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.System
{
    public interface ITracer
    {
        IDisposable StartTrace(string operation);
        IDisposable StartTrace(string operation, string identifier);

        int CurrentDepth { get; set; }
    }
}
