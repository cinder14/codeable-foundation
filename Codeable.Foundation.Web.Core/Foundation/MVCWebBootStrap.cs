using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common;
using Codeable.Foundation.Core;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Codeable.Foundation.UI.Web.Core.MVC;
using System.Web.Hosting;
using Codeable.Foundation.UI.Web.Core.MVC.Embedded;
using Codeable.Foundation.UI.Web.Core.MVC.Razor;
using System.Web;
using Codeable.Foundation.Common.Aspect;
using System.Web.Http;

namespace Codeable.Foundation.UI.Web.Core.Foundation
{
    public class MVCWebBootStrap : WebBootStrap
    {
        public MVCWebBootStrap()
            : base()
        {
        }

        public override void OnAfterBootStrapComplete(IFoundation foundation)
        {
            base.OnAfterBootStrapComplete(foundation);

            // MVC Fixes
            // Resolver
            IDependencyResolver resolver = new UnityDependencyResolver();
            foundation.Container.RegisterInstance<IDependencyResolver>(resolver);
            DependencyResolver.SetResolver(resolver);


            System.Web.Http.Dependencies.IDependencyResolver mvcResolver = new UnityMVC4DependencyResolver();
            foundation.Container.RegisterInstance<System.Web.Http.Dependencies.IDependencyResolver>(mvcResolver);

            // MVC4 Fixes
            // Resolver
            GlobalConfiguration.Configuration.DependencyResolver = mvcResolver;

            // Extra Paths
            foreach (IViewEngine item in ViewEngines.Engines)
            {
                PerformInjectAdditionalViewPaths(item as VirtualPathProviderViewEngine);
            }

            // Embedded Items [plugin support]
            
            // Get the finder (searches for embedded items)
            IWebPluginEmbeddedItemFinder viewFinder = foundation.Container.Resolve<IWebPluginEmbeddedItemFinder>();
            // Intercepts and resolves virtual paths
            WebPluginVirtualPathProvider provider = new WebPluginVirtualPathProvider(foundation, foundation.Container.Resolve<IAspectCoordinator>(), foundation.Container.Resolve<IWebPluginLoader>(), viewFinder.FindEmbeddedItems());
            // Register the interceptor with Unity
            foundation.Container.RegisterInstance<IPluginVirtualPathProvider>(provider, new ContainerControlledLifetimeManager());

            // Register it with the host
            if (HostingEnvironment.IsHosted)
            {
                HostingEnvironment.RegisterVirtualPathProvider(provider);
            }
        }

        protected virtual void PerformInjectAdditionalViewPaths(VirtualPathProviderViewEngine viewEngine)
        {
            if (viewEngine is RazorViewEngine)
            {
                viewEngine.PartialViewLocationFormats = viewEngine.PartialViewLocationFormats.Union(new string[] {
                    "~/Views/_Chained/{0}.cshtml", 
                    "~/Views/_Chained/{0}.vbhtml"
                }).ToArray();
            }
        }

    }
}
