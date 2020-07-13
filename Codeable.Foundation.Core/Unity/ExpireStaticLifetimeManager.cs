using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web;
using System.Diagnostics;
using Codeable.Foundation.Core.System;
using Codeable.Foundation.Common.System;
using System.Collections.Concurrent;
using Microsoft.Practices.ObjectBuilder2;
using System.Runtime.CompilerServices;

namespace Codeable.Foundation.Core.Unity
{
    public class ExpireStaticLifetimeManager : LifetimeManager, IKeyedLifetimeManager
    {
        public ExpireStaticLifetimeManager(string globalKey, TimeSpan lifeSpan, bool renewOnAccess = false)
        {
            this.GlobalKey = "ExpireStaticLifetimeManager" + globalKey;
            this.NestedKey = "ExpireStaticLifetimeManager.Nested" + globalKey;

            this.LifeSpan = lifeSpan;
            this.RenewOnAccess = renewOnAccess;
            ExpireStaticLifetimeDaemon.EnsureDaemon();
        }
        ~ExpireStaticLifetimeManager()
        {
            this.Dispose(false);
        }

        protected string GlobalKey { get; set; }
        protected string NestedKey { get; set; }
        protected TimeSpan LifeSpan { get; set; }
        protected bool RenewOnAccess { get; set; }
        protected static object _creationLock = new object();
        protected static object _accessLock = new object();

