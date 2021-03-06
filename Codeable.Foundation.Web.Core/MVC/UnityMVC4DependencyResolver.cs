﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Core;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using Codeable.Foundation.Common.System;
using System.Web.Http.Dependencies;

namespace Codeable.Foundation.UI.Web.Core.MVC
{
    public class UnityMVC4DependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            // This example does not support child scopes, so we simply return 'this'.
            return this;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return CoreFoundation.Current.Container.Resolve(serviceType);
            }
            catch (Exception ex)
            {
                Trace.Write(string.Format("UnityDependencyResolver.GetService.Error '{0}' :: {1} ", serviceType.ToString(), ex.Message), Category.Error);
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                Type type = typeof(IEnumerable<>).MakeGenericType(serviceType);
                return (IEnumerable<object>)CoreFoundation.Current.Container.Resolve(type);
            }
            catch (Exception ex)
            {
                Trace.Write(string.Format("UnityDependencyResolver.GetServices.Error '{0}' :: {1} ", serviceType.ToString(), ex.Message), Category.Error);
            }
            return Enumerable.Empty<object>();
        }

        public void Dispose()
        {
            // When BeginScope returns 'this', the Dispose method must be a no-op.
        }
    }
   
}
