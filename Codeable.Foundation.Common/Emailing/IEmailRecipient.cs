using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Emailing
{
    public interface IEmailRecipient
    {
        object SysID { get; }
        string DisplayName { get; }
        string EmailAddress { get; }

        string FormattedEmailAddress();
    }
}
