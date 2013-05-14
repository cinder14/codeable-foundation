using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Codeable.Foundation.UI.Web.Common.Plugins;

namespace Codeable.Foundation.UI.Web.Core.MVC
{
    public interface IWebPluginLoader
    {
        CacheDependency CreateDependency(string virtualPath);

        void RegisterPluginRoutes(global::System.Web.Routing.RouteCollection routes);
        void UnRegisterPluginRoutes(global::System.Web.Routing.RouteCollection routes);

        List<IWebPlugin> AcquirePermittedPlugins();
    }
}
