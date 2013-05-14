using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    public interface IJsonViewResponse
    {
        bool hasError { get; set; }
        bool hasPaging { get; set; }

        IViewError error { get; set; }
        IPagingData paging { get; set; }
    }
}
