using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common;
using Codeable.Foundation.Core;
using Codeable.Foundation.UI.Web.Core.Context;
using Microsoft.Practices.Unity;
using Codeable.Foundation.UI.Web.Core.Http;
using System.Web;
using Codeable.Foundation.UI.Web.Core.Unity;
using Codeable.Foundation.UI.Web.Common.Authentication;
using Codeable.Foundation.UI.Web.Core.Authentication;
using Codeable.Foundation.UI.Web.Core.MVC;
using Codeable.Foundation.Web.Core.MVC;

namespace Codeable.Foundation.UI.Web.Core.Foundation
{
    public class WebBootStrap : CoreBootStrap
    {
        public WebBootStrap()
        {
        }

        public override void OnAfterBootStrapComplete(IFoundation foundation)
        {
            base.OnAfterBootStrapComplete(foundation);
        }

        public override void OnAfterSelfRegisters(IFoundation foundation)
        {
            base.OnAfterSelfRegisters(foundation);

            foundation.Container.RegisterType<IUserProvider, MembershipUserProvider>(new HttpRequestLifetimeManager());
            foundation.Container.RegisterType<IViewPageInterceptor, EmptyViewPageInterceptor>(new HttpRequestLifetimeManager());

            foundation.Container.RegisterType<IViewPageCDN, EmptyViewPageCDN>(new HttpRequestLifetimeManager());

        }
    }
}
