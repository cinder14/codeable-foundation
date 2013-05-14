using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Common.Authentication
{
    public interface IUserProviderInfo
    {
        List<string> Roles { get; set; }
        string UserId { get; set; }
        string UserName { get; set; }
        string UserEmail { get; set; }
        string UserAccountName { get; set; }
    }
}
