using SBN.Models;

namespace SBN.Lib.Action.Data
{
    public interface IExtractActionData
    {
        Response ResponseFromActionData(ActionData result, bool transform = false);
    }
}