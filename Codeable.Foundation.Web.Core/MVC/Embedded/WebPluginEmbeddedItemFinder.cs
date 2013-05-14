using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Codeable.Foundation.Core.System;
using System.Reflection;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.Common;
using System.IO;
using Codeable.Foundation.Common.System;
using Codeable.Foundation.Core.Aspect;
using Codeable.Foundation.UI.Web.Common.Plugins;

namespace Codeable.Foundation.UI.Web.Core.MVC.Embedded
{
    public class WebPluginEmbeddedItemFinder : ChokeableClass, IWebPluginEmbeddedItemFinder
    {
        public WebPluginEmbeddedItemFinder(IFoundation iFoundation, IFindClassTypes iFindClassTypes)
            : base(iFoundation)
        {
            _iFindClassTypes = iFindClassTypes;
        }

        private IFindClassTypes _iFindClassTypes;

        public virtual WebPluginEmbeddedCache FindEmbeddedItems()
        {
            return base.ExecuteFunction<WebPluginEmbeddedCache>("FindEmbeddedItems", delegate()
            {
                WebPluginEmbeddedCache result = null;

                string chainToken = "._chained."; // used for file merging. :)
                KeyValuePair<string, string>[] tokens = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>(".views.", "views.{0}"),
                    new KeyValuePair<string, string>(".content.", "content.{0}"),
                    new KeyValuePair<string, string>(".scripts.", "scripts.{0}"),
                    new KeyValuePair<string, string>(".cdn.", "cdn.{0}")
                };

                IList<Assembly> assemblies = _iFindClassTypes.GetAssemblies(null);
                if ((assemblies != null) || (assemblies.Count > 0))
                {
                    result = new WebPluginEmbeddedCache();
                    foreach (Assembly assembly in assemblies)
                    {
                        string[] names = GetNamesOfAssemblyResources(assembly);
                        if ((names != null) && (names.Length > 0))
                        {
                            foreach (string name in names)
                            {
                                foreach (KeyValuePair<string, string> token in tokens)
                                {
                                    string resourceName = name.ToLowerInvariant();
                                    int tokenIX = resourceName.IndexOf(token.Key);
                                    if (tokenIX > -1)
                                    {
                                        bool isChained = resourceName.Contains(chainToken);
                                        resourceName = resourceName.Substring(tokenIX + token.Key.Length);
                                        if (resourceName.Contains("."))
                                        {
                                            string virtualPath = string.Format(token.Value, resourceName);
                                            WebPluginEmbeddedInfo info = new WebPluginEmbeddedInfo 
                                            { 
                                                VirtualItemName = virtualPath,
                                                Name = name,
                                                AssemblyFullName = assembly.FullName,
                                                IsChained = isChained 
                                            };

                                            if (PerformVirtualItemPermitted(info))
                                            {
                                                result.AddEmbeddedItem(info);
                                            }
                                            else
                                            {
                                                base.Logger.Write("Virtual Path Prohibited: " + info.ToString(), Category.Trace);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result = AddLegacyOverrides(result);
                return result; 
            });
        }

        /// <summary>
        /// Allows permissions to be applied, also allows for alteration of the object reference
        /// </summary>
        protected virtual bool PerformVirtualItemPermitted(WebPluginEmbeddedInfo info)
        {
            // designed for override
            return true;
        }

        // inject the legacy overrides, if any
        protected virtual WebPluginEmbeddedCache AddLegacyOverrides(WebPluginEmbeddedCache cache)
        {
            return base.ExecuteFunction<WebPluginEmbeddedCache>("AddLegacyOverrides", delegate()
            {
                IWebPluginLoader pluginLoader = base.IFoundation.Container.Resolve<IWebPluginLoader>();
                if (pluginLoader != null)
                {
                    List<IWebPlugin> permittedPlugins = pluginLoader.AcquirePermittedPlugins();
                    if ((permittedPlugins != null) && (permittedPlugins.Count > 0))
                    {
                        LegacyOverrideCollection overrideCollection = new LegacyOverrideCollection();
                        foreach (IWebPlugin plugin in permittedPlugins)
                        {
                            try
                            {
                                plugin.RegisterLegacyOverrides(overrideCollection);

                                foreach (LegacyOverride legacyOverride in overrideCollection)
                                {
                                    WebPluginEmbeddedInfo info = new WebPluginEmbeddedInfo();
                                    info.AssemblyFullName = plugin.GetType().Assembly.FullName;
                                    info.IsWebFormOverride = true;
                                    info.Name = legacyOverride.SuppressLegacyUrl;
                                    info.VirtualItemName = legacyOverride.SuppressLegacyUrl;

                                    cache.AddEmbeddedItem(info);
                                }

                                plugin.UnRegisterLegacyOverrides(overrideCollection);
                            }
                            catch { }
                        }
                    }
                }
                return cache;
            }, cache);
        }


        private static string[] GetNamesOfAssemblyResources(Assembly assembly)
        {
            //GetManifestResourceNames will throw a NotSupportedException when run on a dynamic assembly
            try
            {
                return assembly.GetManifestResourceNames();
            }
            catch
            {
                return new string[] { };
            }
        }
    }

}
