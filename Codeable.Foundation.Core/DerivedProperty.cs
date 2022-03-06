using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Core
{
    //TODO:Enhancement: Add IEQuality to the derived property itself

    /// <summary>
    /// This property is derived and is not guaranteed to exist or be accurate.
    /// Note: Best Pratice is to always include an instance, even if the value is null.
    /// </summary>
    /// <remarks>Use caution.</remarks>
    public class DerivedProperty<T>
    {
        public DerivedProperty()
        {

        }
        public DerivedProperty(T value)
        {
            this.Value = value;
        }
        public virtual T Value { get; protected set; }
    }
    
    
}
