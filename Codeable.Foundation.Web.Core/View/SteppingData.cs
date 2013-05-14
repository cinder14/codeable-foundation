using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    [Serializable]
    public class SteppingData : ISteppingData
    {
        public virtual long currentStartingRecordNumber { get; set; }
        public virtual long currentEndingRecordNumber { get; set; }
        public virtual long nextStartingRecordNumber { get; set; }
        public virtual long currentCountofRecords { get; set; }

        public virtual bool hasNextStep { get; set; }
        public virtual long currentStepSize { get; set; }
        


        public static ISteppingData Build(long currentStartingRecord, long showingCount, long stepSize, bool hasMore)
        {
            return Build(new SteppingData(), currentStartingRecord, showingCount, stepSize, hasMore);
        }
        public static ISteppingData Build(ISteppingData steppingData, long currentStartingRecord, long showingCount, long stepSize, bool hasMore)
        {
            ISteppingData result = steppingData;

            if (currentStartingRecord < 1) { currentStartingRecord = 1; }
            result.currentCountofRecords = showingCount;
            result.currentEndingRecordNumber = currentStartingRecord + showingCount - 1;
            if (showingCount == 0)
            {
                result.currentStartingRecordNumber = 0;
            }
            else
            {
                result.currentStartingRecordNumber = currentStartingRecord;
            }
            result.currentStepSize = stepSize;
            result.hasNextStep = hasMore;
            if (hasMore)
            {
                result.nextStartingRecordNumber = result.currentEndingRecordNumber + 1; 
            }

            return result;
        }
    }
}
