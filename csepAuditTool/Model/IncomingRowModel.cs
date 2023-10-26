namespace csepAuditTool.Model
{
    internal class IncomingRowModel
    {
        public int RowIdx { get; set; }
        public List<ColumnsModel> RowCols { get; set; }

        public string? LastName
        {
            get
            {
                if (RowCols == null) return null;
                var myLastName = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Last_Name");
                if (myLastName == null) return null;
                return myLastName.ColVal;
            }
        }

        public string? FirstName
        {
            get
            {
                if (RowCols == null) return null;
                var myFirstName = RowCols.FirstOrDefault(p => p.ColNam == "Participant_First_Name");
                if (myFirstName == null) return null;
                return myFirstName.ColVal;
            }
        }

        public string MiddleName
        {
            get
            {
                if (RowCols == null) return "";
                var myMiddleName = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Middle_Name");
                if (myMiddleName == null || myMiddleName.ColVal == null) return "";
                return myMiddleName.ColVal.Length > 0 ? myMiddleName.ColVal : "";
            }
        }

        public string? FirstNameFirst4
        {
            get
            {
                if (RowCols == null) return null;
                var myFirstNameFirst4 = FirstName;
                if (myFirstNameFirst4 == null) return null;
                if (myFirstNameFirst4.Length > 4) return myFirstNameFirst4.Substring(0, 4);
                return myFirstNameFirst4;
            }
        }

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

        //public string? FullName
        //{
        //    get
        //    {
        //        if(RowCols ==null) return null;
        //        var myFirstName = FirstName ?? "";
        //        var myMiddleName = MiddleName ?? "";
        //        var myLastName = LastName ?? "";
        //        var myFullName = myFirstName + " " + myMiddleName + " " + myLastName;
        //        return Regex.Replace(myFullName, @"\s+", " ");
        //    }
        //}

        //public string? FirstLastName
        //{
        //    get
        //    {
        //        if (RowCols == null) return null;
        //        var myFirstName = FirstName;
        //        var myLastName = LastName;
        //        var myFirstLastName = myFirstName + ' ' + myLastName;
        //    }
        //}
        public string? NameModifier
        {
            get
            {
                if (RowCols == null) return null;
                var myNameModifier = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Name_Modifier");
                if (myNameModifier == null) return null;
                return myNameModifier.ColVal;
            }
        }
        public string? Ssn
        {
            get
            {
                if (RowCols == null) return null;
                var mySsn = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Social_Security_Number");
                if (mySsn == null) return null;
                return mySsn.ColVal;
            }
        }
        public string? SsnWithDashes
        {
            get
            {
                if (RowCols == null) return null;
                var mySocialSecurityNumberObject = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Social_Security_Number");
                if (mySocialSecurityNumberObject == null) return null;
                var mySocialSecurityNumber = mySocialSecurityNumberObject.ColVal;
                if (mySocialSecurityNumber != null && mySocialSecurityNumber.Length == 9)
                {
                    return mySocialSecurityNumber.Insert(3, "-").Insert(6, "-");
                }
                return null;
            }
        }
        public string? BirthDateString
        {
            get
            {
                if (RowCols == null) return null;
                var myBDString = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Date_Of_Birth");
                if (myBDString == null) return null;
                return myBDString.ColVal;
            }
        }
        public DateTime? BirthDate
        {
            get
            {
                if (RowCols == null) return null;
                var myBirthDateStringObject = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Date_Of_Birth");
                if (myBirthDateStringObject == null) return null;
                var myBirthDateString = myBirthDateStringObject.ColVal;
                if (myBirthDateString != null)
                {
                    if (DateTime.TryParseExact(myBirthDateString, "MMddyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime myBirthDate))
                    {
                        return myBirthDate;
                    }
                }
                return null;
            }
        }

        public IncomingRowModel() { RowCols = new List<ColumnsModel>(); }
    }
}

//public List<ColumnsModel> SingleRowModel()
//    {
//    return new List<ColumnsModel> {
//        new ColumnsModel(0,21,"Participant_Last_Name"),
//        new ColumnsModel(1,16,"Participant_First_Name"),
//        new ColumnsModel(2,16,"Participant_Middle_Name"),
//        new ColumnsModel(3,3,"Participant_Name_Modifier"),
//        new ColumnsModel(4,9,"Participant_Social_Security_Number"),
//        new ColumnsModel(5,8,"Participant_Date_Of_Birth")
//    };
//}
