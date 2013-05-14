using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Common.Authentication
{
    public interface IUserProvider
    {
        IUserProviderInfo GetUser(string userName);
        IUserProviderInfo CreateUser(string userName, string password, string email);
        bool StartPasswordReset(string userName);
        bool DeleteUser(string userName);

        bool ChangePassword(string userName, string currentPassword, string newPassword);
    }
}
