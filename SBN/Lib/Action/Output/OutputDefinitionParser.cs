using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SBN.Lib.Action.Output
{
    public class OutputDefinitionParser : IOutputDefinitionParser
    {
        private readonly IOutputDefinitionValidator _outputDefinitionValidator;

        public OutputDefinitionParser(IOutputDefinitionValidator outputDefinitionValidator)
        {
            _outputDefinitionValidator = outputDefinitionValidator;
        }

        public OutputDefinition Parse(XElement outputDefinition)
        { 
            if (outputDefinition == null) throw new Exception("No data output definition supplied");

            var result = new OutputDefinition { OutputSets = new List<OutputSetDefinition>() };

            var output = outputDefinition.Element("Output");

            //if (output == null) throw new Exception("Supplied output definition is invalid");
            if (output == null) return null;

            if (output.HasAttributes && output.Attribute("collection") != null)
            {
                result.Collection = output.Attribute("collection").Value;
            }

            foreach (var set in outputDefinition.Element("Output").Elements("Set"))
            {
                var name = set.Attribute("name").Value.Trim();
                var record = set.Attribute("record").Value.Trim();
                var fields = set
                    .Element("Fields")
                    ?.Elements("Field")
                    ?.SelectMany(f => f.Attributes()
                        .Where(a => a.Name == "name")
                        .Select(a => a.Value))
                    .ToList();

                result.OutputSets.Add(new OutputSetDefinition
                {
                    Name = name,
                    Record = record,
                    Fields = fields
                });
            }

            _outputDefinitionValidator.Validate(result, outputDefinition);

            return result;
        }
    }
}
