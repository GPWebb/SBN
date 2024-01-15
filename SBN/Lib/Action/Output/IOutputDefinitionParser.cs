using System.Xml.Linq;

namespace SBN.Lib.Action.Output
{
    public interface IOutputDefinitionParser
    {
        OutputDefinition Parse(XElement outputDefinition);
    }
}