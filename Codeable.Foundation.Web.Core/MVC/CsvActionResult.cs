using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Codeable.Foundation.Web.Core.MVC
{
    public class CsvActionResult : FileResult
    {
        public CsvActionResult(DataTable dataTable)
            : base("text/csv")
        {
            this.DataTable = dataTable;
        }

        protected DataTable DataTable { get; set; }


        protected override void WriteFile(HttpResponseBase response)
        {
            var outputStream = response.OutputStream;
            using (var memoryStream = new MemoryStream())
            {
                WriteDataTable(memoryStream);
                outputStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
        }

        protected virtual void WriteDataTable(Stream stream)
        {
            var streamWriter = new StreamWriter(stream, Encoding.Default);

            WriteHeaderLine(streamWriter);
            streamWriter.WriteLine();
            WriteDataLines(streamWriter);

            streamWriter.Flush();
        }

        protected virtual void WriteHeaderLine(StreamWriter streamWriter)
        {
            foreach (DataColumn dataColumn in this.DataTable.Columns)
            {
                WriteValue(streamWriter, dataColumn.ColumnName);
            }
        }

        protected virtual void WriteDataLines(StreamWriter streamWriter)
        {
            foreach (DataRow dataRow in this.DataTable.Rows)
            {
                foreach (DataColumn dataColumn in this.DataTable.Columns)
                {
                    WriteValue(streamWriter, dataRow[dataColumn.ColumnName].ToString());
                }
                streamWriter.WriteLine();
            }
        }

        protected virtual void WriteValue(StreamWriter writer, string value)
        {
            writer.Write("\"");
            writer.Write(value.Replace("\"", "\"\""));
            writer.Write("\",");
        }
    }
}
