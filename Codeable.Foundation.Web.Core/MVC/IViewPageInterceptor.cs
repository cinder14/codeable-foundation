using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.MVC
{
    public interface IViewPageInterceptor
    {
        string FilterOutput(string output);
        string ProcessUrl(string path);
        bool ShouldFilter(System.Web.Mvc.ViewContext viewContext);
    }
}
