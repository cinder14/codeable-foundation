using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Plugins
{
    public interface IPlugin
    {
        string DisplayName { get; }
        string DisplayVersion { get; }

        T RetrieveMetaData<T>(string token);

        object InvokeCommand(string name, Dictionary<string, object> caseInsensitiveParameters);
        
    }
}
