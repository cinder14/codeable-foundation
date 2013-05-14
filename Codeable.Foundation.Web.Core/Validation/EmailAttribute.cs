using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrapeHeaven.Services.Core.MVC.Attributes
{
    public class EmailAttribute : CaseInsensitiveRegularExpressionAttribute
    {
        public EmailAttribute()
            : base("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")
        {
        }
    }
    
	
}
