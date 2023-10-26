namespace csepAuditTool.Model
{
    internal class OutgoingLocateRequestModel
    {
        public List<ColumnsModel> SingleRowModel()
        {
            return new List<ColumnsModel> {
                    new ColumnsModel(0,32,"Cannabis_Agent_Card_Number")
                    ,new ColumnsModel(1,1,"Cannabis_Agent_Card_Status")
                    ,new ColumnsModel(2,8,"Cannabis_Agent_Card_Expiration_Date")
                    ,new ColumnsModel(3,21,"Agent_Last_Name")
                    ,new ColumnsModel(4,16,"Agent_First_Name")
                    ,new ColumnsModel(5,1,"Agent_Middle_Initial")
                    ,new ColumnsModel(6,3,"Modifier_of_the_Agent")
                    ,new ColumnsModel(7,9,"Agent_Social_Security_Number")
                    ,new ColumnsModel(8,8,"Agent_Date_of_Birth")
                    ,new ColumnsModel(9,3,"Agent_Eye_Color")
                    ,new ColumnsModel(10,3,"Agent_Hair_Color")
                    ,new ColumnsModel(11,3,"Agent_Height")
                    ,new ColumnsModel(12,3,"Agent_Weight")
                    ,new ColumnsModel(13,1,"Agent_Gender")
                    ,new ColumnsModel(14,50,"Agent_Mailing_Address_Line_1")
                    ,new ColumnsModel(15,50,"Agent_Mailing_Address_Line_2")
                    ,new ColumnsModel(16,20,"Agent_Mailing_Address_City")
                    ,new ColumnsModel(17,2,"Agent_Mailing_Address_State")
                    ,new ColumnsModel(18,5,"Agent_Mailing_Address_Zip_Code")
                    ,new ColumnsModel(19,4,"Agent_Mailing_Address_Zip_+_4")
                    ,new ColumnsModel(20,8,"Date_of_Last_Agent_Mailing_Address_Update")
                    ,new ColumnsModel(21,50,"Agent_Residential_Address_Line_1")
                    ,new ColumnsModel(22,50,"Agent_Residential_Address_Line_2")
                    ,new ColumnsModel(23,20,"Agent_Residential_Address_City")
                    ,new ColumnsModel(24,2,"Agent_Residential_Address_State")
                    ,new ColumnsModel(25,5,"Agent_Residential_Address_Zip_Code")
                    ,new ColumnsModel(26,4,"Agent_Residential_Address_Zip_+_4")
                    ,new ColumnsModel(27,8,"Date_of_Last_Agent_Residential_Address_Update")
                    ,new ColumnsModel(28,13,"Agent_Home_Phone_Number")
                    ,new ColumnsModel(29,13,"Agent_Mobile_Phone_Number")
                    ,new ColumnsModel(30,48,"Email_Address")
                    ,new ColumnsModel(31,64,"Entity_Name")
                    ,new ColumnsModel(32,50,"Entity_Mailing_Address_Line_1")
                    ,new ColumnsModel(33,50,"Entity_Mailing_Address_Line_2")
                    ,new ColumnsModel(34,20,"Entity_Mailing_Address_City")
                    ,new ColumnsModel(35,3,"Entity_Mailing_Address_County")
                    ,new ColumnsModel(36,2,"Entity_Mailing_Address_State")
                    ,new ColumnsModel(37,5,"Entity_Mailing_Address_Zip_Code")
                    ,new ColumnsModel(38,4,"Entity_Mailing_Address_Zip_+_4")
                    ,new ColumnsModel(39,13,"Business_Telephone_Number")
                    ,new ColumnsModel(40,6,"Business_Telephone_Extension_Number")
                };
        }

        public int[] SingleRowFieldLengths()
        {
            return SharedFunctionModel.SingleRowFieldLengths(SingleRowModel());
        }

        public int SingleRowFieldCount()
        {
            return SharedFunctionModel.SingleRowFieldCount(SingleRowModel());
        }
    }
}
