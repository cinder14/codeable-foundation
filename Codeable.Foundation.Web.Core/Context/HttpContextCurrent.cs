using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Codeable.Foundation.UI.Web.Core.Context
{
    public class HttpContextCurrent : IHttpContext
    {
        #region IHttpContextHybrid Members

        public HttpContext GetContext()
        {
            return HttpContext.Current;
        }
        public HttpContextBase GetContextBase()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }

        public HttpRequestBase GetRequestBase()
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Request != null))
            {
                return new HttpContextWrapper(HttpContext.Current).Request;
            }
            return null;
        }

        public HttpResponseBase GetResponseBase()
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Response != null))
            {
                return new HttpContextWrapper(HttpContext.Current).Response;
            }
            return null;
        }

        #endregion
    }
}
