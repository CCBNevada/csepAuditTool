﻿namespace csepAuditTool.Model
{
    internal class SharedFunctionModel
    {
        public static int[] SingleRowFieldLengths(List<ColumnsModel> singleRowModel)
        {
            var tList = new List<int>();
            foreach (var column in singleRowModel)
            {
                tList.Add(column.ColLen);
            }
            return tList.ToArray();
        }

        public static int SingleRowFieldCount(List<ColumnsModel> singleRowModel)
        {
            return singleRowModel.Count;
        }
    }
}
