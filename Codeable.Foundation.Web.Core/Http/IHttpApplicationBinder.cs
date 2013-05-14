using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Codeable.Foundation.UI.Web.Core.Http
{
    public interface IHttpApplicationBinder
    {
        void Bind(HttpApplication httpApplication);
        bool GetVaryByCustomString(HttpContext context, string custom, out string result);
    }
}
