using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web;
using System.Diagnostics;

namespace Codeable.Foundation.UI.Web.Core.Unity
{
    public class ManualLifetimeManager<T> : LifetimeManager
    {
        #region Constructor

        public ManualLifetimeManager(T value)
        {
            _object = value;
        } 

        #endregion

        #region Private Properties

        private T _object;

        #endregion

        #region Public Methods

        public override object GetValue()
        {
            return _object;
        }
        public override void RemoveValue()
        {
            _object = default(T);
        }
        public override void SetValue(object newValue)
        {
            _object = (T)newValue;
        } 

        #endregion

    }
}
