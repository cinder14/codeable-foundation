using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Codeable.Foundation.UI.Web.Core.MVC.Embedded
{
    public interface IPluginVirtualPathProvider
    {
        WebPluginEmbeddedCache EmbeddedCache { get; }
        VirtualFile GetFile(string virtualPath);
    }
}
