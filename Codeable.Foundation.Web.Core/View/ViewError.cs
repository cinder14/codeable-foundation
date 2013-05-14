using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    [Serializable]
    public class ViewError : IViewError
    {
        public const string SERVER = "server";
        public const string USER = "user";
        public const string LOGIN = "login";
        public const string UNSPECIFIED = "";

        public ViewError()
        {
        }
        public string type { get; set; }
        public string Message { get; set; }
    }
}
