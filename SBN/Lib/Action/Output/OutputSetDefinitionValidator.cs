using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SBN.Lib.Action.Output
{
    public class OutputSetDefinitionValidator : IOutputSetDefinitionValidator
    {
        public void Validate(DataTable table, OutputSetDefinition outputSetDefinition, IEnumerable<string> defs)
        {
            if (!defs?.Any() ?? false)
            {
                var expected = string.Join(',', outputSetDefinition.Fields);
                var actual = string.Join(',', table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                throw new ArgumentException($"Table does not match supplied output definitions. Expected: {expected}, Actual: {actual} ");
            }
        }
    }
}