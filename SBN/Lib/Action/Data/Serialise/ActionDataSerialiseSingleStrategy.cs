using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SBN.Lib.Action.JsonConvert;
using SBN.Lib.Action.Output;

namespace SBN.Lib.Action.Data.Serialise
{
    public class ActionDataSerialiseSingleStrategy : IActionDataSerialiseStrategy
    {
        private readonly IOutputSetDefinitionValidator _outputSetDefinitionValidator;
        private readonly IJsonDecorator _jsonDecorator;

        public ActionDataSerialiseSingleStrategy(IOutputSetDefinitionValidator outputSetDefinitionValidator,
            IJsonDecorator jsonDecorator)
        {
            _outputSetDefinitionValidator = outputSetDefinitionValidator;
            _jsonDecorator = jsonDecorator;
        }

        public bool Selector(IEnumerable<DataTable> tables)
        {
            return tables.Count() == 1 && (tables.Single().Rows.Count > 1 || tables.Single().Columns.Count > 1);
        }

        public XElement Serialise(IEnumerable<DataTable> tables, OutputDefinition outputDefinition, bool decorateForJson)
        {
            XElement data;

            var table = tables.Single();

            OutputSetDefinition outputSetDefinition;

            try
            {
                outputSetDefinition = outputDefinition.OutputSets.Single();
            }
            catch (Exception ex)
            {
                throw new Exception("No matching output definition found to serialise action data", ex);
            }

            using (var sw = new StringWriter())
            {
                var defs = outputSetDefinition
                    .Fields
                    ?.Where(f => table.Columns.Contains(f));

                _outputSetDefinitionValidator.Validate(table, outputSetDefinition, defs);

                table.DataSet.DataSetName = outputSetDefinition.Name;
                table.TableName = outputSetDefinition.Record;

                table.WriteXml(sw);

                try
                {
                    data = XElement.Parse(sw.ToString());
                }
                catch (Exception e)
                {
                    throw new Exception("Cannot add table to output", e);
                }
                sw.Flush();
            }

            if (decorateForJson)
            {
                _jsonDecorator.JsonDecorate(outputDefinition, data);
            }

            return data;
        }
    }
}