using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeable.Foundation.Core.Unity
{
    public interface IKeyedLifetimeManager
    {
        TData GetKeyedValue<TKey, TData>(TKey key, out bool found);
        void SetKeyedValue<TKey, TData>(TKey key, TData value);
    }
}
