using System.Data;
using System.Xml.Linq;

namespace SBN.Lib.DB
{
    public interface IXMLReader
    {
        XElement ReadFromDataTable(DataTable dataTable);

        bool TryReadFromDataTable(DataTable dataTable, out XElement document);
    }
}