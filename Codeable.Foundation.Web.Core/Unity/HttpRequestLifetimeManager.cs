using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web;
using System.Diagnostics;

namespace Codeable.Foundation.UI.Web.Core.Unity
{
    public class HttpRequestLifetimeManager : LifetimeManager
    {
        private string _key = Guid.NewGuid().ToString();

        public override object GetValue()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains(_key))
                {
                    return HttpContext.Current.Items[_key];
                }

            }
            return null;
        }

        public override void RemoveValue()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items.Remove(_key);
            }
        }

        public override void SetValue(object newValue)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[_key] = newValue;
            }

        }

    }
}
