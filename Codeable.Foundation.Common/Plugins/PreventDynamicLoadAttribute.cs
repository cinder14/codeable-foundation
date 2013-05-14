using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Plugins
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class PreventDynamicLoadAttribute : Attribute
    {
        readonly bool _preventDynamicLoading;

        public PreventDynamicLoadAttribute(bool preventDynamicLoading)
        {
            _preventDynamicLoading = preventDynamicLoading;
        }

        public bool PreventDynamicLoading
        {
            get { return _preventDynamicLoading; }
        }
    }
}
