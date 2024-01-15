using SBN.Models;

namespace SBN.Lib.Action.JsonConvert
{
    public interface IJsonConverter
    {
        string Convert(Response result);
    }
}