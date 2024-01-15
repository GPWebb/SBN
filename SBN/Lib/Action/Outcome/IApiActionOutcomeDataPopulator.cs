using System.Data;
using SBN.Lib.Action.Data;

namespace SBN.Lib.Action.Outcome
{
    public interface IApiActionOutcomeDataPopulator
    {
        ApiActionOutcome Populate(DataRow outcomeDataHeader);
        ApiActionOutcome Populate(ActionData actionData, bool decorateForJson, bool transform = true);
    }
}