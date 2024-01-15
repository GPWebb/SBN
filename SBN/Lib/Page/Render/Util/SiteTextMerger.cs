using System.Collections.Generic;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Render.Util
{
    public class SiteTextMerger : ISiteTextMerger
    {
        private readonly ISiteTextCache _siteTextCache;

        public SiteTextMerger(ISiteTextCache siteTextCache)
        {
            _siteTextCache = siteTextCache;
        }

        public async Task<string> Merge(string pageTemplate)
        {
            var output = pageTemplate;
            var siteTextParams = new List<string>();
            var position = 0;

            while (output.IndexOf("[$", position) > -1)
            {
                var index = output.IndexOf("[$", position);
                var parameter = output.Substring(index + 2, output.IndexOf("]", index) - index - 2);

                siteTextParams.Add(parameter);

                position = index + 1;
            }

            var values = await _siteTextCache.Read(siteTextParams);

            foreach (var value in values)
            {
                output = output.Replace($"[${value.Key}]", value.Value);
            }

            return output;
        }
    }
}
