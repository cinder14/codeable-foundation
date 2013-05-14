using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Web.Core.MVC
{
    public interface IViewPageCDN
    {
        string GetCDNUrl(string path);
        string ReplaceCDNUrls(string contents);
    }
}
