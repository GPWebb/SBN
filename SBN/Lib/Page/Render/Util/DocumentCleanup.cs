using System.Collections.Generic;

namespace SBN.Lib.Page.Render.Util
{
    public class DocumentCleanup : IDocumentCleanup
    {
        List<(string src, string replace)> Replacements = new List<(string src, string replace)>
        {
            { ("&#xA;", "") },  //LF
            { ("&#x9;", "") }   //Tab
        };

        //HACK Just to stop the output getting filled with overenthusiastically encoded whitespace. There'll be a better way but this does for now.
        public string Clean(string document)
        {
            foreach (var replacement in Replacements)
            {
                document = document.Replace(replacement.src, replacement.replace);
            }

            return document;
        }
    }
}
