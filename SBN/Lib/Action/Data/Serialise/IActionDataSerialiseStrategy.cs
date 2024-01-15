using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using SBN.Lib.Action.Output;

namespace SBN.Lib.Action.Data.Serialise
{
    public interface IActionDataSerialiseStrategy
    {
        bool Selector(IEnumerable<DataTable> tables);
        XElement Serialise(IEnumerable<DataTable> tables, OutputDefinition outputDefinition, bool decorateForJson);
    }
}