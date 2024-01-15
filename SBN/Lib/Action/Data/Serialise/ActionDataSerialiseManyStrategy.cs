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
    public class ActionDataSerialiseManyStrategy : IActionDataSerialiseStrategy
    {
        private readonly IJsonDecorator _jsonDecorator;

        public ActionDataSerialiseManyStrategy(IJsonDecorator jsonDecorator)
        {
            _jsonDecorator = jsonDecorator;
        }

        public bool Selector(IEnumerable<DataTable> tables)
        {
            return tables.Count() > 1;
        }

        public XElement Serialise(IEnumerable<DataTable> tables, OutputDefinition outputDefinition, bool decorateForJson)
        {
            XElement root;
            if (outputDefinition.OutputSets.Count() < tables.Count())
            {
                throw new Exception("Insufficient output definitions to serialise the supplied data");
            }

            using (var sw = new StringWriter())
            {
                foreach (var table in tables)
                {
                    var defs = outputDefinition.OutputSets
                        .Where(sd => sd.Fields
                            .All(f => table.Columns.Contains(f)))
                        .ToList();

                    if (!defs.Any())
                    {
                        var expected = string.Join(" or ", outputDefinition.OutputSets.Select(f => string.Join(',', f.Fields)));
                        var actual = string.Join(',', table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                        throw new ArgumentException($"Table does not match supplied output definitions. Expected: {expected}, Actual: {actual} ");
                    }
                    if (defs.Count() > 1) throw new ArgumentException("Output definition fields spec matches multiple results sets");

                    var definition = defs.Single();

                    table.DataSet.DataSetName = definition.Name;
                    table.TableName = definition.Record;

                    table.WriteXml(sw);
                }

                try
                {
                    //HACK This shouldn't be necessary, but other ways complained. Try to find a better way.
                    root = XElement.Parse($"<{outputDefinition.Collection}>{sw}</{outputDefinition.Collection}>");
                }
                catch (Exception e)
                {
                    throw new Exception("Cannot add table to output", e);
                }
                sw.Flush();
            }

            if (decorateForJson)
            {
                _jsonDecorator.JsonDecorate(outputDefinition, root);
            }

            return root;
        }
    }
}