using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web;
using System.Web.Caching;
using System.Collections;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.Common;
using Codeable.Foundation.Common.System;

namespace Codeable.Foundation.UI.Web.Core.MVC.Embedded
{
    public class WebPluginVirtualPathProvider : VirtualPathProvider, IPluginVirtualPathProvider
    {
        public WebPluginVirtualPathProvider(IFoundation iFoundation, IAspectCoordinator aspectCoordinator, IWebPluginLoader iWebPluginLoader, WebPluginEmbeddedCache embeddedCache)
        {
            _embeddedCache = embeddedCache;
            _iWebPluginLoader = iWebPluginLoader;
            _aspectCoordinator = aspectCoordinator;
            _iFoundation = iFoundation;
        }

        private readonly WebPluginEmbeddedCache _embeddedCache;
        private readonly IWebPluginLoader _iWebPluginLoader;
        private readonly IAspectCoordinator _aspectCoordinator;
        private readonly IFoundation _iFoundation;

        public WebPluginEmbeddedCache EmbeddedCache
        {
            get
            {
                return _embeddedCache;
            }
        }

        public override bool FileExists(string virtualPath)
        {
            return _aspectCoordinator.WrapFunctionCall<bool>(this, "WebPluginVirtualPathProvider.FileExists", new object[] { virtualPath }, true, null, delegate()
            {
                if (IsLegacyOverride(virtualPath))
                {
                    return false;
                }
                return (ShouldTrackAsEmbeddedItem(virtualPath) || Previous.FileExists(virtualPath));
            });
        }
        public override VirtualFile GetFile(string virtualPath)
        {
            return _aspectCoordinator.WrapFunctionCall<VirtualFile>(this, "WebPluginVirtualPathProvider.GetFile", new object[] { virtualPath }, true, null, delegate()
            {
                if (IsLegacyOverride(virtualPath))
                {
                    return null;
                }
                if (ShouldEmbeddedItemOverride(virtualPath))
                {
                    string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);
                    WebPluginEmbeddedInfo viewInfo = _embeddedCache.GetEmbeddedItem(virtualPathAppRelative);
                    if (viewInfo.IsChained)
                    {
                        ChainedWebPluginEmbeddedVirtualFile result = new ChainedWebPluginEmbeddedVirtualFile(_embeddedCache.GetChainedEmbeddedItems(virtualPathAppRelative), virtualPath);
                        if (Previous.FileExists(virtualPath))
                        {
                            VirtualFile standardFile = Previous.GetFile(virtualPath);
                            if (standardFile != null)
                            {
                                result.AddExternalFile(standardFile);
                            }
                        }
                        return result;
                    }
                    else
                    {
                        return new WebPluginEmbeddedVirtualFile(viewInfo, virtualPath);
                    }
                }
                return Previous.GetFile(virtualPath);
            });
        }
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return _aspectCoordinator.WrapFunctionCall<CacheDependency>(this, "WebPluginVirtualPathProvider.GetCacheDependency", new object[] { virtualPath, virtualPathDependencies, utcStart }, true, null, delegate()
            {
                if (!ShouldTrackAsEmbeddedItem(virtualPath))
                {
                    return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
                }
                return _iWebPluginLoader.CreateDependency(virtualPath);
            });
        }

        protected virtual bool ShouldTrackAsEmbeddedItem(string virtualPath)
        {
            if (!string.IsNullOrEmpty(virtualPath))
            {
                string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);
                return _embeddedCache.ContainsEmbeddedItem(virtualPathAppRelative);
            }
            return false;
        }
        protected virtual bool ShouldEmbeddedItemOverride(string virtualPath)
        {
            if (!string.IsNullOrEmpty(virtualPath))
            {
                string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);
                return _embeddedCache.HasPermittedEmbeddedItem(virtualPathAppRelative);
            }
            return false;
        }
        protected virtual bool IsLegacyOverride(string virtualPath)
        {
            string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);
            WebPluginEmbeddedInfo info = _embeddedCache.GetEmbeddedItem(virtualPathAppRelative);
            if (info != null)
            {
                return info.IsWebFormOverride;
            }
            return false;
        }

    }
}
