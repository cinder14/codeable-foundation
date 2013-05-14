using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.UI.Web.Common.Authentication;

namespace Codeable.Foundation.UI.Web.Core.Authentication
{
    public class EmptyLogonInfo : ILogonInfo
    {
        public EmptyLogonInfo()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.RememberMe = false;
            this.AuthID = string.Empty; 
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string AuthID { get; set; }
        public string Authorizer { get; set; }
    }
}
