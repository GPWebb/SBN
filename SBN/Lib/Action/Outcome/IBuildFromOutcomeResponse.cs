using System.Data;

namespace SBN.Lib.Action.Outcome
{
    public interface IBuildFromOutcomeResponse
    {
        ApiActionOutcome Build(DataRow outcomeDataHeader, ApiActionOutcome apiActionOutcome);
    }
}