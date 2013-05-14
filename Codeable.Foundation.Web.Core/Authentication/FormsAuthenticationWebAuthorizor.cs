using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.Common;
using System.Web;
using Codeable.Foundation.UI.Web.Core.Context;
using Codeable.Foundation.Core.Aspect;
using Codeable.Foundation.UI.Web.Common.Authentication;

namespace Codeable.Foundation.UI.Web.Core.Authentication
{
    public class FormsAuthenticationWebAuthorizor : ChokeableClass, IWebAuthorizor
    {
        public FormsAuthenticationWebAuthorizor(IFoundation iFoundation, IHttpContext httpContext)
            : base(iFoundation)
        {
            HttpContext = httpContext;
        }

        public static string AUTHORIZER_COOKIE = "_cfv2";


        protected virtual IHttpContext HttpContext { get;set; }

        #region IFormsAuthenticationService

        public virtual ILogonInfo SignIn(string userName, string authorizer, bool createPersistentCookie)
        {
            return base.ExecuteFunction<ILogonInfo>("SignIn", delegate()
            {
                try
                {
                    this.HttpContext.GetContext().Session.Clear();
                }
                catch { }
                ILogonInfo li = new LogonInfo();
                FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
                li.RememberMe = createPersistentCookie;
                li.UserName = userName;
                li.AuthID = userName;
                li.Authorizer = authorizer;
                HttpContext context = HttpContext.GetContext();
                if (context != null && (context.Response != null))
                {
                    if (context.Response.Cookies[FormsAuthentication.FormsCookieName] != null)
                    {
                        li.AuthID = context.Response.Cookies[FormsAuthentication.FormsCookieName].Value;
                    }
                    if (context.Request.Url.Host != "localhost")
                    {
                        context.Response.Cookies[FormsAuthenticationWebAuthorizor.AUTHORIZER_COOKIE].Values["a"] = authorizer;
                        context.Response.Cookies[FormsAuthenticationWebAuthorizor.AUTHORIZER_COOKIE].Expires = DateTime.Today.AddYears(1);
                        context.Response.Cookies[FormsAuthenticationWebAuthorizor.AUTHORIZER_COOKIE].Domain = context.Request.Url.Host;
                    }
                    else
                    {
                        context.Response.Cookies[FormsAuthenticationWebAuthorizor.AUTHORIZER_COOKIE + "l"].Values["a"] = authorizer;
                        context.Response.Cookies[FormsAuthenticationWebAuthorizor.AUTHORIZER_COOKIE + "l"].Expires = DateTime.Today.AddYears(1);
                    }
                    
                }

                return li;                
            });
        }

        public virtual ILogonInfo GetAuthenticatedUser()
        {
            return base.ExecuteFunction<ILogonInfo>("GetAuthenticatedUser", delegate()
            {
                ILogonInfo result = null;

                HttpCookie auth_cookie = this.HttpContext.GetRequestBase().Cookies[FormsAuthentication.FormsCookieName];
                if (auth_cookie != null && !string.IsNullOrEmpty(auth_cookie.Value))
                {
                    try
                    {
                        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(auth_cookie.Value);
                        if (ticket != null)
                        {
                            result = new LogonInfo();
                            result.AuthID = ticket.Name;
                            result.RememberMe = ticket.IsPersistent;
                            result.UserName = ticket.Name;

                            HttpCookie foundationCookie = null;
                            if (this.HttpContext.GetRequestBase().Url.Host != "localhost")
                            {
                                foundationCookie = this.HttpContext.GetRequestBase().Cookies[FormsAuthenticationWebAuthorizor.AUTHORIZER_COOKIE];
                            }
                            else
                            {
                                foundationCookie = this.HttpContext.GetRequestBase().Cookies[FormsAuthenticationWebAuthorizor.AUTHORIZER_COOKIE + "l"];
                            }
                            if (foundationCookie != null)
                            {
                                result.Authorizer = foundationCookie.Values["a"];
                            }
                        }
                    }
                    catch
                    {
                    }
                    
                }
                return result;
            });
        }
        public virtual bool IsAuthenticated()
        {
            return base.ExecuteFunction<bool>("IsAuthenticated", delegate()
            {
                if (this.HttpContext.GetContext().User.Identity.IsAuthenticated)
                {
                    HttpCookie auth_cookie = this.HttpContext.GetRequestBase().Cookies[FormsAuthentication.FormsCookieName];
                    return (auth_cookie != null);
                }
                return false;
            });
            
        }
        public virtual void SignOut()
        {
            base.ExecuteMethod("SignOut", delegate()
            {
                FormsAuthentication.SignOut();
                try
                {
                    this.HttpContext.GetContext().Session.Clear();
                }
                catch { }
            });
        }

        public virtual string GetAuthID()
        {
            return base.ExecuteFunction<string>("GetAuthID", delegate()
            {
                HttpContext context = HttpContext.GetContext();
                if (context == null) { return string.Empty; }

                if (context.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    return context.Request.Cookies[FormsAuthentication.FormsCookieName].Value;
                }
                return string.Empty;
            });
        }

        #endregion


    }
}
