namespace csepAuditTool.Model
{
    internal class ColumnsModel
    {
        public int ColIdx { get; set; }
        public int ColLen { get; set; }
        public string ColNam { get; set; }
        public string ColVal { get; set; }

        public ColumnsModel(int colIdx, int colLen, string colNam)
        {
            ColIdx = colIdx;
            ColLen = colLen;
            ColNam = colNam;
            ColVal = "";
        }

        public ColumnsModel(int colIdx, int colLen, string colName, string colVal)
        {
            ColIdx = colIdx;
            ColLen = colLen;
            ColNam = colName;
            ColVal = colVal;
        }
    }
}
