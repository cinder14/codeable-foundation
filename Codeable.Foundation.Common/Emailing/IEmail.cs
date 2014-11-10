using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Emailing
{
    public interface IEmail
    {
        string FromName { get; set; }
        string FromEmail { get; set; }
        string Subject { get; set; }
        string HTMLBody { get; set; }
        string CustomSmtpUser { get; set; }
        string CustomSmtpPass { get; set; }

        string InternalMessageType { get; set; }
        string InternalTypeID { get; set; }

        Dictionary<string, string> ExtraData { get; set; }
    }
}
