using csepAuditTool.DAL;
using SimpleLogger;

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
                //var nam = incomingRowValues.LastName;
                //if (nam == "Yoda")
                //{
                //    var t = "";
                //}
                var matchedResult = accelaDataModel.GetMatches(incomingRowValues, resultsModel, i);
                if (matchedResult == null || matchedResult.Count == 0) continue;

                for (var j = 0; j < matchedResult.Count(); j++)
                {
                    OutgoingRowsCollection.Add(matchedResult[j]);
                }
            }
            SimpleLog.Info(String.Format("Found {0} locator response records outgoing from CCB (Possible Matches). (OutgoingRowsCollectionModel())", OutgoingRowsCollection.Count()));
            accelaDataModel.CloseOdbcConnection();
        }

        public static string DateToStringMMddyyyy(DateTime dateToConvert)
        {
            return dateToConvert.ToString("MMddyyyy");
        }

        private static int FirstNonNumericCharacter(string colVal)
        {
            if (colVal.Length < 1) return -1;
            int idxFound = 0;
            while (colVal.Length > idxFound && Char.IsDigit(colVal[idxFound]))
            {
                idxFound++;
            }
            if (idxFound == colVal.Length)
            {
                //nothing after numeric found
                return -1;
            }
            return idxFound;
        }

        private static string GetNumeric(string colVal)
        {
            //extract all numeric values
            return new string(colVal.Where(Char.IsDigit).ToArray());
        }

        private static int FirstNumericCharacter(string colVal)
        {
            return colVal.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' });
        }

        private static bool CheckDigitAt0(string colVal)
        {
            return FirstNumericCharacter(colVal) == 0;
        }

        private static string BuildDigitStringIfPossible(string colVal)
        {
            var valFound = false;
            var tempReturn = "";
            colVal = colVal.ToUpper();
            var footList = new List<string> { "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };
            var footVal = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            var inchList = new List<string> { "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE" };
            var inchVal = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            //var footMatch = footList.FirstOrDefault(p => p == colVal.ToUpper());
            for (var i = 0; i < footList.Count; i++)
            {
                if (colVal.Contains(footList[i]) && colVal.IndexOf(footList[i]) == 0)
                {
                    valFound = true;
                    tempReturn += footVal[i] + '\'';
                    colVal = colVal.Length > i ? colVal.Substring(i + 1) : "";
                    break;
                }
            }
            if (valFound)
            {
                for (var i = 0; i < inchList.Count; i++)
                {
                    if (colVal.Contains(inchList[i]))
                    {
                        tempReturn += inchVal[i] + '"';
                        break;
                    }
                }
                return tempReturn;
            }
            return "";
        }

        public static string BuildAgentHeightString(string colVal)
        {
            //var holder = colVal;

            var footString = "";
            var inchString = "";

            //if the first character is not a digit, return ""
            if (!CheckDigitAt0(colVal))
            {
                var intIdx = FirstNumericCharacter(colVal);
                if (intIdx > -1)
                {
                    colVal = colVal.Substring(intIdx);
                }
                if (!CheckDigitAt0(colVal))
                {
                    colVal = BuildDigitStringIfPossible(colVal);
                    if (!CheckDigitAt0(colVal)) return "";
                }
            }

            var firstPosNotNumeric = FirstNonNumericCharacter(colVal);
            if (firstPosNotNumeric > -1 && colVal.Length > firstPosNotNumeric)
            {
                footString = colVal.Substring(0, firstPosNotNumeric);
                colVal = colVal.Substring(firstPosNotNumeric);
            }
            else
            {
                footString = colVal;
                colVal = "";
            }

            //trim non numeric values from front of numeric
            //var thisHolder = holder;
            footString = GetNumeric(footString);
            inchString = GetNumeric(colVal);

            if (inchString == string.Empty && footString != string.Empty && int.TryParse(footString, out int ftInt))
            {
                if (ftInt > 10)
                {
                    if (ftInt < 100)
                    {
                        //inches should be feet not 3 digits - use as inches
                        inchString = footString;
                        footString = "";
                    }
                    else if (ftInt > 99)
                    {
                        //3 digits used as footinchinch (fii like 6'9"=609=69)
                        var ftStr = ftInt.ToString();
                        footString = ftStr.Substring(0, 1);
                        inchString = ftStr.Substring(1);
                    }
                }
            }
            var ftInches = 0;
            var inInches = 0;
            if (int.TryParse(footString, out int ftInchInt))
            {
                ftInches = ftInchInt;
            }
            if (int.TryParse(inchString, out int inInchInt))
            {
                inInches = inInchInt;
            }

            inInches = ftInches * 12 + inInches;

            ftInches = inInches / 12;
            inInches = inInches % 12;

            var ftRetStr = ftInches.ToString();
            var inRetStr = inInches.ToString();

            while (inRetStr.Length < 2)
            {
                inRetStr = '0' + inRetStr;
            }
            //var originalValue = holder;
            var retVal = string.Concat(ftRetStr, inRetStr);
            return retVal;
        }
    }
}
