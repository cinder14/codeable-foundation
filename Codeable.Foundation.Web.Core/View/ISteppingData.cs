using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    public interface ISteppingData
    {
        long currentStartingRecordNumber { get; set; }
        long currentEndingRecordNumber { get; set; }
        long nextStartingRecordNumber { get; set; }

        long currentCountofRecords { get; set; }
        bool hasNextStep { get; set; }
        long currentStepSize { get; set; }
    }
}
