using System;
using System.Threading.Tasks;

namespace SBN.Lib
{
    public interface ISessionTokenAccessor
    {
        Guid SessionToken();
        Task<Guid> SetSessionToken();
        void ResetSessionToken();
    }
}