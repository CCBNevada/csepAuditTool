using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace csepAuditTool.Model
{
    internal class UploadLocateRequestModel
    {
        public OutgoingRowsCollectionModel OutgoingRows { get; set; }

        public List<string> UploadContents { get; set; }
        public bool FileUploaded { get; set; }
        public UploadLocateRequestModel()
        {
            OutgoingRows = new OutgoingRowsCollectionModel();
            UploadContents = new List<string>();
        }
        public UploadLocateRequestModel(OutgoingRowsCollectionModel outgoingRows, FtpConnectModel ftpConn)
        {
            OutgoingRows = outgoingRows;

            UploadContents = BuildUploadContents(outgoingRows);

            var fileSaved = ftpConn.SaveUploadFile(UploadContents);

            if (fileSaved) FileUploaded = ftpConn.UploadFile();
        }

        public List<string> BuildUploadContents(OutgoingRowsCollectionModel outgoingRows)
        {
            var uploadFileContents = new List<string>();

            for (var i = 0; i < outgoingRows.OutgoingRowsCollection.Count; i++)
            {
                var newLine = "";
                for (var j = 0; j < outgoingRows.OutgoingRowsCollection[i].RowCols.Count; j++)
                {
                    var thisColumn = outgoingRows.OutgoingRowsCollection[i].RowCols[j];
                    var colIdx = thisColumn.ColIdx;
                    var colLen = thisColumn.ColLen;
                    var colNam = thisColumn.ColNam;
                    var colVal = thisColumn.ColVal;
                    if ((colNam.Contains("Date") || colNam.Contains("date")) && DateTime.TryParse(colVal, out DateTime dt))
                    {
                        colVal = outgoingRows.DateToStringMMddyyyy(dt);
                    }
                    if (colVal.Length > 0 && colVal.Length > colLen)
                    {
                        colVal = colVal.Substring(0, colLen);
                    }
                    newLine += colVal.PadRight(colLen);
                }
                if (newLine.Trim() == "") continue;
                uploadFileContents.Add(newLine);
            }
            return uploadFileContents;
        }
    }
}
