using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Codeable.Foundation.UI.Web.Core.Context
{
    public interface IHttpContext
    {
        HttpContextBase GetContextBase();
        HttpRequestBase GetRequestBase();
        HttpResponseBase GetResponseBase();

        /// <summary>
        /// Are you sure you should be using this version and not GetContextBase()?
        /// </summary>
        HttpContext GetContext();
    }
}
