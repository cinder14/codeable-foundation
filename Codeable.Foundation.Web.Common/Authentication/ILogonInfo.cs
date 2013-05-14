using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Common.Authentication
{
    public interface ILogonInfo
    {
        string UserName { get; set; }
        string Password { get; set; }
        bool RememberMe { get; set; }
        string AuthID { get; set; }
        string Authorizer { get; set; }
    }
}
