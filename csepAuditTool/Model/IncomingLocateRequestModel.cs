using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csepAuditTool.Model
{
    internal class IncomingLocateRequestModel
    {
        public List<ColumnsModel> SingleRowModel()
        {
            return new List<ColumnsModel> {
                    new ColumnsModel(0,21,"Participant_Last_Name"),
                    new ColumnsModel(1,16,"Participant_First_Name"),
                    new ColumnsModel(2,16,"Participant_Middle_Name"),
                    new ColumnsModel(3,3,"Participant_Name_Modifier"),
                    new ColumnsModel(4,9,"Participant_Social_Security_Number"),
                    new ColumnsModel(5,8,"Participant_Date_Of_Birth")
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
