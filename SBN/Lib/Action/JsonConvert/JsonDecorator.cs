using System.Xml.Linq;
using SBN.Lib.Action.Output;

namespace SBN.Lib.Action.JsonConvert
{
    public class JsonDecorator : IJsonDecorator
    {
        public XElement JsonDecorate(OutputDefinition outputDefinition, XElement root)
        {
            foreach (var outputSet in outputDefinition.OutputSets)
            {
                if (root.Name == outputSet.Name)
                {
                    var node = root.Element(outputSet.Record);

                    if (node != null)
                    {
                        XNamespace ns = XNamespace.Get("http://james.newtonking.com/projects/json");
                        XAttribute array = new XAttribute(ns + "Array", "true");

                        node.Add(array);
                    }
                }
            }

            return root;
        }
    }
}