using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace Codeable.Foundation.UI.Web.Common.Plugins
{
    public class PluginConfig
    {
        public string AssemblyFileName { get; set; }
        public string FriendlyName { get; set; }
        public string SystemName { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string Feature { get; set; }
        public string Description { get; set; }

        [XmlIgnore, Browsable(false)]
        public Assembly ReferencedAssembly { get; set; }
        [XmlIgnore, Browsable(false)]
        public FileInfo SourceAssemblyFile { get; set; }
        [XmlIgnore, Browsable(false)]
        public Type PluginType { get; set; }
        [XmlIgnore, Browsable(false)]
        public Type ConfigClassType { get; set; }

        public int CompareTo(PluginConfig other)
        {
            return FriendlyName.CompareTo(other.FriendlyName);
        }

        public override string ToString()
        {
            return FriendlyName;
        }
        public override bool Equals(object obj)
        {
            var other = obj as PluginConfig;
            return other != null &&
                SystemName != null &&
                SystemName.Equals(other.SystemName);
        }
        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }











        
    }
}
