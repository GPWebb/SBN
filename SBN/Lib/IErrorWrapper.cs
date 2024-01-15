namespace SBN.Lib
{
    public interface IErrorWrapper
    {
        void Wrap(string actionType, System.Action action);
    }
}