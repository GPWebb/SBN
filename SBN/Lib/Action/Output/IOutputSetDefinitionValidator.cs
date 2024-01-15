using System.Collections.Generic;
using System.Data;

namespace SBN.Lib.Action.Output
{
    public interface IOutputSetDefinitionValidator
    {
        void Validate(DataTable table, OutputSetDefinition outputSetDefinition, IEnumerable<string> defs);
    }
}