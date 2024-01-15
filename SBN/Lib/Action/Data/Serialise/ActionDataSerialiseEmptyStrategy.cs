using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using SBN.Lib.Action.JsonConvert;
using SBN.Lib.Action.Output;

namespace SBN.Lib.Action.Data.Serialise
{
    public class ActionDataSerialiseEmptyStrategy : IActionDataSerialiseStrategy
    {
        private readonly IOutputSetDefinitionValidator _outputSetDefinitionValidator;
        private readonly IJsonDecorator _jsonDecorator;

        public ActionDataSerialiseEmptyStrategy(IOutputSetDefinitionValidator outputSetDefinitionValidator,
            IJsonDecorator jsonDecorator)
        {
            _outputSetDefinitionValidator = outputSetDefinitionValidator;
            _jsonDecorator = jsonDecorator;
        }

        public bool Selector(IEnumerable<DataTable> tables)
        {
            return tables.Count() == 0;
        }

        public XElement Serialise(IEnumerable<DataTable> tables, OutputDefinition outputDefinition, bool decorateForJson)
        {
            return null;
        }
    }
}