using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Core
{
    public static class DerivedPropertyExtensions
    {
        public static T GetValueOrDefault<T>(this DerivedProperty<T> derivedProperty)
        {
            if ((derivedProperty != null) && (derivedProperty.Value != null))
            {
                return derivedProperty.Value;
            }
            return default(T);
        }
        public static bool HasValue<T>(this DerivedProperty<T> derivedProperty)
        {
            if (derivedProperty != null)
            {
                if (derivedProperty.Value != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
