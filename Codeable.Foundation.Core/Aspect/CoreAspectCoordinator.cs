﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.Common;
using Codeable.Foundation.Common.System;
using Codeable.Foundation.Common.Plugins;
using System.Diagnostics;
using Codeable.Foundation.Core.System;
using System.Security;

namespace Codeable.Foundation.Core.Aspect
{
    public partial class CoreAspectCoordinator : IAspectCoordinator
    {
        [DebuggerNonUserCode]
        public CoreAspectCoordinator(IFoundation iFoundation)
        {
            this.Foundation = iFoundation;
        }

        protected virtual IFoundation Foundation { get; set;}

        [DebuggerNonUserCode]
        public virtual void WrapMethodCall(object invoker, string methodName, object[] parameters, bool forceThrow, IHandleExceptionProvider exceptionProvider, Action action)
        {
            ILogger iLogger = this.Foundation.GetLogger();
            iLogger.Write("Executing Method: " + methodName, Category.Trace);
            using (this.Foundation.Container.Resolve<ITracer>(ChokeLocation.Method).StartTrace(methodName))
            {
                try
                {
                    IAspectCoordinator iAspectCoordinator = this.Foundation.GetAspectCoordinator();
                    ChokePointResult chokeResult = iAspectCoordinator.EnterChokePoint(invoker, new MethodChokeArgs(parameters, methodName));
                    if (chokeResult.Choke)
                    {
                        iLogger.Write("Result Choked. Reason Given: " + chokeResult.DisplayReason, Category.Trace);
                        return;
                    }
                    action();
                    iAspectCoordinator.ExitChokePoint(invoker, new MethodChokeArgs(parameters, methodName));
                }
                catch (Exception ex)
                {
                    LogError(iLogger, ex);
                    bool rethrow = false;
                    Exception replacedException = null;
                    bool expectsRethrow = false;
                    IHandleException exceptionHandler = exceptionProvider.CreateHandler();
                    if (exceptionHandler != null)
                    {
                        if (exceptionHandler.HandleException(ex, out expectsRethrow, out replacedException))
                        {
                            rethrow = expectsRethrow;
                            if (replacedException != null)
                            {
                                throw replacedException;
                            }
                        }
                    }

                    if (forceThrow || rethrow)
                    {
                        throw;
                    }
                }
            }
        }
        [DebuggerNonUserCode]
        public virtual T WrapFunctionCall<T>(object invoker, string methodName, object[] parameters, bool forceThrow, IHandleExceptionProvider exceptionProvider, Func<T> function)
        {
            ILogger iLogger = this.Foundation.GetLogger();
            iLogger.Write("Executing Function: " + methodName, Category.Trace);
            using (this.Foundation.Container.Resolve<ITracer>(ChokeLocation.Method).StartTrace(methodName))
            {
                try
                {
                    IAspectCoordinator iAspectCoordinator = this.Foundation.GetAspectCoordinator();
                    ChokePointResult<T> chokeResult = iAspectCoordinator.EnterChokePoint<T>(invoker, new MethodChokeArgs(parameters, methodName));
                    if (chokeResult.Choke)
                    {
                        iLogger.Write("Result Choked. Reason Given: " + chokeResult.DisplayReason, Category.Trace);
                        if (chokeResult.ForceResult)
                        {
                            return chokeResult.NewResult;
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                    
                    T result = function();
                    chokeResult = iAspectCoordinator.ExitChokePoint<T>(invoker, new MethodChokeArgs(parameters, methodName, result));
                    if (chokeResult.ForceResult)
                    {
                        iLogger.Write("Result Choked. Reason Given: " + chokeResult.DisplayReason, Category.Trace);
                        return chokeResult.NewResult;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    LogError(iLogger, ex);
                    bool rethrow = false;
                    Exception replacedException = null;
                    bool expectsRethrow = false;
                    IHandleException exceptionHandler = exceptionProvider.CreateHandler();
                    if (exceptionHandler != null)
                    {
                        if (exceptionHandler.HandleException(ex, out expectsRethrow, out replacedException))
                        {
                            rethrow = expectsRethrow;
                            if (replacedException != null)
                            {
                                throw replacedException;
                            }
                        }
                    }

                    if (forceThrow || rethrow)
                    {
                        throw;
                    }
                    return default(T);
                }
            }
        }

        [DebuggerNonUserCode]
        public virtual ChokePointResult EnterChokePoint(object invoker, EventArgs args)
        {
            return EnterChokePoint<object>(invoker, args);
        }
        [DebuggerNonUserCode]
        public virtual ChokePointResult<TReturn> EnterChokePoint<TReturn>(object invoker, EventArgs args)
        {
            try
            {
                IPluginManager iPluginManager = this.Foundation.GetPluginManager();
                if (iPluginManager != null)
                {
                    if (iPluginManager.ChokingPlugins.Count > 0)
                    {
                        ILogger iLogger = this.Foundation.GetLogger();
                        ChokePointResult<TReturn> result = null;
                        foreach (IFoundationPlugin item in iPluginManager.ChokingPlugins)
                        {
                            try
                            {
                                iLogger.Write(string.Format("Processing EnterChokePoint on: '{0}-{1}' ", item.DisplayName, item.DisplayVersion), Category.Trace);
                                ChokePointResult<TReturn> chokeResult = null;
                                if (item.ProcessChokeEnter<TReturn>(invoker, args, result, out chokeResult))
                                {
                                    result = chokeResult;
                                    if (result.PreventChokePropogation)
                                    {
                                        iLogger.Write(string.Format("PreventPropogation Detected on: '{0}-{1}' ", item.DisplayName, item.DisplayVersion), Category.Trace);
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                iLogger.Write(string.Format("Choking Plugin: '{0}-{1}' encountered an error while choking: {2}", item.DisplayName, item.DisplayVersion, ex.Message), Category.Warning);
                            }
                        }
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex) // don't throw exceptions, we're just a choker
            {
                try
                {
                    ILogger iLogger = this.Foundation.GetLogger();
                    iLogger.Write(string.Format("CoreAspectCoordinator.EnterChokePoint: Severe Exception: {0}", ex.Message), Category.Error);
                }
                catch (Exception iex)
                {
                    // Ouch.
                    Trace.Write("CoreAspectCoordinator.EnterChokePoint:  Extremely Severe Exception: " + iex.Message, Category.Error);
                }
            }
            return Codeable.Foundation.Core.System.Single.GetDefault<ChokePointResult<TReturn>>();
        }

        [DebuggerNonUserCode]
        public virtual ChokePointResult ExitChokePoint(object invoker, EventArgs args)
        {
            return ExitChokePoint<object>(invoker, args);
        }
        [DebuggerNonUserCode]
        public virtual ChokePointResult<TReturn> ExitChokePoint<TReturn>(object invoker, EventArgs args)
        {
            try
            {
                IPluginManager iPluginManager = this.Foundation.GetPluginManager();
                if (iPluginManager != null)
                {
                    if (iPluginManager.ChokingPlugins.Count > 0)
                    {
                        ILogger iLogger = this.Foundation.GetLogger();
                        ChokePointResult<TReturn> result = null;
                        foreach (IFoundationPlugin item in iPluginManager.ChokingPlugins)
                        {
                            try
                            {
                                iLogger.Write(string.Format("Processing ExitChokePoint on: '{0}-{1}' ", item.DisplayName, item.DisplayVersion), Category.Trace);
                                ChokePointResult<TReturn> chokeResult = null;
                                if (item.ProcessChokeExit<TReturn>(invoker, args, result, out chokeResult))
                                {
                                    result = chokeResult;
                                    if (result.PreventChokePropogation)
                                    {
                                        iLogger.Write(string.Format("PreventPropogation Detected on: '{0}-{1}' ", item.DisplayName, item.DisplayVersion), Category.Trace);
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                iLogger.Write(string.Format("Choking Plugin: '{0}-{1}' encountered an error while choking: {2}", item.DisplayName, item.DisplayVersion, ex.Message), Category.Warning);
                            }
                        }
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex) // don't throw exceptions, we're just a choker
            {
                try
                {
                    ILogger iLogger = this.Foundation.GetLogger();
                    iLogger.Write(string.Format("CoreAspectCoordinator.EnterChokePoint: Severe Exception: {0}", ex.Message), Category.Error);
                }
                catch (Exception iex)
                {
                    // Ouch.
                    Trace.Write("CoreAspectCoordinator.EnterChokePoint:  Extremely Severe Exception: " + iex.Message, Category.Error);
                }
            }

            return Codeable.Foundation.Core.System.Single.GetDefault<ChokePointResult<TReturn>>();
        }

        protected virtual void LogError(ILogger iLogger, Exception ex)
        {
            try
            {
                string message = CoreUtility.FormatException(ex);
                iLogger.Write(message, Category.Error);
            }
            catch
            {
                // Gulp
            }
        }
    }
}
