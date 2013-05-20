using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Codeable.Foundation.Common;
using Codeable.Foundation.Core;
using Codeable.Foundation.Common.System;

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
        public static void LogError<T>(this IFoundation iFoundation, Exception ex, string tag = "")
        {
            iFoundation.GetLogger().Write(CoreUtility.FormatException(ex, tag), Category.Error);
        }
        public static void LogError<T>(this IFoundation iFoundation, string message, string tag = "")
        {
            iFoundation.GetLogger().Write(message, Category.Error);
        }
        public static void LogWarning<T>(this IFoundation iFoundation, string warning)
        {
            iFoundation.GetLogger().Write(warning, Category.Warning);
        }
    }
}
