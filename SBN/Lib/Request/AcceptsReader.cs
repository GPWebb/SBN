using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using SBN.Lib.Definitions;

namespace SBN.Lib.Request
{
    public class AcceptsReader : IAcceptsReader
    {
        public ResponseDataType Read(HttpRequest request)
        {
            var path = request.Path.ToString();
            if (path.LastIndexOf(".") > path.LastIndexOf("/"))
            {
                var extension = path.Substring(path.LastIndexOf(".") + 1);

                switch (extension.ToUpperInvariant())
                {
                    case "XML":
                        return ResponseDataType.Xml;

                    case "JSON":
                        return ResponseDataType.Json;

                    default:
                        throw new ArgumentException("No supported media type found");
                }
            }

            var accepts = request
                .GetTypedHeaders()
                .Accept
                ?.Where(x => x.MediaType != "*/*")
                ?.ToList();

            if (accepts == null || accepts.Count() == 0 || accepts.Contains(new MediaTypeHeaderValue("application/xml")))
            {
                return ResponseDataType.Xml;
            }

            if (accepts.Contains(new MediaTypeHeaderValue("application/json")))
            {
                return ResponseDataType.Json;
            }

            throw new ArgumentException("No supported media type found");
        }
    }
}
