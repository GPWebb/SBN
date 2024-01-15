namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IParameterEncodeChecker
    {
        bool CheckEncodeParameter(ref string parameter, Definitions.ParamEncode encode);
    }
}