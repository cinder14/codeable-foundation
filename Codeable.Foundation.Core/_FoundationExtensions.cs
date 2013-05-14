using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Codeable.Foundation.Common;

namespace Codeable.Foundation.Common
{
    public static class _FoundationExtensions
    {
        public static T Resolve<T>(this IFoundation iFoundation, params ResolverOverride[] overrides)
        {
            return iFoundation.Container.Resolve<T>(overrides);
        }
        public static T Resolve<T>(this IFoundation iFoundation, string name, params ResolverOverride[] overrides)
        {
            return iFoundation.Container.Resolve<T>(name, overrides);
        }
    }
}
