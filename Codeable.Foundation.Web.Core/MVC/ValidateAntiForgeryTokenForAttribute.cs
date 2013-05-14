using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Codeable.Foundation.Web.Core.MVC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateAntiForgeryTokenForAttribute : FilterAttribute, IAuthorizationFilter
    {
        public ValidateAntiForgeryTokenForAttribute(HttpVerbs verbs)
        {
            this._verbs = new AcceptVerbsAttribute(verbs);
            this._validator = new ValidateAntiForgeryTokenAttribute();
        }


        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();
            if (this._verbs.Verbs.Contains(httpMethodOverride, StringComparer.OrdinalIgnoreCase))
            {
                this._validator.OnAuthorization(filterContext);
            }
        }
    }
}
