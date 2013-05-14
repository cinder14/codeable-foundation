using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Core.System
{
    public class SingleList<T> : Single<IList<T>>
    {
        static SingleList()
        {
            Single<IList<T>>.Instance = new List<T>();
        }

        public new static IList<T> Instance
        {
            get { return Single<IList<T>>.Instance; }
            set
            {
                Single<IList<T>>.Instance = value;
            }
        }
    }
}
