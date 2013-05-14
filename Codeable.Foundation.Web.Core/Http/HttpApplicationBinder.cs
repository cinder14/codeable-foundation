using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common;
using System.Threading;
using System.Diagnostics;
using System.Web;
using Codeable.Foundation.Common.System;
using System.IO.Compression;

namespace Codeable.Foundation.UI.Web.Core.Http
{
    //Should not use aspect tracking [too deep]
    public class HttpApplicationBinder : IHttpApplicationBinder
    {
        public HttpApplicationBinder(IFoundation iFoundation)
        {
            IFoundation = iFoundation;
        }

        protected IFoundation IFoundation { get; set; }

        public void Bind(System.Web.HttpApplication httpApplication)
        {
            httpApplication.AuthenticateRequest += new EventHandler(OnAuthenticateRequest);
            httpApplication.BeginRequest += new EventHandler(OnBeginRequest);
            httpApplication.PreSendRequestHeaders +=  new EventHandler(OnPreSendRequestHeaders);
        }

        public virtual void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            try
            {
                // ensure that if GZip/Deflate Encoding is applied that headers are set
                // also works when error occurs if filters are still active
                HttpResponse response = HttpContext.Current.Response;
                if (response.Filter is GZipStream && response.Headers["Content-encoding"] != "gzip")
                    response.AppendHeader("Content-encoding", "gzip");
                else if (response.Filter is DeflateStream && response.Headers["Content-encoding"] != "deflate")
                    response.AppendHeader("Content-encoding", "deflate");
            }
            catch
            {
                // gulp
            }
        }

        public virtual bool GetVaryByCustomString(System.Web.HttpContext context, string custom, out string result)
        {
            result = string.Empty;

            if (custom.Equals(WebAssumptions.CACHE_BY_CULTURE_KEY, StringComparison.OrdinalIgnoreCase))
            {
                //TODO:Foundation: Vary by Culture
            }
            return false;
        }

        public virtual void OnAuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                //TODO:Foundation: Apply Culture
            }
            catch (Exception ex)
            {
                Trace.Write(String.Format("Unable to set culture: {0}", ex.Message), Category.Error);
            }
        }

        public virtual void OnBeginRequest(object sender, EventArgs e)
        {
            WebCoreUtility.FormsAuthenticationCookieFix();
        }
    }
}
