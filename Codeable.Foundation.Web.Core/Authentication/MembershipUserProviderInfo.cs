using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.Common;
using System.Web.Security;
using System.Web;
using Codeable.Foundation.UI.Web.Common.Authentication;

namespace Codeable.Foundation.UI.Web.Core.Authentication
{
    public class MembershipUserProviderInfo : IUserProviderInfo
    {
        public List<string> Roles
        {
            get;
            set;
        }

        public string UserId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string UserEmail
        {
            get;
            set;
        }

        public string UserAccountName
        {
            get;
            set;
        }

    }
}
