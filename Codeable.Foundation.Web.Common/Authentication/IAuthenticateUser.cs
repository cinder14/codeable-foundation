using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Common.Authentication
{
    public interface IAuthenticateUser
    {
        string Name { get; }

        bool ValidateUser(string userName, string password);
        /// <summary>
        /// User is part of membership and can log in
        /// </summary>
        bool IsActiveUser(string userName);
        /// <summary>
        /// User is part of membership, but may not be able to log in (revoked, disabled, etc)
        /// </summary>
        bool IsValidUser(string userName);
    }
}