        public static void CleanExpiredValues()
        {
            CleanExpiredStaticValues();
            CleanExpiredNestedValues();
        }
        public static void CleanExpiredStaticValues()
        {
            try
            {
                ConcurrentDictionary<string, ExpireStaticValue> staticItems = null;
                KeyValuePair<string, ExpireStaticValue>[] values = null;
                lock (_accessLock)
                {
                    staticItems = Single<ConcurrentDictionary<string, ExpireStaticValue>>.Instance;
                    if (staticItems != null)
                    {
                        values = staticItems.ToArray();
                    }
                }

                if (staticItems != null && values != null)
                {
                    foreach (KeyValuePair<string, ExpireStaticValue> item in values)
                    {
                        if (item.Value != null && !item.Value.AllowAccess(false))
                        {
                            ExpireStaticValue expired = null;
                            if (staticItems.TryRemove(item.Key, out expired))
                            {
                                if (item.Value != null)
                                {
                                    try
                                    {
                                        item.Value.Dispose();
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // gulp
            }
        }
        public static void CleanExpiredNestedValues()
        {
            try
            {
                ConcurrentDictionary<string, ConcurrentDictionary<object, ExpireStaticValue>> staticNestedItems = null;
                KeyValuePair<string, ConcurrentDictionary<object, ExpireStaticValue>>[] values = null;
                lock (_accessLock)
                {
                    staticNestedItems = Single<ConcurrentDictionary<string, ConcurrentDictionary<object, ExpireStaticValue>>>.Instance;
                    if (staticNestedItems != null)
                    {
                        values = staticNestedItems.ToArray();
                    }
                }

                if (staticNestedItems != null && values != null)
                {
                    foreach (KeyValuePair<string, ConcurrentDictionary<object, ExpireStaticValue>> nested in values)
                    {
                        if (nested.Value != null)
                        {
                            KeyValuePair<object, ExpireStaticValue>[] entries = nested.Value.ToArray();
                            foreach (KeyValuePair<object, ExpireStaticValue> entry in entries)
                            {
                                if (entry.Value != null && !entry.Value.AllowAccess(false))
                                {
                                    ExpireStaticValue expired = null;
                                    if (nested.Value.TryRemove(entry.Key, out expired))
                                    {
                                        if (entry.Value != null)
                                        {
                                            try
                                            {
                                                entry.Value.Dispose();
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // gulp
            }
        }

        protected ConcurrentDictionary<string, ExpireStaticValue> StaticItems
        {
            get
            {
                if (Single<ConcurrentDictionary<string, ExpireStaticValue>>.Instance == null)
                {
                    lock (_creationLock)
                    {
                        if (Single<ConcurrentDictionary<string, ExpireStaticValue>>.Instance == null)
                        {
                            Single<ConcurrentDictionary<string, ExpireStaticValue>>.Instance = new ConcurrentDictionary<string, ExpireStaticValue>(StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }
                return Single<ConcurrentDictionary<string, ExpireStaticValue>>.Instance;
            }
        }
        protected ConcurrentDictionary<string, ConcurrentDictionary<object, ExpireStaticValue>> StaticNestedItems
        {
            get
            {
                if (Single<ConcurrentDictionary<string, ConcurrentDictionary<object, ExpireStaticValue>>>.Instance == null)
                {
                    lock (_creationLock)
                    {
                        if (Single<ConcurrentDictionary<string, ConcurrentDictionary<object, ExpireStaticValue>>>.Instance == null)
                        {
                            Single<ConcurrentDictionary<string, ConcurrentDictionary<object, ExpireStaticValue>>>.Instance = new ConcurrentDictionary<string, ConcurrentDictionary<object, ExpireStaticValue>>(StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }
                return Single<ConcurrentDictionary<string, ConcurrentDictionary<object, ExpireStaticValue>>>.Instance;
            }
        }

        public bool HasExpired()
        {
            ExpireStaticValue value = null;
            lock (_accessLock)
            {
                if (this.StaticItems.ContainsKey(this.GlobalKey))
                {
                    value = this.StaticItems[GlobalKey];
                }
            }

            if (value != null)
            {
                return !value.AllowAccess(false);
            }
            return true;
        }
        public override object GetValue()
        {
            object result = null;

            ExpireStaticValue value = null;
            lock (_accessLock)
            {
                if (this.StaticItems.ContainsKey(this.GlobalKey))
                {
                    value = this.StaticItems[GlobalKey];
                }
            }
            if (value != null)
            {
                if (value.AllowAccess(true))
                {
                    result = value.Value;
                }
                else
                {
                    this.RemoveValue();
                }
            }
            return result;
        }
        public override void RemoveValue()
        {
            ExpireStaticValue value = null;
            lock (_accessLock)
            {
                this.StaticItems.TryRemove(this.GlobalKey, out value);
            }

            if (value != null)
            {
                value.Dispose();
            }
        }
        public override void SetValue(object newValue)
        {
            this.RemoveValue();
            lock (_accessLock)
            {
                this.StaticItems[this.GlobalKey] = new ExpireStaticValue(this.RenewOnAccess, this.LifeSpan, newValue);
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


        public TData GetKeyedValue<TKey, TData>(TKey key, out bool found)
        {
            found = false;

            ConcurrentDictionary<object, ExpireStaticValue> dictionary = null;
            lock (_accessLock)
            {
                if (this.StaticNestedItems.ContainsKey(this.NestedKey))
                {
                    dictionary = this.StaticNestedItems[this.NestedKey];
                }
            }

            if(dictionary != null && dictionary.TryGetValue(key, out ExpireStaticValue value))
            {
                found = value.AllowAccess(true);
                if(found)
                {
                    try
                    {
                        return (TData)value.Value;

                    }
                    catch
                    {
                        // not possible by use-case
                    }
                }
            }
            return default(TData);
        }

        public void SetKeyedValue<TKey, TData>(TKey key, TData value)
        {
            ConcurrentDictionary<object, ExpireStaticValue> dictionary = null;
            this.StaticNestedItems.TryGetValue(this.NestedKey, out dictionary);

            if (dictionary == null)
            {
                lock (_accessLock)
                {
                    if (!this.StaticNestedItems.TryGetValue(this.NestedKey, out dictionary))
                    {
                        if (dictionary == null)
                        {
                            dictionary = new ConcurrentDictionary<object, ExpireStaticValue>();
                            this.StaticNestedItems[this.NestedKey] = dictionary;
                        }
                    }
                }
            }

            dictionary[key] = new ExpireStaticValue(this.RenewOnAccess, this.LifeSpan, value);
        }

        public class ExpireStaticValue : IDisposable
        {
            public ExpireStaticValue(bool renewOnAccess, TimeSpan lifeSpan, object value)
            {
                this.LifeSpan = lifeSpan;
                this.RenewOnAccess = renewOnAccess;
                this.Value = value;
                this.RenewLease();
            }
            ~ExpireStaticValue()
            {
                this.Dispose(false);
            }

            public bool RenewOnAccess;
            public TimeSpan LifeSpan;
            public object Value;
            public DateTime UtcExpire;

            public void RenewLease()
            {
                UtcExpire = DateTime.UtcNow.Add(this.LifeSpan);
            }
            public bool AllowAccess(bool allowRenew)
            {
                if (UtcExpire < DateTime.UtcNow)
                {
                    return false;
                }
                if (allowRenew && this.RenewOnAccess)
                {
                    this.RenewLease();
                }
                return true;
            }
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
