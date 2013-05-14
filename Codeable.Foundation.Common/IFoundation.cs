using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.Common.Plugins;
using Codeable.Foundation.Common.System;
using Codeable.Foundation.Common.Daemons;

namespace Codeable.Foundation.Common
{
    public interface IFoundation
    {
        void Start(IBootStrap bootStrap);
        void Stop();
        IUnityContainer Container { get; }
        ILogger GetLogger();
        ITracer GetTracer();
        IAspectCoordinator GetAspectCoordinator();
        IPluginManager GetPluginManager();
        IDaemonManager GetDaemonManager();

        T SafeResolve<T>();
        T SafeResolve<T>(string name);

        void RegisterTypeWithRollback<TInterface, TNewConcrete>(string token, bool allowSelfChained, LifetimeManager lifetimeManager) where TNewConcrete : TInterface;
        void RollbackRegisterType<TInterface>(string token);
    }
}
