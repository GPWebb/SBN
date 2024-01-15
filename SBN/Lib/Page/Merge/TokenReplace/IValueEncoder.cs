namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IValueEncoder
    {
        string Encode(string val, Definitions.EncodeType encodeType);
    }
}