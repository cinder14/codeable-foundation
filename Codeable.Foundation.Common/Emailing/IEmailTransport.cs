using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Emailing
{
    public interface IEmailTransport
    {
        IEmail CreateEmail();
        object SendEmail(IEmail email, IEmailRecipient recipient, bool checkUserPreferences);
    }
}
