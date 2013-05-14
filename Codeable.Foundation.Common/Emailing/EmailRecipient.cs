using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Emailing
{
    public class EmailRecipient : IEmailRecipient
    {
        public EmailRecipient()
        {
        }
        public EmailRecipient(string recipient)
        {
            this.SetEmail(recipient);
        }

        public virtual string DisplayName { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual object SysID { get; set; }

        public virtual void SetEmail(string recipient)
        {
            //TODO:EmailRecipient: use regex or underlying system here
            if (recipient.Contains(">") && recipient.Contains("<"))
            {
                this.DisplayName = recipient.Substring(0, recipient.IndexOf(">")).Replace("<","").Trim();
                this.EmailAddress = recipient.Substring(recipient.IndexOf(">")).Replace(">","").Trim();
            }
            else
            {
                this.EmailAddress = recipient;
            }
        }
        
        public virtual string FormattedEmailAddress()
        {
            if (!string.IsNullOrEmpty(DisplayName))
            {
                //TODO:EmailRecipient: use RegEx Here
                return string.Format("<{0}> {1}", this.DisplayName.Replace("<","").Replace(">",""), this.EmailAddress);
            }
            else
            {
                return this.EmailAddress;
            }
        }
    }
}
