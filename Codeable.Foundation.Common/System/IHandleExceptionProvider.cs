using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.System
{
    public interface IHandleExceptionProvider
    {
        string PolicyName { get; set; }
        IHandleException CreateHandler();
    }
}
