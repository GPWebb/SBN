using System;

namespace SBN.Lib
{
    public class ErrorWrapper : IErrorWrapper
    {
        public void Wrap(string actionType, System.Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                throw new Exception($"Error {actionType}", ex);
            }
        }
    }
}
