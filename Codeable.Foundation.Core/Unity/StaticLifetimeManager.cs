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
        protected static object _creationLock = new object();
        protected static object _accessLock = new object();
        protected Dictionary<string, StaticValue> StaticItems
        {
            get
            {
                if (Single<Dictionary<string, StaticValue>>.Instance == null)
                {
                    lock (_creationLock)
                    {
                        if (Single<Dictionary<string, StaticValue>>.Instance == null)
                        {
                            Single<Dictionary<string, StaticValue>>.Instance = new Dictionary<string, StaticValue>(StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }
                return Single<Dictionary<string, StaticValue>>.Instance;
            }
        }

        public override object GetValue()
        {
            object result = null;
            lock (_accessLock)
            {
                if (this.StaticItems.ContainsKey(this.GlobalKey))
                {
                    StaticValue value = this.StaticItems[GlobalKey];
                    if (value != null)
                    {
                        result = value.Value;
                    }
                }
            }
            return result;
        }
        public override void RemoveValue()
        {
            StaticValue value = null;
            lock (_accessLock)
            {
                if (this.StaticItems.ContainsKey(this.GlobalKey))
                {
                    value = this.StaticItems[this.GlobalKey];
                    this.StaticItems.Remove(this.GlobalKey);
                }
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
                this.StaticItems[this.GlobalKey] = new StaticValue(newValue);
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
