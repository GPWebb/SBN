using Microsoft.AspNetCore.Mvc;
using SBN.Lib.Page.Render.Util;
using System.Net;

namespace SBN.Lib.Page.Outcome
{
    public class HttpResult : IHttpResult
    {
        private readonly IHtml5Renderer _html5Renderer;

        public HttpResult(IHtml5Renderer html5Renderer)
        {
            _html5Renderer = html5Renderer;
        }

        public IActionResult Generate(string body)
        {
            body = $"{Definitions.Doctypes.HTML5}{body}";

            body = _html5Renderer.FixSelfClosingTags(body);

            return new ContentResult
            {
                Content = body,
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
