using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.MVC
{
    public interface IWebPluginDependency
    {
        void RaiseDependencyChanged(object sender, EventArgs args);
        string VirtualPath { get; }
    }
}
