using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Codeable.Foundation.Common;
using Codeable.Foundation.Core;
using System.Web;
using Microsoft.Practices.Unity;
using System.IO;
using Codeable.Foundation.Web.Core.MVC;

namespace Codeable.Foundation.UI.Web.Core.MVC.Razor
{
    // Not Fully chokeable, too concrete
    public abstract class FoundationWebViewPage<TModel> : WebViewPage<TModel>
    {
        private IViewPageInterceptor _iViewPageInterceptor;
        private IViewPageCDN _iViewPageCDN;


        public string CDN(string path)
        {
            if (_iViewPageCDN != null && !string.IsNullOrEmpty(path))
            {
                return _iViewPageCDN.GetCDNUrl(path);
            }
            return path;
        }
        public override void InitHelpers()
        {
            base.InitHelpers();

            _iViewPageInterceptor = CoreFoundation.Current.SafeResolve<IViewPageInterceptor>();
            _iViewPageCDN = CoreFoundation.Current.SafeResolve<IViewPageCDN>();
        }
        public override void ExecutePageHierarchy()
        {
            if ((_iViewPageInterceptor != null) && _iViewPageInterceptor.ShouldFilter(ViewContext))
            {
                TextWriter origWriter = this.OutputStack.Pop(); //TODO: Encoding

                StringWriter filterWriter = new StringWriter();
                this.OutputStack.Push(filterWriter);
                
                base.ExecutePageHierarchy();

                string output = _iViewPageInterceptor.FilterOutput(filterWriter.ToString());
                origWriter.Write(output);

                this.OutputStack.Pop();
                this.OutputStack.Push(origWriter);
            }
            else
            {
                base.ExecutePageHierarchy();
            }
        }

        public override void Execute()
        {
        }

    }

    public abstract class FoundationWebViewPage : FoundationWebViewPage<dynamic>
    {
    }
}
