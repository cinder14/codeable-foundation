using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common;
using System.Web.Routing;
using Codeable.Foundation.Common.Plugins;
using System.Collections.ObjectModel;

namespace Codeable.Foundation.UI.Web.Common.Plugins
{
    public interface IWebPlugin : IPlugin
    {
        bool WebInitialize(IFoundation foundation, IDictionary<string, string> pluginConfig);

        /// <summary>
        /// Register any custom routes that do not already adhere to the expected format.
        /// [Special url mapping, Custom administration, etc]
        /// </summary>
        void RegisterCustomRouting(RouteCollection routes);
        void UnRegisterCustomRouting(RouteCollection routes);

        void RegisterLegacyOverrides(LegacyOverrideCollection overrides);
        void UnRegisterLegacyOverrides(LegacyOverrideCollection overrides);

        int DesiredRegistrationPriority { get; }

        void OnWebPluginRegistered(IWebPlugin plugin);
        void OnWebPluginUnRegistered(IWebPlugin iWebPlugin);

        void OnAfterWebPluginsRegistered(IEnumerable<IWebPlugin> allWebPlugins);
        void OnAfterWebPluginsUnRegistered(IWebPlugin[] iWebPlugin);
    }
}
