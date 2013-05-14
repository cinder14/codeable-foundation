using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    [Serializable]
    public class JsonViewResponse : IJsonViewResponse
    {
        public virtual bool hasError { get; set; }
        public virtual bool hasPaging { get; set; }
        public virtual bool hasStepping { get; set; }

        public virtual IViewError error { get; set; }
        public virtual IPagingData paging { get; set; }
        public virtual ISteppingData stepping { get; set; }

        public virtual void SetError(IViewError viewError)
        {
            this.error = viewError;
            this.hasError = (this.error != null);
        }
        public virtual void SetPaging(IPagingData pagingData)
        {
            this.paging = pagingData;
            this.hasPaging = (this.paging != null);
        }
        public virtual void SetStepping(ISteppingData steppingData)
        {
            this.stepping = steppingData;
            this.hasStepping = (this.stepping != null);
        }

    }
}
