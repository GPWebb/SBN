using System.Xml.Linq;

namespace SBN.Lib.Action.Output
{
    public interface IOutputDefinitionValidator
    {
        void Validate(OutputDefinition result, XElement outputDefinition);
    }
}