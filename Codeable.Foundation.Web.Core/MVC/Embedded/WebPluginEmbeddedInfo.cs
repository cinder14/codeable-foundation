using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Codeable.Foundation.UI.Web.Core.MVC.Embedded
{
    [Serializable]
    public class WebPluginEmbeddedInfo
    {
        public string Name { get; set; }
        public string AssemblyFullName { get; set; }
        public string VirtualItemName { get; set; }
        public bool IsChained { get; set; }
        public HttpCacheability? Cacheability { get; set; }
        public string CacheExtension { get; set; }

        /// <summary>
        /// Flags the embedded item as an override only, (only prevents actual file from being loaded)
        /// </summary>
        public bool IsWebFormOverride { get; set; }

        public override string ToString()
        {
            return string.Format("{1} -> {0}", this.AssemblyFullName, this.VirtualItemName);
        }
    }
}
