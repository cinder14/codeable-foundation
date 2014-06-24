using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    public interface IPagingData
    {
        bool hasFirst { get; set; }
        bool hasNext { get; set; }
        bool hasPrevious { get; set; }
        bool hasLast { get; set; }
        bool isPaged { get; set; }

        long currentCountofRecords { get; set; }
        long currentStartingRecordNumber { get; set; }
        long currentEndingRecordNumber { get; set; }
        long currentPageSize { get; set; }
        long currentPageNumber { get; set; }

        long totalRecordCount { get; set; }
        long totalPageCount { get; set; }

        long prevUrlStartRecord { get; set; }
        long nextUrlStartRecord { get; set; }

        long lastRecordPageStart { get; set; }
    }
}
