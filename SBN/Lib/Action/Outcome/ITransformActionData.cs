using SBN.Lib.Action.Data;

namespace SBN.Lib.Action.Outcome
{
    public interface ITransformActionData
    {
        void Transform(ActionData actionData, ApiActionOutcome apiActionOutcome);
    }
}