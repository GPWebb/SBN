using System;
using System.Linq;
using System.Xml.Linq;

namespace SBN.Lib.Action.Output
{
    public class OutputDefinitionValidator : IOutputDefinitionValidator
    {
        public void Validate(OutputDefinition result, XElement outputDefinition)
        {
            if (!result.OutputSets.Any()) throw new ArgumentException($"No output definitions supplied : {outputDefinition}");

            if (result.OutputSets.Any(r => string.IsNullOrWhiteSpace(r.Name))) throw new ArgumentException($"Result contains missing name(s) : {outputDefinition}");

            if (result.OutputSets.Any(r => string.IsNullOrWhiteSpace(r.Record))) throw new ArgumentException($"Result contains missing record name(s) : {outputDefinition}");

            if (result.OutputSets.Any(r => r.Fields != null && r.Fields.Any(f => string.IsNullOrWhiteSpace(f)))) throw new ArgumentException($"Result contains missing field name(s) : {outputDefinition}");

            if (result.OutputSets.Count() > 1 && string.IsNullOrWhiteSpace(result.Collection)) throw new ArgumentException("Collection name must be supplied for multiple output sets");
        }
    }
}
