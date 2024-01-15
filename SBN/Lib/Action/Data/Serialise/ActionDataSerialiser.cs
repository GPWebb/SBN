using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using SBN.Lib.Action.Output;

namespace SBN.Lib.Action.Data.Serialise
{
    public class ActionDataSerialiser : IActionDataSerialiser
    {
        private readonly IEnumerable<IActionDataSerialiseStrategy> _actionDataSerialiseStrategies;

        public ActionDataSerialiser(IEnumerable<IActionDataSerialiseStrategy> actionDataSerialiseStrategies)
        {
            _actionDataSerialiseStrategies = actionDataSerialiseStrategies;
        }

        public XElement Serialise(IEnumerable<DataTable> tables, OutputDefinition outputDefinitions, bool decorateForJson)
        {
            return _actionDataSerialiseStrategies
                .Single(x => x.Selector(tables))
                .Serialise(tables, outputDefinitions, decorateForJson);
        }
    }
}