using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web;
using Codeable.Foundation.UI.Web.Core.Context;
using Codeable.Foundation.Core;

namespace Codeable.Foundation.UI.Web.Core.Caching
{
    public class UserIsolatedFlushableOutputCache : FlushableOutputCache
    {
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            return base.Add(InjectUserKey(key), entry, utcExpiry);
        }
        public override object Get(string key)
        {
            return base.Get(InjectUserKey(key));
        }
        public override void Remove(string key)
        {
            base.Remove(InjectUserKey(key));
        }
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            base.Set(InjectUserKey(key), entry, utcExpiry);
        }

        protected virtual string InjectUserKey(string key)
        {
            // get user?
            IHttpContext context = CoreFoundation.Current.SafeResolve<IHttpContext>();
            if (context != null)
            {
                HttpContext httpContext = context.GetContext();
                if ((httpContext != null))
                {
                    IPrincipal user = httpContext.User;
                    if(user != null)
                    {
                        IIdentity identity = user.Identity;
                        if(identity != null)
                        {
                            return string.Format("{0}:{1}", user.Identity.Name, key);
                        }
                    }
                }
            }
            return key;
        }
    }
}
