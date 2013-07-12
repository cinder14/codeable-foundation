using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.System;
using Codeable.Foundation.Core.Aspect;
using System.Web;
using Codeable.Foundation.Common;
using Microsoft.Practices.Unity;
using Codeable.Foundation.UI.Web.Common.Plugins;
using Codeable.Foundation.Core.System;
using Codeable.Foundation.Common.Plugins;
using Codeable.Foundation.Common.Aspect;

namespace Codeable.Foundation.UI.Web.Core.MVC
{
    public class WebPluginLoader : ChokeableClass, IWebPluginLoader, IDisposable
    {
        #region Constructor
        public WebPluginLoader(IFoundation iFoundation, IHandleExceptionProvider iHandleExceptionProvider)
            : base(iFoundation)
        {
            this.IHandleExceptionProvider = iHandleExceptionProvider;
        }
        ~WebPluginLoader()
        {
            Dispose(false);
        }
        #endregion

        #region Static Methods

        private static IDictionary<string, IWebPluginDependency> _Dependencies
        {
            get
            {
                return SingleDictionary<string, IWebPluginDependency>.Instance;
            }
        }
        private static Dictionary<Type, IWebPlugin> _LoadedPlugins = new Dictionary<Type, IWebPlugin>();

        public static List<IWebPluginDependency> DependencyGetAll()
        {
            return _Dependencies.Values.ToList();
        }
        public static IWebPluginDependency DependencyGet(string virtualPath)
        {
            return _Dependencies[virtualPath];
        }
        public static bool DependencyExists(string virtualPath)
        {
            return _Dependencies.ContainsKey(virtualPath);
        }
        public static bool DependencyRemove(string virtualPath)
        {
            return _Dependencies.Remove(virtualPath);
        }
        public static void DependencyAdd(IWebPluginDependency dependency)
        {
            _Dependencies[dependency.VirtualPath] = dependency;
        } 

        #endregion

        #region Private Properties
        
        protected virtual LegacyOverrideCollection LegacyOverrides
        {
            get
            {
                if (Single<LegacyOverrideCollection>.Instance == null)
                {
                    Single<LegacyOverrideCollection>.Instance = new LegacyOverrideCollection();
                }
                return Single<LegacyOverrideCollection>.Instance;
            }
        }
        protected static List<IWebPlugin> _LATEST_REGISTERED_PLUGINS = null;
        
        #endregion
    
        #region Public Methods

        public virtual List<IWebPlugin> GetRegisteredPlugins()
        {
            return base.ExecuteFunction<List<IWebPlugin>>("GetRegisteredPlugins", delegate()
            {
                List<IWebPlugin> result = new List<IWebPlugin>();
                if (_LATEST_REGISTERED_PLUGINS != null)
                {
                    result.AddRange(_LATEST_REGISTERED_PLUGINS);
                }
                return result;
            });
        }

        public virtual void RegisterPluginRoutes(global::System.Web.Routing.RouteCollection routes)
        {
            base.ExecuteMethod("RegisterPluginRoutes", delegate()
            {
                List<IWebPlugin> allPlugins = AcquirePermittedPlugins();
                List<IWebPlugin> loadedPlugins = new List<IWebPlugin>();

                foreach (IWebPlugin item in allPlugins)
                {
                    try
                    {
                        base.Logger.Write(string.Format("Registering routes for '{0}-{1}'", item.DisplayName, item.DisplayVersion), Category.Trace);
                        item.RegisterCustomRouting(routes);
                        item.RegisterLegacyOverrides(this.LegacyOverrides);
                        RaiseOnWebPluginRegistered(loadedPlugins, item);
                        loadedPlugins.Add(item);
                    }
                    catch (Exception ex)
                    {
                        base.Logger.Write(string.Format("Error While Loading '{0}-{1}': {2}", item.DisplayName, item.DisplayVersion, ex.Message), Category.Error);
                        bool rethrow = false;
                        Exception newExeption = null;
                        if (this.IHandleExceptionProvider != null)
                        {
                            IHandleException handler = this.IHandleExceptionProvider.CreateHandler();
                            if (handler != null)
                            {
                                handler.HandleException(ex, out rethrow, out newExeption);
                                // not throwing.. just enabling logging
                            }
                        }
                    }
                }
                RaiseOnAfterWebPluginsRegistered(loadedPlugins);
                _LATEST_REGISTERED_PLUGINS = loadedPlugins;
                InvalidateDependencies();
            });
        }
        public virtual void UnRegisterPluginRoutes(global::System.Web.Routing.RouteCollection routes)
        {
            base.ExecuteMethod("UnRegisterPluginRoutes", delegate()
            {
                List<IWebPlugin> allPlugins = AcquirePermittedPlugins();
                List<IWebPlugin> unLoadedPlugins = new List<IWebPlugin>();

                foreach (IWebPlugin item in allPlugins)
                {
                    try
                    {
                        base.Logger.Write(string.Format("UnRegistering routes for '{0}-{1}'", item.DisplayName, item.DisplayVersion), Category.Trace);
                        item.UnRegisterCustomRouting(routes);
                        item.UnRegisterLegacyOverrides(this.LegacyOverrides);
                        RaiseOnWebPluginUnRegistered(unLoadedPlugins, item);
                        unLoadedPlugins.Add(item);
                    }
                    catch (Exception ex)
                    {
                        base.Logger.Write(string.Format("Error While Loading '{0}-{1}': {2}", item.DisplayName, item.DisplayVersion, ex.Message), Category.Error);
                        bool rethrow = false;
                        Exception newExeption = null;
                        if (this.IHandleExceptionProvider != null)
                        {
                            IHandleException handler = this.IHandleExceptionProvider.CreateHandler();
                            if (handler != null)
                            {
                                handler.HandleException(ex, out rethrow, out newExeption);
                                // not throwing.. just logging
                            }
                        }
                    }
                }
                RaiseOnAfterWebPluginsUnRegistered(unLoadedPlugins);
                InvalidateDependencies();
            });
        }

