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
            lock (_accessLock)
            {
                if (this.StaticItems.ContainsKey(this.GlobalKey))
                {
                    this.StaticItems.Remove(this.GlobalKey);
                }
            }
        }
        public override void SetValue(object newValue)
        {
            lock (_accessLock)
            {
                this.StaticItems[GlobalKey] = new StaticValue(newValue);
            }
        }

        public class StaticValue
        {
            public StaticValue(object value)
            {
                this.Value = value;
            }
            public object Value;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {

        }




    }
}
