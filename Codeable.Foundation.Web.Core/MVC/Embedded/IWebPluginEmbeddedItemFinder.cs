using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.MVC.Embedded
{
    public interface IWebPluginEmbeddedItemFinder
    {
        WebPluginEmbeddedCache FindEmbeddedItems();
    }
}
