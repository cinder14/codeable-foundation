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

namespace Codeable.Foundation.Core.Unity
{
    public class ExpireStaticLifetimeManager : LifetimeManager
    {
        public ExpireStaticLifetimeManager(string globalKey, TimeSpan lifeSpan, bool renewOnAccess = false)
        {
            this.GlobalKey = "ExpireStaticLifetimeManager" + globalKey;
            this.LifeSpan = lifeSpan;
            this.RenewOnAccess = renewOnAccess;
            ExpireStaticLifetimeDaemon.EnsureDaemon();
        }
        ~ExpireStaticLifetimeManager()
        {
            this.Dispose(false);
        }

        protected string GlobalKey { get; set; }
        protected TimeSpan LifeSpan { get; set; }
        protected bool RenewOnAccess { get; set; }
        protected static object _creationLock = new object();
        protected static object _accessLock = new object();

        public static void CleanExpiredValues()
        {
            try
            {
                ConcurrentDictionary<string, ExpireStaticValue> staticItems;
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
                        if(!item.Value.AllowAccess(false))
                        {
                            ExpireStaticValue expired = null;
                            if(staticItems.TryRemove(item.Key, out expired))
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

        public bool HasExpired()
        {
            lock (_accessLock)
            {
                if (this.StaticItems.ContainsKey(this.GlobalKey))
                {
                    ExpireStaticValue value = this.StaticItems[GlobalKey];
                    if (value != null)
                    {
                        return !value.AllowAccess(false);
                    }
                }
            }
            return true;
        }
        public override object GetValue()
        {
            object result = null;
            lock (_accessLock)
            {
                if (this.StaticItems.ContainsKey(this.GlobalKey))
                {
                    ExpireStaticValue value = this.StaticItems[GlobalKey];
                    if (value != null)
                    {
                        if(value.AllowAccess(true))
                        {
                            result = value.Value;
                        }
                        else
                        {
                            value.Dispose();
                        }
                    }
                }
            }
            return result;
        }
        public override void RemoveValue()
        {
            lock (_accessLock)
            {
                if (this.StaticItems.TryRemove(this.GlobalKey, out ExpireStaticValue value))
                {
                    if (value != null)
                    {
                        value.Dispose();
                    }
                }
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
