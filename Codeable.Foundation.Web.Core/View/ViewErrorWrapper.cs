using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    [Serializable]
    public class ViewErrorWrapper : IContainViewError
    {
        public bool isError { get; set; }
        public IViewError error { get; set; }
    }
}
