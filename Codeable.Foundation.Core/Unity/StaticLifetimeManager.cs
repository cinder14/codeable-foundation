using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web;
using System.Diagnostics;
using Codeable.Foundation.Core.System;
using Codeable.Foundation.Common.System;
using System.Threading;
using System.Collections.Concurrent;

namespace Codeable.Foundation.Core.Unity
{
    public class StaticLifetimeManager : LifetimeManager
    {
        public StaticLifetimeManager(string globalKey)
        {
            this.GlobalKey = "StaticLifetimeManager" + globalKey;
        }
        ~StaticLifetimeManager()
        {
            this.Dispose(false);
        }
        protected string GlobalKey { get; set; }
        protected static object _creationRoot = new object();

        protected static ReaderWriterLockSlim _accessLock = new ReaderWriterLockSlim();

        protected static TimeSpan _LockTimeout = TimeSpan.FromMilliseconds(300);
        protected static TimeSpan _LockRemoveTimeout = TimeSpan.FromMilliseconds(50);


        protected ConcurrentDictionary<string, StaticValue> StaticItems
        {
            get
            {
                if (Single<ConcurrentDictionary<string, StaticValue>>.Instance == null)
                {
                    lock (_creationRoot)
                    {
                        if (Single<ConcurrentDictionary<string, StaticValue>>.Instance == null)
                        {
                            Single<ConcurrentDictionary<string, StaticValue>>.Instance = new ConcurrentDictionary<string, StaticValue>(StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }
                return Single<ConcurrentDictionary<string, StaticValue>>.Instance;
            }
        }

        public override object GetValue()
        {
            object result = null;

            if (_accessLock.TryEnterReadLock(_LockTimeout))
            {
                try
                {
                    if (this.StaticItems.TryGetValue(this.GlobalKey, out StaticValue value))
                    {
                        if (value != null)
                        {
                            result = value.Value;
                        }
                    }
                }
                finally
                {
                    _accessLock.ExitReadLock();
                }
            }

            return result;
        }
        public override void RemoveValue()
        {
            StaticValue found = null;

            if (_accessLock.TryEnterWriteLock(_LockRemoveTimeout))
            {
                try
                {
                    this.StaticItems.TryRemove(this.GlobalKey, out found);
                }
                finally
                {
                    _accessLock.ExitWriteLock();
                }
            }

            if (found != null)
            {
                try
                {
                    found.Dispose();
                }
                catch { } // gulp
            }
        }
        public override void SetValue(object newValue)
        {
            StaticValue old = null;

            if (_accessLock.TryEnterWriteLock(_LockTimeout))
            {
                try
                {
                    this.StaticItems.TryRemove(this.GlobalKey, out old);
                    this.StaticItems[this.GlobalKey] = new StaticValue(newValue);
                }
                finally
                {
                    _accessLock.ExitWriteLock();
                }
            }

            if (old != null)
            {
                try
                {
                    old.Dispose();
                }
                catch { } // gulp
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            
        }


        public class StaticValue : IDisposable
        {
            public StaticValue(object value)
            {
                this.Value = value;
            }
            ~StaticValue()
            {
                this.Dispose(false);
            }
            public object Value;

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected void Dispose(bool disposing)
            {
                try
                {
                    if (disposing)
                    {
                        object value = this.Value;
                        this.Value = null;
                        if (value != null)
                        {
                            IDisposable disposable = value as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                }
                catch { }
            }
        }


    }
}
