using csepAuditTool.DAL;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csepAuditTool.Model
{
    internal class OutgoingRowsCollectionModel
    {
        public List<OutgoingRowModel> OutgoingRowsCollection { get; set; }
        public OutgoingRowsCollectionModel() { OutgoingRowsCollection = new List<OutgoingRowModel>(); }
        public OutgoingRowsCollectionModel(IncomingRowsCollectionModel incomingRows)
        {
            OutgoingRowsCollection = new List<OutgoingRowModel>();

            var resultsModel = new OutgoingLocateRequestModel();
            var columnTemplate = resultsModel.SingleRowModel();
            var columnFieldsLengths = resultsModel.SingleRowFieldLengths();
            var columnFieldsCount = resultsModel.SingleRowFieldCount();

            var accelaDataModel = new AccelaDataModel();
            accelaDataModel.BuildOdbcConnection();

            for (var i = 0; i < incomingRows.IncomingRowsCollection.Count(); i++)
            {
                var incomingRowValues = incomingRows.IncomingRowsCollection[i];

                var matchedResult = accelaDataModel.GetMatches(incomingRowValues, resultsModel);
                if (matchedResult == null) continue;

                for (var j = 0; j < matchedResult.Count(); j++)
                {
                    OutgoingRowsCollection.Add(matchedResult[j]);
                }

            }

            accelaDataModel.CloseOdbcConnection();
        }

        public string DateToStringMMddyyyy(DateTime dateToConvert)
        {
            return dateToConvert.ToString("MMddyyyy");
        }
    }
}
