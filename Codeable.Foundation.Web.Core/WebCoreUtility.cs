using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using System.Diagnostics;
using System.Web.Security;
using System.Configuration;
using System.Collections.Specialized;
using System.Security.Policy;

namespace Codeable.Foundation.UI.Web.Core
{
    public static class WebCoreUtility
    {

        private static AspNetHostingPermissionLevel? _trustLevel = null;

        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in
                        new AspNetHostingPermissionLevel[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal 
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return _trustLevel.Value;
        }

        #region Assembly Information

        private static Dictionary<string, string> _assemblyInformation = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        [DebuggerNonUserCode]
        public static string GetInformationalVersion(System.Reflection.Assembly assembly)
        {
            string result = string.Empty;
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            if (attributes.Length > 0)
            {
                result = ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion.ToString();
            }
            return result;
        }
        internal static string AssemblyProduct
        {
            get
            {
                if (!_assemblyInformation.ContainsKey("AssemblyProductAttribute"))
                {
                    string result = string.Empty;
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attributes.Length > 0)
                    {
                        result = ((AssemblyProductAttribute)attributes[0]).Product;
                    }
                    _assemblyInformation["AssemblyProductAttribute"] = result;
                }
                return _assemblyInformation["AssemblyProductAttribute"];
            }
        }
        internal static string AssemblyInfoVersion
        {
            get
            {
                if (!_assemblyInformation.ContainsKey("AssemblyInformationalVersionAttribute"))
                {
                    _assemblyInformation["AssemblyInformationalVersionAttribute"] = GetInformationalVersion(Assembly.GetExecutingAssembly());
                }
                return _assemblyInformation["AssemblyInformationalVersionAttribute"];
            }
        }
        internal static string AssemblyVersion
        {
            get
            {
                if (!_assemblyInformation.ContainsKey("AssemblyVersion"))
                {
                    _assemblyInformation["AssemblyVersion"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                return _assemblyInformation["AssemblyVersion"];

            }
        }
        internal static string AssemblyTitle
        {
            get
            {
                if (!_assemblyInformation.ContainsKey("AssemblyTitle"))
                {
                    _assemblyInformation["AssemblyTitle"] = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                        if (titleAttribute.Title != "")
                        {
                            _assemblyInformation["AssemblyTitle"] = titleAttribute.Title;
                        }
                    }

                }
                return _assemblyInformation["AssemblyTitle"];
            }
        }
        internal static string AssemblyDescription
        {
            get
            {
                if (!_assemblyInformation.ContainsKey("AssemblyDescription"))
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if (attributes.Length == 0)
                    {
                        _assemblyInformation["AssemblyDescription"] = string.Empty;
                    }
                    else
                    {
                        _assemblyInformation["AssemblyDescription"] = ((AssemblyDescriptionAttribute)attributes[0]).Description;
                    }
                }
                return _assemblyInformation["AssemblyDescription"];

            }
        }
        internal static string AssemblyCopyright
        {
            get
            {
                if (!_assemblyInformation.ContainsKey("AssemblyCopyright"))
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if (attributes.Length == 0)
                    {
                        _assemblyInformation["AssemblyCopyright"] = string.Empty;
                    }
                    else
                    {
                        _assemblyInformation["AssemblyCopyright"] = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                    }
                }
                return _assemblyInformation["AssemblyCopyright"];
            }
        }
        internal static string AssemblyCompany
        {
            get
            {
                if (!_assemblyInformation.ContainsKey("AssemblyCompany"))
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if (attributes.Length == 0)
                    {
                        _assemblyInformation["AssemblyCompany"] = string.Empty;
                    }
                    else
                    {
                        _assemblyInformation["AssemblyCompany"] = ((AssemblyCompanyAttribute)attributes[0]).Company;
                    }
                }
                return _assemblyInformation["AssemblyCompany"];
            }
        }

        #endregion

        public static void FormsAuthenticationCookieFix()
        {
            /* Fix for the Flash Player Cookie bug in Non-IE browsers.
            * Since Flash Player always sends the IE cookies even in FireFox
            * we have to bypass the cookies by sending the values as part of the POST or GET
            * and overwrite the cookies with the passed in values.
            *
            * The theory is that at this point (BeginRequest) the cookies have not been ready by
            * the Session and Authentication logic and if we update the cookies here we'll get our
            * Session and Authentication restored correctly
            */
            try
            {
                //STANDARD
                string session_param_name = WebAssumptions.SESSION_ID_QUERYSTRING_TOKEN;
                string session_cookie_name = WebAssumptions.SESSION_ID_COOKIE_NAME; 
                if (HttpContext.Current.Request.Form[session_param_name] != null)
                {
                    UpdateCookie(session_cookie_name, HttpContext.Current.Request.Form[session_param_name]);
                }
                else if (HttpContext.Current.Request.QueryString[session_param_name] != null)
                {
                    UpdateCookie(session_cookie_name, HttpContext.Current.Request.QueryString[session_param_name]);
                }
            }
            catch (Exception)
            {
            }
            try
            {
                //CUSTOM
                string auth_param_name = WebAssumptions.AUTH_ID_QUERYSTRING_TOKEN;
                string auth_cookie_name = FormsAuthentication.FormsCookieName;
                if (HttpContext.Current.Request.Form[auth_param_name] != null)
                {
                    UpdateCookie(auth_cookie_name, HttpContext.Current.Request.Form[auth_param_name]);
                }
                else if (HttpContext.Current.Request.QueryString[auth_param_name] != null)
                {
                    UpdateCookie(auth_cookie_name, HttpContext.Current.Request.QueryString[auth_param_name]);
                }
            }
            catch (Exception)
            {
            }

        }
        public static void UpdateCookie(string cookie_name, string cookie_value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(cookie_name);
            if (cookie != null)
            {
                cookie.Value = cookie_value;
                HttpContext.Current.Request.Cookies.Set(cookie);
            }
            else
            {
                cookie = new HttpCookie(cookie_name, cookie_value);
                HttpContext.Current.Request.Cookies.Add(cookie);
            }

        }


        /// <summary>
        /// Injects an app setting into the current configuration manager
        /// </summary>
        /// <param name="settingName">The name of the AppSetting to inject</param>
        /// <param name="value">The vlaue to assign to the settingName</param>
        /// <param name="overwriteDefault">If true, it will overwrite the default value provided in the web/app config.</param>
        public static void EnsureAppSetting(string settingName, string value, bool overwriteDefault)
        {
            bool shouldInject = false;
            string injectKey = string.Format("was_{0}_injected", settingName);
            if (overwriteDefault)
            {
                shouldInject = string.IsNullOrEmpty(ConfigurationManager.AppSettings[injectKey]);
            }
            if (shouldInject || string.IsNullOrEmpty(ConfigurationManager.AppSettings[settingName]))
            {
                FieldInfo fi = typeof(NameObjectCollectionBase).GetField("_readOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic);
                fi.SetValue(ConfigurationManager.AppSettings, false);

                ConfigurationManager.AppSettings[settingName] = value;
                ConfigurationManager.AppSettings[injectKey] = "true";

                fi.SetValue(ConfigurationManager.AppSettings, true);
            }
        }

        public static Uri ExtractOriginalUrl(HttpRequestBase request)
        {
            string hostHeader = request.Headers["host"];
            return new Uri(string.Format("{0}://{1}{2}",
               request.Url.Scheme,
               hostHeader,
               request.RawUrl));
        }

    }
}
