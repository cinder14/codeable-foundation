using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    public interface IContainViewError
    {
        bool isError { get; set; }
        IViewError error { get; set; }
    }
}
