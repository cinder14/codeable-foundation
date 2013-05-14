using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Codeable.Foundation.Core.System;

namespace Codeable.Foundation.UI.Web.Core.Caching
{
    public class FlushableOutputCache : System.Web.Caching.OutputCacheProvider, IDisposable
    {
        #region Statics

        private static List<FlushableOutputCache> _OutPutCaches = new List<FlushableOutputCache>();

        public static void FlushAllCaches()
        {
            foreach (FlushableOutputCache item in _OutPutCaches)
            {
                item.FlushAll();
            }
        } 

        #endregion

        #region Constructor & Finalizer

        public FlushableOutputCache()
        {
            _OutPutCaches.Add(this);
        }
        ~FlushableOutputCache()
        {
            Dispose(false);
        } 

        #endregion

        #region Private Properties
        
        private ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private Dictionary<string, FlushableOutputCacheItem> _cache = new Dictionary<string, FlushableOutputCacheItem>();

        #endregion

        #region Public Methods

        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            Set(key, entry, utcExpiry);
            return entry;
        }
        public override object Get(string key)
        {
            _cacheLock.EnterReadLock();
            try
            {
                FlushableOutputCacheItem cached = null;
                if (_cache.TryGetValue(key, out cached) && (cached != null))
                {
                    if (cached.UtcExpiry >= DateTime.UtcNow)
                    {
                        return cached.Value;
                    }
                }
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
            // outside of read lock
            Remove(key);
            return null;
        }
        public override void Remove(string key)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                _cache.Remove(key);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                FlushableOutputCacheItem cached = null;
                if (_cache.TryGetValue(key, out cached))
                {
                    if ((cached != null) && (cached.Value == entry)) // reference/lock saving
                    {
                        cached.UtcExpiry = utcExpiry;
                    }
                    else
                    {
                        _cacheLock.EnterWriteLock();
                        try
                        {
                            _cache[key] = new FlushableOutputCacheItem(entry, utcExpiry);
                        }
                        finally
                        {
                            _cacheLock.ExitWriteLock();
                        }
                    }
                }
                else
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        _cache.Add(key, new FlushableOutputCacheItem(entry, utcExpiry));
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        public void FlushAll()
        {
            _cacheLock.EnterWriteLock();
            try
            {
                _cache.Clear();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        } 

        #endregion

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cache.Clear();
                _OutPutCaches.Remove(this);
                if (_cacheLock != null)
                {
                    _cacheLock.Dispose();
                }
                _cacheLock = null;
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
