using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.UI.Web.Core.MVC;
using Microsoft.Practices.Unity;
using Codeable.Foundation.Common.System;
using Codeable.Foundation.UI.Web.Core.MVC.Embedded;
using System.Web;
using Codeable.Foundation.UI.Web.Core.Context;
using Codeable.Foundation.UI.Web.Core.Http;
using Codeable.Foundation.UI.Web.Common.Authentication;
using Codeable.Foundation.UI.Web.Core.Authentication;

namespace Codeable.Foundation.UI.Web.Core.SelfRegisters
{
    public class WebSelfRegisters : IDynamicallySelfRegisterWithUnity
    {
        #region IDynamicallySelfRegisterWithUnity Members

        public void SelfRegister(Microsoft.Practices.Unity.IUnityContainer container)
        {
            container.RegisterType<IWebPluginLoader, WebPluginLoader>(new ContainerControlledLifetimeManager());
            container.RegisterType<IWebPluginEmbeddedItemFinder, WebPluginEmbeddedItemFinder>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHttpContext, HttpContextCurrent>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHttpApplicationBinder, HttpApplicationBinder>(new ContainerControlledLifetimeManager());
            container.RegisterType<IWebAuthorizor, FormsAuthenticationWebAuthorizor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAuthenticateUser, MembershipAuthenticateUser>(new ContainerControlledLifetimeManager());
        }

        #endregion
    }
}
