using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Core.Aspect;
using Codeable.Foundation.Common;
using Microsoft.Practices.Unity;
using Codeable.Foundation.Common.Aspect;

namespace Codeable.Foundation.Core.Caching
{
    //TODO: More efficient locks (one for each type)
    /// <summary>
    /// Enables caching across instance boundaries
    /// </summary>
    public class AspectCache : ChokeableClass
    {
        public AspectCache(string ownerToken)
            : base(CoreFoundation.Current)
        {
            this.OwnerToken = ownerToken;
            this.Lifetime = new ContainerControlledLifetimeManager();
            this.InstanceCache = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public AspectCache(string ownerToken, LifetimeManager lifeTime)
            : base(CoreFoundation.Current)
        {
            this.OwnerToken = ownerToken;
            this.Lifetime = lifeTime;
            this.InstanceCache = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public AspectCache(string ownerToken, IFoundation iFoundation)
            : base(iFoundation)
        {
            this.OwnerToken = ownerToken;
            this.InstanceCache = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.Lifetime = new ContainerControlledLifetimeManager();
        }
        public AspectCache(string ownerToken, IFoundation iFoundation, LifetimeManager lifeTime)
            : base(iFoundation)
        {
            this.OwnerToken = ownerToken;
            this.InstanceCache = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.Lifetime = lifeTime;
        }

        private object _syncLock = new object();

        public virtual LifetimeManager Lifetime { get; protected set; }
        public virtual string OwnerToken { get; protected set; }
        public virtual Dictionary<string, object> InstanceCache { get; set; }

        /// <summary>
        /// Gets the value from cache if it exists, otherwise executes the retrievemethod then caches the result.
        /// </summary>
        public virtual T KeyedPerInstance<T, K>(K key, string callerName, Func<T> retrieveMethod)
        {
            return base.ExecuteFunction<T>("KeyedPerInstance", delegate()
            {
                Dictionary<K, T> dictionary = PerInstance(callerName, delegate()
                {
                    return new Dictionary<K, T>();
                });
                T result = default(T);
                lock (_syncLock)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        result = retrieveMethod();
                        dictionary[key] = result;
                    }
                    else
                    {
                        result = dictionary[key];
                    }
                }
                return result;
            });
        }
        /// <summary>
        /// Gets the value from cache if it exists, otherwise executes the retrievemethod then caches the result.
        /// </summary>
        public virtual T KeyedPerFoundation<T, K>(K key, string callerName, Func<T> retrieveMethod)
        {
            return base.ExecuteFunction<T>("KeyedPerFoundation", delegate()
            {
                Dictionary<K, T> dictionary = PerFoundation(callerName, delegate()
                {
                    return new Dictionary<K, T>();
                });
                T result = default(T);
                lock (_syncLock)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        result = retrieveMethod();
                        dictionary[key] = result;
                    }
                    else
                    {
                        result = dictionary[key];
                    }
                }
                return result;
            });
        }
        /// <summary>
        /// Gets the value from cache if it exists, otherwise executes the retrievemethod then caches the result.
        /// </summary>
        public virtual T KeyedPerLifetime<T, K>(K key, string callerName, Func<T> retrieveMethod)
        {
            return base.ExecuteFunction<T>("KeyedPerLifetime", delegate()
            {
                Dictionary<K, T> dictionary = PerLifetime(callerName, delegate()
                {
                    return new Dictionary<K, T>();
                });
                T result = default(T);
                lock (_syncLock)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        result = retrieveMethod();
                        dictionary[key] = result;
                    }
                    else
                    {
                        result = dictionary[key];
                    }
                }
                return result;
            });
        }

        /// <summary>
        /// Gets the value from cache if it exists, otherwise executes the retrievemethod then caches the result.
        /// </summary>
        public virtual T PerInstance<T>(string callerName, Func<T> retrieveMethod)
        {
            return base.ExecuteFunction<T>("PerInstance", delegate()
            {
                lock (_syncLock)
                {
                    if (!this.InstanceCache.ContainsKey(callerName))
                    {
                        this.InstanceCache[callerName] = retrieveMethod();
                    }
                }
                return (T)this.InstanceCache[callerName]; //TODO:Performance: Can we Get rid of this casting.
            });
        }
        /// <summary>
        /// Gets the value from cache if it exists, otherwise executes the retrievemethod then caches the result.
        /// </summary>
        public virtual T PerFoundation<T>(string callerName, Func<T> retrieveMethod)
        {
            return base.ExecuteFunction<T>("PerFoundation", delegate()
            {
                AspectCache cache = base.IFoundation.Container.Resolve<AspectCache>();
                return cache.KeyedPerInstance<T, string>(this.OwnerToken, callerName, retrieveMethod);
            });
        }
        /// <summary>
        /// Gets the value from cache if it exists, otherwise executes the retrievemethod then caches the result.
        /// </summary>
        public virtual T PerLifetime<T>(string callerName, Func<T> retrieveMethod)
        {
            return base.ExecuteFunction<T>("PerLifetime", delegate()
            {
                AspectCache cache = null;
                lock (_syncLock)
                {
                    cache = Lifetime.GetValue() as AspectCache;
                    if (cache == null)
                    {
                        cache = new AspectCache(this.OwnerToken, base.IFoundation, this.Lifetime);
                        Lifetime.SetValue(cache);
                    }
                }
                return cache.PerInstance(callerName, retrieveMethod);
            });
        }

        /// <summary>
        /// Forcibly updates the cache with the supplied value
        /// </summary>
        public virtual T SetKeyedPerInstance<T, K>(K key, string callerName, T value)
        {
            return base.ExecuteFunction<T>("SetKeyedPerInstance", delegate()
            {
                Dictionary<K, T> dictionary = PerInstance(callerName, delegate()
                {
                    return new Dictionary<K, T>();
                });
                lock (_syncLock)
                {
                    dictionary[key] = value;
                }
                return value;
            });
        }
        /// <summary>
        /// Forcibly updates the cache with the supplied value
        /// </summary>
        public virtual T SetKeyedPerFoundation<T, K>(K key, string callerName, T value)
        {
            return base.ExecuteFunction<T>("SetKeyedPerFoundation", delegate()
            {
                Dictionary<K, T> dictionary = PerFoundation(callerName, delegate()
                {
                    return new Dictionary<K, T>();
                });
                lock (_syncLock)
                {
                    dictionary[key] = value;
                }
                return value;
            });
        }
        /// <summary>
        /// Forcibly updates the cache with the supplied value
        /// </summary>
        public virtual T SetKeyedPerLifetime<T, K>(K key, string callerName, T value)
        {
            return base.ExecuteFunction<T>("SetKeyedPerLifetime", delegate()
            {
                Dictionary<K, T> dictionary = PerLifetime(callerName, delegate()
                {
                    return new Dictionary<K, T>();
                });
                lock (_syncLock)
                {
                    dictionary[key] = value;
                }
                return value;
            });
        }

        /// <summary>
        /// Forcibly updates the cache with the supplied value
        /// </summary>
        public virtual T SetPerInstance<T>(string callerName, T value)
        {
            return base.ExecuteFunction<T>("SetPerInstance", delegate()
            {
                lock (_syncLock)
                {
                    this.InstanceCache[callerName] = value;
                }
                return value;
            });
        }
        /// <summary>
        /// Forcibly updates the cache with the supplied value
        /// </summary>
        public virtual T SetPerFoundation<T>(string callerName, T value)
        {
            return base.ExecuteFunction<T>("SetPerFoundation", delegate()
            {
                AspectCache cache = base.IFoundation.Container.Resolve<AspectCache>();
                return cache.SetKeyedPerInstance<T, string>(this.OwnerToken, callerName, value);
            });
        }
        /// <summary>
        /// Forcibly updates the cache with the supplied value
        /// </summary>
        public virtual T SetPerLifetime<T>(string callerName, T value)
        {
            return base.ExecuteFunction<T>("SetPerLifetime", delegate()
            {
                AspectCache cache = null;
                lock (_syncLock)
                {
                    cache = Lifetime.GetValue() as AspectCache;
                    if (cache == null)
                    {
                        cache = new AspectCache(this.OwnerToken, base.IFoundation, this.Lifetime);
                        Lifetime.SetValue(cache);
                    }
                }
                return cache.SetPerInstance<T>(callerName, value);
            });
        }

        public virtual void ClearInstanceCache()
        {
            base.ExecuteMethod("ClearInstanceCache", delegate()
            {
                this.InstanceCache.Clear();
            });
        }
        public virtual void ClearFoundationCache()
        {
            base.ExecuteMethod("ClearFoundationCache", delegate()
            {
                AspectCache cache = base.IFoundation.Container.Resolve<AspectCache>();
                cache.ClearInstanceCache();
            });
        }
        public virtual void ClearLifetimeCache()
        {
            base.ExecuteMethod("ClearLifetimeCache", delegate()
            {
                AspectCache cache = Lifetime.GetValue() as AspectCache;
                if (cache != null)
                {
                    cache.ClearInstanceCache();
                }
            });
        }
        public virtual void ClearAll()
        {
            base.ExecuteMethod("ClearAll", delegate()
            {
                ClearInstanceCache();
                ClearLifetimeCache();
            });
        }

    }
}
