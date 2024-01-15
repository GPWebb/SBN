using SBN.Lib.Sys;

namespace SBN.Lib.Page.Render.Util
{
    public class Html5Renderer : IHtml5Renderer
    {
        private readonly string[] VoidElements = new[]
        {
            "area",
            "base",
            "br",
            "col",
            "command",
            "embed",
            "hr",
            "img",
            "input",
            "keygen",
            "link",
            "meta",
            "param",
            "source",
            "track",
            "wbr"
        };

        public string FixSelfClosingTags(string body)
        {
            while (body.Contains("/>"))
            {
                var position = body.IndexOf("/>");
                var tagStart = body.LastIndexOf("<", position);
                var tagBody = body.Substring(tagStart + 1, position - tagStart - 1);
                var tag = tagBody.Substring(0, tagBody.IndexOfAny(new[] { ' ', '\t', '\r', '\n' }));

                body = body.Substring(0, position - 1)
                       + (tag.IsIn(VoidElements)
                            ? ">"
                              : $"></{tag}>")
                       + body.Substring(position + 2);
            }

            return body;
        }
    }
}
