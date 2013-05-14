using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    public interface IViewError
    {
        /// <summary>
        /// Use ViewError.*
        /// </summary>
        string type { get; set; }
        string Message { get; set; }
    }
}
