using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Render.Util
{
    public class URLParameterReplacer : IURLParameterReplacer
    {
        public string Replace(string pageTemplate, ValidatedRequest validatedRequest)
        {
            var templateText = pageTemplate;

            foreach (var param in validatedRequest.AllParams)
            {
                templateText = templateText.Replace($"{{{param}}}", $"[?{param}]");
            }

            return templateText;
        }
    }
}
