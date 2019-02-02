using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Core.Aspect;
using Codeable.Foundation.Common;
using Microsoft.Practices.Unity;
using Codeable.Foundation.Common.Aspect;
using System.Threading;
using System.Collections.Concurrent;

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
            this.InstanceCache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public AspectCache(string ownerToken, LifetimeManager lifeTime)
            : base(CoreFoundation.Current)
        {
            this.OwnerToken = ownerToken;
            this.Lifetime = lifeTime;
            this.InstanceCache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public AspectCache(string ownerToken, IFoundation iFoundation)
            : base(iFoundation)
        {
            this.OwnerToken = ownerToken;
            this.InstanceCache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.Lifetime = new ContainerControlledLifetimeManager();
        }
        public AspectCache(string ownerToken, IFoundation iFoundation, LifetimeManager lifeTime)
            : base(iFoundation)
        {
            this.OwnerToken = ownerToken;
            this.InstanceCache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.Lifetime = lifeTime;
        }

        private ReaderWriterLockSlim _accessLock = new ReaderWriterLockSlim(); 

        public virtual LifetimeManager Lifetime { get; protected set; }
        public virtual string OwnerToken { get; protected set; }
        public virtual ConcurrentDictionary<string, object> InstanceCache { get; protected set; }

        /// <summary>
        /// Gets the value from cache if it exists, otherwise executes the retrievemethod then caches the result.
        /// </summary>
        public virtual T KeyedPerInstance<T, K>(K key, string callerName, Func<T> retrieveMethod)
        {
            return base.ExecuteFunction<T>("KeyedPerInstance", delegate()
            {
                ConcurrentDictionary<K, T> dictionary = PerInstance(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                T result = default(T);
                bool found = false;
                _accessLock.EnterReadLock();
                try
                {
                    found = dictionary.ContainsKey(key);
                    if(found)
                    {
                        result = dictionary[key];
                    }
                }
                finally
                {
                    _accessLock.ExitReadLock();
                }

                if (!found)
                {
                    result = retrieveMethod(); // race condition here is ok, last one should win
                    _accessLock.EnterWriteLock();
                    try
                    {
                        
                        dictionary[key] = result;
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
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
                ConcurrentDictionary<K, T> dictionary = PerFoundation(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                T result = default(T);
                bool found = false;
                _accessLock.EnterReadLock();
                try
                {
                    found = dictionary.ContainsKey(key);
                    if (found)
                    {
                        result = dictionary[key];
                    }
                }
                finally
                {
                    _accessLock.ExitReadLock();
                }

                if (!found)
                {
                    result = retrieveMethod(); // race condition here is ok, last one should win
                    _accessLock.EnterWriteLock();
                    try
                    {

                        dictionary[key] = result;
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
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
                ConcurrentDictionary<K, T> dictionary = PerLifetime(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                T result = default(T);
                bool found = false;
                _accessLock.EnterReadLock();
                try
                {
                    found = dictionary.ContainsKey(key);
                    if (found)
                    {
                        result = dictionary[key];
                    }
                }
                finally
                {
                    _accessLock.ExitReadLock();
                }

                if (!found)
                {
                    result = retrieveMethod(); // race condition here is ok, last one should win
                    _accessLock.EnterWriteLock();
                    try
                    {

                        dictionary[key] = result;
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
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
                T result = default(T);
                bool found = false;
                _accessLock.EnterReadLock();
                try
                {
                    found = this.InstanceCache.ContainsKey(callerName);
                    if(found)
                    {
                        result = (T)this.InstanceCache[callerName];
                    }
                }
                finally
                {
                    _accessLock.ExitReadLock();
                }
                if (!found)
                {
                    result = retrieveMethod();
                    _accessLock.EnterWriteLock(); // race condition here is ok, last one should win
                    try
                    {
                        this.InstanceCache[callerName] = result;
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
                    }
                }
                return result;
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

                _accessLock.EnterReadLock();
                try
                {
                    cache = Lifetime.GetValue() as AspectCache;
                }
                finally
                {
                    _accessLock.ExitReadLock();
                }

                if (cache == null)
                {
                    _accessLock.EnterWriteLock(); // race condition here is ok, last one should win
                    try
                    {
                        cache = new AspectCache(this.OwnerToken, base.IFoundation, this.Lifetime);
                        Lifetime.SetValue(cache);
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
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
                ConcurrentDictionary<K, T> dictionary = PerInstance(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                _accessLock.EnterWriteLock();
                try
                {
                    dictionary[key] = value;
                }
                finally
                {
                    _accessLock.ExitWriteLock();
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
                ConcurrentDictionary<K, T> dictionary = PerFoundation(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                _accessLock.EnterWriteLock();
                try
                {
                    dictionary[key] = value;
                }
                finally
                {
                    _accessLock.ExitWriteLock();
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
                ConcurrentDictionary<K, T> dictionary = PerLifetime(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                _accessLock.EnterWriteLock();
                try
                {
                    dictionary[key] = value;
                }
                finally
                {
                    _accessLock.ExitWriteLock();
                }
                return value;
            });
        }

        /// <summary>
        /// Forcibly removes the cache with the supplied value
        /// </summary>
        public virtual void ClearKeyedPerInstance<T, K>(K key, string callerName)
        {
            base.ExecuteMethod("ClearKeyedPerInstance", delegate()
            {
                ConcurrentDictionary<K, T> dictionary = PerInstance(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                _accessLock.EnterWriteLock();
                try
                {
                    T ignore = default(T);
                    dictionary.TryRemove(key, out ignore);
                }
                finally
                {
                    _accessLock.ExitWriteLock();
                }
            });
        }
        /// <summary>
        /// Forcibly removes the cache with the supplied value
        /// </summary>
        public virtual void ClearKeyedPerFoundation<T, K>(K key, string callerName)
        {
            base.ExecuteMethod("ClearKeyedPerFoundation", delegate()
            {
                ConcurrentDictionary<K, T> dictionary = PerFoundation(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                _accessLock.EnterWriteLock();
                try
                {
                    T ignore = default(T);
                    dictionary.TryRemove(key, out ignore);
                }
                finally
                {
                    _accessLock.ExitWriteLock();
                }
            });
        }
        /// <summary>
        /// Forcibly removes the cache with the supplied value
        /// </summary>
        public virtual void ClearKeyedPerLifetime<T, K>(K key, string callerName)
        {
            base.ExecuteMethod("ClearKeyedPerLifetime", delegate()
            {
                ConcurrentDictionary<K, T> dictionary = PerLifetime(callerName, delegate()
                {
                    return new ConcurrentDictionary<K, T>();
                });
                _accessLock.EnterWriteLock();
                try
                {
                    T ignore = default(T);
                    dictionary.TryRemove(key, out ignore);
                }
                finally
                {
                    _accessLock.ExitWriteLock();
                }
            });
        }

        /// <summary>
        /// Forcibly updates the cache with the supplied value
        /// </summary>
        public virtual T SetPerInstance<T>(string callerName, T value)
        {
            return base.ExecuteFunction<T>("SetPerInstance", delegate()
            {
                _accessLock.EnterWriteLock();
                try
                {
                    this.InstanceCache[callerName] = value;
                }
                finally
                {
                    _accessLock.ExitWriteLock();
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
                _accessLock.EnterReadLock();
                try
                {
                    cache = Lifetime.GetValue() as AspectCache;
                }
                finally
                {
                    _accessLock.ExitReadLock();
                }

                if (cache == null)
                {
                    cache = new AspectCache(this.OwnerToken, base.IFoundation, this.Lifetime);
                    Lifetime.SetValue(cache);
                }
                return cache.SetPerInstance<T>(callerName, value);
            });
        }

        /// <summary>
        /// Forcibly removes the cache with the supplied value
        /// </summary>
        public virtual void ClearPerInstance(string callerName)
        {
            base.ExecuteMethod("ClearPerInstance", delegate()
            {
                _accessLock.EnterWriteLock();
                try
                {
                    object ignore = null;
                    this.InstanceCache.TryRemove(callerName, out ignore);
                }
                finally
                {
                    _accessLock.ExitWriteLock();
                }
            });
        }
        /// <summary>
        /// Forcibly removes the cache with the supplied value
        /// </summary>
        public virtual void ClearPerFoundation<T>(string callerName)
        {
            base.ExecuteMethod("ClearPerFoundation", delegate()
            {
                AspectCache cache = base.IFoundation.Container.Resolve<AspectCache>();
                cache.ClearKeyedPerInstance<T, string>(this.OwnerToken, callerName);
            });
        }
        /// <summary>
        /// Forcibly removes the cache with the supplied callerName
        /// </summary>
        public virtual void ClearPerLifetime<T>(string callerName)
        {
            base.ExecuteMethod("ClearPerLifetime", delegate()
            {
                AspectCache cache = null;
                _accessLock.EnterReadLock();
                try
                {
                    cache = Lifetime.GetValue() as AspectCache;
                }
                finally
                {
                    _accessLock.ExitReadLock();
                }
                if (cache != null)
                {
                    cache.ClearPerInstance(callerName);
                }
            });
        }

        public virtual void ClearInstanceCache()
        {
            base.ExecuteMethod("ClearInstanceCache", delegate()
            {
                _accessLock.EnterWriteLock();
                try
                {
                    this.InstanceCache.Clear();
                }
                finally
                {
                    _accessLock.ExitWriteLock();
                }
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
                ClearFoundationCache();
            });
        }

    }
}
