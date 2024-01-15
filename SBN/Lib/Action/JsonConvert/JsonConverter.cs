using SBN.Models;

namespace SBN.Lib.Action.JsonConvert
{
    public class JsonConverter : IJsonConverter
    {
        public string Convert(Response result)
        {
            if (result.Data == null) return "";

            else return Newtonsoft.Json.JsonConvert.SerializeXNode(result.Data);
        }
    }
}
