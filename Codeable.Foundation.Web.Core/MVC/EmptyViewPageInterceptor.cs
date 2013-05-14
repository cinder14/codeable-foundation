using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.UI.Web.Core.MVC;

namespace Codeable.Foundation.Web.Core.MVC
{
    public class EmptyViewPageInterceptor : IViewPageInterceptor
    {
        #region IViewPageInterceptor Members

        public string FilterOutput(string output)
        {
            return output;
        }

        public string ProcessUrl(string path)
        {
            return path;
        }

        public bool ShouldFilter(System.Web.Mvc.ViewContext viewContext)
        {
            return false;
        }

        #endregion
    }
}
