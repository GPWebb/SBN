using SBN.Lib.Action.Data;

namespace SBN.Lib.Action.Outcome
{
    public interface IBuildApiActionOutcome
    {
        ApiActionOutcome Build(ActionData actionData);
    }
}