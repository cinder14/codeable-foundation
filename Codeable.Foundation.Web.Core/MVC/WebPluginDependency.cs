using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;

namespace Codeable.Foundation.UI.Web.Core.MVC
{
    public class WebPluginDependency : CacheDependency, IWebPluginDependency
    {
        public WebPluginDependency(string virtualPath)
        {
            VirtualPath = virtualPath;
        }
        public string VirtualPath { get; set; }

        protected override void DependencyDispose()
        {
            WebPluginLoader.DependencyRemove(this.VirtualPath);
        }
        public virtual void RaiseDependencyChanged(object sender, EventArgs args)
        {
            if (!HasChanged)
            {
                base.NotifyDependencyChanged(sender, args);
            }
        }
    }
}