        public virtual List<IWebPlugin> AcquirePermittedPlugins()
        {
            return base.ExecuteFunction<List<IWebPlugin>>("AcquirePermittedPlugins", delegate()
            {
                List<IWebPlugin> plugins = new List<IWebPlugin>();
                if (WebPreApplicationStartMethod.LoadedPlugins == null)
                {
                    return plugins;
                }
                List<PluginConfig> pluginConfigs = WebPreApplicationStartMethod.LoadedPlugins.ToList();
                foreach (PluginConfig item in pluginConfigs)
                {
                    try
                    {
                        OnBeforePluginAcquireItem(item);

                        if (!PerformPluginPermitted(item))
                        {
                            base.Logger.Write(string.Format("Item not permitted, Skipping: '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                            continue;
                        }
                        if (item.PluginType == null)
                        {
                            base.Logger.Write(string.Format("No Plugin type found for plugin, no Instance will be createdf '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                            continue;
                        }
                        base.Logger.Write(string.Format("Attempting to load existing instance of '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                        
                        IWebPlugin instance = null;
                        bool alreadyLoaded = false;
                        if (_LoadedPlugins.ContainsKey(item.PluginType))
                        {
                            instance = _LoadedPlugins[item.PluginType];
                            if (instance != null)
                            {
                                plugins.Add(instance);
                                base.Logger.Write(string.Format("Added existing instance of '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                                alreadyLoaded = true;
                            }
                        }
                        base.Logger.Write(string.Format("Attempting to load existing foundation instance of '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                        
                        if (instance == null)
                        {
                            instance = base.IFoundation.GetPluginManager().FoundationPlugins.FirstOrDefault(p => p.GetType() == item.PluginType) as IWebPlugin;
                            if (instance != null)
                            {
                                plugins.Add(instance);
                                _LoadedPlugins[item.PluginType] = instance;
                                base.Logger.Write(string.Format("Added existing foundation instance of '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                                alreadyLoaded = true;
                            }
                        }

                        base.Logger.Write(string.Format("Attempting to create instance of '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                        
                        // resolve or create
                        if (instance == null)
                        {
                            bool resolveSuccess = false;
                            if (!resolveSuccess)
                            {
                                try
                                {
                                    instance = base.IFoundation.Container.Resolve(item.PluginType) as IWebPlugin;
                                    resolveSuccess = true;
                                }
                                catch { } // ah well
                            }
                            if (!resolveSuccess)
                            {
                                try
                                {
                                    // is this any good ??
                                    instance = Activator.CreateInstance(item.PluginType) as IWebPlugin;
                                }
                                catch { } // ah well
                            }
                        }
                        if (instance != null)
                        {
                            IDictionary<string, string> pluginConfigOptions = PerformLoadPluginConfigOptions(item);
                            
                            if (!alreadyLoaded)
                            {
                                if (instance.WebInitialize(base.IFoundation, pluginConfigOptions))
                                {
                                    plugins.Add(instance);
                                    _LoadedPlugins[item.PluginType] = instance;
                                    base.Logger.Write(string.Format("Added instance of '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                                }
                                else
                                {
                                    instance = null;
                                    base.Logger.Write(string.Format("The following plugin did not initialize: '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                                }
                            }
                            if (instance != null)
                            {
                                OnAfterPluginAcquireItem(item, instance, pluginConfigOptions, !alreadyLoaded);
                            }
                        }
                        else
                        {
                            base.Logger.Write(string.Format("Unable to create instance of '{0}-{1}'", item.SystemName, item.Version), Category.Trace);
                        }
                    }
                    catch (Exception ex)
                    {
                        base.Logger.Write(string.Format("Unable to load plugin for '{0}-{1}': {2}", item.SystemName, item.Version, ex.Message), Category.Error);
                    }
                }
                plugins.Sort(delegate(IWebPlugin l, IWebPlugin r) { return l.DesiredRegistrationPriority.CompareTo(r.DesiredRegistrationPriority); });
                return plugins;
            });
        }
        public System.Web.Caching.CacheDependency CreateDependency(string virtualPath)
        {
            if (DependencyExists(virtualPath))
            {
                return DependencyGet(virtualPath) as System.Web.Caching.CacheDependency;
            }
            else
            {
                WebPluginDependency dependency = new WebPluginDependency(virtualPath);
                DependencyAdd(dependency);
                return dependency;
            }
        } 

        #endregion

        #region Protected Methods

        protected virtual void RaiseOnWebPluginUnRegistered(List<IWebPlugin> webPlugins, IWebPlugin iWebPlugin)
        {
            base.ExecuteMethod("RaiseOnWebPluginUnRegistered", delegate()
            {
                foreach (IWebPlugin item in webPlugins)
                {
                    try
                    {
                        item.OnWebPluginUnRegistered(iWebPlugin);
                    }
                    catch (Exception ex)
                    {
                        base.Logger.Write(string.Format("Error raising OnWebPluginUnRegistered for '{0}-{1}': {2}", item.DisplayName, item.DisplayVersion, ex.Message), Category.Warning);
                    }
                }
            });
        }
        protected virtual void RaiseOnAfterWebPluginsUnRegistered(List<IWebPlugin> webPlugins)
        {
            base.ExecuteMethod("RaiseOnAfterWebPluginsUnRegistered", delegate()
            {
                foreach (IWebPlugin item in webPlugins)
                {
                    try
                    {
                        item.OnAfterWebPluginsUnRegistered(webPlugins.ToArray());
                    }
                    catch (Exception ex)
                    {
                        base.Logger.Write(string.Format("Error raising OnAfterWebPluginsUnRegistered for '{0}-{1}': {2}", item.DisplayName, item.DisplayVersion, ex.Message), Category.Warning);
                    }
                }
            });
        }

        protected virtual void RaiseOnWebPluginRegistered(List<IWebPlugin> webPlugins, IWebPlugin iWebPlugin)
        {
            base.ExecuteMethod("RaiseOnWebPluginRegistered", delegate()
            {
                foreach (IWebPlugin item in webPlugins)
                {
                    try
                    {
                        item.OnWebPluginRegistered(iWebPlugin);
                    }
                    catch (Exception ex)
                    {
                        base.Logger.Write(string.Format("Error raising OnWebPluginRegistered for '{0}-{1}': {2}", item.DisplayName, item.DisplayVersion, ex.Message), Category.Warning);
                    }
                }
            });
        }
        protected virtual void RaiseOnAfterWebPluginsRegistered(List<IWebPlugin> webPlugins)
        {
            base.ExecuteMethod("RaiseOnAfterWebPluginsRegistered", delegate()
            {
                foreach (IWebPlugin item in webPlugins)
                {
                    try
                    {
                        item.OnAfterWebPluginsRegistered(webPlugins.ToArray());
                    }
                    catch (Exception ex)
                    {
                        base.Logger.Write(string.Format("Error raising OnAfterWebPluginsRegistered for '{0}-{1}': {2}", item.DisplayName, item.DisplayVersion, ex.Message), Category.Warning);
                    }
                }
            });
        }

        protected virtual void InvalidateDependencies()
        {
            base.ExecuteMethod("InvalidateDependencies", delegate()
            {
                List<IWebPluginDependency> dependencies = DependencyGetAll();
                for (int i = 0; i < dependencies.Count; i++)
                {
                    dependencies[i].RaiseDependencyChanged(this, EventArgs.Empty);
                }
            });
        }

        protected virtual bool PerformPluginPermitted(PluginConfig pluginConfig)
        {
            // designed for override
            return true;
        }
        protected virtual IDictionary<string, string> PerformLoadPluginConfigOptions(PluginConfig pluginConfig)
        {
            // designed for override
            return null;
        }

        protected virtual void OnBeforePluginAcquireItem(PluginConfig pluginConfig)
        {
            // designed for override
        }
        protected virtual void OnAfterPluginAcquireItem(PluginConfig pluginConfig, IWebPlugin webPlugin, IDictionary<string, string> pluginConfigOptions, bool initialLoad)
        {
            // designed for override
        }

        #endregion

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    InvalidateDependencies();
                    _Dependencies.Clear();
                }
                catch { }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}