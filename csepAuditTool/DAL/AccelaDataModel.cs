using csepAuditTool.Model;
using SimpleLogger;
using System.Configuration;
using System.Data.Odbc;

namespace csepAuditTool.DAL
{
    internal class AccelaDataModel
    {
        private OdbcConnection? _odbcConnection { get; set; }
        public void BuildOdbcConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["accelaSqlConnectionString"].ConnectionString;
            _odbcConnection = new OdbcConnection(connStr);
            _odbcConnection.Open();
        }
        public AccelaDataModel() { }

        public List<OutgoingRowModel> GetMatches(IncomingRowModel incomingRowValues, OutgoingLocateRequestModel outgoingLocateRequest, int lineNumber)
        {
            lineNumber += 1;
            if (_odbcConnection == null) BuildOdbcConnection();

            //var mySingleRow = outgoingLocateRequest.SingleRowModel();

            var matchedResults = new List<OutgoingRowModel>();

            try
            {
                var query1Text = DataQueryModel.Query1;
                //DO NOT ALTER QUERY RETURNING FIELDS, COLUMN NAMES USED FOR MATCH TO OBJECT
                var search1Command = _odbcConnection.CreateCommand();
                search1Command.CommandText = query1Text;
                search1Command.Parameters.Add("@B1_SOCIAL_SECURITY_NUMBER", OdbcType.VarChar).Value = incomingRowValues.SsnWithDashes;
                search1Command.Parameters.Add("@B1_FNAME", OdbcType.VarChar).Value = incomingRowValues.FirstNameFirst4;
                var dr1 = search1Command.ExecuteReader();
                if (dr1 != null && dr1.HasRows)
                {
                    //var count = dr1.FieldCount;
                    while (dr1.Read())
                    {
                        var outgoingRow = new OutgoingRowModel();
                        for (var i = 0; i < dr1.FieldCount; i++)
                        {
                            var thisColumn = outgoingLocateRequest.SingleRowModel().FirstOrDefault(p => p.ColNam == dr1.GetName(i));
                            if (thisColumn == null) continue;
                            var newColumn = new ColumnsModel(i, thisColumn.ColLen, thisColumn.ColNam, dr1.GetValue(i).ToString() ?? "");
                            outgoingRow.RowCols.Add(newColumn);
                        }
                        matchedResults.Add(outgoingRow);
                    }
                    dr1.Close();
                }
                search1Command.Dispose();
                if (matchedResults.Count > 0)
                {
                    SimpleLog.Info(String.Format("{0} Matches Found Query1. criteria:{1}. (File Line:{2})", matchedResults.Count, DataQueryModel.Query1Fields, lineNumber));
                    return matchedResults;
                }
            }
            catch (Exception ex)
            {
                var errTxt = String.Format("File Line:{0} Error in Query 1. Processes Stopped. Values [SsnWithDashes:{1},FirstNameFirst4:{2}]", lineNumber, incomingRowValues.SsnWithDashes, incomingRowValues.FirstNameFirst4);
                SimpleLog.Error(errTxt);
                SimpleLog.Error(ex.Message);
                throw new Exception(ex.Message);
            }

            try
            {
                if (incomingRowValues.BirthDate != null)
                {
                    var query2Text = DataQueryModel.Query2;
                    //DO NOT ALTER QUERY RETURNING FIELDS, COLUMN NAMES USED FOR MATCH TO OBJECT
                    var search2Command = _odbcConnection.CreateCommand();
                    search2Command.CommandText = query2Text;
                    search2Command.Parameters.Add("@B1_LNAME", OdbcType.VarChar).Value = incomingRowValues.LastName;
                    search2Command.Parameters.Add("@B1_MNAME", OdbcType.VarChar).Value = incomingRowValues.MiddleName;
                    search2Command.Parameters.Add("@B1_MNAME", OdbcType.VarChar).Value = incomingRowValues.MiddleName;
                    search2Command.Parameters.Add("@B1_FNAME", OdbcType.VarChar).Value = incomingRowValues.FirstName;
                    search2Command.Parameters.Add("@B1_BIRTH_DATE", OdbcType.DateTime).Value = incomingRowValues.BirthDate;
                    var dr2 = search2Command.ExecuteReader();
                    if (dr2 != null && dr2.HasRows)
                    {
                        while (dr2.Read())
                        {
                            var outgoingRow = new OutgoingRowModel();
                            for (var i = 0; i < dr2.FieldCount; i++)
                            {
                                var thisColumn = outgoingLocateRequest.SingleRowModel().FirstOrDefault(p => p.ColNam == dr2.GetName(i));
                                if (thisColumn == null) continue;
                                var newColumn = new ColumnsModel(i, thisColumn.ColLen, thisColumn.ColNam, dr2.GetValue(i).ToString() ?? "");
                                outgoingRow.RowCols.Add(newColumn);
                            }
                            matchedResults.Add(outgoingRow);
                        }
                    }
                    search2Command.Dispose();
                    if (matchedResults.Count > 0)
                    {
                        SimpleLog.Info(String.Format("{0} Matches Found Query2. criteria:{1}. (File Line:{2})", matchedResults.Count, DataQueryModel.Query2Fields, lineNumber));
                        return matchedResults;
                    }
                }
            }
            catch (Exception ex)
            {
                var errTxt = String.Format("File Line:{0} Error in Query 2. Processes Stopped: Values [LastName:{1},MiddleName:{2},FirstName:{3},BirthDate:{4}]", lineNumber, incomingRowValues.LastName, incomingRowValues.MiddleName, incomingRowValues.FirstName, incomingRowValues.BirthDate);
                SimpleLog.Error(errTxt);
                SimpleLog.Error(ex.Message);
                throw new Exception(ex.Message);
            }

            try
            {
                if (incomingRowValues.BirthDate != null)
                {
                    var query3TextNoMiddleName = DataQueryModel.Query3;
                    //DO NOT ALTER QUERY RETURNING FIELDS, COLUMN NAMES USED FOR MATCH TO OBJECT
                    var search3CommandNoMiddleName = _odbcConnection.CreateCommand();
                    search3CommandNoMiddleName.CommandText = query3TextNoMiddleName;
                    search3CommandNoMiddleName.Parameters.Add("@B1_LNAME", OdbcType.VarChar).Value = incomingRowValues.LastName;
                    search3CommandNoMiddleName.Parameters.Add("@B1_FNAME", OdbcType.VarChar).Value = incomingRowValues.FirstName;
                    search3CommandNoMiddleName.Parameters.Add("@B1_BIRTH_DATE", OdbcType.DateTime).Value = incomingRowValues.BirthDate;
                    var dr3 = search3CommandNoMiddleName.ExecuteReader();
                    if (dr3 != null && dr3.HasRows)
                    {
                        while (dr3.Read())
                        {
                            var outgoingRow = new OutgoingRowModel();
                            for (var i = 0; i < dr3.FieldCount; i++)
                            {
                                var thisColumn = outgoingLocateRequest.SingleRowModel().FirstOrDefault(p => p.ColNam == dr3.GetName(i));
                                if (thisColumn == null) continue;
                                var newColumn = new ColumnsModel(i, thisColumn.ColLen, thisColumn.ColNam, dr3.GetValue(i).ToString() ?? "");
                                outgoingRow.RowCols.Add(newColumn);
                            }
                            matchedResults.Add(outgoingRow);
                        }
                    }
                    search3CommandNoMiddleName.Dispose();
                }
            }
            catch (Exception ex)
            {
                var errTxt = String.Format("File Line:{0} Error in Query 3. Processes Stopped: Values [LastName:{1},FirstName:{2},BirthDate:{3}]", lineNumber, incomingRowValues.LastName, incomingRowValues.FirstName, incomingRowValues.BirthDate);
                SimpleLog.Error(errTxt);
                SimpleLog.Error(ex.Message);
                throw new Exception(ex.Message);
            }

            if (matchedResults.Count > 0) SimpleLog.Info(String.Format("{0} Matches Found Query3. criteria:{1}. (File Line:{2})\"", matchedResults.Count, DataQueryModel.Query3Fields, lineNumber));

            SimpleLog.Info(String.Format("File Line:{0} NO MATCHES FOUND FOR: [SsnLast4:{1},FirstNameFirst4:{2},LastName:{3},MiddleName:{4},FirstName:{5},BirthDate:{6}]",
                lineNumber, incomingRowValues.SsnLast4, incomingRowValues.FirstNameFirst4, incomingRowValues.LastName, incomingRowValues.MiddleName, incomingRowValues.FirstName, incomingRowValues.BirthDateString));
            return matchedResults;
        }

        public void CloseOdbcConnection()
        {
            if (_odbcConnection != null) { _odbcConnection?.Close(); }
        }
    }
}
