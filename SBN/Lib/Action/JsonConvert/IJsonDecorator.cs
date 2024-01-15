using System.Xml.Linq;
using SBN.Lib.Action.Output;

namespace SBN.Lib.Action.JsonConvert
{
    public interface IJsonDecorator
    {
        XElement JsonDecorate(OutputDefinition outputDefinition, XElement root);
    }
}