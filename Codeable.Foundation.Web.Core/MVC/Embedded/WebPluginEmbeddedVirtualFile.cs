using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.IO;
using System.Reflection;
using System.Web;

namespace Codeable.Foundation.UI.Web.Core.MVC.Embedded
{
    public class WebPluginEmbeddedVirtualFile : VirtualFile
    {
        private readonly WebPluginEmbeddedInfo _viewInfo;

        public WebPluginEmbeddedVirtualFile(WebPluginEmbeddedInfo viewInfo, string virtualPath)
            : base(virtualPath)
        {
            _viewInfo = viewInfo;
        }

        public override Stream Open()
        {
            return Open(true);
        }
        public virtual Stream Open(bool applyResponseCaching)
        {
            if (applyResponseCaching && this._viewInfo.Cacheability.HasValue)
            {
                System.Web.HttpContext.Current.Response.Cache.SetCacheability(_viewInfo.Cacheability.Value);
                if (!string.IsNullOrEmpty(_viewInfo.CacheExtension))
                {
                    System.Web.HttpContext.Current.Response.Cache.AppendCacheExtension(_viewInfo.CacheExtension);
                }
            }
            Assembly assembly = GetResourceAssembly();
            if (assembly != null)
            {
                return assembly.GetManifestResourceStream(_viewInfo.Name);
            }
            return null;
        }

        protected virtual Assembly GetResourceAssembly()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => (_viewInfo != null) && string.Equals(assembly.FullName, _viewInfo.AssemblyFullName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }
    }
}
