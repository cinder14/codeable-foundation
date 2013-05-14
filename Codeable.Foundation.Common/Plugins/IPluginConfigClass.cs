using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Plugins
{
    public interface IPluginConfigClass
    {
        void DeserializeFrom(IDictionary<string, string> payload);
        void SerializeInto(IDictionary<string, string> payload);
    }
}
