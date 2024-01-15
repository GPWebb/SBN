namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class ParameterReader : IParameterReader
    {
        public string Read(string output)
        {
            var position = output.IndexOf("[#");
            var end = position;
            var depth = 0;

            do
            {
                end++;
                if (output.Substring(end, 1) == "[") depth++;
                if (output.Substring(end, 1) == "]")
                {
                    if (depth <= 0) break;
                    depth--;
                }
            } while (true);
            
            var parameter = output.Substring(position + 2, end - position - 2);
            return parameter;
        }
    }
}
