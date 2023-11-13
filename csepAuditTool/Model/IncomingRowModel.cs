namespace csepAuditTool.Model
{
    internal class IncomingRowModel
    {
        public int RowIdx { get; set; }
        public List<ColumnsModel> RowCols { get; set; }

        public string LastName
        {
            get
            {
                if (RowCols == null) return string.Empty;
                var myLastName = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Last_Name");
                if (myLastName == null) return string.Empty;
                return myLastName.ColVal;
            }
        }

        public string FirstName
        {
            get
            {
                if (RowCols == null) return string.Empty;
                var myFirstName = RowCols.FirstOrDefault(p => p.ColNam == "Participant_First_Name");
                if (myFirstName == null) return string.Empty;
                return myFirstName.ColVal;
            }
        }

        public string MiddleName
        {
            get
            {
                if (RowCols == null) return string.Empty;
                var myMiddleName = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Middle_Name");
                if (myMiddleName == null || myMiddleName.ColVal == null) return string.Empty;
                return myMiddleName.ColVal.Length > 0 ? myMiddleName.ColVal : string.Empty;
            }
        }

        public string FirstNameFirst4
        {
            get
            {
                if (RowCols == null) return string.Empty;
                var myFirstNameFirst4 = FirstName ?? string.Empty;
                if (myFirstNameFirst4 == null) return string.Empty;
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
        public string NameModifier
        {
            get
            {
                if (RowCols == null) return string.Empty;
                var myNameModifier = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Name_Modifier");
                if (myNameModifier == null) return string.Empty;
                return myNameModifier.ColVal;
            }
        }
        public string Ssn
        {
            get
            {
                if (RowCols == null) return string.Empty;
                var mySsn = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Social_Security_Number");
                if (mySsn == null) return string.Empty;
                return mySsn.ColVal;
            }
        }
        public string SsnWithDashes
        {
            get
            {
                if (RowCols == null) return string.Empty;
                var mySocialSecurityNumberObject = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Social_Security_Number");
                if (mySocialSecurityNumberObject == null) return string.Empty;
                var mySocialSecurityNumber = mySocialSecurityNumberObject.ColVal;
                if (mySocialSecurityNumber != null && mySocialSecurityNumber.Length == 9)
                {
                    return mySocialSecurityNumber.Insert(3, "-").Insert(6, "-");
                }
                return string.Empty;
            }
        }
        public string SsnLast4
        {
            get
            {
                if (RowCols == null || Ssn == string.Empty) return string.Empty;
                if (Ssn.Length > 4 && Ssn.Length < 10) return Ssn.Substring(Ssn.Length - 4);
                return string.Empty;
            }
        }
        public string BirthDateString
        {
            get
            {
                if (RowCols == null) return string.Empty;
                var myBDString = RowCols.FirstOrDefault(p => p.ColNam == "Participant_Date_Of_Birth");
                if (myBDString == null) return string.Empty;
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
