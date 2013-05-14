using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web;
using System.Diagnostics;

namespace Codeable.Foundation.UI.Web.Core.Unity
{
    public class HttpUserSessionLifetimeManager : HttpSessionLifetimeManager
    {
        public HttpUserSessionLifetimeManager(string sessionKey)
            : base (sessionKey)
        {
        }
        protected override string GenerateSessionKey()
        {
            if (string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                return Guid.NewGuid().ToString();//don't save anything until we have a user.
            }
            return string.Format("{0}:{1}", HttpContext.Current.User.Identity.Name, this.SessionKey);
        }
    }
}
