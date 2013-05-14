using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common.System;

namespace Codeable.Foundation.Common.Aspect
{
    public interface IAspectCoordinator
    {
        void WrapMethodCall(object invoker, string methodName, object[] parameters, bool forceThrow, IHandleExceptionProvider exceptionProvider, Action action);
        T WrapFunctionCall<T>(object invoker, string methodName, object[] parameters, bool forceThrow, IHandleExceptionProvider exceptionProvider, Func<T> function);

        ChokePointResult EnterChokePoint(object invoker, EventArgs args);
        ChokePointResult<TReturn> EnterChokePoint<TReturn>(object invoker, EventArgs args);
        //TODO: Should have success bit flag and always execute
        ChokePointResult ExitChokePoint(object invoker, EventArgs args);
        //TODO: Should have success bit flag and always execute
        ChokePointResult<TReturn> ExitChokePoint<TReturn>(object invoker, EventArgs args);
    }
}
