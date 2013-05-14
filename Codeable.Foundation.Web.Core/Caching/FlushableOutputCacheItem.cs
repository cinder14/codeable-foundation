using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.Caching
{
    public class FlushableOutputCacheItem
    {
        public FlushableOutputCacheItem(object value, DateTime utcExpiry)
        {
            Value = value;
            UtcExpiry = utcExpiry;
        }

        public DateTime UtcExpiry;
        public object Value;

    }
}
