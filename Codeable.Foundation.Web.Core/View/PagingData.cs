using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.View
{
    [Serializable]
    public class PagingData : IPagingData
    {
        public virtual bool hasFirst { get; set; }
        public virtual bool hasNext { get; set; }
        public virtual bool hasPrevious { get; set; }
        public virtual bool hasLast { get; set; }
        public virtual bool isPaged { get; set; }

        public virtual long currentCountofRecords { get; set; }
        public virtual long currentStartingRecordNumber { get; set; }
        public virtual long currentEndingRecordNumber { get; set; }
        public virtual long currentPageSize { get; set; }

        public virtual long totalRecordCount { get; set; }
        public virtual long totalPageCount { get; set; }

        public virtual long prevUrlStartRecord { get; set; }
        public virtual long nextUrlStartRecord { get; set; }

        public virtual long currentPageNumber { get; set; }

        public virtual long lastRecordPageStart { get; set; }

        public static IPagingData Build(long startRecord, long showingCount, long totalCount, long pageSize)
        {
            return Build(new PagingData(), startRecord, showingCount, totalCount, pageSize);
        }
        public static IPagingData Build(IPagingData pagingData, long startRecord, long showingCount, long totalCount, long pageSize)
        {
            IPagingData result = pagingData;

            if(startRecord < 1) { startRecord = 1; }
            result.currentCountofRecords = showingCount;
            result.currentEndingRecordNumber = startRecord + showingCount - 1;
            if (showingCount == 0)
            {
                result.currentStartingRecordNumber = 0;
            }
            else
            {
                result.currentStartingRecordNumber = startRecord;
            }
            result.currentPageSize = pageSize;
            result.totalRecordCount = totalCount;
            if(result.currentPageSize > 0)
            {
                result.isPaged = true;
                result.totalPageCount = (long)Math.Ceiling((double)totalCount / (double)result.currentPageSize);
                if(result.currentStartingRecordNumber > 0)
                {
                    result.currentPageNumber = (long)Math.Ceiling((double)result.currentStartingRecordNumber / (double)result.currentPageSize);
                }
                else
                {
                    result.currentPageNumber = 0;
                }
            }
            else
            {
                result.isPaged = false;
                result.totalPageCount = 0;
                result.currentPageNumber = 0;
            }

            if (result.totalRecordCount > 0)
            {
                if ((pageSize == 0) || (result.totalPageCount <= 3))
                {
                    result.hasFirst = false;
                    result.hasLast = false;
                }
                else
                {
                    result.hasFirst = (result.currentPageNumber > 2);
                    result.hasLast = ((result.totalPageCount - result.currentPageNumber) > 1);
                }
                
                result.hasNext = (result.currentEndingRecordNumber < result.totalRecordCount);
                result.hasPrevious = (result.currentStartingRecordNumber > 1);

                if (result.hasPrevious)
                {
                    if (pageSize == 0)
                    {
                        result.prevUrlStartRecord = result.currentStartingRecordNumber - 1;
                    }
                    else
                    {
                        result.prevUrlStartRecord = result.currentStartingRecordNumber - result.currentPageSize;
                    }
                    
                    if (result.prevUrlStartRecord < 1)
                    {
                        result.prevUrlStartRecord = 1;
                    }
                }

                if (result.hasNext)
                {
                    result.nextUrlStartRecord = result.currentEndingRecordNumber + 1;
                }
                if (result.hasLast)
                {
                    result.lastRecordPageStart = (result.currentPageSize * (result.totalPageCount - 1)) + 1;
                }
            }
            else
            {
                result.hasFirst = false;
                result.hasLast = false;
                result.hasNext = false;
                result.hasPrevious = false;
            }
            return result;
        }

    }
}
