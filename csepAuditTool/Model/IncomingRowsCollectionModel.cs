using Microsoft.VisualBasic.FileIO;
using SimpleLogger;

namespace csepAuditTool.Model
{
    internal class IncomingRowsCollectionModel
    {
        public List<IncomingRowModel> IncomingRowsCollection { get; set; }
        public IncomingRowsCollectionModel() { IncomingRowsCollection = new List<IncomingRowModel>(); }
        public IncomingRowsCollectionModel(FtpConnectModel ftpConnection)
        {
            IncomingRowsCollection = new List<IncomingRowModel>();

            var resultsModel = new IncomingLocateRequestModel();
            var columnTemplate = resultsModel.SingleRowModel();
            var columnFieldsLengths = resultsModel.SingleRowFieldLengths();
            var columnFieldsCount = resultsModel.SingleRowFieldCount();

            using (var reader = new TextFieldParser(ftpConnection.Sftp_LocalFullPath))
            {
                reader.TextFieldType = FieldType.FixedWidth;

                reader.SetFieldWidths(columnFieldsLengths);

                var currentRow = new string[columnFieldsCount];

                var iRow = 0;
                while (!reader.EndOfData)
                {
                    var rowModel = new IncomingRowModel() { RowIdx = iRow, RowCols = new List<ColumnsModel>() };
                    var thisColumns = new List<ColumnsModel>();

                    currentRow = reader.ReadFields();

                    if (currentRow == null) continue;

                    var iCol = 0;

                    foreach (var field in currentRow)
                    {
                        var colItem = columnTemplate.FirstOrDefault(p => p.ColIdx == iCol);
                        if (colItem == null) continue;
                        thisColumns.Add(new ColumnsModel(iCol, colItem.ColLen, colItem.ColNam) { ColVal = field.ToString() });
                        iCol++;
                    }
                    rowModel.RowCols = thisColumns;
                    IncomingRowsCollection.Add(rowModel);
                    iRow++;
                }
                if (iRow > 0) SimpleLog.Info(String.Format("Found {0} locator request records incoming to CCB. (Lookup Values). (IncomingRowsCollectionModel())", iRow));
            }
        }
    }
}
