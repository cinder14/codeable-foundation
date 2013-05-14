using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.IO;
using System.Reflection;
using Codeable.Foundation.UI.Web.Core.IO;

namespace Codeable.Foundation.UI.Web.Core.MVC.Embedded
{
    public class ChainedWebPluginEmbeddedVirtualFile : VirtualFile
    {
        public ChainedWebPluginEmbeddedVirtualFile(IEnumerable<WebPluginEmbeddedInfo> viewInfos, string virtualPath)
            : base(virtualPath)
        {
            _viewInfos.AddRange(viewInfos);
        }

        private List<WebPluginEmbeddedInfo> _viewInfos = new List<WebPluginEmbeddedInfo>();
        private VirtualFile _externalFile;

        public override Stream Open()
        {
            return new MultiStreamReader(this.GetStreams());
        }
        public virtual void AddExternalFile(VirtualFile virtualFile)
        {
            _externalFile = virtualFile;
        }
        public virtual List<Stream> GetStreams()
        {
            List<Stream> streams = new List<Stream>();
            if (_externalFile != null)
            {
                Stream stream = _externalFile.Open();
                if (stream != null)
                {
                    streams.Add(stream);
                }
            }
            foreach (WebPluginEmbeddedInfo item in _viewInfos)
            {
                Assembly assembly = GetResourceAssembly(item);
                if (assembly != null)
                {
                    Stream stream = assembly.GetManifestResourceStream(item.Name);
                    if (stream != null)
                    {
                        streams.Add(stream);
                    }
                }
            }
            return streams;
        }

        protected virtual Assembly GetResourceAssembly(WebPluginEmbeddedInfo viewInfo)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => (viewInfo != null) && string.Equals(assembly.FullName, viewInfo.AssemblyFullName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

    }
}
