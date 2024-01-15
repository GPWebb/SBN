using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SBN.Lib.Xml.XPath;

namespace SBN.Lib.Action.Outcome
{
    public class ResourceURLGenerator : IResourceURLGenerator
    {
        private readonly IXPathFacade _xpathFacade;

        public ResourceURLGenerator(IXPathFacade xpathFacade)
        {
            _xpathFacade = xpathFacade;
        }

        public string Generate(PathString path, IQueryCollection query, string APIRoute, XElement outputParameters)
        {
            if (outputParameters == null)
            {
                return path.Value + (query.Count > 0 ? query.ToString() : "");
            }

            if (!APIRoute.ToLowerInvariant().StartsWith("/api")) APIRoute = $"/api{APIRoute}";

            var routeParts = APIRoute.Split(new[] { '/' });
            var pathParts = path.Value.Split(new[] { '/' });

            for (var i = 0; i < routeParts.Length; i++)
            {
                var part = routeParts[i];
                if (part.StartsWith("{") && part.EndsWith("}"))
                {
                    var partId = part.Substring(1, part.Length - 2);

                    var outputParameter = _xpathFacade.SelectElement(outputParameters, $"//Parameter[Name='{partId}']/Value")?.Value;

                    routeParts[i] = string.IsNullOrWhiteSpace(outputParameter)
                        ? pathParts[i]
                        : outputParameter;
                }
            }

            var generatedUrl = string.Join('/', routeParts);

            if (query.Count > 0)
            {
                var outputQuery = new StringBuilder();
                foreach (var key in query.Keys)
                {
                    var outputParameter = (string)(_xpathFacade.Select(outputParameters, $"/Parameters/Parameter[Name='{key}']/Value").FirstOrDefault());
                    if (string.IsNullOrWhiteSpace(outputParameter))
                    {
                        query.TryGetValue(key, out StringValues queryValues);
                        outputParameter = queryValues.ToArray()[0];
                    }

                    outputQuery.AppendFormat($"{HttpUtility.UrlEncode(key)}={outputParameter}");
                }

                generatedUrl += "?" + outputQuery.ToString();
            }

            return generatedUrl;
        }
    }
}
