using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using SBN.Lib.Action.Output;

namespace SBN.Lib.Action.Data.Serialise
{
    public interface IActionDataSerialiser
    {
        XElement Serialise(IEnumerable<DataTable> tables, OutputDefinition outputDefinitions, bool decorateForJson);
    }
}