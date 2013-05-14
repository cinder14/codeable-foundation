using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web.Mvc;

namespace Codeable.Foundation.UI.Web.Core.Unity
{
    public class MVCPageLifetimeManager : LifetimeManager
    {
        #region Constructor

        public MVCPageLifetimeManager(WebViewPage viewPage, string datakey)
        {
            _viewPage = viewPage;
            _dataKey = "__MVCPageLifetimeManager_" + datakey;
        } 

        #endregion

        #region Private Properties

        private string _dataKey;
        private WebViewPage _viewPage;

        #endregion

        #region Public Methods

        public override object GetValue()
        {
            return _viewPage.Context.Items[_dataKey];;
        }
        public override void RemoveValue()
        {
            _viewPage.Context.Items.Remove(_dataKey);
        }
        public override void SetValue(object newValue)
        {
            _viewPage.Context.Items[_dataKey] = newValue;
        }

        #endregion
    }
}
