using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace Codeable.Foundation.Common.System
{
    public interface IDynamicallySelfRegisterWithUnity
    {
        void SelfRegister(IUnityContainer container);
    }
}
