using SBN.Lib.DB;
using SBN.Models;
using System.Data;

namespace SBN.Lib.Action.Outcome
{
    public class OutcomeBuilderXmlPopulator : IOutcomeBuilderXmlPopulator
    {
        private readonly IXMLReader _xmlReader;

        public OutcomeBuilderXmlPopulator(IXMLReader xmlReader)
        {
            _xmlReader = xmlReader;
        }

        public bool PopulateXmlResponse(DataTable inputData, bool populated, Response response)
        {
            if (inputData.Columns.Count == 1
                && (inputData.Columns[0].ColumnName.StartsWith("XML") || inputData.Rows[0].ToString().StartsWith("<")))
            {
                populated = _xmlReader.TryReadFromDataTable(inputData, out var data);
                response.Data = data;

                return populated;
            }

            return false;
        }
    }
}