using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web;
using System.Diagnostics;
using Codeable.Foundation.Core.System;
using Codeable.Foundation.Common.System;

namespace Codeable.Foundation.Core.Unity
{
    public class ExpireStaticLifetimeManager : LifetimeManager
    {
        public ExpireStaticLifetimeManager(string globalKey, TimeSpan lifeSpan, bool renewOnAccess = false)
        {
            this.GlobalKey = "ExpireStaticLifetimeManager" + globalKey;
            this.LifeSpan = lifeSpan;
            this.RenewOnAccess = renewOnAccess;
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
        protected Dictionary<string, ExpireStaticValue> StaticItems
        {
            get
            {
                if (Single<Dictionary<string, ExpireStaticValue>>.Instance == null)
                {
                    lock (_creationLock)
                    {
                        if (Single<Dictionary<string, ExpireStaticValue>>.Instance == null)
                        {
                            Single<Dictionary<string, ExpireStaticValue>>.Instance = new Dictionary<string, ExpireStaticValue>(StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }
                return Single<Dictionary<string, ExpireStaticValue>>.Instance;
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

                if (this.StaticItems.ContainsKey(this.GlobalKey))
                {
                    ExpireStaticValue value = this.StaticItems[this.GlobalKey];
                    this.StaticItems.Remove(this.GlobalKey);
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
