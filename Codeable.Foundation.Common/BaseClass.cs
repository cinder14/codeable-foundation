using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.System;
using Microsoft.Practices.Unity;

namespace Codeable.Foundation.Common
{
    public abstract partial class BaseClass
    {
        /// <summary>
        /// Dependencies will attempt to be resolved by using the provided IFoundation
        /// </summary>
        public BaseClass(IFoundation iFoundation)
        {
            IFoundation = iFoundation;
        }

        protected virtual IFoundation IFoundation { get; set; }
        protected virtual ILogger Logger 
        {
            get
            {
                return IFoundation.Container.Resolve<ILogger>();
            }
        }
        protected virtual ITracer Tracer 
        {
            get
            {
                return IFoundation.Container.Resolve<ITracer>();
            }
        }

    }
}
