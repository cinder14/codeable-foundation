using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web;
using System.Diagnostics;

namespace Codeable.Foundation.UI.Web.Core.Unity
{
    public class HttpSessionLifetimeManager : LifetimeManager
    {
        public HttpSessionLifetimeManager(string sessionKey)
        {
            this.SessionKey = "HttpSessionLifetimeManager" + sessionKey;
        }

        protected string SessionKey { get; set; }

        public override object GetValue()
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                object result = HttpContext.Current.Session[GenerateSessionKey()];
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        public override void RemoveValue()
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                HttpContext.Current.Session.Remove(GenerateSessionKey());
            }
        }
        public override void SetValue(object newValue)
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                HttpContext.Current.Session[GenerateSessionKey()] = newValue;
            }
        }

        protected virtual string GenerateSessionKey()
        {
            return this.SessionKey;
        }
    }
}
