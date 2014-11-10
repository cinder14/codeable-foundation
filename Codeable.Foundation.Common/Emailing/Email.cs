using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Emailing
{
    public class Email : IEmail
    {
        public Email()
        {
            this.ExtraData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        public virtual string FromName { get; set; }
        public virtual string FromEmail { get; set; }
        public virtual string Subject { get; set; }
        public virtual string HTMLBody { get; set; }
        public virtual string CustomSmtpUser { get; set; }
        public virtual string CustomSmtpPass { get; set; }

        public virtual string InternalMessageType { get; set; }
        public virtual string InternalTypeID { get; set; }
        public virtual Dictionary<string, string> ExtraData { get; set; }

    }
}
