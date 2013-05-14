using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Codeable.Foundation.UI.Web.Core.MVC.Razor
{
    public class LiteralString : IHtmlString
    {
        public LiteralString(string value)
        {
            _value = value;
        }

        private string _value;

        #region IHtmlString Members

        public string ToHtmlString()
        {
            return _value;
        }

        #endregion
    }
}
