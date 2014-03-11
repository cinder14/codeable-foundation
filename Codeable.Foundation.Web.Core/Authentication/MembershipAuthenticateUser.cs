using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Codeable.Foundation.Common;
using Codeable.Foundation.Core.Aspect;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.UI.Web.Common.Authentication;

namespace Codeable.Foundation.UI.Web.Core.Authentication
{
    public class MembershipAuthenticateUser : ChokeableClass, IAuthenticateUser
    {
        public MembershipAuthenticateUser(IFoundation iFoundation)
            : base(iFoundation)
        {
 
        }

        public const string AUTHENTICATER_NAME = "Default";

        #region IAuthenticateUser Members

        public string Name
        {
            get { return AUTHENTICATER_NAME; }
        }

        public virtual bool ValidateUser(string userName, string password)
        {
            return base.ExecuteFunction<bool>("ValidateUser", delegate()
            {
                return Membership.ValidateUser(userName, password);
            });
        }

        public virtual bool IsActiveUser(string userName, bool setUserOnline = false)
        {
            return base.ExecuteFunction<bool>("IsActiveUser", delegate()
            {
                MembershipUser user = Membership.GetUser(userName, setUserOnline);

                if ((user == null) || !user.IsApproved || user.IsLockedOut)
                {
                    return false;
                }

                return true;
            });
        }
        public virtual bool IsValidUser(string userName, bool setUserOnline = false)
        {
            return base.ExecuteFunction<bool>("IsValidUser", delegate()
            {
                MembershipUser user = Membership.GetUser(userName, setUserOnline);
                return (user != null);
            });
        }

        #endregion

    }
}
