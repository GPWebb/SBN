using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using SBN.Lib.Action.JsonConvert;
using SBN.Lib.Action.Output;
using SBN.Lib.DB;

namespace SBN.Lib.Action.Data.Serialise
{
    public class ActionDataSerialiseXmlStrategy : IActionDataSerialiseStrategy
    {
        private readonly IOutputSetDefinitionValidator _outputSetDefinitionValidator;
        private readonly IJsonDecorator _jsonDecorator;
        private readonly IXMLReader _xmlReader;

        public ActionDataSerialiseXmlStrategy(IOutputSetDefinitionValidator outputSetDefinitionValidator,
            IJsonDecorator jsonDecorator,
            IXMLReader xmlReader)
        {
            _outputSetDefinitionValidator = outputSetDefinitionValidator;
            _jsonDecorator = jsonDecorator;
            _xmlReader = xmlReader;
        }

        public bool Selector(IEnumerable<DataTable> tables)
        {
            return tables.Count() == 1 && tables.Single().Columns.Count == 1 && tables.Single().Rows.Count == 1;
        }

        public XElement Serialise(IEnumerable<DataTable> tables, OutputDefinition outputDefinition, bool decorateForJson)
        {
            XElement data;

            data = _xmlReader.ReadFromDataTable(tables.Single());

            if (decorateForJson)
            {
                _jsonDecorator.JsonDecorate(outputDefinition, data);
            }

            return data;
        }
    }
}