using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

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

        public bool Disabled { get; set; }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            string httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();
            if (!Disabled && this._verbs.Verbs.Contains(httpMethodOverride, StringComparer.OrdinalIgnoreCase))
            {
                this.PreProcessValidation(filterContext);
                this._validator.OnAuthorization(filterContext);
            }
        }

        protected virtual void PreProcessValidation(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.ContentType.ToLower().Contains("json"))
            {
                try
                {
                    var bytes = new byte[filterContext.HttpContext.Request.InputStream.Length];
                    filterContext.HttpContext.Request.InputStream.Read(bytes, 0, bytes.Length);
                    filterContext.HttpContext.Request.InputStream.Position = 0;
                    var json = Encoding.ASCII.GetString(bytes);
                    var jsonObject = JObject.Parse(json);
                    filterContext.HttpContext.Request.Form.GetType().GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty).SetValue(filterContext.HttpContext.Request.Form, false, null);
                    filterContext.HttpContext.Request.Form["__RequestVerificationToken"] = (string)jsonObject["__RequestVerificationToken"];
                }
                catch
                {
                    // gulp
                }
            }
        }
    }
}
