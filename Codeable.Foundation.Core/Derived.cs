using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Core
{
    /// <summary>
    /// This supplies static instances of Derived properties. (for performance)
    /// </summary>
    public static class Derived
    {
        public static DerivedProperty<bool> TrueOrFalse(bool value)
        {
            if (value)
            {
                return Derived.True;
            }
            else
            {
                return Derived.False;
            }
        }

        public static DerivedProperty<T> NullFor<T>()
        {
            if (Codeable.Foundation.Core.System.Single<DerivedProperty<T>>.Instance == null)
            {
                Codeable.Foundation.Core.System.Single<DerivedProperty<T>>.Instance = new DerivedProperty<T>(default(T));
            }
            return Codeable.Foundation.Core.System.Single<DerivedProperty<T>>.Instance;
        }

        public static readonly DerivedProperty<long> ZeroLong = new DerivedProperty<long>(0);
        public static readonly DerivedProperty<int> ZeroInt = new DerivedProperty<int>(0);
        public static readonly DerivedProperty<bool> False = new DerivedProperty<bool>(false);
        public static readonly DerivedProperty<bool> True = new DerivedProperty<bool>(true);
        public static readonly DerivedProperty<Guid> GuidEmpty = new DerivedProperty<Guid>(Guid.Empty);
        public static readonly DerivedProperty<Guid?> GuidNull = new DerivedProperty<Guid?>(null);
        public static readonly DerivedProperty<string> StringEmpty = new DerivedProperty<string>(string.Empty);
    }
}
