using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.UI.Web.Common.Authentication;

namespace Codeable.Foundation.UI.Web.Core.Authentication
{
    public class LogonInfo : ILogonInfo
    {
        public string user_name { get; set; }
        public string password { get; set; }

        public bool RememberMe { get; set; }
        public string AuthID { get; set; }
        public string Authorizer { get; set; }

        string ILogonInfo.UserName
        {
            get
            {
                return user_name;
            }
            set
            {
                user_name = value;
            }
        }

        string ILogonInfo.Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }

    }
}
