using System.Data;

namespace SBN.Lib.Page.Outcome
{
    public interface IPageTable
    {
        DataTable Read(DataSet pageData, PageDataTables table);
    }
}