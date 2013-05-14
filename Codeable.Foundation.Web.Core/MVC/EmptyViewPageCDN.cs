using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Web.Core.MVC
{
    public class EmptyViewPageCDN : IViewPageCDN
    {
        #region IViewPageCDN Members

        public string GetCDNUrl(string path)
        {
            return path;
        }
        public string ReplaceCDNUrls(string contents)
        {
            return contents;
        }
        #endregion

    }
}
