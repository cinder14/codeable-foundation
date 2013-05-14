using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core
{
    /// <summary>
    /// Contains globally accepted assumptions about the API
    /// </summary>
    public static class WebAssumptions
    {
        public const string WEB_PLUGIN_FOLDER_NAME = "Plugins";
        public const string WEB_PLUGIN_PATH = "~/Plugins";
        public const string WEB_PLUGIN_SHADOWCOPY_PATH = "~/Plugins/bin";
        public const string WEB_PLUGIN_CONFIG_NAME = "PluginConfig.txt";
        public const string WEB_MAINTENANCE_PATH = "Maintenance.htm";
        public const string CACHE_BY_CULTURE_KEY = "culture";

        public const string DEFAULT_CULTURE = "en-US";
        public const string CULTURE_COOKIE_NAME = "_codeableCulture";

        public const string JSON_MIME_TYPE = "application/json";

        public const string SESSION_ID_QUERYSTRING_TOKEN = "ASPSESSID";
        public const string SESSION_ID_COOKIE_NAME = "ASP.NET_SESSIONID";
        public const string AUTH_ID_QUERYSTRING_TOKEN = "AUTHID";

        public const string EMAIL_TYPE_PASSWORDRESET = "PasswordReset";
    }
}
