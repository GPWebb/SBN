using System.Data;
using System;

namespace SBN.Lib.Page.Outcome
{
    public class PageTable : IPageTable
    {
        public DataTable Read(DataSet pageData, PageDataTables table)
        {
            var index = (int)table;

            if (pageData.Tables.Count > index) return pageData.Tables[(int)table];

            throw new ArgumentException($"Page data does not contain data of type '{table}'");
        }
    }
}
