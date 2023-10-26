namespace csepAuditTool.Model
{
    internal class OutgoingRowModel
    {
        public int RowIdx { get; set; }
        public List<ColumnsModel> RowCols { get; set; }
        public OutgoingRowModel() { RowCols = new List<ColumnsModel>(); }
    }
}

//public DateTime? BirthDate
//{
//    get
//    {
//        if (RowCols == null || RowCols.ToList().Count() < 6) return null; //index 5 is dob
//        var myBirthDateString = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Date_Of_Birth").ColVal;
//        if (myBirthDateString != null)
//        {
//            if (DateTime.TryParseExact(myBirthDateString, "MMddyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime myBirthDate))
//            {
//                return myBirthDate;
//            }
//        }
//        return null;
//    }
//}

//public string? SsnWithDashes
//{
//    get
//    {
//        if (RowCols == null || RowCols.ToList().Count() < 5) return null; //index 4 is SSN
//        var mySocialSecurityNumber = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Social_Security_Number").ColVal;
//        if (mySocialSecurityNumber != null && mySocialSecurityNumber.Length == 9)
//        {
//            return mySocialSecurityNumber.Insert(3, "-").Insert(6, "-");
//        }
//        return null;
//    }
//}
//public string? MiddleInitial_ForResults
//{
//    get
//    {
//        if (RowCols == null) return null;
//        var myMiddleInitial = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Middle_Name");
//        if (myMiddleInitial == null) return null;
//        if (myMiddleInitial.ColVal != null && myMiddleInitial.ColVal.Length > 1) return myMiddleInitial.ColVal.Substring(0, 1);
//        return myMiddleInitial.ColVal;
//    }
//}
