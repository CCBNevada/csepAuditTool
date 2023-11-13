using SimpleLogger;

namespace csepAuditTool.Model
{
    internal class UploadLocateRequestModel
    {
        public OutgoingRowsCollectionModel OutgoingRows { get; set; }

        public List<string> UploadContents { get; set; }
        public bool FileUploaded { get; set; }
        public bool NoStringListMatchesCreated { get; set; }
        public UploadLocateRequestModel()
        {
            OutgoingRows = new OutgoingRowsCollectionModel();
            UploadContents = new List<string>();
        }
        public UploadLocateRequestModel(OutgoingRowsCollectionModel outgoingRows, FtpConnectModel ftpConn)
        {
            OutgoingRows = outgoingRows;

            UploadContents = BuildUploadContents(outgoingRows);

            NoStringListMatchesCreated = UploadContents.Count == 0;

            //should have values, skipped if no count from calling method
            if (NoStringListMatchesCreated) return;

            FileUploaded = ftpConn.SaveUploadFile(UploadContents);

            if (FileUploaded) SimpleLog.Info("Locator Response File Successfully Uploaded to SFTP Server. (UploadLocateRequestModel())");
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
                    var colVal = thisColumn.ColVal.Trim();
                    if (colNam.ToUpper().Contains("DATE") && DateTime.TryParse(colVal, out DateTime dt))
                    {
                        colVal = OutgoingRowsCollectionModel.DateToStringMMddyyyy(dt);
                    }
                    if (colNam == "Agent_Height")
                    {
                        //var nam = OutgoingRows.OutgoingRowsCollection[i].RowCols.FirstOrDefault(p => p.ColNam.Contains("Agent_First_Name"));
                        //if (nam != null && nam.ColVal == "Daisy")
                        //{
                        //    var t = "";
                        //}
                        colVal = OutgoingRowsCollectionModel.BuildAgentHeightString(colVal);
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
            if (uploadFileContents.Count > 0)
                SimpleLog.Info(String.Format("{0} Matches Found, Successfully Prepared File Content For Upload. (UploadLocateRequestModel.BuildUploadContents())", uploadFileContents.Count));
            return uploadFileContents;
        }
    }
}
