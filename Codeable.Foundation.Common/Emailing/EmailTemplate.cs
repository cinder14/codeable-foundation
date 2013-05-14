using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Emailing
{
    public class EmailTemplate
    {
        public virtual string FromName { get; set; }
        public virtual string FromEmail { get; set; }
        public virtual string Subject { get; set; }
        public virtual string HTMLBody { get; set; }
        public virtual string DateFormat { get; set; }
    }
}
