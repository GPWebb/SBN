using SBN.Lib.Action.Data;
using SBN.Lib.DB;
using SBN.Lib.Xml.XPath;
using System;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class ValueFromData : IValueFromData
    {
        private readonly IXMLReader _xmlReader;
        private readonly IMergeFromDocument _mergeFromDocument;

        public ValueFromData(IXMLReader xmlReader,
            IMergeFromDocument mergeFromDocument)
        {
            _xmlReader = xmlReader;
            _mergeFromDocument = mergeFromDocument;
        }

        public string Value(DataMergeParameter dataMergeParameter, ActionData actionData)
        {
            if (actionData.Outcome.Body.Data != null)
            {
                return _mergeFromDocument.Merge(dataMergeParameter, actionData.Outcome.Body.Data);
            }
            else if (actionData.Data.Tables.Count == 0)
            {
                throw new ArgumentException($"Action data {actionData.ActionID + actionData.ActionParameters} not populated, cannot merge {dataMergeParameter.Parameter}");
            }
            else
            {                        
                var dataTable = actionData.Data.Tables[0];
                var dataRow = dataTable.Rows[0];

                if (dataRow.Table.Columns.Count == 1
                    && (dataRow.Table.Columns[0].ColumnName.StartsWith("XML") || dataRow[0].ToString().StartsWith("<")))
                {
                    var document = _xmlReader.ReadFromDataTable(dataTable);

                    if (document != null)
                    {
                        return _mergeFromDocument.Merge(dataMergeParameter, document);
                    }
                    else
                    {
                        return dataRow[dataMergeParameter.Parameter].ToString();
                    }
                }
                else
                {
                    return dataRow[dataMergeParameter.Parameter].ToString();
                }
            }
        }
    }
}