using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Codeable.Foundation.Core;
using Codeable.Foundation.Common;
using Codeable.Foundation.Common.System;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Codeable.Foundation.UI.Web.Core.MVC
{
    [DebuggerStepThrough]
    public class CoreApiController : ApiController
    {
        public CoreApiController()
            : this(CoreFoundation.Current)
        {
        }
        public CoreApiController(IFoundation iFoundation)
            : this(iFoundation, null)
        {
        }
        public CoreApiController(IFoundation iFoundation, IHandleExceptionProvider iHandleExceptionProvider)
        {
            this.IFoundation = iFoundation;
            this.IHandleExceptionProvider = iHandleExceptionProvider;
            if (this.IHandleExceptionProvider == null)
            {
                this.IHandleExceptionProvider = this.IFoundation.Container.Resolve<IHandleExceptionProvider>();
                this.IHandleExceptionProvider.PolicyName = this.GetType().ToString();
            }

        }

        protected virtual void Construct()
        {
            // virtual constructor
        }
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            Construct();
        }
        protected virtual IFoundation IFoundation { get; set; }
        protected virtual IHandleExceptionProvider IHandleExceptionProvider { get; set; }

        protected virtual T ExecuteFunction<T>(string methodName, Func<T> function, params object[] parameters)
        {
            return IFoundation.GetAspectCoordinator().WrapFunctionCall<T>(this, methodName, parameters, false, this.IHandleExceptionProvider, function);
        }
        protected virtual T ExecuteFunction<T>(string methodName, bool forceThrow, Func<T> function, params object[] parameters)
        {
            return IFoundation.GetAspectCoordinator().WrapFunctionCall<T>(this, methodName, parameters, forceThrow, this.IHandleExceptionProvider, function);
        }
        protected virtual void ExecuteMethod(string methodName, Action action, params object[] parameters)
        {
            this.IFoundation.GetAspectCoordinator().WrapMethodCall(this, methodName, parameters, false, this.IHandleExceptionProvider, action);
        }
        protected virtual void ExecuteMethod(string methodName, bool forceThrow, Action action, params object[] parameters)
        {
            this.IFoundation.GetAspectCoordinator().WrapMethodCall(this, methodName, parameters, forceThrow, this.IHandleExceptionProvider, action);
        }

    }
}
