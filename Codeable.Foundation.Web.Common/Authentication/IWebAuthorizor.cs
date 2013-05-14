using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Common.Authentication
{
    public interface IWebAuthorizor
    {
        ILogonInfo SignIn(string userName, string authorizer, bool createPersistentCookie);
        void SignOut();
        ILogonInfo GetAuthenticatedUser();
        bool IsAuthenticated();
        string GetAuthID();
    }
}
